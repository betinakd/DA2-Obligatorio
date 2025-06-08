import { SimClassResponse } from './SimClassResponse';

export interface Namespace {
  classes: any;
  id: string;
  name: string;
  baseNamespaceId: string | null;
  elements: SimClassResponse[];
}