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
  patternIdClasses: string[] = ['b27b0c0b-9c4b-4f19-a9d7-84d29554cbc6', 'f9681b3e-9f95-4ac9-8a69-2b68e2c8b4b9', 'efbbe130-5753-4866-95c0-c93cb023efac', 'ee8461c4-5a6f-492f-b70c-2bc14d234f0d', '2dd354f9-d03d-4f03-801c-79c61b922722' ]
  executionInfo: MethodExecutionRequest[] = 
   [{
      "methodName": "main",
      "parameters": [],
      "idReferenceType": "2DD354F9-D03D-4F03-801C-79C61B922722",
      "idInstanceType": "2DD354F9-D03D-4F03-801C-79C61B922722",
      "idReturnType": "22222222-2222-2222-2222-222222222222"
    }];
   executionsOptions: string[] = [];
}
