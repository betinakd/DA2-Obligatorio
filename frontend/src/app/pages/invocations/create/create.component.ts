import { Component } from '@angular/core';
import { ParameterSignatureRequest } from '../../../models/request/ParameterSignatureRequest';
import { ParameterSignatureListComponent } from '../../../components/parameter-signature-list/parameter-signature-list.component';
import { ClassSelectorComponent } from '../../../components/class-selector/class-selector.component';
import { MethodSelectorComponent } from '../../../components/method-selector/method-selector.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TypeReferenceSelectorComponent } from '../../../components/type-reference-selector/type-reference-selector.component';
import { AttributeSelectorComponent } from '../../../components/attribute-selector/attribute-selector.component';
import { StaticAttributeSelectorComponent } from '../../../components/static-attribute-selector/static-attribute-selector.component';
import { VariableSelectorComponent } from '../../../components/variable-selector/variable-selector.component';
import { ParameterSelectorComponent } from '../../../components/parameter-selector/parameter-selector.component';
import { MethodService } from '../../../services/method.service';
import { InvocationRequest } from '../../../models/request/InvocationRequest';

@Component({
  selector: 'app-create',
  imports: [ParameterSignatureListComponent, MethodSelectorComponent, CommonModule,
    ClassSelectorComponent, FormsModule, TypeReferenceSelectorComponent,
    AttributeSelectorComponent, StaticAttributeSelectorComponent
    , VariableSelectorComponent, ParameterSelectorComponent],
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss'
})
export class CreateComponent {

  constructor(private methodService: MethodService) { }

  methodId: string = '';
  loading: boolean = false;
  invocationRequest: InvocationRequest = new InvocationRequest();
  error: string | null = null;
  success: string | null = null;

  onParametersChange(parameters: ParameterSignatureRequest[]): void {
    this.invocationRequest.parameters = parameters;
  }

  onMethodsChange(event: string): void {
    this.methodId = event;
  }


  onReturnTypeChange(event: string): void {
    this.invocationRequest.idReturnType = event;
  }

  onTypeReferenceSelected(type: string): void {
    this.invocationRequest.typeReference = type;
  }

  onClassThisSelected(event: string): void {
    this.invocationRequest.idReference = event;
  }

  onClassBaseSelected(event: string): void {
    this.invocationRequest.idReference = event;
  }

  onClassStaticSelected(event: string): void {
    this.invocationRequest.idReference = event;
  }

  onAttributeSelected(attributeId: string): void {
    this.invocationRequest.idReference = attributeId;
  }

  onStaticAttributeSelected(attributeId: string): void {
    this.invocationRequest.idReference = attributeId;
  }

  onVariableSelected(variableId: string): void {
    this.invocationRequest.idReference = variableId;
  }

  onParameterSelected(parameterId: string): void {
    this.invocationRequest.idReference = parameterId;
  }

  clickButton(): void {
    this.loading = true;
    this.methodService.createInvocation(this.methodId, this.invocationRequest).subscribe({
      next: (response) => {
        this.loading = false;
        this.success = response.message;
        this.error = '';
      },
      error: (error) => {
        console.error('Error creating invocation:', error);
        this.loading = false;
        this.error = error.error?.message || 'An error occurred while creating the invocation.';
        this.success = '';
      }
    });
  }
}
