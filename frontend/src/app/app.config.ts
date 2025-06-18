import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors, HttpErrorResponse } from '@angular/common/http';
import { routes } from './app.routes';
import { catchError, throwError } from 'rxjs';

function errorInterceptor(req: any, next: any) {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 0) {
        console.error('Error de conexión. Verifica que el backend esté ejecutándose.');
      } else if (error.status === 404) {
        console.error(`Endpoint no encontrado: ${req.url}`);
      } else if (error.status === 500) {
        console.error('Error del servidor:', error.error);
      }
      return throwError(() => error);
    })
  );
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([errorInterceptor]))
  ]
};
