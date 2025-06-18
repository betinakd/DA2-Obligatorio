import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../shared/constants/api-endpoints';
import { NamespaceRequest } from '../models/request/NamespaceRequest.model';
import { CreatedNamespaceResponse } from '../models/response/CreatedNamespaceResponse';
import { NamespaceResponse } from '../models/response/NamespaceResponse';

@Injectable({
  providedIn: 'root'
})
export class NamespaceService {
  constructor(private http: HttpClient) { }

  fetchNamespaces(): Observable<NamespaceResponse[]> {
    return this.http.get<NamespaceResponse[]>(API_ENDPOINTS.NAMESPACES);
  }

  createNamespace(namespace: NamespaceRequest): Observable<CreatedNamespaceResponse> {
    return this.http.post<CreatedNamespaceResponse>(API_ENDPOINTS.NAMESPACES, namespace);
  }
}