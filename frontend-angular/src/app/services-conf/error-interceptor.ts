import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthStateService } from '../services/auth-state.service';

export const ErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const authState = inject(AuthStateService);

  return next(req).pipe(
    catchError((error) => {
      let userMessage = 'Ocurrió un error inesperado. Intenta de nuevo.';

      if (error.status === 0) {
        userMessage = 'No se pudo conectar con el servidor. Verifica tu conexión.';
      } else if (error.status === 429) {
        userMessage = 'Demasiados intentos. Espera un momento antes de continuar.';
      } else if (error.status >= 500) {
        userMessage = 'Error interno del servidor. Estamos trabajando en ello.';
      } else if (error.status === 400 && error.error?.message) {
        // Usar el mensaje del backend si está disponible
        userMessage = error.error.message;
      } else if (error.status === 401) {
        userMessage = 'Sesión expirada. Inicia sesión nuevamente.';
        authState.clearAuth();
      } else if (error.status === 403) {
        userMessage = 'No tienes permiso para realizar esta acción.';
      } else if (error.status === 404) {
        userMessage = 'El recurso solicitado no fue encontrado.';
      }

      error.userMessage = userMessage;

      return throwError(() => error);
    })
  );
};