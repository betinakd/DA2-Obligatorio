import { MethodResponse } from './method-response.model';

export interface InterfaceResponse {
  id?: string;
  name?: string;
  methods: MethodResponse[];
}