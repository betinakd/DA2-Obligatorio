import { Component, Input } from '@angular/core';
import { ListClassesContainerComponent } from '../list-classes-container/list-classes-container.component';
import { ListMethodsContainerComponent } from '../list-methods-container/list-methods-container.component';
import { ExecutionContainer } from '../execution-container/execution-container';

@Component({
  selector: 'app-treefeature-grid',
  imports: [ListClassesContainerComponent, ListMethodsContainerComponent, ExecutionContainer],
  templateUrl: './treefeature-grid.component.html',
  styleUrl: './treefeature-grid.component.scss'
})
export class TreefeatureGridComponent {
  @Input() classList: any[] = [];
  @Input() methodList: any[] = [];
  @Input() executionOutput = '';
}
