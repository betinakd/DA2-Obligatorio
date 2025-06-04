import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
import { Variable } from '../models/variable.model';
import { VariableRequest } from '../models/variable-request.model';

@Injectable({
  providedIn: 'root'
})
export class VariableService {
  private apiUrl = 'http://localhost:5128/api/v1';

  constructor(private http: HttpClient) { }

  getVariable(id: string): Observable<Variable> {
    return this.http.get<Variable>(`${this.apiUrl}/variables/${id}`);
  }

  createVariable(methodId: string, variable: VariableRequest): Observable<any> {
    console.log('Enviando:', variable); // Para verificar los datos que envías
    return this.http.post<any>(`${this.apiUrl}/methods/${methodId}/variables`, variable)
      .pipe(
        catchError(error => {
          console.error('Error detallado:', error);
          return throwError(() => error);
        })
      );
  }
}