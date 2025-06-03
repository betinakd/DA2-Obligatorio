import { Component, OnInit, Output } from '@angular/core';
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
  @Output() baseClassId! : string;
  @Output() baseNamespaceId! : string;
  @Output() state!: string;
  @Output() name!: string;

  availableClasses: any[] = [];
  availableNamespaces: any[] = [];
  
  constructor(private fb: FormBuilder) {}
  
  ngOnInit() {
    this.initForm();
  }
  
  initForm() {
    this.classForm = this.fb.group({
      name: ['', Validators.required]
    });
    
    this.classForm.get('name')?.valueChanges.subscribe(value => {
      this.name = value;
    });
  }
  

  onClassSelected(classId: string | null): void {
    if (classId) {
      this.classForm.get('idBaseClass')?.setValue(classId);
      this.baseClassId = classId;
    }
  }

  onNamespaceSelected(namespaceId: string | null): void {
    if (namespaceId) {
      this.classForm.get('idBaseNamespace')?.setValue(namespaceId);
      this.baseNamespaceId = namespaceId;
    }
  }

  onStateSelected(type: string): void {
    this.classForm.get('state')?.setValue(type);
    this.state = type;
  }

  onSubmit() {
    if (this.classForm.valid) {
      const formData = this.classForm.value;
      console.log('Form submitted:', formData);
    }
  }
}