import { Component } from '@angular/core';
import { MethodExecutionRequest } from '../../../models/request/MethodExecutionRequest';
import { PatternExamplesComponent } from '../pattern-examples.component';

@Component({
  selector: 'app-composite',
  imports: [PatternExamplesComponent],
  templateUrl: './composite.component.html',
  styleUrl: './composite.component.scss'
})
export class CompositeComponent {
  patternName = 'Composite pattern';
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
