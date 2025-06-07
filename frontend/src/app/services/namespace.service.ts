import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Namespace } from '../models/namespace.model';
import { API_ENDPOINTS } from '../shared/constants/api-endpoints';
import { NamespaceRequest } from '../models/request/NamespaceRequest.model';
import { NamespaceResponse } from '../models/NamespaceResponse';
import { CreatedNamespaceResponse } from '../models/CreatedNamespaceResponse';

@Injectable({
  providedIn: 'root'
})
export class NamespaceService {
  constructor(private http: HttpClient) { }

  fetchNamespaces(): Observable<Namespace[]> {
    return this.http.get<Namespace[]>(API_ENDPOINTS.NAMESPACES);
  }

  createNamespace(namespace: NamespaceRequest): Observable<CreatedNamespaceResponse> {
    return this.http.post<CreatedNamespaceResponse>(API_ENDPOINTS.NAMESPACES, namespace);
  }
}