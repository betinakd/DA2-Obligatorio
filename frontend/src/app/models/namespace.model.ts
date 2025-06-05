import { SimClassResponse } from './SimClassResponse';

export interface Namespace {
  id: string;
  name: string;
  baseNamespaceId: string | null;
  elements: SimClassResponse[];
}