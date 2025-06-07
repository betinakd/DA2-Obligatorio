import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../shared/constants/api-endpoints';
import { MethodExecutionRequest } from '../models/request/MethodExecutionRequest';

@Injectable({
  providedIn: 'root'
})
export class TransformersService {

  private apiUrl = API_ENDPOINTS.TRANSFORMERS;

  constructor(private http: HttpClient) { }

  getTransformers(): Observable<string[]> {
    return this.http.get<string[]>(this.apiUrl);
  }

  executeWithTransform(execution: MethodExecutionRequest, transformerName: string): Observable<any> {
    const request = {
      execution: execution,
      transformerName: transformerName
    };

    return this.http.post(this.apiUrl, request);
  }
}