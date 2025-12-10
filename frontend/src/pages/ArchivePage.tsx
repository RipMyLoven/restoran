import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../components/Card';
import { Button } from '../components/Button';
import { Select } from '../components/Select';
import { DataTable } from '../components/DataTable';
import { archiveApi, ordersApi, restaurantsApi } from '../api';
import type { ArchivedOrder, Statistics, Order, Restaurant } from '../types';

export function ArchivePage() {
  const [archivedOrders, setArchivedOrders] = useState<ArchivedOrder[]>([]);
  const [statistics, setStatistics] = useState<Statistics | null>(null);
  const [completedOrders, setCompletedOrders] = useState<Order[]>([]);
  const [restaurants, setRestaurants] = useState<Restaurant[]>([]);
  const [selectedRestaurant, setSelectedRestaurant] = useState<number | ''>('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    loadArchive();
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

  const loadArchive = async () => {
    try {
      const [archiveData, statsData, ordersData] = await Promise.all([
        archiveApi.getAll(selectedRestaurant || undefined),
        archiveApi.getStatistics(selectedRestaurant || undefined),
        ordersApi.getAll()
      ]);
      setArchivedOrders(archiveData);
      setStatistics(statsData);
      // Filter only completed orders that can be archived
      setCompletedOrders(ordersData.filter(o => o.status === 'Completed'));
    } catch (error) {
      console.error('Failed to load archive:', error);
    }
  };

  const handleArchiveOrder = async (orderId: number) => {
    try {
      await archiveApi.archive(orderId);
      loadArchive();
    } catch (error) {
      console.error('Failed to archive order:', error);
    }
  };

  const columns = [
    { key: 'id' as const, header: 'ID' },
    { key: 'originalOrderId' as const, header: 'Original Order' },
    { key: 'tableNumber' as const, header: 'Table' },
    {
      key: 'total' as const,
      header: 'Total',
      render: (order: ArchivedOrder) => `€${order.total.toFixed(2)}`
    },
    {
      key: 'orderCreatedAt' as const,
      header: 'Order Date',
      render: (order: ArchivedOrder) => new Date(order.orderCreatedAt).toLocaleString()
    },
    {
      key: 'archivedAt' as const,
      header: 'Archived',
      render: (order: ArchivedOrder) => new Date(order.archivedAt).toLocaleString()
    }
  ];

  if (loading) return <div>Loading...</div>;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold text-gray-900">Archive & Statistics</h1>
      </div>

      {/* Statistics Cards */}
      {statistics && (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <Card>
            <CardHeader>
              <CardTitle>Total Orders</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-3xl font-bold">{statistics.totalOrders}</p>
              <p className="text-sm text-gray-500">Archived orders</p>
            </CardContent>
          </Card>
          <Card>
            <CardHeader>
              <CardTitle>Total Revenue</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-3xl font-bold">€{statistics.totalRevenue.toFixed(2)}</p>
              <p className="text-sm text-gray-500">From archived orders</p>
            </CardContent>
          </Card>
          <Card>
            <CardHeader>
              <CardTitle>Average Order</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-3xl font-bold">€{statistics.averageOrderValue.toFixed(2)}</p>
              <p className="text-sm text-gray-500">Average order value</p>
            </CardContent>
          </Card>
        </div>
      )}

      {/* Archive completed orders */}
      {completedOrders.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle>Archive Completed Orders</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-2">
              {completedOrders.map(order => (
                <div key={order.id} className="flex justify-between items-center bg-gray-50 p-3 rounded">
                  <span>Order #{order.id} - Table {order.tableId} - €{order.total?.toFixed(2) || '0.00'}</span>
                  <Button size="sm" onClick={() => handleArchiveOrder(order.id)}>
                    Archive
                  </Button>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      )}

      {/* Filter and Table */}
      <div className="flex gap-4">
        <Select
          label="Filter by Restaurant"
          value={selectedRestaurant}
          onChange={(e) => setSelectedRestaurant(e.target.value ? parseInt(e.target.value) : '')}
          options={[
            { value: '', label: 'All Restaurants' },
            ...restaurants.map(r => ({ value: r.id, label: r.name }))
          ]}
        />
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Archived Orders</CardTitle>
        </CardHeader>
        <CardContent className="p-0">
          <DataTable data={archivedOrders} columns={columns} keyField="id" />
        </CardContent>
      </Card>
    </div>
  );
}
