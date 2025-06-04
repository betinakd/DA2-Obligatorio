import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';

import { VariableService } from '../../../services/variable.service';
import { Variable } from '../../../models/variable.model';
import { ClassSelectorComponent } from '../../../components/class-selector/class-selector.component';
import { MethodSelectorComponent } from '../../../components/method-selector/method-selector.component';
import { VariableRequest } from '../../../models/variable-request.model';
import { DataService } from '../../../services/data.service';
import { Router } from '@angular/router';

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
  variable: Variable = {};
  varName = '';
  methodId = '';
  referenceClassId = '';
  instanceClassId = '';
  isLoading = false;
  error = '';
  successMessage = '';
  selectedClassForMethods: string = '';


  constructor(
    private variableService: VariableService,
    private fb: FormBuilder,
    private dataService: DataService,
    private router: Router
  ) {
    this.dataService.loadAllData().subscribe({
      next: () => {
        console.log('Datos cargados para la creación de variable');
      },
      error: (err) => {
        console.error('Error cargando datos:', err);
      }
    });
  }

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
        this.successMessage = 'Variable successfully created';
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

  onMethodSelected(value: string): void {
    console.log('Método seleccionado:', value);
    this.methodId = value;
  }

  onClassSelectedForMethod(value: string): void {
    console.log('Clase para métodos seleccionada:', value);
    this.selectedClassForMethods = value;

    const methods = this.dataService.getMethodsByClassId(value);
    console.log(`Clase ${value} tiene ${methods.length} métodos disponibles`);

    this.methodId = '';
  }
}