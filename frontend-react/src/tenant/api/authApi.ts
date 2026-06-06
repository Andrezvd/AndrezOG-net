import api from '../../axios/axiosConf'
import type { AuthResponse, RegisterRequest } from '../types/authTypes'

export async function registerUser(payload: RegisterRequest): Promise<AuthResponse> {
	const response = await api.post<AuthResponse>('/auth/register', payload)
	return response.data
}