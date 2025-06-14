import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
import { AttributeRequest } from '../models/request/AttributeRequest';
import { API_ENDPOINTS } from '../shared/constants/api-endpoints';
import { CreatedAttributeResponse } from '../models/response/CreatedAttributeResponse';

@Injectable({
    providedIn: 'root'
})
export class AttributeService {
    private apiUrl = API_ENDPOINTS.BASE;

    constructor(private http: HttpClient) { }

    createAttribute(classId: string, attribute: AttributeRequest): Observable<CreatedAttributeResponse> {
        return this.http.post<CreatedAttributeResponse>(`${this.apiUrl}classes/${classId}/attributes`, attribute)
            .pipe(
                catchError(error => {
                    console.error(error);
                    return throwError(() => error);
                })
            );
    }

    updateAttribute(attribute: AttributeRequest): Observable<any> {
        return this.http.put<any>(`${this.apiUrl}attributes`, attribute)
            .pipe(
                catchError(error => {
                    console.error(error);
                    return throwError(() => error);
                })
            );
    }

    deleteAttribute(attributeId: string): Observable<any> {
        return this.http.delete<any>(`${this.apiUrl}attributes/${attributeId}`)
            .pipe(
                catchError(error => {
                    console.error(error);
                    return throwError(() => error);
                })
            );
    }
}