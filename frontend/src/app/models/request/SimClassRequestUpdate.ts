import { MethodRequest } from './MethodRequest';
import { AttributeRequest } from './AttributeRequest';
import { InterfaceRequestUpdate } from './InterfaceRequestUpdate';

export class SimClassRequestUpdate {
    id: string = '';
    name: string = '';
    state: string = 'Normal'; // 'Normal' | 'Abstract' | 'Sealed'
    idBaseClass: string = '11111111-1111-1111-1111-111111111111';
    methods: MethodRequest[] = [];
    attributes: AttributeRequest[] = [];
    implements: InterfaceRequestUpdate[] = [];
    idBaseNamespace: string = '';

    toString(): string {
        return `SimClassRequestUpdate(id=${this.id}, name=${this.name}, state=${this.state}, methods=${this.methods.length}, attributes=${this.attributes.length})`;
    }
}