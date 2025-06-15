import { Component, Input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-execution-container',
  imports: [MatCardModule, MatSelectModule],
  templateUrl: './execution-container.html',
  styleUrl: './execution-container.scss'
})
export class ExecutionContainer {
  @Input() executionOutput = '';
}
