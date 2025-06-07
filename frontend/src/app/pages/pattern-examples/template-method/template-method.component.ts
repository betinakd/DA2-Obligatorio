import { Component } from '@angular/core';
import { MethodExecutionRequest } from '../../../models/request/MethodExecutionRequest';
import { P } from '@angular/cdk/keycodes';
import { PatternExamplesComponent } from '../pattern-examples.component';

@Component({
  selector: 'app-template-method',
  imports: [PatternExamplesComponent],
  templateUrl: './template-method.component.html',
  styleUrl: './template-method.component.scss'
})
export class TemplateMethodComponent {
patternName = 'Template Method pattern';
  patternIdClasses: string[] = ['11111111-1111-1111-1111-111111111111'] //mock data
  executionInfo: MethodExecutionRequest = 
   {
      "methodName": "Finalize",
      "parameters": [],
      "idReferenceType": "11111111-1111-1111-1111-111111111111",
      "idInstanceType": "11111111-1111-1111-1111-111111111111",
      "idReturnType": "22222222-2222-2222-2222-222222222222"
    }; //mock data
}
