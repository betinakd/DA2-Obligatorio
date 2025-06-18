import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
import { VariableRequest } from '../models/request/VariableRequest.model';
import { VariableResponse } from '../models/response/VariableResponse';

@Injectable({
  providedIn: 'root'
})
export class VariableService {
  private apiUrl = 'http://localhost:5128/api/v1';

  constructor(private http: HttpClient) { }

  getVariable(id: string): Observable<VariableResponse> {
    return this.http.get<VariableResponse>(`${this.apiUrl}/variables/${id}`);
  }

  createVariable(methodId: string, variable: VariableRequest): Observable<any> {
    console.log('Enviando:', variable);
    return this.http.post<any>(`${this.apiUrl}/methods/${methodId}/variables`, variable)
      .pipe(
        catchError(error => {
          console.error('Error detallado:', error);
          return throwError(() => error);
        })
      );
  }
}