import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { Observable, map, take, of } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';
import { AuthBootstrapService } from './auth-bootstrap.service';
import { AuthStateService } from './auth-state.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(
    @Inject(PLATFORM_ID) private platformId: object,
    private authBootstrap: AuthBootstrapService,
    private authState: AuthStateService,
    private router: Router
  ) {}

  canActivate(): Observable<boolean | UrlTree> {
    // Durante prerendering SSR, no hay sesión - redirigir al login
    if (!isPlatformBrowser(this.platformId)) {
      return of(this.router.createUrlTree(['/login']));
    }

    return this.authBootstrap.ensureSession().pipe(
      take(1),
      map((isAuthenticated) => {
        if (isAuthenticated) {
          return true;
        }
        return this.router.createUrlTree(['/login']);
      })
    );
  }
}

@Injectable({ providedIn: 'root' })
export class AdminGuard implements CanActivate {
  constructor(
    @Inject(PLATFORM_ID) private platformId: object,
    private authBootstrap: AuthBootstrapService,
    private authState: AuthStateService,
    private router: Router
  ) {}

  canActivate(): Observable<boolean | UrlTree> {
    // Durante prerendering SSR, no hay sesión - redirigir al login
    if (!isPlatformBrowser(this.platformId)) {
      return of(this.router.createUrlTree(['/login']));
    }

    return this.authBootstrap.ensureSession().pipe(
      take(1),
      map((isAuthenticated) => {
        if (isAuthenticated && this.authState.role === 'Admin') {
          return true;
        }
        if (isAuthenticated) {
          return this.router.createUrlTree(['/']);
        }
        return this.router.createUrlTree(['/login']);
      })
    );
  }
}
