import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { VariableService } from '../../../services/variable.service';
import { Variable } from '../../../models/variable.model';
import { ClassSelectorComponent } from '../../../components/class-selector/class-selector.component';
import { VariableRequest } from '../../../models/variable-request.model';

@Component({
  selector: 'app-create',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, ClassSelectorComponent],
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss'
})
export class CreateComponent implements OnInit {
  variable: Variable = {};
  varName = '';
  methodId = '';
  referenceClassId = '';
  instanceClassId = '';
  isLoading = false;
  error = '';
  successMessage = '';


  constructor(
    private variableService: VariableService,
  ) {}

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
        this.successMessage = 'Variable creada exitosamente';
        this.error = '';

      },
      error: (err) => {
        this.isLoading = false;
        this.error = 'Error: ' + (err.error?.message || err.message || 'Unknown error');
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
}