import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClassService } from '../../../services/class.service';
import { SimClass } from '../../../models/SimClass.model';
import { ClassSelectorComponent } from '../../../components/class-selector/class-selector.component';
import { NamespaceSelectorComponent } from '../../../components/namespace-selector/namespace-selector.component';
import { ClassTypeSelectorComponent } from '../../../components/accesibility-selector/accesibility-selector.component';

@Component({
  selector: 'app-create',
  standalone: true,
  imports: [CommonModule, ClassSelectorComponent, NamespaceSelectorComponent, ClassTypeSelectorComponent],
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss'
})

export class CreateComponent implements OnInit {
  loading = false;
  error = '';
  createdClass: SimClass | null = null;
  name = '';
  baseClassId = '';
  baseNamespaceId = '';
  state = '';
  successMessage = '';

  constructor(private classService: ClassService) { }

  ngOnInit(): void {

  }

  onClassSelected(value: string): void {
    this.baseClassId = value;
  }

  onNamespaceSelected(value: string): void {
    this.baseNamespaceId = value;
  }

  onStateSelected(value: string): void {
    this.state = value;
  }

  handleButtonClick() {

    const classData = {
      name: this.name,
      idBaseClass: this.baseClassId,
      idBaseNamespace: this.baseNamespaceId,
      state: this.state
    };

    this.successMessage = '';

    this.classService.createClass(classData).subscribe({
      next: (response) => {
        this.successMessage = response.message + ' - ID: ' + response.simClass.id + ' - Name: ' + response.simClass.name;
        this.loading = true;
        this.createdClass = response.simClass;
        this.error = '';
        this.name = '';
      },
      error: (err) => {
        this.error = 'Error: ' + (err.error?.message || err.message || 'Unknown error');
        this.loading = false;
        this.successMessage = '';
      }
    });
  }
}