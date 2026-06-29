import { API_URL } from '../../../services-conf/api-config';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, switchMap } from 'rxjs';
import { AuthResponse, LoginRequest, RegisterRequest, ExternalLoginRequest } from '../types/auth.types';
import { AuthStateService } from '../../../services/auth-state.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(
    private http: HttpClient,
    private authState: AuthStateService
  ) {}

  register(payload: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${API_URL}/auth/register`, payload, {
      withCredentials: true
    }).pipe(
      tap(res => this.authState.setAuth(res))
    );
  }

  login(payload: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${API_URL}/auth/login`, payload, {
      withCredentials: true
    }).pipe(
      tap(res => this.authState.setAuth(res))
    );
  }

  externalLogin(provider: string, idToken: string): Observable<AuthResponse> {
    const payload: ExternalLoginRequest = { provider, idToken };
    return this.http.post<AuthResponse>(`${API_URL}/auth/external`, payload, {
      withCredentials: true
    }).pipe(
      tap(res => this.authState.setAuth(res))
    );
  }

  refreshToken(): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${API_URL}/auth/refresh`, {}, {
      withCredentials: true
    }).pipe(
      tap(res => this.authState.setAuth(res))
    );
  }

  logout(): Observable<any> {
    return this.http.post(`${API_URL}/auth/logout`, {}, {
      withCredentials: true
    }).pipe(
      tap(() => this.authState.clearAuth())
    );
  }
}