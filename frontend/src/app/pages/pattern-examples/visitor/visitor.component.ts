import { Component } from '@angular/core';
import { PatternExamplesComponent } from '../pattern-examples.component';
import { MethodExecutionRequest } from '../../../models/request/MethodExecutionRequest';

@Component({
  selector: 'app-visitor',
  imports: [PatternExamplesComponent],
  templateUrl: './visitor.component.html',
  styleUrl: './visitor.component.scss'
})
export class VisitorComponent {
patternName = 'Visitor pattern';
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
