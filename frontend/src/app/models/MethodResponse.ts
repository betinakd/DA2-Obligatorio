import { InvocationResponse } from "./invocation-response.model";
import { ParameterRequest } from "./request/ParameterRequest.model";
import { VariableResponse } from "./VariableResponse";


export class MethodResponse {
  name: string = '';
  privacity: string = '';
  accesibility: string = '';
  returnTypeId: string = '';
  isStatic: boolean = false;
  isVirtual: boolean = false;
  isOverride: boolean = false;
  parameters: ParameterRequest[] = [];
  invocations: InvocationResponse[] = [];
  variables: VariableResponse[] = [];
}