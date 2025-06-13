import { MethodRequest } from './MethodRequest';

export interface ImplementRequest {
    idInterface: string;
    methods: MethodRequest[];
}