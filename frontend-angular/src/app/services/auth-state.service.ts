import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface AuthState {
  token: string | null;
  userId: number | null;
  email: string | null;
  name: string | null;
  role: string | null;
  isAuthenticated: boolean;
}

const initialState: AuthState = {
  token: null,
  userId: null,
  email: null,
  name: null,
  role: null,
  isAuthenticated: false
};

@Injectable({ providedIn: 'root' })
export class AuthStateService {
  private _state = new BehaviorSubject<AuthState>(initialState);

  get state(): AuthState {
    return this._state.value;
  }

  get state$(): Observable<AuthState> {
    return this._state.asObservable();
  }

  get token(): string | null {
    return this._state.value.token;
  }

  get isAuthenticated(): boolean {
    return this._state.value.isAuthenticated;
  }

  get role(): string | null {
    return this._state.value.role;
  }

  setAuth(response: { token: string; userId: number; email: string; name: string; role: string }): void {
    this._state.next({
      token: response.token,
      userId: response.userId,
      email: response.email,
      name: response.name,
      role: response.role,
      isAuthenticated: true
    });
  }

  clearAuth(): void {
    this._state.next(initialState);
  }

  /**
   * Verifica si el JWT expira pronto (útil para refresh preventivo).
   */
  isTokenExpiringSoon(minutesBeforeExpiry: number = 2): boolean {
    const token = this._state.value.token;
    if (!token) return false;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const exp = payload.exp * 1000;
      return Date.now() > exp - minutesBeforeExpiry * 60 * 1000;
    } catch {
      return true;
    }
  }
}