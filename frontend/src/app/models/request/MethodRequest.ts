import { ParameterRequest } from './ParameterRequest.model';

export class MethodRequest {
  name: string = '';
  privacity: string = '';
  accesibility: string = '';
  idReturnType: string = '';
  isStatic: boolean = false;
  isVirtual: boolean = false;
  isOverride: boolean = false;
  parameters: ParameterRequest[] = [];
}