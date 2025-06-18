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
  patternIdClasses: string[] = ["39543612-ba65-4a51-9982-0a6f825a7533", "47378fe9-3742-4984-8a9b-60780b3cd630", "182c1cd6-7bcb-4f49-980b-a500454f6f58", "1DAD63D4-6D7F-42DC-8B4B-CE9D68835B09", "2830eeff-191d-4400-b0ce-a83b27fac848", "2ec687dc-feb6-407b-b4b0-4280fdb6c5cf"]
  executionInfo: MethodExecutionRequest[] = 
   [{
      "methodName": "Aceptar",
      "parameters": [
        {
          "name": "visitorCaritas",
          "idReference": "39543612-ba65-4a51-9982-0a6f825a7533",
          "idInstance": "2ec687dc-feb6-407b-b4b0-4280fdb6c5cf" 
        }
      ],
      "idReferenceType": "47378fe9-3742-4984-8a9b-60780b3cd630",
      "idInstanceType": "182c1cd6-7bcb-4f49-980b-a500454f6f58",
      "idReturnType": "22222222-2222-2222-2222-222222222222"
    },
    {
      "methodName": "Aceptar",
      "parameters": [
        {
          "name": "visitorCaritas",
          "idReference": "39543612-ba65-4a51-9982-0a6f825a7533",
          "idInstance": "2830eeff-191d-4400-b0ce-a83b27fac848"
        }
      ],
      "idReferenceType": "47378fe9-3742-4984-8a9b-60780b3cd630",
      "idInstanceType": "182c1cd6-7bcb-4f49-980b-a500454f6f58",
      "idReturnType": "22222222-2222-2222-2222-222222222222"
    },
    {
      "methodName": "Aceptar",
      "parameters": [
        {
          "name": "visitorCaritas",
          "idReference": "39543612-ba65-4a51-9982-0a6f825a7533",
          "idInstance": "2ec687dc-feb6-407b-b4b0-4280fdb6c5cf"
        }
      ],
      "idReferenceType": "47378fe9-3742-4984-8a9b-60780b3cd630",
      "idInstanceType": "1DAD63D4-6D7F-42DC-8B4B-CE9D68835B09",
      "idReturnType": "22222222-2222-2222-2222-222222222222"
    },
    {
      "methodName": "Aceptar",
      "parameters": [
        {
          "name": "visitorCaritas",
          "idReference": "39543612-ba65-4a51-9982-0a6f825a7533",
          "idInstance": "2830eeff-191d-4400-b0ce-a83b27fac848"
        }
      ],
      "idReferenceType": "47378fe9-3742-4984-8a9b-60780b3cd630",
      "idInstanceType": "1DAD63D4-6D7F-42DC-8B4B-CE9D68835B09",
      "idReturnType": "22222222-2222-2222-2222-222222222222"
    }
  ];
  executionsOptions: string[] = ["simulate reel-VisitanteFiltroCaritas execute Aceptar",
    "simulate reel-VisitanteBYN execute Aceptar",
    "simulate historia-VisitanteFiltroCaritas execute Aceptar",
    "simulate historia-VisitanteBYN execute Aceptar"];
}
