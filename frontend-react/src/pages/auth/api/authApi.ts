import api from '../../../axios/axiosConf';
import type { RegisterRequest, LoginRequest, AuthResponse } from '../types/auth.types';

export async function registerUser(payload: RegisterRequest): Promise<AuthResponse> {
    const { data } = await api.post<AuthResponse>('/auth/register', payload);
    return data;
}

export async function loginUser(payload: LoginRequest): Promise<AuthResponse> {
    const { data } = await api.post<AuthResponse>('/auth/login', payload);
    return data;
}