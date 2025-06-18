export class AttributeRequest {
  id: string = '';
  name: string = '';
  idReference: string = '';
  idInstance: string = '';
  privacity: string = 'Public';
  idRelatedClass: string = '';
  isStatic: boolean = false;

  toString(): string {
    return `AttributeRequest(id=${this.id}, name=${this.name}, referenceId=${this.idReference}, instanceId=${this.idInstance}, privacity=${this.privacity}, relatedClassId=${this.idRelatedClass}, isStatic=${this.isStatic})`;
  }
}