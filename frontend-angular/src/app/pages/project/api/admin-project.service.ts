import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../../../services-conf/api-config';
import { AdminProjectDto } from '../types/admin-project.types';

@Injectable({ providedIn: 'root' })
export class AdminProjectService {
    constructor(private http: HttpClient) { }

    getAll(): Observable<AdminProjectDto[]> {
        return this.http.get<AdminProjectDto[]>(`${API_URL}/project`);
    }

    getById(id: number): Observable<AdminProjectDto> {
        return this.http.get<AdminProjectDto>(`${API_URL}/project/${id}`);
    }

    create(formData: FormData): Observable<AdminProjectDto> {
        return this.http.post<AdminProjectDto>(`${API_URL}/project`, formData);
    }

    update(id: number, formData: FormData): Observable<AdminProjectDto> {
        return this.http.patch<AdminProjectDto>(`${API_URL}/project/${id}`, formData);
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${API_URL}/project/${id}`);
    }
}