import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';


@Component({
  selector: 'app-list-classes-container',
  imports: [MatCardModule, CommonModule],
  templateUrl: './list-classes-container.component.html',
  styleUrl: './list-classes-container.component.scss'
})
export class ListClassesContainerComponent {
  @Input() classList: any[] = [];
}
