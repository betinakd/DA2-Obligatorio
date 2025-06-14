import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../shared/constants/api-endpoints';
import { ImplementRequest } from '../models/request/ImplementRequest';
import { CreatedSimClassResponse } from '../models/response/CreatedSimClassResponse.model';

@Injectable({
    providedIn: 'root'
})
export class ImplementsService {
    private apiUrl = API_ENDPOINTS.CLASSES;

    constructor(private http: HttpClient) { }

    implementInterface(classId: string, implementRequest: ImplementRequest): Observable<CreatedSimClassResponse> {
        return this.http.post<CreatedSimClassResponse>(`${this.apiUrl}/${classId}/implements`, implementRequest);
    }
}