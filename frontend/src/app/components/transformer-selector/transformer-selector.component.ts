import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { TransformersService } from '../../services/transformers.service';

@Component({
  selector: 'app-transformer-selector',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule
  ],
  templateUrl: './transformer-selector.component.html',
  styleUrl: './transformer-selector.component.scss'
})
export class TransformerSelectorComponent implements OnInit {
  @Output() transformerSelected = new EventEmitter<string>();
  @Input() labelText = 'Select Transformer';

  transformerControl = new FormControl('');
  transformers: string[] = [];

  constructor(private transformersService: TransformersService) { }

  ngOnInit(): void {
    this.transformersService.getTransformers().subscribe({
      next: (data) => {
        this.transformers = data;

        if (this.transformers.length > 0) {
          this.transformerControl.setValue(this.transformers[0]);
          this.transformerSelected.emit(this.transformers[0]);
        }
      },
      error: (error) => console.error('Error loading transformers:', error)
    });

    this.transformerControl.valueChanges.subscribe(value => {
      if (value) {
        this.transformerSelected.emit(value);
      }
    });
  }
}