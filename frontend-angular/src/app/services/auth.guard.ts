import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { Observable, map, take } from 'rxjs';
import { AuthBootstrapService } from './auth-bootstrap.service';
import { AuthStateService } from './auth-state.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(
    private authBootstrap: AuthBootstrapService,
    private authState: AuthStateService,
    private router: Router
  ) {}

  canActivate(): Observable<boolean | UrlTree> {
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
    private authBootstrap: AuthBootstrapService,
    private authState: AuthStateService,
    private router: Router
  ) {}

  canActivate(): Observable<boolean | UrlTree> {
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