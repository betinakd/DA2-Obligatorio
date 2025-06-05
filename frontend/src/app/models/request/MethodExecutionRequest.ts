import { ParameterSignatureRequest } from './ParameterSignatureRequest';

export class MethodExecutionRequest {
    methodName: string = '';
    parameters: ParameterSignatureRequest[] = [];
    idReferenceType: string = '';
    idInstanceType: string = '';
    idReturnType: string = '';

    toString(): string {
        return `MethodExecutionRequest(methodName=${this.methodName}, 
    parameters=${this.parameters.length}, idReferenceType=${this.idReferenceType}, 
    idInstanceType=${this.idInstanceType}, idReturnType=${this.idReturnType})`;
    }
}