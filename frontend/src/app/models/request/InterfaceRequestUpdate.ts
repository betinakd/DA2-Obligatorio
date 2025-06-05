export class InterfaceRequestUpdate {
  id: string = '';
  name: string = '';
  
  toString(): string {
    return `InterfaceRequestUpdate(id=${this.id}, name=${this.name})`;
  }
}