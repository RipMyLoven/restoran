import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../components/Card';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { DataTable } from '../components/DataTable';
import { Modal } from '../components/Modal';
import { restaurantsApi } from '../api';
import type { Restaurant, CreateRestaurantDto } from '../types';

export function RestaurantsPage() {
  const [restaurants, setRestaurants] = useState<Restaurant[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRestaurant, setEditingRestaurant] = useState<Restaurant | null>(null);
  const [formData, setFormData] = useState<CreateRestaurantDto>({
    name: '',
    tableCount: 0,
    allergyTags: '',
    dietTags: ''
  });

  useEffect(() => {
    loadRestaurants();
  }, []);

  const loadRestaurants = async () => {
    try {
      const data = await restaurantsApi.getAll();
      setRestaurants(data);
    } catch (error) {
      console.error('Failed to load restaurants:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editingRestaurant) {
        await restaurantsApi.update(editingRestaurant.id, formData);
      } else {
        await restaurantsApi.create(formData);
      }
      setIsModalOpen(false);
      setEditingRestaurant(null);
      setFormData({ name: '', tableCount: 0, allergyTags: '', dietTags: '' });
      loadRestaurants();
    } catch (error: any) {
      const errorData = error.response?.data;
      const message = typeof errorData === 'string' ? errorData : errorData?.title || 'Failed to save restaurant';
      alert(message);
    }
  };

  const handleEdit = (restaurant: Restaurant) => {
    setEditingRestaurant(restaurant);
    setFormData({
      name: restaurant.name,
      tableCount: restaurant.tableCount,
      allergyTags: restaurant.allergyTags || '',
      dietTags: restaurant.dietTags || ''
    });
    setIsModalOpen(true);
  };

  const handleDelete = async (id: number) => {
    if (confirm('Are you sure you want to delete this restaurant?')) {
      try {
        await restaurantsApi.delete(id);
        loadRestaurants();
      } catch (error) {
        console.error('Failed to delete restaurant:', error);
      }
    }
  };

  const columns = [
    { key: 'id' as const, header: 'ID' },
    { key: 'name' as const, header: 'Name' },
    { key: 'tableCount' as const, header: 'Tables' },
    { key: 'allergyTags' as const, header: 'Allergy Tags' },
    { key: 'dietTags' as const, header: 'Diet Tags' },
    {
      key: 'actions',
      header: 'Actions',
      render: (restaurant: Restaurant) => (
        <div className="flex gap-2">
          <Button size="sm" variant="outline" onClick={() => handleEdit(restaurant)}>
            Edit
          </Button>
          <Button size="sm" variant="ghost" onClick={() => handleDelete(restaurant.id)}>
            Delete
          </Button>
        </div>
      )
    }
  ];

  if (loading) return <div>Loading...</div>;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold text-gray-900">Restaurants</h1>
        <Button onClick={() => setIsModalOpen(true)}>Add Restaurant</Button>
      </div>

      <Card>
        <CardContent className="p-0">
          <DataTable data={restaurants} columns={columns} keyField="id" />
        </CardContent>
      </Card>

      <Modal
        isOpen={isModalOpen}
        onClose={() => {
          setIsModalOpen(false);
          setEditingRestaurant(null);
          setFormData({ name: '', tableCount: 0, allergyTags: '', dietTags: '' });
        }}
        title={editingRestaurant ? 'Edit Restaurant' : 'Add Restaurant'}
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          <Input
            label="Name"
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            required
          />
          <Input
            label="Table Count"
            type="number"
            value={formData.tableCount}
            onChange={(e) => setFormData({ ...formData, tableCount: parseInt(e.target.value) || 0 })}
            required
          />
          <Input
            label="Allergy Tags (comma separated)"
            value={formData.allergyTags}
            onChange={(e) => setFormData({ ...formData, allergyTags: e.target.value })}
            placeholder="nuts, dairy, gluten"
          />
          <Input
            label="Diet Tags (comma separated)"
            value={formData.dietTags}
            onChange={(e) => setFormData({ ...formData, dietTags: e.target.value })}
            placeholder="vegan, vegetarian, halal"
          />
          <div className="flex justify-end gap-2">
            <Button type="button" variant="outline" onClick={() => setIsModalOpen(false)}>
              Cancel
            </Button>
            <Button type="submit">
              {editingRestaurant ? 'Update' : 'Create'}
            </Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
