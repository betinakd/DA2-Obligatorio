import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-accesibility-selector',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule
  ],
  templateUrl: './accesibility-selector.component.html',
  styleUrls: ['./accesibility-selector.component.scss']
})
export class ClassTypeSelectorComponent implements OnInit {
  @Output() typeSelected = new EventEmitter<string>();
  @Input() labelText = '';

  typeControl = new FormControl('Normal');

  classTypes = [
    { value: 'Normal', label: 'Normal' },
    { value: 'Abstract', label: 'Abstract' },
    { value: 'Sealed', label: 'Sealed' },
    { value: 'Interface', label: 'Interface' }
  ];

  ngOnInit(): void {
    this.typeSelected.emit(this.typeControl.value || '');

    this.typeControl.valueChanges.subscribe(value => {
      this.typeSelected.emit(value || '');
    });
  }
}