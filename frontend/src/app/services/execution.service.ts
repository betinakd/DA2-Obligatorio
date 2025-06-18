import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../shared/constants/api-endpoints';
import { MethodExecutionRequest } from '../models/request/MethodExecutionRequest';

@Injectable({
    providedIn: 'root'
})
export class ExecutionService {
    constructor(private http: HttpClient) { }

    executeMethod(request: MethodExecutionRequest): Observable<any> {
        return this.http.post<any>(
            API_ENDPOINTS.EXECUTIONS,
            request
        );
    }
}