import { API_URL } from '../../../services-conf/api-config';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthResponse, LoginRequest, RegisterRequest } from '../types/auth.types';

@Injectable({ providedIn: 'root' })
export class AuthService {
    constructor(private http: HttpClient) { }

    register(payload: RegisterRequest): Observable<AuthResponse> {
        return this.http.post<AuthResponse>(`${API_URL}/auth/register`, payload);
    }

    login(payload: LoginRequest): Observable<AuthResponse> {
        return this.http.post<AuthResponse>(`${API_URL}/auth/login`, payload);
    }
}
