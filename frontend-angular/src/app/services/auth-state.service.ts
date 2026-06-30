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

const AUTH_STORAGE_KEY = 'andrezog_auth_state';

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

  constructor() {
    this.hydrateFromStorage();
  }

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
    const nextState: AuthState = {
      token: response.token,
      userId: response.userId,
      email: response.email,
      name: response.name,
      role: response.role,
      isAuthenticated: true
    };

    this._state.next(nextState);
    this.saveToStorage(nextState);
  }

  clearAuth(): void {
    this._state.next(initialState);
    this.removeFromStorage();
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

  private hydrateFromStorage(): void {
    if (typeof window === 'undefined') return;

    try {
      const raw = window.localStorage.getItem(AUTH_STORAGE_KEY);
      if (!raw) return;

      const parsed = JSON.parse(raw) as AuthState;
      if (!parsed?.token) {
        this.removeFromStorage();
        return;
      }

      this._state.next(parsed);

      if (this.isTokenExpiringSoon(0)) {
        this.clearAuth();
      }
    } catch {
      this.clearAuth();
    }
  }

  private saveToStorage(state: AuthState): void {
    if (typeof window === 'undefined') return;
    window.localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(state));
  }

  private removeFromStorage(): void {
    if (typeof window === 'undefined') return;
    window.localStorage.removeItem(AUTH_STORAGE_KEY);
  }
}