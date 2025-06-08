import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { catchError, map, Observable, throwError } from 'rxjs';
import { API_ENDPOINTS } from '../shared/constants/api-endpoints';
import { MethodExecutionRequest } from '../models/request/MethodExecutionRequest';
import { ErrorResponse } from '../models/ErrorResponse.model';

@Injectable({
  providedIn: 'root'
})
export class TransformersService {

  private apiUrl = API_ENDPOINTS.TRANSFORMERS;

  constructor(private http: HttpClient) { }

  getTransformers(): Observable<string[]> {
    return this.http.get<string[]>(this.apiUrl);
  }

  executeWithTransform(
    execution: MethodExecutionRequest,
    transformerName: string): Observable<any> {
    const request = {
      execution: execution,
      transformerName: transformerName
    };
    const token = localStorage.getItem('authToken') ?? '';

    const headers = new HttpHeaders({
      'Authorization': token
    });

    return this.http.post(this.apiUrl, request, {
      headers: headers,
      responseType: 'text'
    }).pipe(
      map(response => {
        try {
          return JSON.parse(response);
        } catch (e) {
          return response;
        }
      }),
      catchError(httpError => {
        let errorResponse: ErrorResponse;

        try {
          if (typeof httpError.error === 'string') {
            errorResponse = JSON.parse(httpError.error) as ErrorResponse;
          } else {
            errorResponse = httpError.error;
          }
        } catch (e) {
          errorResponse = {
            message: 'Unexpected error format',
            original: httpError.error
          } as any;
        }

        return throwError(() => errorResponse);
      })
    );
  }
}