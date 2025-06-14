import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../shared/constants/api-endpoints';
import { MethodRequest } from '../models/request/MethodRequest';
import { MethodCreatedResponse } from '../models/response/MethodCreatedResponse';
import { InvocationRequest } from '../models/request/InvocationRequest';
import { InvocationResponse } from '../models/response/InvocationResponse.model';
import { CreatedInvocationResponse } from '../models/response/CreatedInvocationResponse.model';
import { ParameterRequest } from '../models/request/ParameterRequest.model';


@Injectable({
  providedIn: 'root'
})
export class MethodService {
  private apiUrl = API_ENDPOINTS.METHODS;

  constructor(private http: HttpClient) { }


  createMethod(classId: string, method: MethodRequest): Observable<MethodCreatedResponse> {
    return this.http.post<MethodCreatedResponse>(`${API_ENDPOINTS.CLASSES}/${classId}/methods`, method);
  }

  deleteMethod(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  createParameter(methodId: string, parameter: any): Observable<ParameterRequest> {
    return this.http.post<ParameterRequest>(`${this.apiUrl}/${methodId}/parameters`, parameter);
  }

  createVariable(methodId: string, variable: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${methodId}/variables`, variable);
  }

  createInvocation(methodId: string, invocation: InvocationRequest): Observable<CreatedInvocationResponse> {
    return this.http.post<CreatedInvocationResponse>(`${this.apiUrl}/${methodId}/invocations`, invocation);
  }
}