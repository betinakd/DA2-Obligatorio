import { ParameterSignatureResponse } from './ParameterSignatureResponse.model';

export interface InvocationResponse {
    id: string;
    idReference: string;
    idReturnType: string;
    typeReference?: string;
    methodName?: string;
    parameters?: ParameterSignatureResponse[];
}