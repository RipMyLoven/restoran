import { client } from './client';
import type {
    Restaurant,
    CreateRestaurantDto,
    UpdateRestaurantDto,
    Table,
    CreateTableDto,
    MenuItem,
    CreateMenuItemDto,
    UpdateMenuItemDto,
    Order,
    CreateOrderDto,
    UpdateOrderStatusDto,
    Bill,
    CreateBillDto,
    Notification,
    ArchivedOrder,
    Statistics,
    User,
    CreateUserDto,
    AuthResponse,
    RegisterDto
} from '../types';

// Auth API
export const authApi = {
    register: async (data: RegisterDto): Promise<AuthResponse> => {
        const response = await client.post<AuthResponse>('/auth/register', data);
        return response.data;
    }
};

// Restaurants API
export const restaurantsApi = {
    getAll: async (): Promise<Restaurant[]> => {
        const response = await client.get<Restaurant[]>('/restaurants');
        return response.data;
    },

    getById: async (id: number): Promise<Restaurant> => {
        const response = await client.get<Restaurant>(`/restaurants/${id}`);
        return response.data;
    },

    create: async (data: CreateRestaurantDto): Promise<Restaurant> => {
        const response = await client.post<Restaurant>('/restaurants', data);
        return response.data;
    },

    update: async (id: number, data: UpdateRestaurantDto): Promise<void> => {
        await client.put(`/restaurants/${id}`, data);
    },

    delete: async (id: number): Promise<void> => {
        await client.delete(`/restaurants/${id}`);
    }
};

// Tables API
export const tablesApi = {
    getAll: async (restaurantId?: number): Promise<Table[]> => {
        const params = restaurantId ? { restaurantId } : {};
        const response = await client.get<Table[]>('/tables', { params });
        return response.data;
    },

    getById: async (id: number): Promise<Table> => {
        const response = await client.get<Table>(`/tables/${id}`);
        return response.data;
    },

    create: async (data: CreateTableDto): Promise<Table> => {
        const response = await client.post<Table>('/tables', data);
        return response.data;
    },

    delete: async (id: number): Promise<void> => {
        await client.delete(`/tables/${id}`);
    }
};

// MenuItems API
export const menuItemsApi = {
    getAll: async (restaurantId?: number): Promise<MenuItem[]> => {
        const params = restaurantId ? { restaurantId } : {};
        const response = await client.get<MenuItem[]>('/menuitems', { params });
        return response.data;
    },

    getById: async (id: number): Promise<MenuItem> => {
        const response = await client.get<MenuItem>(`/menuitems/${id}`);
        return response.data;
    },

    create: async (data: CreateMenuItemDto): Promise<MenuItem> => {
        const response = await client.post<MenuItem>('/menuitems', data);
        return response.data;
    },

    update: async (id: number, data: UpdateMenuItemDto): Promise<void> => {
        await client.put(`/menuitems/${id}`, data);
    },

    delete: async (id: number): Promise<void> => {
        await client.delete(`/menuitems/${id}`);
    }
};

// Orders API
export const ordersApi = {
    getAll: async (tableId?: number): Promise<Order[]> => {
        const params = tableId ? { tableId } : {};
        const response = await client.get<Order[]>('/orders', { params });
        return response.data;
    },

    getById: async (id: number): Promise<Order> => {
        const response = await client.get<Order>(`/orders/${id}`);
        return response.data;
    },

    create: async (data: CreateOrderDto): Promise<Order> => {
        const response = await client.post<Order>('/orders', data);
        return response.data;
    },

    updateStatus: async (id: number, data: UpdateOrderStatusDto): Promise<void> => {
        await client.patch(`/orders/${id}/status`, data);
    },

    delete: async (id: number): Promise<void> => {
        await client.delete(`/orders/${id}`);
    }
};

// Bills API
export const billsApi = {
    getAll: async (): Promise<Bill[]> => {
        const response = await client.get<Bill[]>('/bills');
        return response.data;
    },

    getById: async (id: number): Promise<Bill> => {
        const response = await client.get<Bill>(`/bills/${id}`);
        return response.data;
    },

    create: async (data: CreateBillDto): Promise<Bill> => {
        const response = await client.post<Bill>('/bills', data);
        return response.data;
    },

    pay: async (id: number): Promise<void> => {
        await client.patch(`/bills/${id}/pay`);
    }
};

// Notifications API
export const notificationsApi = {
    getAll: async (userId?: number): Promise<Notification[]> => {
        const params = userId ? { userId } : {};
        const response = await client.get<Notification[]>('/notifications', { params });
        return response.data;
    },

    markAsRead: async (id: number): Promise<void> => {
        await client.patch(`/notifications/${id}/read`);
    },

    markAllAsRead: async (userId: number): Promise<void> => {
        await client.patch(`/notifications/read-all?userId=${userId}`);
    }
};

// Archive API
export const archiveApi = {
    getAll: async (restaurantId?: number): Promise<ArchivedOrder[]> => {
        const params = restaurantId ? { restaurantId } : {};
        const response = await client.get<ArchivedOrder[]>('/archive', { params });
        return response.data;
    },

    archive: async (orderId: number): Promise<ArchivedOrder> => {
        const response = await client.post<ArchivedOrder>(`/archive/${orderId}`);
        return response.data;
    },

    getStatistics: async (restaurantId?: number): Promise<Statistics> => {
        const params = restaurantId ? { restaurantId } : {};
        const response = await client.get<Statistics>('/archive/statistics', { params });
        return response.data;
    }
};

// Users API
export const usersApi = {
    getAll: async (): Promise<User[]> => {
        const response = await client.get<User[]>('/users');
        return response.data;
    },

    getById: async (id: number): Promise<User> => {
        const response = await client.get<User>(`/users/${id}`);
        return response.data;
    },

    create: async (data: CreateUserDto): Promise<User> => {
        const response = await client.post<User>('/users', data);
        return response.data;
    },

    delete: async (id: number): Promise<void> => {
        await client.delete(`/users/${id}`);
    }
};
