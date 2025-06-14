import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';

import { VariableService } from '../../../services/variable.service';
import { ClassSelectorComponent } from '../../../components/class-selector/class-selector.component';
import { MethodSelectorComponent } from '../../../components/method-selector/method-selector.component';
import { VariableRequest } from '../../../models/request/VariableRequest.model';
import { VariableResponse } from '../../../models/response/VariableResponse';

@Component({
  selector: 'app-create',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ClassSelectorComponent,
    MethodSelectorComponent
  ],
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss'
})
export class CreateComponent implements OnInit {
  variable: VariableResponse = {};
  varName = '';
  methodId = '';
  referenceClassId = '';
  instanceClassId = '';
  isLoading = false;
  error = '';
  successMessage = '';
  selectedClassForMethods: string = '';


  constructor(private variableService: VariableService) { }

  ngOnInit(): void {

  }


  onSubmit(): void {

    this.isLoading = true;

    const variableData: VariableRequest = {
      name: this.varName,
      idReference: this.referenceClassId,
      idInstance: this.instanceClassId
    };

    this.variableService.createVariable(this.methodId, variableData).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.successMessage = `${response.message}\n\n${JSON.stringify(variableData, null, 2)}`;
        this.error = '';

      },
      error: (err) => {
        this.isLoading = false;
        this.error = 'Error: ' + (err.error?.message || 'Invalid inputs.');
        this.successMessage = '';
      }
    });
  }

  onClassSelectedReference(value: string): void {
    console.log('Clase seleccionada:', value);
    this.referenceClassId = value;
  }

  onClassSelectedInstance(value: string): void {
    console.log('Clase seleccionada:', value);
    this.instanceClassId = value;
  }

  onMethodSelected(value: string): void {
    console.log('Método seleccionado:', value);
    this.methodId = value;
  }
}