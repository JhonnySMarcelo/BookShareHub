import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';

import { catchError, throwError } from 'rxjs';

import { NotificationService } from '../services/notification.service';
import { AuthService } from '../services/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const notification = inject(NotificationService);

  return next(req).pipe(
    catchError((error) => {
      console.error('[HTTP ERROR]', {
        method: req.method,
        url: req.url,
        status: error.status,
        error,
      });

      if (error.status === 401 && router.url !== '/login') {
        authService.logout();
        notification.error('Your session has expired. Please sign in again.');
        router.navigate(['/login']);
      } else if (error.status === 0) {
        notification.error('Unable to connect to the server.');
      } else if (error.status >= 500) {
        notification.error('Internal server error.');
      } else {
        notification.error(error.error?.message ?? 'Unexpected error.');
      }

      return throwError(() => error);
    }),
  );
};
