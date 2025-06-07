import { Component } from '@angular/core';
import { TransformerSelectorComponent } from '../../components/transformer-selector/transformer-selector.component';

@Component({
  selector: 'app-transformers',
  imports: [TransformerSelectorComponent],
  templateUrl: './transformers.component.html',
  styleUrl: './transformers.component.scss'
})
export class TransformersComponent {

  onTransformerSelected(transformer: string): void {
    console.log('Selected transformer:', transformer);
  }
}
