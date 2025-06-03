import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { VariableService } from '../../../services/variable.service';
import { Variable } from '../../../models/variable.model';

@Component({
  selector: 'app-create',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss'
})
export class CreateComponent implements OnInit {
  variable: Variable = {};
  variableForm!: FormGroup;
  isLoading = false;
  error = '';
  successMessage = '';
  viewMode = false;

  constructor(
    private variableService: VariableService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initForm();
    
    // Si quieres cargar una variable existente por ID
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.viewMode = true;
      this.loadVariable(id);
    }
    
    // Si hay un methodId en la URL, lo usamos para la creación
    const methodId = this.route.snapshot.queryParamMap.get('methodId');
    if (methodId) {
      this.variableForm.get('methodId')?.setValue(methodId);
    }
  }

  initForm(): void {
    this.variableForm = this.fb.group({
      name: ['', [Validators.required, Validators.pattern(/^[a-zA-Z][a-zA-Z0-9_]*$/)]],
      idReference: ['', [Validators.required, Validators.pattern(/^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/)]],
      idInstance: ['', [Validators.required, Validators.pattern(/^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/)]],
      methodId: ['', Validators.required]
    });
  }

  loadVariable(id: string): void {
    this.isLoading = true;
    this.variableService.getVariable(id).subscribe({
      next: (data) => {
        this.variable = data;
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Error al cargar la variable: ' + err.message;
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.variableForm.invalid) {
      this.variableForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const formValues = this.variableForm.value;
    const methodId = formValues.methodId;
    
    // Eliminar methodId del objeto a enviar
    const { methodId: _, ...variableData } = formValues;

    this.variableService.createVariable(methodId, variableData).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.successMessage = 'Variable creada exitosamente';
        this.error = '';
        
        // Redirigir a la vista de detalle después de la creación
        if (response?.variable?.id) {
          setTimeout(() => {
            this.router.navigate(['/variables', response.variable.id]);
          }, 1500);
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.error = 'Error al crear la variable: ' + (err.error?.message || err.message || 'Error desconocido');
        this.successMessage = '';
      }
    });
  }

  get nameControl() { return this.variableForm.get('name'); }
  get idReferenceControl() { return this.variableForm.get('idReference'); }
  get idInstanceControl() { return this.variableForm.get('idInstance'); }
  get methodIdControl() { return this.variableForm.get('methodId'); }
}