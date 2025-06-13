import { Component } from '@angular/core';
import { ClassSelectorComponent } from '../../components/class-selector/class-selector.component';
import { ImplementRequest } from '../../models/request/ImplementRequest';
import { MethodListComponent } from '../../components/method-list/method-list.component';
import { MethodRequest } from '../../models/request/MethodRequest';
import { ImplementsService } from '../../services/implements.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-implements',
  standalone: true,
  imports: [ClassSelectorComponent, MethodListComponent, CommonModule],
  templateUrl: './implements.component.html',
  styleUrl: './implements.component.scss'
})
export class ImplementsComponent {

  constructor(private implementService: ImplementsService) { }

  reload = false;
  classId = '';
  interfaceRequest: ImplementRequest = {
    idInterface: '',
    methods: []
  };
  error = '';
  success = '';

  interfaceSelected(id: string): void {
    this.interfaceRequest.idInterface = id;
  }

  methodsSelected(methods: MethodRequest[]): void {
    this.interfaceRequest.methods = methods;
  }

  classImplements(id: string): void {
    this.classId = id;
  }

  onCreateImplement(): void {
    this.implementService.implementInterface(this.classId, this.interfaceRequest).subscribe({
      next: (response) => {
        const simClass = response.simClass;
        this.success =
          `Implementación exitosa:
            Nombre: ${simClass.name}
            ID: ${simClass.id}
            Estado: ${simClass.state}
            Métodos: ${simClass.methods?.length ?? 0}
            Atributos: ${simClass.attributes?.length ?? 0}
            Interfaces implementadas: ${simClass.implements?.map((i: any) => i.name).join(', ') || 'Ninguna'}
            Namespace: ${simClass.namespace?.name ?? 'Sin namespace'}`;
        this.reload = true;
        this.error = '';
      },
      error: (err) => {
        this.error = 'Error: ' + (err.error?.message || err.message || 'Unknown error');
        this.reload = false;
        this.success = '';
      }
    });
  }
}
