export interface LoginCredentials {
    usernameOrEmail: string;
    password: string;
}

export interface RegisterCredentials {
    username: string;
    email: string;
    password: string;
    firstName: string;
    lastName: string;
    phoneNumber?: string;
    restaurantId?: number;
}
