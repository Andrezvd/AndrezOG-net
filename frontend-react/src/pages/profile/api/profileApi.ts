import api from '../../../axios/axiosConf';
import type { MyProfileDto } from '../types/profile.types';

export async function getPublicProfile(): Promise<MyProfileDto> {
    const { data } = await api.get<MyProfileDto>('/profile');
    return data;
}