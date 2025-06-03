import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { HttpEvent, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpHandler, HttpRequest } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';

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
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([errorInterceptor]))
  ]
};
