import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ParameterSignatureRequest } from '../../models/request/ParameterSignatureRequest';
import { ClassSelectorComponent } from '../class-selector/class-selector.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-parameter-signature-list',
  imports: [ClassSelectorComponent, FormsModule, CommonModule, MatFormFieldModule],
  standalone: true,
  templateUrl: './parameter-signature-list.component.html',
  styleUrl: './parameter-signature-list.component.scss'
})
export class ParameterSignatureListComponent {
  @Output() parametersChange = new EventEmitter<ParameterSignatureRequest[]>();
  @Input() parameters: ParameterSignatureRequest[] = [];

  constructor() { }

  addParameter(): void {
    const newParameter: ParameterSignatureRequest = {
      name: '',
      idReference: '',
      idInstance: ''
    };
    this.parameters.push(newParameter);
    this.parametersChange.emit(this.parameters);
  }

  removeParameter(i: number): void {
    this.parameters.splice(i, 1);
    this.parametersChange.emit(this.parameters);
  }

  onReferenceTypeSelected(value: string, i: number): void {
    this.parameters[i].idReference = value;
    this.parametersChange.emit(this.parameters);
  }

  onInstanceTypeSelected(value: string, i: number): void {
    this.parameters[i].idInstance = value;
    this.parametersChange.emit(this.parameters);
  }
}