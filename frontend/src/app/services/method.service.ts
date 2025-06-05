import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../shared/constants/api-endpoints';
import { MethodRequest } from '../models/MethodRequest';
import { MethodCreatedResponse } from '../models/MethodCreatedResponse';


@Injectable({
  providedIn: 'root'
})
export class MethodService {
  private apiUrl = API_ENDPOINTS.METHODS;

  constructor(private http: HttpClient) { }

  getMethod(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  createMethod(classId: string, method: MethodRequest): Observable<MethodCreatedResponse> {
    return this.http.post<MethodCreatedResponse>(`${API_ENDPOINTS.CLASSES}/${classId}/methods`, method);
  }

  deleteMethod(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  createParameter(methodId: string, parameter: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${methodId}/parameters`, parameter);
  }

  createVariable(methodId: string, variable: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${methodId}/variables`, variable);
  }

  createInvocation(methodId: string, invocation: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${methodId}/invocations`, invocation);
  }

  executeMethod(executionRequest: any): Observable<any> {
    return this.http.post<any>(`${API_ENDPOINTS.EXECUTIONS}`, executionRequest);
  }
}