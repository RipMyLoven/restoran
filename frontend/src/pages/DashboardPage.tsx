import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../components/Card';
import { Badge } from '../components/Badge';
import { ordersApi, restaurantsApi, archiveApi, notificationsApi } from '../api';
import type { Order, Restaurant, Statistics, Notification } from '../types';
import { useAuth } from '../context/AuthContext';

export function DashboardPage() {
  const { user } = useAuth();
  const [orders, setOrders] = useState<Order[]>([]);
  const [restaurants, setRestaurants] = useState<Restaurant[]>([]);
  const [statistics, setStatistics] = useState<Statistics | null>(null);
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadData();
  }, [user]);

  const loadData = async () => {
    try {
      // Загружаем данные параллельно, обрабатывая ошибки отдельно
      const [ordersResult, restaurantsResult, statsResult, notificationsResult] = await Promise.allSettled([
        ordersApi.getAll(),
        restaurantsApi.getAll(),
        archiveApi.getStatistics(),
        user?.id ? notificationsApi.getAll(user.id) : Promise.resolve([])
      ]);

      if (ordersResult.status === 'fulfilled') setOrders(ordersResult.value);
      if (restaurantsResult.status === 'fulfilled') setRestaurants(restaurantsResult.value);
      if (statsResult.status === 'fulfilled') setStatistics(statsResult.value);
      if (notificationsResult.status === 'fulfilled') setNotifications(notificationsResult.value);
    } catch (error) {
      console.error('Failed to load dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  const activeOrders = orders.filter(o => o.status !== 'Completed' && o.status !== 'Cancelled');
  const pendingOrders = orders.filter(o => o.status === 'New');
  const unreadNotifications = notifications.filter(n => !n.isRead);

  if (loading) {
    return <div className="flex items-center justify-center h-64">Loading...</div>;
  }

  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
      
      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        <Card>
          <CardHeader>
            <CardTitle>Active Orders</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-3xl font-bold">{activeOrders.length}</p>
            <p className="text-sm text-gray-500">{pendingOrders.length} pending</p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>Restaurants</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-3xl font-bold">{restaurants.length}</p>
            <p className="text-sm text-gray-500">Total restaurants</p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>Total Revenue</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-3xl font-bold">€{statistics?.totalRevenue.toFixed(2) || '0.00'}</p>
            <p className="text-sm text-gray-500">From {statistics?.totalOrders || 0} archived orders</p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>Notifications</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-3xl font-bold">{unreadNotifications.length}</p>
            <p className="text-sm text-gray-500">Unread notifications</p>
          </CardContent>
        </Card>
      </div>

      {/* Recent Orders */}
      <Card>
        <CardHeader>
          <CardTitle>Recent Orders</CardTitle>
        </CardHeader>
        <CardContent>
          {activeOrders.length === 0 ? (
            <p className="text-gray-500">No active orders</p>
          ) : (
            <div className="space-y-3">
              {activeOrders.slice(0, 5).map(order => (
                <div key={order.id} className="flex justify-between items-center p-3 bg-gray-50 rounded-lg">
                  <div>
                    <span className="font-medium">Order #{order.id}</span>
                    <span className="text-gray-500 ml-2">Table {order.tableId}</span>
                  </div>
                  <div className="flex items-center gap-3">
                    <span className="text-gray-600">€{order.total?.toFixed(2) || '0.00'}</span>
                    <Badge 
                      variant={
                        order.status === 'Ready' ? 'success' :
                        order.status === 'InProgress' ? 'info' :
                        order.status === 'New' ? 'warning' : 'default'
                      }
                    >
                      {order.status}
                    </Badge>
                  </div>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>

      {/* Recent Notifications */}
      {unreadNotifications.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle>Unread Notifications</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {unreadNotifications.slice(0, 5).map(notification => (
                <div key={notification.id} className="flex justify-between items-center p-3 bg-yellow-50 rounded-lg">
                  <div>
                    <Badge variant={notification.type === 'OrderCreated' ? 'info' : 'success'}>
                      {notification.type === 'OrderCreated' ? 'New Order' : 'Order Ready'}
                    </Badge>
                    <span className="ml-2">{notification.message}</span>
                  </div>
                  <span className="text-sm text-gray-500">
                    {new Date(notification.createdAt).toLocaleTimeString()}
                  </span>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
