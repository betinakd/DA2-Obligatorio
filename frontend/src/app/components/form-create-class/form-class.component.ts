import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-form-class',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatFormFieldModule
  ],
  templateUrl: './form-class.component.html',
  styleUrls: ['./form-class.component.scss']
})
export class FormClassComponent implements OnInit {
  classForm!: FormGroup;
  
  // Estos arrays deberían venir de un servicio o API
  availableClasses: any[] = [];
  availableNamespaces: any[] = [];
  
  constructor(private fb: FormBuilder) {}
  
  ngOnInit() {
    this.initForm();
    // Aquí deberías cargar las clases y namespaces disponibles
    this.loadNamespaces();
    this.loadClasses();
  }
  
  initForm() {
    this.classForm = this.fb.group({
      name: ['', Validators.required],
      state: ['Normal', Validators.required],
      idBaseClass: ['11111111-1111-1111-1111-111111111111'],
      idBaseNamespace: ['', Validators.required]
    });
  }
  
  loadNamespaces() {
    // Aquí deberías llamar a tu API para obtener los namespaces
    // Por ahora, usamos datos de ejemplo
    this.availableNamespaces = [
      { id: '22222222-2222-2222-2222-222222222222', name: 'System' },
      { id: '33333333-3333-3333-3333-333333333333', name: 'System.Collections' },
      { id: '44444444-4444-4444-4444-444444444444', name: 'MyProject' }
    ];
  }
  
  loadClasses() {
    // Aquí deberías llamar a tu API para obtener las clases disponibles
    // Por ahora, usamos datos de ejemplo
    this.availableClasses = [
      { id: '55555555-5555-5555-5555-555555555555', name: 'Object' },
      { id: '66666666-6666-6666-6666-666666666666', name: 'Exception' },
      { id: '77777777-7777-7777-7777-777777777777', name: 'BaseClass' }
    ];
  }
  
  onSubmit() {
    if (this.classForm.valid) {
      const formData = this.classForm.value;
      console.log('Form submitted:', formData);
      
      // Aquí enviarías los datos a tu API
      // this.classService.createClass(formData).subscribe(...)
    }
  }
  
  resetForm() {
    this.classForm.reset({
      state: 'Normal',
      idBaseClass: '11111111-1111-1111-1111-111111111111'
    });
  }
}