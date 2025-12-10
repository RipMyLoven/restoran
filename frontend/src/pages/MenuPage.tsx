import { useState, useEffect } from 'react';
import { Card, CardContent } from '../components/Card';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { Select } from '../components/Select';
import { DataTable } from '../components/DataTable';
import { Modal } from '../components/Modal';
import { menuItemsApi, restaurantsApi } from '../api';
import { useAuth } from '../context/AuthContext';
import type { MenuItem, CreateMenuItemDto, Restaurant } from '../types';

export function MenuPage() {
  const { user } = useAuth();
  const isAdmin = user?.role === 'Admin';
  const isCook = user?.role === 'Cook';
  const canManageMenu = isAdmin || isCook;
  const [menuItems, setMenuItems] = useState<MenuItem[]>([]);
  const [restaurants, setRestaurants] = useState<Restaurant[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<MenuItem | null>(null);
  const [selectedRestaurant, setSelectedRestaurant] = useState<number | ''>('');
  const [formData, setFormData] = useState<CreateMenuItemDto>({
    name: '',
    price: 0,
    restaurantId: 0
  });

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    loadMenuItems();
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

  const loadMenuItems = async () => {
    try {
      const data = await menuItemsApi.getAll(selectedRestaurant || undefined);
      setMenuItems(data);
    } catch (error) {
      console.error('Failed to load menu items:', error);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editingItem) {
        await menuItemsApi.update(editingItem.id, { name: formData.name, price: formData.price });
      } else {
        await menuItemsApi.create(formData);
      }
      setIsModalOpen(false);
      setEditingItem(null);
      setFormData({ name: '', price: 0, restaurantId: 0 });
      loadMenuItems();
    } catch (error: any) {
      const errorData = error.response?.data;
      const message = typeof errorData === 'string' ? errorData : errorData?.title || 'Failed to save menu item';
      alert(message);
    }
  };

  const handleEdit = (item: MenuItem) => {
    setEditingItem(item);
    setFormData({
      name: item.name,
      price: item.price,
      restaurantId: item.restaurantId
    });
    setIsModalOpen(true);
  };

  const handleDelete = async (id: number) => {
    if (confirm('Are you sure you want to delete this menu item?')) {
      try {
        await menuItemsApi.delete(id);
        loadMenuItems();
      } catch (error) {
        console.error('Failed to delete menu item:', error);
      }
    }
  };

  const getRestaurantName = (restaurantId: number) => {
    return restaurants.find(r => r.id === restaurantId)?.name || 'Unknown';
  };

  const columns = [
    { key: 'id' as const, header: 'ID' },
    { key: 'name' as const, header: 'Name' },
    {
      key: 'price' as const,
      header: 'Price',
      render: (item: MenuItem) => `€${item.price.toFixed(2)}`
    },
    {
      key: 'restaurantId' as const,
      header: 'Restaurant',
      render: (item: MenuItem) => getRestaurantName(item.restaurantId)
    },
    ...(canManageMenu ? [{
      key: 'actions',
      header: 'Actions',
      render: (item: MenuItem) => (
        <div className="flex gap-2">
          <Button size="sm" variant="outline" onClick={() => handleEdit(item)}>
            Edit
          </Button>
          <Button size="sm" variant="ghost" onClick={() => handleDelete(item.id)}>
            Delete
          </Button>
        </div>
      )
    }] : [])
  ];

  if (loading) return <div>Loading...</div>;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold text-gray-900">Menu Items</h1>
        {canManageMenu && (
          <Button onClick={() => setIsModalOpen(true)}>Add Menu Item</Button>
        )}
      </div>

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
        <CardContent className="p-0">
          <DataTable data={menuItems} columns={columns} keyField="id" />
        </CardContent>
      </Card>

      <Modal
        isOpen={isModalOpen}
        onClose={() => {
          setIsModalOpen(false);
          setEditingItem(null);
          setFormData({ name: '', price: 0, restaurantId: 0 });
        }}
        title={editingItem ? 'Edit Menu Item' : 'Add Menu Item'}
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          {!editingItem && (
            <Select
              label="Restaurant"
              value={formData.restaurantId}
              onChange={(e) => setFormData({ ...formData, restaurantId: parseInt(e.target.value) })}
              options={[
                { value: 0, label: 'Select Restaurant' },
                ...restaurants.map(r => ({ value: r.id, label: r.name }))
              ]}
              required
            />
          )}
          <Input
            label="Name"
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            required
          />
          <Input
            label="Price (€)"
            type="number"
            step="0.01"
            value={formData.price}
            onChange={(e) => setFormData({ ...formData, price: parseFloat(e.target.value) || 0 })}
            required
          />
          <div className="flex justify-end gap-2">
            <Button type="button" variant="outline" onClick={() => setIsModalOpen(false)}>
              Cancel
            </Button>
            <Button type="submit">
              {editingItem ? 'Update' : 'Create'}
            </Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
