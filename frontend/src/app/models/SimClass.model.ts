import { Method } from './method.model';
import { Attribute } from './attribute.model';
import { Namespace } from './namespace.model';

export interface SimClass {
  id: string;
  name: string;
  idBaseClass: string | null;
  state: string;
  methods: Method[];
  attributes: Attribute[];
  implements: any[];
  idBaseNamespace: string | null;
}