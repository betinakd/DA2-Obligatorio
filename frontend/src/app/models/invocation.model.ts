export interface Invocation {
  id: string;
  idReference: string;
  idReturnType: string;
  typeReference: string;
  methodName: string;
  parameters: any[]; // Puedes definir una interfaz para estos parámetros
}