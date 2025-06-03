import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, HttpRequest, HttpEvent } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { routes } from './app.routes';

export function errorInterceptor(req: HttpRequest<unknown>, next: (req: HttpRequest<unknown>) => Observable<HttpEvent<unknown>>): Observable<HttpEvent<unknown>> {
  return next(req).pipe(
    catchError(error => {
      console.error('Error interceptado:', error);
      return throwError(() => error);
    })
  );
}

export const appConfig: ApplicationConfig = {
  providers: [provideZoneChangeDetection({ eventCoalescing: true }), 
    provideRouter(routes),
    provideHttpClient()]
};
