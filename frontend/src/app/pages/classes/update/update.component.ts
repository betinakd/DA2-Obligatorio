import { Component } from '@angular/core';
import { AttributeListComponent } from '../../../components/attribute-list/attribute-list.component';
import { Attribute } from '../../../models/attribute.model';
import { AttributeRequest } from '../../../models/AttributeRequest';
import { ClassSelectorComponent } from '../../../components/class-selector/class-selector.component';
import { MethodRequest } from '../../../models/MethodRequest';
import { NamespaceSelectorComponent } from '../../../components/namespace-selector/namespace-selector.component';
import { ClassTypeSelectorComponent } from '../../../components/accesibility-selector/accesibility-selector.component';
import { ImplementsListComponent } from '../../../components/implements-list/implements-list.component';
import { Interface } from '../../../models/Interface';
import { ParameterRequest } from '../../../models/ParameterRequest.model';
import { MethodListComponent } from '../../../components/method-list/method-list.component';
import { ClassService } from '../../../services/class.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-update',
  standalone: true,
  imports: [CommonModule, AttributeListComponent, ClassSelectorComponent,
    NamespaceSelectorComponent,
    ClassTypeSelectorComponent,
    ImplementsListComponent,
    MethodListComponent, FormsModule],
  templateUrl: './update.component.html',
  styleUrl: './update.component.scss'
})
export class UpdateComponent {

  constructor(private classService: ClassService) { }

  attributes: AttributeRequest[] = [];
  classId: string = '';
  name: string = '';
  baseClassId: string = '';
  namespaceId: string = '';
  state: string = '';
  error: string = '';
  success: string = '';
  loading: boolean = false;
  currentClassId: string = '';
  labelText: string = 'Update Class';
  implements: Interface[] = [];
  methods: MethodRequest[] = [];
  parameterRequest: ParameterRequest[] = [];

  onClassSelected(value: string): void {
    this.classId = value;
  }

  onBaseClassSelected(value: string): void {
    this.baseClassId = value;
  }

  onNamespaceSelected(value: string): void {
    this.namespaceId = value;
  }

  onStateSelected(value: string): void {
    this.state = value;
  }

  onAttributesUpdated(attributes: Attribute[]): void {
    this.attributes = attributes;
  }

  onInterfacesUpdated(value: Interface[]): void {
    this.implements = value;
  }

  onParametersUpdated(value: ParameterRequest[]): void {
    this.parameterRequest = value;
  }

  onMethodsUpdated(value: MethodRequest[]): void {
    this.methods = value;
  }

  updateClass(): void {
    this.loading = true;
    this.error = '';
    this.success = '';

    const dataToSend = {
      id: this.classId,
      name: this.name,
      IdbaseClass: this.baseClassId,
      IdBaseNamespace: this.namespaceId,
      state: this.state,
      attributes: this.attributes,
      implements: this.implements,
      methods: this.methods,
      parameters: this.parameterRequest
    };
    console.log('Sending data:', JSON.stringify(dataToSend));

    this.classService.updateClass(dataToSend).subscribe({
      next: () => {
        this.success = 'Class updated successfully';
        this.loading = false;
        this.error = '';
      },
      error: (err) => {
        this.error = 'Error: ' + (err.error?.message || err.message || 'Unknown error');
        this.loading = false;
        this.success = '';
      }
    });
  }
}