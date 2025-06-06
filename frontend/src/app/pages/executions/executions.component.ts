import { Component } from '@angular/core';
import { AttributeSelectorComponent } from '../../components/attribute-selector/attribute-selector.component';
import { ParameterSelectorComponent } from '../../components/parameter-selector/parameter-selector.component';
import { TypeReferenceSelectorComponent } from '../../components/type-reference-selector/type-reference-selector.component';
import { MethodSelectorComponent } from '../../components/method-selector/method-selector.component';
import { CommonModule } from '@angular/common';
import { ParameterSignatureListComponent } from '../../components/parameter-signature-list/parameter-signature-list.component';
import { VariableSelectorComponent } from '../../components/variable-selector/variable-selector.component';
import { FormsModule } from '@angular/forms';
import { StaticAttributeSelectorComponent } from '../../components/static-attribute-selector/static-attribute-selector.component';
import { ClassSelectorComponent } from '../../components/class-selector/class-selector.component';
import { MethodExecutionRequest } from '../../models/request/MethodExecutionRequest';
import { ParameterSignatureRequest } from '../../models/request/ParameterSignatureRequest';
import { ExecutionService } from '../../services/execution.service';

@Component({
  selector: 'app-executions',
  standalone: true,
  imports: [ParameterSignatureListComponent, CommonModule,
    ClassSelectorComponent, FormsModule],
  templateUrl: './executions.component.html',
  styleUrl: './executions.component.scss'
})
export class ExecutionsComponent {

  constructor(private executionService: ExecutionService) { }

  executionRequest: MethodExecutionRequest = new MethodExecutionRequest();
  loading: boolean = false;
  error: string = '';
  success: string = '';

  onReturnTypeChange(value: string): void {
    this.executionRequest.idReturnType = value;
  }

  onParametersChange(value: ParameterSignatureRequest[]): void {
    this.executionRequest.parameters = value;
  }

  onTypeReferenceSelected(value: string): void {
    this.executionRequest.idReferenceType = value;
  }

  onTypeInstanceSelected(value: string): void {
    this.executionRequest.idInstanceType = value;
  }

  onCreateExecution(): void {
    this.loading = true;
    this.error = '';
    this.success = '';
    console.log('Creating execution with request:', this.executionRequest);
    this.executionService.executeMethod(this.executionRequest).subscribe({
      next: (response) => {
        this.success = response.execution;
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
