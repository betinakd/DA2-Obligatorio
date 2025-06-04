import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClassSelectorComponent } from '../../../components/class-selector/class-selector.component';

@Component({
  selector: 'app-create',
  imports: [ClassSelectorComponent, CommonModule],
  standalone: true,
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss'
})
export class CreateComponent {
  
  onClassSelected(value: string): void {
    console.log('Clase seleccionada:', value);
  }
}
