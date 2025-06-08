import { Component } from '@angular/core';
import { TransformerSelectorComponent } from '../../components/transformer-selector/transformer-selector.component';
import { ParameterSignatureListComponent } from '../../components/parameter-signature-list/parameter-signature-list.component';
import { FormsModule } from '@angular/forms';
import { ClassSelectorComponent } from '../../components/class-selector/class-selector.component';
import { CommonModule } from '@angular/common';
import { MethodExecutionRequest } from '../../models/request/MethodExecutionRequest';
import { ParameterSignatureRequest } from '../../models/request/ParameterSignatureRequest';
import { TransformersService } from '../../services/transformers.service';

@Component({
  selector: 'app-transformers',
  imports: [TransformerSelectorComponent, ParameterSignatureListComponent, CommonModule,
    ClassSelectorComponent, FormsModule],
  templateUrl: './transformers.component.html',
  styleUrl: './transformers.component.scss'
})
export class TransformersComponent {

  constructor(private transformerService: TransformersService) { }

  transformerName: string = '';
  executionRequest: MethodExecutionRequest = new MethodExecutionRequest();
  loading: boolean = false;
  error = '';
  success = '';

  onTransformerSelected(transformer: string): void {
    this.transformerName = transformer;
  }

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

  onCreateExecutionTransformed(): void {
    this.loading = true;
    this.error = '';
    this.success = '';

    this.transformerService.executeWithTransform(
      this.executionRequest,
      this.transformerName
    ).subscribe({
      next: (response) => {
        this.success = response;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Error: ' + (err.error?.message || err.message || 'Unknown error');
        console.error('Error:', err);
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }
}
