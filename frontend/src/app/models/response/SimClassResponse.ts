import { MethodResponse } from './MethodResponse';
import { AttributeResponse } from './AttributeResponse';
import { NamespaceResponse } from './NamespaceResponse';
import { InterfaceResponse } from './InterfaceResponse.model';

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