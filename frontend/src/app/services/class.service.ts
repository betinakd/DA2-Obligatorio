import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SimClass } from '../models/SimClass.model';
import { API_ENDPOINTS } from '../shared/constants/api-endpoints';
import { CreatedSimClassResponse } from '../models/CreatedSimClassResponse.model';

@Injectable({
  providedIn: 'root'
})
export class ClassService {
  private apiUrl = API_ENDPOINTS.CLASSES;

  constructor(private http: HttpClient) {}

  getAllClasses(): Observable<SimClass[]> {
    return this.http.get<SimClass[]>(this.apiUrl);
  }
  
  getClass(id: string): Observable<SimClass> {
    return this.http.get<SimClass>(`${this.apiUrl}/${id}`);
  }

  createClass(classData: any): Observable<CreatedSimClassResponse> {
    return this.http.post<CreatedSimClassResponse>(this.apiUrl, classData);
  }

  updateClass(id: string, classData: any): Observable<SimClass> {
    return this.http.put<SimClass>(`${this.apiUrl}/${id}`, classData);
  }

  deleteClass(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}