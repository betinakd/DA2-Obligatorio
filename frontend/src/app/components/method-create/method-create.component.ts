import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ParameterRequest } from '../../models/ParameterRequest.model';
import { ParameterListComponent } from '../parameter-list/parameter-list.component';
import { MethodRequest } from '../../models/MethodRequest';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCheckboxChange, MatCheckboxModule } from '@angular/material/checkbox';
import { ClassSelectorComponent } from '../class-selector/class-selector.component';
import { PrivacitySelectorComponent } from '../privacity-selector/privacity-selector.component';
import { ClassTypeSelectorComponent } from '../accesibility-selector/accesibility-selector.component';

@Component({
  selector: 'app-method-create',
  standalone: true,
  imports: [ParameterListComponent, CommonModule,
    FormsModule,
    MatCheckboxModule,
    ClassSelectorComponent,
    PrivacitySelectorComponent, ClassTypeSelectorComponent],
  templateUrl: './method-create.component.html',
  styleUrl: './method-create.component.scss'
})
export class MethodCreateComponent {
  @Input() method: MethodRequest = new MethodRequest();
  @Output() methodChange = new EventEmitter<MethodRequest>();
  methodName: string = '';
  parameters: ParameterRequest[] = [];

  onParametersChange(value: ParameterRequest[]) {
    this.parameters = value;
    this.methodChange.emit(this.method);
  }

  onPrivacitySelected(value: string): void {
    this.method.privacity = value;
    this.methodChange.emit(this.method);
  }

  onAccessibilitySelected(value: string): void {
    this.method.accesibility = value;
    this.methodChange.emit(this.method);
  }

  onReturnTypeSelected(value: string): void {
    this.method.idReturnType = value;
    this.methodChange.emit(this.method);
  }

  onStaticChanged(event: MatCheckboxChange): void {
    this.method.isStatic = event.checked;
    this.methodChange.emit(this.method);
  }

  onVirtualChanged(event: MatCheckboxChange): void {
    this.method.isVirtual = event.checked;
    this.methodChange.emit(this.method);
  }

  onOverrideChanged(event: MatCheckboxChange): void {
    this.method.isOverride = event.checked;
    this.methodChange.emit(this.method);
  }
}
