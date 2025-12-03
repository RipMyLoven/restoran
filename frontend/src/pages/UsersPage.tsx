import { useState, useEffect } from 'react';
import { Card, CardContent } from '../components/Card';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { Select } from '../components/Select';
import { DataTable } from '../components/DataTable';
import { Modal } from '../components/Modal';
import { Badge } from '../components/Badge';
import { usersApi, restaurantsApi } from '../api';
import type { User, CreateUserDto, Restaurant } from '../types';

export function UsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [restaurants, setRestaurants] = useState<Restaurant[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [formData, setFormData] = useState<CreateUserDto>({
    username: '',
    password: '',
    role: 'Waiter',
    restaurantId: undefined
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [usersData, restaurantsData] = await Promise.all([
        usersApi.getAll(),
        restaurantsApi.getAll()
      ]);
      setUsers(usersData);
      setRestaurants(restaurantsData);
    } catch (error) {
      console.error('Failed to load data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await usersApi.create(formData);
      setIsModalOpen(false);
      setFormData({ username: '', password: '', role: 'Waiter', restaurantId: undefined });
      loadData();
    } catch (error: any) {
      const errorData = error.response?.data;
      const message = typeof errorData === 'string' ? errorData : errorData?.title || 'Failed to create user';
      alert(message);
    }
  };

  const handleDelete = async (id: number) => {
    if (confirm('Are you sure you want to delete this user?')) {
      try {
        await usersApi.delete(id);
        loadData();
      } catch (error) {
        console.error('Failed to delete user:', error);
      }
    }
  };

  const getRestaurantName = (restaurantId?: number) => {
    if (!restaurantId) return '-';
    return restaurants.find(r => r.id === restaurantId)?.name || 'Unknown';
  };

  const roleColors: Record<string, 'info' | 'success' | 'warning'> = {
    Admin: 'info',
    Waiter: 'success',
    Cook: 'warning'
  };

  const columns = [
    { key: 'id' as const, header: 'ID' },
    { key: 'username' as const, header: 'Username' },
    {
      key: 'role' as const,
      header: 'Role',
      render: (user: User) => (
        <Badge variant={roleColors[user.role] || 'default'}>{user.role}</Badge>
      )
    },
    {
      key: 'restaurantId' as const,
      header: 'Restaurant',
      render: (user: User) => getRestaurantName(user.restaurantId)
    },
    {
      key: 'actions',
      header: 'Actions',
      render: (user: User) => (
        <Button size="sm" variant="ghost" onClick={() => handleDelete(user.id)}>
          Delete
        </Button>
      )
    }
  ];

  if (loading) return <div>Loading...</div>;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold text-gray-900">Users</h1>
        <Button onClick={() => setIsModalOpen(true)}>Add User</Button>
      </div>

      <Card>
        <CardContent className="p-0">
          <DataTable data={users} columns={columns} keyField="id" />
        </CardContent>
      </Card>

      <Modal
        isOpen={isModalOpen}
        onClose={() => {
          setIsModalOpen(false);
          setFormData({ username: '', password: '', role: 'Waiter', restaurantId: undefined });
        }}
        title="Add User"
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          <Input
            label="Username"
            value={formData.username}
            onChange={(e) => setFormData({ ...formData, username: e.target.value })}
            required
          />
          <Input
            label="Password"
            type="password"
            value={formData.password}
            onChange={(e) => setFormData({ ...formData, password: e.target.value })}
            required
          />
          <Select
            label="Role"
            value={formData.role}
            onChange={(e) => setFormData({ ...formData, role: e.target.value as 'Admin' | 'Waiter' | 'Cook' })}
            options={[
              { value: 'Admin', label: 'Admin' },
              { value: 'Waiter', label: 'Waiter' },
              { value: 'Cook', label: 'Cook' }
            ]}
          />
          <Select
            label="Restaurant (optional)"
            value={formData.restaurantId || ''}
            onChange={(e) => setFormData({ 
              ...formData, 
              restaurantId: e.target.value ? parseInt(e.target.value) : undefined 
            })}
            options={[
              { value: '', label: 'No Restaurant' },
              ...restaurants.map(r => ({ value: r.id, label: r.name }))
            ]}
          />
          <div className="flex justify-end gap-2">
            <Button type="button" variant="outline" onClick={() => setIsModalOpen(false)}>
              Cancel
            </Button>
            <Button type="submit">Create</Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
