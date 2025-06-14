import { InvocationResponse } from "./InvocationResponse.model";
import { ParameterResponse } from "./ParameterResponse.model";
import { VariableResponse } from "./VariableResponse";

export class MethodResponse {
  id: string = '';
  name: string = '';
  idClassOwner: string = '';
  privacity: string = '';
  accesibility: string = '';
  returnTypeId: string = '';
  parameters: ParameterResponse[] = [];
  variables: VariableResponse[] = [];
  invocations: InvocationResponse[] = [];
  isStatic: boolean = false;
  isVirtual: boolean = false;
  isOverride: boolean = false;
}