import { Method } from './method.model';
import { Attribute } from './attribute.model';
import { Namespace } from './namespace.model';
import { Interface } from './Interface';

export interface SimClass {
  id: string;
  name: string;
  idBaseClass: string | null;
  state: string;
  methods: Method[];
  attributes: Attribute[];
  implements: Interface[];
  idBaseNamespace: string | null;
}