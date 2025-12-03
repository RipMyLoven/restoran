export interface User {
    id: number;
    username: string;
    email: string;
    firstName: string;
    lastName: string;
    role: string;
    restaurantId?: number;
}

export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
    expiresAt: string;
    user: User;
}
