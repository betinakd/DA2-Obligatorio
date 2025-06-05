export class AttributeRequest {
  id: string = '';
  name: string = '';
  referenceId: string = '';
  instanceId: string = '';
  privacity: string = 'Public';
  relatedClassId: string = '';
  isStatic: boolean = false;

  toString(): string {
    return `AttributeRequest(id=${this.id}, name=${this.name}, referenceId=${this.referenceId}, instanceId=${this.instanceId}, privacity=${this.privacity}, relatedClassId=${this.relatedClassId}, isStatic=${this.isStatic})`;
  }
}