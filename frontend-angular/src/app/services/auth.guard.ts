import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { AuthStateService } from './auth-state.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(
    private authState: AuthStateService,
    private router: Router
  ) {}

  canActivate(): boolean | UrlTree {
    if (this.authState.isAuthenticated) {
      return true;
    }
    return this.router.createUrlTree(['/login']);
  }
}

@Injectable({ providedIn: 'root' })
export class AdminGuard implements CanActivate {
  constructor(
    private authState: AuthStateService,
    private router: Router
  ) {}

  canActivate(): boolean | UrlTree {
    if (this.authState.isAuthenticated && this.authState.role === 'Admin') {
      return true;
    }
    if (this.authState.isAuthenticated) {
      return this.router.createUrlTree(['/']);
    }
    return this.router.createUrlTree(['/login']);
  }
}