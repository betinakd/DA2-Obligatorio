export class ParameterSignatureRequest {
  name: string = '';
  idReference: string = '';
  idInstance: string = '';

  toString(): string {
    return `ParameterSignatureRequest(name=${this.name}, idReference=${this.idReference}, 
    idInstance=${this.idInstance})`;
  }
}