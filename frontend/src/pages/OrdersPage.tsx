import { useState, useEffect } from 'react';
import { Card, CardContent } from '../components/Card';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { Select } from '../components/Select';
import { DataTable } from '../components/DataTable';
import { Modal } from '../components/Modal';
import { Badge } from '../components/Badge';
import { ordersApi, tablesApi, menuItemsApi, restaurantsApi } from '../api';
import { useAuth } from '../context/AuthContext';
import type { Order, CreateOrderDto, Table, MenuItem, Restaurant, OrderStatus } from '../types';

const statusColors: Record<OrderStatus, 'default' | 'warning' | 'info' | 'success' | 'danger'> = {
  Pending: 'warning',
  Preparing: 'info',
  Ready: 'success',
  Completed: 'default',
  Cancelled: 'danger'
};

export function OrdersPage() {
  const { user } = useAuth();
  const [orders, setOrders] = useState<Order[]>([]);
  const [tables, setTables] = useState<Table[]>([]);
  const [menuItems, setMenuItems] = useState<MenuItem[]>([]);
  const [restaurants, setRestaurants] = useState<Restaurant[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isStatusModalOpen, setIsStatusModalOpen] = useState(false);
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);
  const [selectedRestaurant, setSelectedRestaurant] = useState<number | ''>('');
  const [selectedTable, setSelectedTable] = useState<number | ''>('');
  const [formData, setFormData] = useState<CreateOrderDto>({
    tableId: 0,
    notes: '',
    orderItems: []
  });
  const [newStatus, setNewStatus] = useState<OrderStatus>('Pending');

  // Проверка прав
  const canCreateOrder = user?.role === 'Admin' || user?.role === 'Waiter';
  const canDeleteOrder = user?.role === 'Admin';

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    loadOrders();
  }, [selectedTable]);

  useEffect(() => {
    if (selectedRestaurant) {
      loadTablesAndMenu();
    }
  }, [selectedRestaurant]);

  const loadData = async () => {
    try {
      const restaurantsData = await restaurantsApi.getAll();
      setRestaurants(restaurantsData);
    } catch (error) {
      console.error('Failed to load data:', error);
    } finally {
      setLoading(false);
    }
  };

  const loadTablesAndMenu = async () => {
    try {
      const [tablesData, menuData] = await Promise.all([
        tablesApi.getAll(selectedRestaurant || undefined),
        menuItemsApi.getAll(selectedRestaurant || undefined)
      ]);
      setTables(tablesData);
      setMenuItems(menuData);
    } catch (error) {
      console.error('Failed to load tables and menu:', error);
    }
  };

  const loadOrders = async () => {
    try {
      const data = await ordersApi.getAll(selectedTable || undefined);
      setOrders(data);
    } catch (error) {
      console.error('Failed to load orders:', error);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (formData.orderItems.length === 0) {
      alert('Please add at least one item to the order');
      return;
    }
    try {
      await ordersApi.create(formData);
      setIsModalOpen(false);
      setFormData({ tableId: 0, notes: '', orderItems: [] });
      loadOrders();
    } catch (error: any) {
      const errorData = error.response?.data;
      const message = typeof errorData === 'string' ? errorData : errorData?.title || 'Failed to create order';
      alert(message);
    }
  };

  const handleStatusUpdate = async () => {
    if (!selectedOrder) return;
    try {
      await ordersApi.updateStatus(selectedOrder.id, { status: newStatus });
      setIsStatusModalOpen(false);
      setSelectedOrder(null);
      loadOrders();
    } catch (error) {
      console.error('Failed to update order status:', error);
    }
  };

  const handleDelete = async (id: number) => {
    if (confirm('Are you sure you want to delete this order?')) {
      try {
        await ordersApi.delete(id);
        loadOrders();
      } catch (error) {
        console.error('Failed to delete order:', error);
      }
    }
  };

  const openStatusModal = (order: Order) => {
    setSelectedOrder(order);
    setNewStatus(order.status);
    setIsStatusModalOpen(true);
  };

  const addOrderItem = (menuItemId: number) => {
    const existingItem = formData.orderItems.find(item => item.menuItemId === menuItemId);
    if (existingItem) {
      setFormData({
        ...formData,
        orderItems: formData.orderItems.map(item =>
          item.menuItemId === menuItemId
            ? { ...item, quantity: item.quantity + 1 }
            : item
        )
      });
    } else {
      setFormData({
        ...formData,
        orderItems: [...formData.orderItems, { menuItemId, quantity: 1 }]
      });
    }
  };

  const removeOrderItem = (menuItemId: number) => {
    setFormData({
      ...formData,
      orderItems: formData.orderItems.filter(item => item.menuItemId !== menuItemId)
    });
  };

  const getMenuItemName = (menuItemId: number) => {
    return menuItems.find(m => m.id === menuItemId)?.name || 'Unknown';
  };

  const columns = [
    { key: 'id' as const, header: 'ID' },
    { key: 'tableId' as const, header: 'Table' },
    {
      key: 'status' as const,
      header: 'Status',
      render: (order: Order) => (
        <Badge variant={statusColors[order.status]}>{order.status}</Badge>
      )
    },
    { key: 'notes' as const, header: 'Notes' },
    {
      key: 'orderItems' as const,
      header: 'Items',
      render: (order: Order) => order.orderItems?.length || 0
    },
    {
      key: 'total' as const,
      header: 'Total',
      render: (order: Order) => `€${order.total?.toFixed(2) || '0.00'}`
    },
    {
      key: 'actions',
      header: 'Actions',
      render: (order: Order) => (
        <div className="flex gap-2">
          <Button size="sm" variant="outline" onClick={() => openStatusModal(order)}>
            Status
          </Button>
          {canDeleteOrder && (
            <Button size="sm" variant="ghost" onClick={() => handleDelete(order.id)}>
              Delete
            </Button>
          )}
        </div>
      )
    }
  ];

  if (loading) return <div>Loading...</div>;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold text-gray-900">Orders</h1>
        {canCreateOrder && (
          <Button onClick={() => setIsModalOpen(true)}>New Order</Button>
        )}
      </div>

      <div className="flex gap-4">
        <Select
          label="Filter by Restaurant"
          value={selectedRestaurant}
          onChange={(e) => {
            setSelectedRestaurant(e.target.value ? parseInt(e.target.value) : '');
            setSelectedTable('');
          }}
          options={[
            { value: '', label: 'All Restaurants' },
            ...restaurants.map(r => ({ value: r.id, label: r.name }))
          ]}
        />
        {selectedRestaurant && (
          <Select
            label="Filter by Table"
            value={selectedTable}
            onChange={(e) => setSelectedTable(e.target.value ? parseInt(e.target.value) : '')}
            options={[
              { value: '', label: 'All Tables' },
              ...tables.map(t => ({ value: t.id, label: `Table ${t.tableNumber}` }))
            ]}
          />
        )}
      </div>

      <Card>
        <CardContent className="p-0">
          <DataTable data={orders} columns={columns} keyField="id" />
        </CardContent>
      </Card>

      {/* Create Order Modal */}
      <Modal
        isOpen={isModalOpen}
        onClose={() => {
          setIsModalOpen(false);
          setFormData({ tableId: 0, notes: '', orderItems: [] });
        }}
        title="New Order"
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          <Select
            label="Restaurant"
            value={selectedRestaurant}
            onChange={(e) => setSelectedRestaurant(parseInt(e.target.value) || '')}
            options={[
              { value: '', label: 'Select Restaurant' },
              ...restaurants.map(r => ({ value: r.id, label: r.name }))
            ]}
            required
          />
          {selectedRestaurant && (
            <>
              <Select
                label="Table"
                value={formData.tableId}
                onChange={(e) => setFormData({ ...formData, tableId: parseInt(e.target.value) })}
                options={[
                  { value: 0, label: 'Select Table' },
                  ...tables.map(t => ({ value: t.id, label: `Table ${t.tableNumber}` }))
                ]}
                required
              />
              <Input
                label="Notes"
                value={formData.notes}
                onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                placeholder="Special requests..."
              />
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">Menu Items</label>
                <div className="grid grid-cols-2 gap-2 max-h-40 overflow-y-auto">
                  {menuItems.map((item) => (
                    <Button
                      key={item.id}
                      type="button"
                      variant="outline"
                      size="sm"
                      onClick={() => addOrderItem(item.id)}
                    >
                      {item.name} - €{item.price.toFixed(2)}
                    </Button>
                  ))}
                </div>
              </div>
              {formData.orderItems.length > 0 && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Order Items</label>
                  <div className="space-y-2">
                    {formData.orderItems.map((item) => (
                      <div key={item.menuItemId} className="flex justify-between items-center bg-gray-50 p-2 rounded">
                        <span>{getMenuItemName(item.menuItemId)} x{item.quantity}</span>
                        <Button
                          type="button"
                          variant="ghost"
                          size="sm"
                          onClick={() => removeOrderItem(item.menuItemId)}
                        >
                          Remove
                        </Button>
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </>
          )}
          <div className="flex justify-end gap-2">
            <Button type="button" variant="outline" onClick={() => setIsModalOpen(false)}>
              Cancel
            </Button>
            <Button type="submit" disabled={!formData.tableId || formData.orderItems.length === 0}>
              Create Order
            </Button>
          </div>
        </form>
      </Modal>

      {/* Update Status Modal */}
      <Modal
        isOpen={isStatusModalOpen}
        onClose={() => {
          setIsStatusModalOpen(false);
          setSelectedOrder(null);
        }}
        title="Update Order Status"
      >
        <div className="space-y-4">
          <Select
            label="Status"
            value={newStatus}
            onChange={(e) => setNewStatus(e.target.value as OrderStatus)}
            options={[
              { value: 'Pending', label: 'Pending' },
              { value: 'Preparing', label: 'Preparing' },
              { value: 'Ready', label: 'Ready' },
              { value: 'Completed', label: 'Completed' },
              { value: 'Cancelled', label: 'Cancelled' }
            ]}
          />
          <div className="flex justify-end gap-2">
            <Button type="button" variant="outline" onClick={() => setIsStatusModalOpen(false)}>
              Cancel
            </Button>
            <Button onClick={handleStatusUpdate}>Update</Button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
