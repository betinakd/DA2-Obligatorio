import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ClassSelectorComponent } from '../class-selector/class-selector.component';
import { NamespaceSelectorComponent } from '../../components/namespace-selector/namespace-selector.component';
import { ClassTypeSelectorComponent } from '../../components/accesibility-selector/accesibility-selector.component';

@Component({
  selector: 'app-form-class',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatFormFieldModule,
    ClassSelectorComponent,
    NamespaceSelectorComponent,
    ClassTypeSelectorComponent
  ],
  templateUrl: './form-class.component.html',
  styleUrls: ['./form-class.component.scss']
})
export class FormClassComponent implements OnInit {
  classForm!: FormGroup;
  @Output() name = new EventEmitter<string>();
  @Output() baseClassId = new EventEmitter<string>();
  @Output() baseNamespaceId = new EventEmitter<string>();
  @Output() state = new EventEmitter<string>();
  @Output() buttonClicked = new EventEmitter<void>();

  availableClasses: any[] = [];
  availableNamespaces: any[] = [];
  
  constructor(private fb: FormBuilder) {}
  
  ngOnInit() {
    this.initForm();
  }
  
  initForm() {
    this.classForm = this.fb.group({
      name: ['', Validators.required],
      state: [''],
      idBaseClass: [''],
      idBaseNamespace: ['']
    });
    
    // Change this - don't assign directly to this.name
    this.classForm.get('name')?.valueChanges.subscribe(value => {
      // Emit the value instead of assigning it
      this.name.emit(value);
    });
  }
  

  onClassSelected(classId: string | null): void {
    if (classId) {
      this.classForm.get('idBaseClass')?.setValue(classId);
      this.baseClassId.emit(classId);
    }
  }

  onNamespaceSelected(namespaceId: string | null): void {
    if (namespaceId) {
      this.classForm.get('idBaseNamespace')?.setValue(namespaceId);
      this.baseNamespaceId.emit(namespaceId);
    }
  }

  onStateSelected(type: string): void {
    this.classForm.get('state')?.setValue(type);
    this.state.emit(type);
  }

  logClick() {
    console.log('Button clicked');
    console.log('Form values:', this.classForm.value);
    console.log('Form valid:', this.classForm.valid);
    console.log('Namespace ID:', this.classForm.get('idBaseNamespace')?.value);
    
    // Make sure you're emitting all values
    this.name.emit(this.classForm.get('name')?.value);
    this.baseNamespaceId.emit(this.classForm.get('idBaseNamespace')?.value);
    this.baseClassId.emit(this.classForm.get('idBaseClass')?.value);
    this.state.emit(this.classForm.get('state')?.value);
    
    // Only emit buttonClicked if form is valid
    if (this.classForm.valid) {
      this.buttonClicked.emit();
    } else {
      console.error('Form is invalid, cannot submit');
    }
  }
  
  onSubmit() {
    if (this.classForm.valid) {
      this.logClick();
    }
  }
}