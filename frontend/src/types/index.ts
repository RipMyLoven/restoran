// User types
export interface User {
    id: number;
    username: string;
    role: 'Admin' | 'Waiter' | 'Cook';
    restaurantId?: number;
}

export interface AuthResponse {
    token: string;
    user: User;
}

export interface LoginCredentials {
    username: string;
    password: string;
}

export interface RegisterDto {
    username: string;
    password: string;
    role: 'Admin' | 'Waiter' | 'Cook';
}

export interface CreateUserDto {
    username: string;
    password: string;
    role: 'Admin' | 'Waiter' | 'Cook';
    restaurantId?: number;
}

// Restaurant types
export interface Restaurant {
    id: number;
    name: string;
    tableCount: number;
    allergyTags?: string;
    dietTags?: string;
}

export interface CreateRestaurantDto {
    name: string;
    tableCount: number;
    allergyTags?: string;
    dietTags?: string;
}

export interface UpdateRestaurantDto {
    name: string;
    tableCount: number;
    allergyTags?: string;
    dietTags?: string;
}

// Table types
export interface Table {
    id: number;
    number: number;
    seats: number;
    restaurantId: number;
}

export interface CreateTableDto {
    number: number;
    seats: number;
    restaurantId: number;
}

// MenuItem types
export interface MenuItem {
    id: number;
    name: string;
    price: number;
    restaurantId: number;
}

export interface CreateMenuItemDto {
    name: string;
    price: number;
    restaurantId: number;
}

export interface UpdateMenuItemDto {
    name: string;
    price: number;
}

// Order types
export type OrderStatus = 'New' | 'InProgress' | 'Ready' | 'Completed' | 'Cancelled';

export interface OrderItem {
    id: number;
    menuItemId: number;
    menuItemName?: string;
    quantity: number;
    price: number;
}

export interface Order {
    id: number;
    tableId: number;
    tableNumber?: number;
    status: OrderStatus;
    notes?: string;
    orderItems: OrderItem[];
    total: number;
}

export interface CreateOrderItemDto {
    menuItemId: number;
    quantity: number;
}

export interface CreateOrderDto {
    tableId: number;
    restaurantId: number;
    notes?: string;
    orderItems: CreateOrderItemDto[];
}

export interface UpdateOrderStatusDto {
    status: OrderStatus;
}

// Bill types
export interface Bill {
    id: number;
    orderId: number;
    total: number;
    isPaid: boolean;
    createdAt: string;
    paidAt?: string;
}

export interface CreateBillDto {
    orderId: number;
}

// Notification types
export type NotificationType = 'OrderCreated' | 'OrderReady';

export interface Notification {
    id: number;
    userId: number;
    orderId: number;
    type: NotificationType;
    message: string;
    isRead: boolean;
    createdAt: string;
}

// Archive types
export interface ArchivedOrder {
    id: number;
    originalOrderId: number;
    restaurantId: number;
    tableNumber: number;
    orderItemsJson: string;
    total: number;
    createdAt: string;
    archivedAt: string;
}

export interface Statistics {
    totalOrders: number;
    totalRevenue: number;
    averageOrderValue: number;
}
