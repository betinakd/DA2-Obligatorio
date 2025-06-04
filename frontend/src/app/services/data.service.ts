import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of, tap, catchError } from 'rxjs';
import { Namespace } from '../models/namespace.model';
import { SimClass } from '../models/SimClass.model';
import { Method } from '../models/method.model';
import { API_ENDPOINTS } from '../shared/constants/api-endpoints';

@Injectable({
    providedIn: 'root'
})
export class DataService {
    private apiUrl = API_ENDPOINTS.BASE_URL;

    private namespacesData: Namespace[] = [];
    private classesMap = new Map<string, SimClass>();
    private methodsMap = new Map<string, Method[]>();

    private dataLoadedSubject = new BehaviorSubject<boolean>(false);
    public dataLoaded$ = this.dataLoadedSubject.asObservable();

    constructor(private http: HttpClient) { }

    loadAllData(): Observable<any> {
        console.log('Cargando todos los datos...');

        if (this.namespacesData.length > 0) {
            console.log('Usando datos en caché');
            return of({ success: true });
        }

        return this.http.get<Namespace[]>(`${this.apiUrl}/namespaces`).pipe(
            tap(namespaces => {
                console.log('Namespaces cargados:', namespaces);
                this.processNamespacesData(namespaces);
            }),
            catchError(error => {
                console.error('Error cargando namespaces, intentando ruta alternativa:', error);

                return this.http.get<Namespace[]>(`${this.apiUrl}/simulator/namespaces`).pipe(
                    tap(namespaces => {
                        console.log('Namespaces cargados (ruta alternativa):', namespaces);
                        this.processNamespacesData(namespaces);
                    }),
                    catchError(err => {
                        console.error('No se pudieron cargar los namespaces:', err);
                        return of({ success: false });
                    })
                );
            })
        );
    }

    private processNamespacesData(namespaces: Namespace[]): void {
        this.namespacesData = namespaces;

        namespaces.forEach(namespace => {
            if (namespace.elements && Array.isArray(namespace.elements)) {
                namespace.elements.forEach(simClass => {
                    this.classesMap.set(simClass.id, simClass);

                    if (simClass.methods && Array.isArray(simClass.methods)) {
                        this.methodsMap.set(simClass.id, simClass.methods);
                        console.log(`Clase ${simClass.name} tiene ${simClass.methods.length} métodos`);
                    }
                });
            }
        });

        this.dataLoadedSubject.next(true);
        console.log(`Datos procesados: ${this.classesMap.size} clases y métodos para ${this.methodsMap.size} clases`);
    }

    getNamespaces(): Namespace[] {
        return this.namespacesData;
    }

    getAllClasses(): SimClass[] {
        return Array.from(this.classesMap.values());
    }

    getClassById(classId: string): SimClass | undefined {
        return this.classesMap.get(classId);
    }

    getMethodsByClassId(classId: string): Method[] {
        if (!classId) return [];

        const methods = this.methodsMap.get(classId);
        if (methods) {
            console.log(`Encontrados ${methods.length} métodos para clase ${classId}`);
            return methods;
        }

        const classData = this.classesMap.get(classId);
        if (classData && classData.methods && Array.isArray(classData.methods)) {
            console.log(`Encontrados ${classData.methods.length} métodos para clase ${classId} (desde objeto de clase)`);
            return classData.methods;
        }

        console.log(`No se encontraron métodos para clase ${classId}`);
        return [];
    }
}