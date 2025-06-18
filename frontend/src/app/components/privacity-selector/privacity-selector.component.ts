import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-privacity-selector',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule
  ],
  templateUrl: './privacity-selector.component.html',
  styleUrls: ['./privacity-selector.component.scss']
})
export class PrivacitySelectorComponent implements OnInit {
  @Output() privacitySelected = new EventEmitter<string>();
  @Input() labelText = 'Select Visibility';

  privacityControl = new FormControl('Public');

  privacityTypes = [
    { value: 'Public', label: 'Public' },
    { value: 'Protected', label: 'Protected' },
    { value: 'Private', label: 'Private' }
  ];

  ngOnInit(): void {
    this.privacitySelected.emit(this.privacityControl.value || 'Public');

    this.privacityControl.valueChanges.subscribe(value => {
      this.privacitySelected.emit(value || 'Public');
    });
  }
}