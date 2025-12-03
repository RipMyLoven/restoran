import { useState, useEffect } from 'react';
import { Card, CardContent } from '../components/Card';
import { Button } from '../components/Button';
import { Select } from '../components/Select';
import { DataTable } from '../components/DataTable';
import { Badge } from '../components/Badge';
import { billsApi, ordersApi } from '../api';
import type { Bill, Order } from '../types';

export function BillsPage() {
  const [bills, setBills] = useState<Bill[]>([]);
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedOrder, setSelectedOrder] = useState<number | ''>('');

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [billsData, ordersData] = await Promise.all([
        billsApi.getAll(),
        ordersApi.getAll()
      ]);
      setBills(billsData);
      setOrders(ordersData);
    } catch (error) {
      console.error('Failed to load data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateBill = async () => {
    if (!selectedOrder) return;
    try {
      await billsApi.create({ orderId: selectedOrder });
      setSelectedOrder('');
      loadData();
    } catch (error) {
      console.error('Failed to create bill:', error);
    }
  };

  const handlePayBill = async (id: number) => {
    try {
      await billsApi.pay(id);
      loadData();
    } catch (error) {
      console.error('Failed to pay bill:', error);
    }
  };

  // Filter orders that don't have bills yet
  const ordersWithoutBills = orders.filter(
    order => !bills.some(bill => bill.orderId === order.id) && 
             (order.status === 'Ready' || order.status === 'Completed')
  );

  const columns = [
    { key: 'id' as const, header: 'ID' },
    { key: 'orderId' as const, header: 'Order ID' },
    {
      key: 'total' as const,
      header: 'Total',
      render: (bill: Bill) => `€${bill.total.toFixed(2)}`
    },
    {
      key: 'isPaid' as const,
      header: 'Status',
      render: (bill: Bill) => (
        <Badge variant={bill.isPaid ? 'success' : 'warning'}>
          {bill.isPaid ? 'Paid' : 'Unpaid'}
        </Badge>
      )
    },
    {
      key: 'createdAt' as const,
      header: 'Created',
      render: (bill: Bill) => new Date(bill.createdAt).toLocaleString()
    },
    {
      key: 'paidAt' as const,
      header: 'Paid At',
      render: (bill: Bill) => bill.paidAt ? new Date(bill.paidAt).toLocaleString() : '-'
    },
    {
      key: 'actions',
      header: 'Actions',
      render: (bill: Bill) => (
        !bill.isPaid && (
          <Button size="sm" variant="primary" onClick={() => handlePayBill(bill.id)}>
            Mark as Paid
          </Button>
        )
      )
    }
  ];

  if (loading) return <div>Loading...</div>;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold text-gray-900">Bills</h1>
      </div>

      <Card>
        <CardContent>
          <h3 className="text-lg font-medium mb-4">Create Bill</h3>
          <div className="flex gap-4">
            <Select
              label="Select Order"
              value={selectedOrder}
              onChange={(e) => setSelectedOrder(e.target.value ? parseInt(e.target.value) : '')}
              options={[
                { value: '', label: 'Select an order' },
                ...ordersWithoutBills.map(o => ({ 
                  value: o.id, 
                  label: `Order #${o.id} - Table ${o.tableId} - €${o.total?.toFixed(2) || '0.00'}` 
                }))
              ]}
            />
            <div className="flex items-end">
              <Button onClick={handleCreateBill} disabled={!selectedOrder}>
                Create Bill
              </Button>
            </div>
          </div>
          {ordersWithoutBills.length === 0 && (
            <p className="text-gray-500 text-sm mt-2">No eligible orders available for billing</p>
          )}
        </CardContent>
      </Card>

      <Card>
        <CardContent className="p-0">
          <DataTable data={bills} columns={columns} keyField="id" />
        </CardContent>
      </Card>
    </div>
  );
}
