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
  patternIdClasses: string[] = ["053a2387-4d95-4242-87cd-dca5972f95f6", "39dd195e-a485-459d-84a5-eb4b92ff434a", "45b88b14-e018-4b98-bdf5-b77a9a4cc7fe"] 
  executionInfo: MethodExecutionRequest[] = 
   [{
     "methodName": "prepararPizza",
      "parameters": [],
     "idReferenceType": "053a2387-4d95-4242-87cd-dca5972f95f6",
     "idInstanceType": "39dd195e-a485-459d-84a5-eb4b92ff434a",
      "idReturnType": "22222222-2222-2222-2222-222222222222"
    },
    {
      "methodName": "prepararPizza",
      "parameters": [],
      "idReferenceType": "053a2387-4d95-4242-87cd-dca5972f95f6",
      "idInstanceType": "45b88b14-e018-4b98-bdf5-b77a9a4cc7fe",
      "idReturnType": "22222222-2222-2222-2222-222222222222"
    },
  ];
  executionsOptions: string[] = ['simulate hawaiian pizza preparation', 'simulate pepperoni pizza preparation'];
}
