import { Component, EventEmitter, Output } from '@angular/core';
import { ParameterRequest } from '../../models/request/ParameterRequest.model';
import { ClassSelectorComponent } from '../class-selector/class-selector.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-parameter-list',
  imports: [ClassSelectorComponent, FormsModule, CommonModule, MatFormFieldModule],
  standalone: true,
  templateUrl: './parameter-list.component.html',
  styleUrl: './parameter-list.component.scss'
})
export class ParameterListComponent {
  @Output() parametersChange = new EventEmitter<ParameterRequest[]>();
  parameters: ParameterRequest[] = [];

  addParameter(): void {
    const newParameter: ParameterRequest = {
      name: '',
      idReference: '',
    };
    this.parameters.push(newParameter);
    this.parametersChange.emit(this.parameters);
  }

  removeParameter(i: number): void {
    this.parameters.splice(i, 1);
    this.parametersChange.emit(this.parameters);
  }

  onTypeSelected(value: string, i: number): void {
    this.parameters[i].idReference = value;
    this.parametersChange.emit(this.parameters);
  }
}
