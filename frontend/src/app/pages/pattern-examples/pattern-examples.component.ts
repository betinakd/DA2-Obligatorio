import { Component, Input, OnInit } from '@angular/core';
import { ErrorResponse } from '../../models/ErrorResponse.model';
import { SimClassResponse } from '../../models/SimClassResponse';
import { TreefeatureGridComponent } from '../../components/treefeature-grid/treefeature-grid.component';
import { CommonModule } from '@angular/common';
import { ExecutionService } from '../../services/execution.service';
import { ClassService } from '../../services/class.service';
import { MethodExecutionRequest } from '../../models/request/MethodExecutionRequest';

@Component({
  selector: 'app-pattern-examples',
  imports: [TreefeatureGridComponent, CommonModule],
  templateUrl: './pattern-examples.component.html',
  styleUrl: './pattern-examples.component.scss'
})
export class PatternExamplesComponent implements OnInit{
  //@Input() patternName = '';
  //@Input() patternIdClasses: string[] = [];
  //@Input() executionInfo : any[] = [];

  patternName = 'here goes the pattern name';
  patternIdClasses: string[] = ['11111111-1111-1111-1111-111111111111'];
  executionInfo: MethodExecutionRequest = 
   {
      "methodName": "Finalize",
      "parameters": [],
      "idReferenceType": "11111111-1111-1111-1111-111111111111",
      "idInstanceType": "11111111-1111-1111-1111-111111111111",
      "idReturnType": "22222222-2222-2222-2222-222222222222"
    };
  classList: any[] = [];
  methodList: any[] = [];

  executionOutput = 'empty execution output';

  typesList: SimClassResponse[] = [];


  constructor(private classService: ClassService, private executionService: ExecutionService) { }
  ngOnInit(): void {
    this.getExecutionOutput();
  }

  getExecutionOutput() {
    this.executionService.executeMethod(this.executionInfo).subscribe({
      next: (response) => {
        this.executionOutput = String(response.execution);
      },
      error: (error) => {
        this.executionOutput = 'Error loading execution information. Message: ' + error.message;
      }
    });
  }
}
