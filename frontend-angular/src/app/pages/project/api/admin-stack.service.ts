import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../../../services-conf/api-config';
import { AdminStackDto } from '../types/admin-stack.types';

@Injectable({ providedIn: 'root' })
export class AdminStackService {
    constructor(private http: HttpClient) { }

    getAll(): Observable<AdminStackDto[]> {
        return this.http.get<AdminStackDto[]>(`${API_URL}/stack`);
    }

    getById(id: number): Observable<AdminStackDto> {
        return this.http.get<AdminStackDto>(`${API_URL}/stack/${id}`);
    }

    create(payload: { summary: string; category: string; isActive: boolean; skillIds: number[] }): Observable<AdminStackDto> {
        return this.http.post<AdminStackDto>(`${API_URL}/stack`, payload);
    }

    update(id: number, payload: { summary: string; category: string; isActive: boolean; skillIds: number[] }): Observable<AdminStackDto> {
        return this.http.patch<AdminStackDto>(`${API_URL}/stack/${id}`, payload);
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${API_URL}/stack/${id}`);
    }
}