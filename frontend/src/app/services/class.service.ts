import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { SimClass } from '../models/SimClass.model';
import { API_ENDPOINTS } from '../shared/constants/api-endpoints';
import { CreatedSimClassResponse } from '../models/CreatedSimClassResponse.model';

@Injectable({
  providedIn: 'root'
})
export class ClassService {
  private apiUrl = API_ENDPOINTS.CLASSES;

  private cachedClasses: SimClass[] = [];

  constructor(private http: HttpClient) { }

  getAllClasses(): Observable<SimClass[]> {
    if (this.cachedClasses.length > 0) {
      return of(this.cachedClasses);
    }

    return this.http.get<any>(`${this.apiUrl}/simulator/namespaces`).pipe(
      map(namespaces => {
        let allClasses: SimClass[] = [];
        if (Array.isArray(namespaces)) {
          namespaces.forEach(ns => {
            if (ns.classes && Array.isArray(ns.classes)) {
              allClasses = allClasses.concat(ns.classes);
            }
          });
        }
        this.cachedClasses = allClasses;
        return allClasses;
      }),
      catchError(error => {
        console.error('Error getting classes from namespaces:', error);

        return this.http.get<SimClass[]>(`${this.apiUrl}/classes`).pipe(
          tap(classes => this.cachedClasses = classes),
          catchError(err => {
            console.error('Error getting classes directly:', err);
            return of([]);
          })
        );
      })
    );
  }

  getClass(id: string): Observable<SimClass> {
    return this.http.get<SimClass>(`${this.apiUrl}/${id}`);
  }

  createClass(classData: any): Observable<CreatedSimClassResponse> {
    return this.http.post<CreatedSimClassResponse>(this.apiUrl, classData);
  }

  updateClass(classData: any): Observable<SimClass> {
    return this.http.put<SimClass>(`${this.apiUrl}`, classData);
  }

  deleteClass(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}