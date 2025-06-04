import { ParameterRequest } from './ParameterRequest.model';
import { Variable } from './variable.model';
import { Invocation } from './invocation.model';

export interface Method {
  id?: string;
  name?: string;
  idClassOwner?: string;
  relatedClassId?: string;
  returnTypeId?: string;
  privacity?: string;
  accesibility?: string;
  isStatic?: boolean;
  isVirtual?: boolean;
  isOverride?: boolean;
  parameters?: any[];
  localVariables?: any[];
  invocations?: any[];
}