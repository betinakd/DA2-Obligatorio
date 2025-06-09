import { Component } from '@angular/core';
import { MethodExecutionRequest } from '../../../models/request/MethodExecutionRequest';
import { PatternExamplesComponent } from '../pattern-examples.component';

@Component({
  selector: 'app-strategy',
  imports: [PatternExamplesComponent],
  templateUrl: './strategy.component.html',
  styleUrl: './strategy.component.scss'
})
export class StrategyComponent {
  patternName = 'Strategy pattern';
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
