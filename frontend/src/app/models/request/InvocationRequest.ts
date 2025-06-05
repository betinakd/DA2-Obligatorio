import { ParameterSignatureRequest } from './ParameterSignatureRequest';


export class InvocationRequest {
  idReference: string = '';
  methodName: string = '';
  parameters: ParameterSignatureRequest[] = [];
  typeReference: string = ''; // 'Base' | 'This' | 'LocalVariable' | 'Attribute' | 'Parameter'
  idReturnType: string = '';
}