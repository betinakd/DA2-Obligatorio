import { Component, Input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

interface ClassItem {
  id: string;
  name: string;
  idBaseClass: string;
  attributes: any[];
  methods: any[];
  implements: string[];
}

@Component({
  selector: 'app-list-classes-container',
  imports: [MatCardModule],
  templateUrl: './list-classes-container.component.html',
  styleUrl: './list-classes-container.component.scss'
})
export class ListClassesContainerComponent {
  @Input() classList: ClassItem[] = [];
}
