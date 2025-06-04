import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, tap, map, shareReplay } from 'rxjs/operators';
import { Method } from '../models/method.model';

@Injectable({
    providedIn: 'root'
})
export class MethodService {
    private apiUrl = 'http://localhost:5128/api/v1';

    private methodsCache = new Map<string, Method[]>();
    private loadingStatus = new Map<string, boolean>();

    private methodsSubject = new BehaviorSubject<Map<string, Method[]>>(new Map());
    public methods$ = this.methodsSubject.asObservable();

    constructor(private http: HttpClient) { }

    
    getMethodsByClassId(classId: string): Observable<Method[]> {
        if (this.methodsCache.has(classId)) {
            console.log(`Usando caché para métodos de clase ${classId}`);
            return of(this.methodsCache.get(classId) || []);
        }

        if (this.loadingStatus.get(classId)) {
            console.log(`Ya se está cargando métodos para clase ${classId}`);
            return of([]);
        }

        console.log(`Cargando métodos para clase: ${classId}`);
        this.loadingStatus.set(classId, true);

        return this.http.get<any>(`${this.apiUrl}/classes/${classId}`).pipe(
            tap(data => console.log('Respuesta recibida:', data)),
            map(response => {
                const methods = this.extractMethodsFromResponse(response);
                console.log(`Se encontraron ${methods.length} métodos`);

                this.methodsCache.set(classId, methods);
                this.methodsSubject.next(this.methodsCache);

                return methods;
            }),
            catchError(error => {
                console.error(`Error obteniendo métodos para clase ${classId}:`, error);
                this.methodsCache.set(classId, []);
                return of([]);
            }),
            tap(() => this.loadingStatus.set(classId, false)),
            shareReplay(1)
        );
    }

    private extractMethodsFromResponse(response: any): Method[] {
        if (response && response.methods && Array.isArray(response.methods)) {
            return response.methods;
        }

        if (response && response.simClass && response.simClass.methods) {
            return response.simClass.methods;
        }

        if (Array.isArray(response)) {
            return response;
        }

        console.warn('Formato de respuesta desconocido, no se pudieron extraer métodos');
        return [];
    }

    clearCache(classId?: string): void {
        if (classId) {
            this.methodsCache.delete(classId);
        } else {
            this.methodsCache.clear();
        }
        this.methodsSubject.next(this.methodsCache);
    }
}