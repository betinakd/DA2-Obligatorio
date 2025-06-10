import { ParameterResponse } from './parameter-response.model';
import { InvocationResponse } from './invocation-response.model';
import { VariableResponse } from './VariableResponse';

export interface MethodResponse {
    id?: string;
    name?: string;
    idClassOwner?: string;
    privacity: string;
    accesibility: string;
    returnTypeId?: string;
    parameters: ParameterResponse[];
    variables: VariableResponse[];
    invocations: InvocationResponse[];
    isStatic: boolean;
    isVirtual: boolean;
    isOverride: boolean;
}