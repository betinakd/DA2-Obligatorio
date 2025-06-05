import { Component } from '@angular/core';
import { MethodCreateComponent } from '../../../components/method-create/method-create.component';
import { MethodRequest } from '../../../models/MethodRequest';
import { CommonModule } from '@angular/common';
import { MethodService } from '../../../services/method.service';
import { ClassSelectorComponent } from '../../../components/class-selector/class-selector.component';

@Component({
  selector: 'app-create',
  standalone: true,
  imports: [MethodCreateComponent, CommonModule, ClassSelectorComponent],
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss'
})
export class CreateComponent {

  constructor(private methodService: MethodService) { }

  method: MethodRequest = new MethodRequest();
  error: string = '';
  success: string = '';
  loading: boolean = false;
  classId: string | null = null;

  onMethodChange(event: MethodRequest): void {
    this.method = event;
  }

  onClassChange(event: string): void {
    this.classId = event;
  }

  createMethod(): void {

    if (!this.classId) {
      this.error = 'Please select a class to add this method to.';
      return;
    }

    if (!this.method.name) {
      this.error = 'Method name is required';
      return;
    }

    this.loading = true;
    this.error = '';
    this.success = '';

    console.log('Creating method:', this.method);
    console.log('For class:', this.classId);

    this.methodService.createMethod(this.classId, this.method).subscribe({
      next: (response) => {
        this.success = response.message;
        this.error = '';
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Error: ' + (err.error?.message || err.message || 'Unknown error');
        this.success = '';
        this.loading = false;
      }
    });
  }

}
