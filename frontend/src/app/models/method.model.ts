import { ParameterRequest } from './ParameterRequest.model';
import { Variable } from './variable.model';
import { Invocation } from './invocation.model';

export interface Method {
  id: string;
  name: string;
  idClassOwner: string;
  privacity: string;
  accesibility: string;
  returnTypeId: string;
  parameters: ParameterRequest[];
  variables: Variable[];
  invocations: Invocation[];
  isStatic: boolean;
  isVirtual: boolean;
  isOverride: boolean;
}