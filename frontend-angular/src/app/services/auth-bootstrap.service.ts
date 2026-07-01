import { Injectable } from '@angular/core';
import { catchError, map, Observable, of, shareReplay, tap } from 'rxjs';
import { AuthService } from '../pages/auth/api/auth.service';
import { AuthStateService } from './auth-state.service';

@Injectable({ providedIn: 'root' })
export class AuthBootstrapService {
  private bootstrap$?: Observable<boolean>;
  private initialized = false;

  constructor(
    private authService: AuthService,
    private authState: AuthStateService
  ) {}

  ensureSession(): Observable<boolean> {
    if (this.initialized) {
      return of(this.authState.isAuthenticated);
    }

    if (this.bootstrap$) {
      return this.bootstrap$;
    }

    if (this.authState.isAuthenticated) {
      this.initialized = true;
      return of(true);
    }

    this.bootstrap$ = this.authService.refreshToken().pipe(
      tap((response) => this.authState.setAuth(response)),
      map(() => true),
      catchError(() => {
        this.authState.clearAuth();
        return of(false);
      }),
      tap(() => {
        this.initialized = true;
      }),
      shareReplay(1)
    );

    return this.bootstrap$;
  }
}
