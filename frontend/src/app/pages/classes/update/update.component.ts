import { Component } from '@angular/core';
import { AttributeListComponent } from '../../../components/attribute-list/attribute-list.component';
import { Attribute } from '../../../models/attribute.model';
import { AttributeRequest } from '../../../models/request/AttributeRequest';
import { ClassSelectorComponent } from '../../../components/class-selector/class-selector.component';
import { MethodRequest } from '../../../models/request/MethodRequest';
import { NamespaceSelectorComponent } from '../../../components/namespace-selector/namespace-selector.component';
import { ClassTypeSelectorComponent } from '../../../components/accesibility-selector/accesibility-selector.component';
import { ImplementsListComponent } from '../../../components/implements-list/implements-list.component';
import { Interface } from '../../../models/Interface';
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

  method: MethodRequest = new MethodRequest();

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

  onAttributesUpdated(attributes: AttributeRequest[]): void {
    this.attributes = attributes;
  }

  onInterfacesUpdated(value: Interface[]): void {
    this.implements = value;
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
      methods: this.methods
    };

    this.classService.updateClass(dataToSend).subscribe({
      next: (response) => {
        this.success = `Class updated successfully!\n\n${JSON.stringify(response.simClass, null, 2)}`;
        this.loading = false;
        this.error = '';
      },
      error: (err) => {
        this.error = 'Error: ' + (err.error?.message || 'Invalid inputs. Please check the form and try again.');
        this.loading = false;
        this.success = '';
      }
    });
  }

  onChangeMethod(event: MethodRequest): void {
    this.method = event;
  }
}