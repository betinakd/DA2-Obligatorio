import { SimClass } from './SimClass.model';

export interface Namespace {
  id: string;
  name: string;
  baseNamespaceId: string | null;
  elements: SimClass[];
}