import { ParameterRequest } from "./request/ParameterRequest.model";

export class MethodResponse {
  name: string = '';
  privacity: string = '';
  accesibility: string = '';
  returnTypeId: string = '';
  isStatic: boolean = false;
  isVirtual: boolean = false;
  isOverride: boolean = false;
  parameters: ParameterRequest[] = [];
}