import { client } from './client';
import type { AuthResponse } from '@/types/user';
import type { LoginCredentials, RegisterCredentials } from '@/types/auth';

export const authApi = {
    login: async (credentials: LoginCredentials): Promise<AuthResponse> => {
        const response = await client.post<AuthResponse>('/auth/login', credentials);
        return response.data;
    },

    register: async (data: RegisterCredentials): Promise<AuthResponse> => {
        const response = await client.post<AuthResponse>('/auth/register', data);
        return response.data;
    },

    logout: async (): Promise<void> => {
        const refreshToken = localStorage.getItem('refreshToken');
        if (refreshToken) {
            await client.post('/auth/logout', { refreshToken });
        }
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
    },
};
