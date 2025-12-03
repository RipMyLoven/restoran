import { useState, useEffect } from 'react';
import { Card, CardContent } from '../components/Card';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { Select } from '../components/Select';
import { DataTable } from '../components/DataTable';
import { Modal } from '../components/Modal';
import { tablesApi, restaurantsApi } from '../api';
import type { Table, CreateTableDto, Restaurant } from '../types';

export function TablesPage() {
  const [tables, setTables] = useState<Table[]>([]);
  const [restaurants, setRestaurants] = useState<Restaurant[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedRestaurant, setSelectedRestaurant] = useState<number | ''>('');
  const [formData, setFormData] = useState<CreateTableDto>({
    tableNumber: 1,
    restaurantId: 0
  });

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    loadTables();
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

  const loadTables = async () => {
    try {
      const data = await tablesApi.getAll(selectedRestaurant || undefined);
      setTables(data);
    } catch (error) {
      console.error('Failed to load tables:', error);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await tablesApi.create(formData);
      setIsModalOpen(false);
      setFormData({ tableNumber: 1, restaurantId: 0 });
      loadTables();
    } catch (error) {
      console.error('Failed to create table:', error);
    }
  };

  const handleDelete = async (id: number) => {
    if (confirm('Are you sure you want to delete this table?')) {
      try {
        await tablesApi.delete(id);
        loadTables();
      } catch (error) {
        console.error('Failed to delete table:', error);
      }
    }
  };

  const getRestaurantName = (restaurantId: number) => {
    return restaurants.find(r => r.id === restaurantId)?.name || 'Unknown';
  };

  const columns = [
    { key: 'id' as const, header: 'ID' },
    { key: 'tableNumber' as const, header: 'Table Number' },
    {
      key: 'restaurantId' as const,
      header: 'Restaurant',
      render: (table: Table) => getRestaurantName(table.restaurantId)
    },
    {
      key: 'actions',
      header: 'Actions',
      render: (table: Table) => (
        <Button size="sm" variant="ghost" onClick={() => handleDelete(table.id)}>
          Delete
        </Button>
      )
    }
  ];

  if (loading) return <div>Loading...</div>;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold text-gray-900">Tables</h1>
        <Button onClick={() => setIsModalOpen(true)}>Add Table</Button>
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
          <DataTable data={tables} columns={columns} keyField="id" />
        </CardContent>
      </Card>

      <Modal
        isOpen={isModalOpen}
        onClose={() => {
          setIsModalOpen(false);
          setFormData({ tableNumber: 1, restaurantId: 0 });
        }}
        title="Add Table"
      >
        <form onSubmit={handleSubmit} className="space-y-4">
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
          <Input
            label="Table Number"
            type="number"
            value={formData.tableNumber}
            onChange={(e) => setFormData({ ...formData, tableNumber: parseInt(e.target.value) || 1 })}
            required
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
