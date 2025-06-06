import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClassSelectorComponent } from '../../../components/class-selector/class-selector.component';
import { MethodSelectorComponent } from '../../../components/method-selector/method-selector.component';
import { MethodService } from '../../../services/method.service';

@Component({
  selector: 'app-create',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ClassSelectorComponent,
    MethodSelectorComponent
  ],
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss'
})
export class CreateComponent {
  constructor(private methodService: MethodService) { }

  parameter = {
    name: '',
    idReference: ''
  };

  methodId: string = '';
  error: string = '';
  success: string = '';
  loading: boolean = false;

  onTypeSelected(value: string): void {
    this.parameter.idReference = value;
  }

  onMethodSelected(value: string): void {
    this.methodId = value;
  }

  clickCreate(): void {
    this.loading = true;
    this.error = '';
    this.success = '';

    this.methodService.createParameter(this.methodId, this.parameter)
      .subscribe({
        next: (response) => {
          this.success = `Parameter ${this.parameter.name} created successfully!`;
          this.resetForm();
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Error: ' + (err.error?.message || err.message || 'Unknown error');
          this.loading = false;
        }
      });
  }

  resetForm(): void {
    this.parameter = {
      name: '',
      idReference: ''
    };
  }
}