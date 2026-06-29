import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthStateService } from '../services/auth-state.service';
import { AuthService } from '../pages/auth/api/auth.service';
import { catchError, switchMap, throwError, of } from 'rxjs';

export const AuthInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
  const authState = inject(AuthStateService);
  const authService = inject(AuthService);

  let clonedReq = req;

  // Adjuntar token de memoria si existe
  const token = authState.token;
  if (token) {
    clonedReq = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${token}`)
    });
  }

  return next(clonedReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // Solo intentar refresh en 401 que no sea del endpoint de refresh/login/register
      if (error.status === 401 && token && !req.url.includes('/auth/refresh') && !req.url.includes('/auth/login') && !req.url.includes('/auth/register') && !req.url.includes('/auth/external')) {
        return authService.refreshToken().pipe(
          switchMap((res) => {
            // Reintentar la petición original con el nuevo token
            const retryReq = req.clone({
              headers: req.headers.set('Authorization', `Bearer ${res.token}`)
            });
            return next(retryReq);
          }),
          catchError(() => {
            authState.clearAuth();
            return throwError(() => error);
          })
        );
      }
      return throwError(() => error);
    })
  );
};