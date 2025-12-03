import { useState, useEffect } from 'react';
import { Card, CardContent } from '../components/Card';
import { Button } from '../components/Button';
import { DataTable } from '../components/DataTable';
import { Badge } from '../components/Badge';
import { notificationsApi } from '../api';
import type { Notification } from '../types';
import { useAuth } from '../context/AuthContext';

export function NotificationsPage() {
  const { user } = useAuth();
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadNotifications();
  }, []);

  const loadNotifications = async () => {
    try {
      const data = await notificationsApi.getAll(user?.id);
      setNotifications(data);
    } catch (error) {
      console.error('Failed to load notifications:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleMarkAsRead = async (id: number) => {
    try {
      await notificationsApi.markAsRead(id);
      loadNotifications();
    } catch (error) {
      console.error('Failed to mark notification as read:', error);
    }
  };

  const handleMarkAllAsRead = async () => {
    if (!user) return;
    try {
      await notificationsApi.markAllAsRead(user.id);
      loadNotifications();
    } catch (error) {
      console.error('Failed to mark all notifications as read:', error);
    }
  };

  const unreadCount = notifications.filter(n => !n.isRead).length;

  const columns = [
    { key: 'id' as const, header: 'ID' },
    {
      key: 'type' as const,
      header: 'Type',
      render: (notification: Notification) => (
        <Badge variant={notification.type === 'OrderCreated' ? 'info' : 'success'}>
          {notification.type === 'OrderCreated' ? 'New Order' : 'Order Ready'}
        </Badge>
      )
    },
    { key: 'message' as const, header: 'Message' },
    { key: 'orderId' as const, header: 'Order ID' },
    {
      key: 'isRead' as const,
      header: 'Status',
      render: (notification: Notification) => (
        <Badge variant={notification.isRead ? 'default' : 'warning'}>
          {notification.isRead ? 'Read' : 'Unread'}
        </Badge>
      )
    },
    {
      key: 'createdAt' as const,
      header: 'Created',
      render: (notification: Notification) => new Date(notification.createdAt).toLocaleString()
    },
    {
      key: 'actions',
      header: 'Actions',
      render: (notification: Notification) => (
        !notification.isRead && (
          <Button size="sm" variant="outline" onClick={() => handleMarkAsRead(notification.id)}>
            Mark as Read
          </Button>
        )
      )
    }
  ];

  if (loading) return <div>Loading...</div>;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Notifications</h1>
          {unreadCount > 0 && (
            <p className="text-gray-600">{unreadCount} unread notification{unreadCount !== 1 ? 's' : ''}</p>
          )}
        </div>
        {unreadCount > 0 && (
          <Button onClick={handleMarkAllAsRead}>Mark All as Read</Button>
        )}
      </div>

      <Card>
        <CardContent className="p-0">
          <DataTable data={notifications} columns={columns} keyField="id" />
        </CardContent>
      </Card>
    </div>
  );
}
