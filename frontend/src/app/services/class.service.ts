import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { API_ENDPOINTS } from '../shared/constants/api-endpoints';
import { CreatedSimClassResponse } from '../models/CreatedSimClassResponse.model';
import { SimClassResponse } from '../models/SimClassResponse';
import { SimClassRequestUpdate } from '../models/request/SimClassRequestUpdate';

@Injectable({
  providedIn: 'root'
})
export class ClassService {
  private apiUrl = API_ENDPOINTS.CLASSES;

  constructor(private http: HttpClient) { }

  getAllClasses(): Observable<SimClassResponse[]> {
    return this.http.get<SimClassResponse[]>(this.apiUrl);
  }

  getClass(id: string): Observable<SimClassResponse> {
    return this.http.get<SimClassResponse>(`${this.apiUrl}/${id}`);
  }

  createClass(classData: any): Observable<CreatedSimClassResponse> {
    return this.http.post<CreatedSimClassResponse>(this.apiUrl, classData);
  }

  updateClass(classData: any): Observable<CreatedSimClassResponse> {
    return this.http.put<CreatedSimClassResponse>(`${this.apiUrl}`, classData);
  }

  deleteClass(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getAllClassesForPatterns(): Observable<SimClassResponse[]> {
    return this.http.get<SimClassResponse[]>(this.apiUrl);
  }
}