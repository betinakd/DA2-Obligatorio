import { SimClassResponse } from './SimClassResponse';

export interface NamespaceResponse {
  id: string;
  name: string;
  baseNamespaceId: string;
  elements: SimClassResponse[];
}