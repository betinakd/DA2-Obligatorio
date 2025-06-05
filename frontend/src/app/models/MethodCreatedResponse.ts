import { MethodResponse } from "./MethodResponse";

export class MethodCreatedResponse {
  message: string = '';
  methodResponse: MethodResponse = new MethodResponse();
}