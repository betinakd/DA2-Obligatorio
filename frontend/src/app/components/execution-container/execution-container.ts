import { Component, Input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-execution-container',
  imports: [MatCardModule],
  templateUrl: './execution-container.html',
  styleUrl: './execution-container.scss'
})
export class ExecutionContainer {
  @Input() executionOutput = '';
}
