import { MethodResponse } from './method-response.model';
import { AttributeResponse } from './attribute-response.model';
import { NamespaceResponse } from './NamespaceResponse';
import { InterfaceResponse } from './interface-response.model';

export interface SimClassResponse {
    id?: string;
    name?: string;
    idBaseClass?: string;
    state: string;
    methods: MethodResponse[];
    attributes: AttributeResponse[];
    implements: InterfaceResponse[];
    namespace?: NamespaceResponse;
}