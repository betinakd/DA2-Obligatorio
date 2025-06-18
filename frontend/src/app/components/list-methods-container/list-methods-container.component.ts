import { Component, Input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-list-methods-container',
  imports: [MatCardModule],
  templateUrl: './list-methods-container.component.html',
  styleUrl: './list-methods-container.component.scss'
})
export class ListMethodsContainerComponent {
  @Input() methodList: any[] = [];
}
