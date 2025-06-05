import { ParameterSignatureResponse } from './parameter-signature-response.model';

export interface InvocationResponse {
    id: string;
    idReference: string;
    idReturnType: string;
    typeReference?: string;
    methodName?: string;
    parameters?: ParameterSignatureResponse[];
}