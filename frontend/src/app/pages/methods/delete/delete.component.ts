import { Component } from '@angular/core';
import { MethodSelectorComponent } from '../../../components/method-selector/method-selector.component';
import { CommonModule } from '@angular/common';
import { MethodService } from '../../../services/method.service';

@Component({
  selector: 'app-delete',
  standalone: true,
  imports: [MethodSelectorComponent, CommonModule],
  templateUrl: './delete.component.html',
  styleUrl: './delete.component.scss'
})
export class DeleteComponent {

  constructor(private methodService: MethodService) { }

  selectedMethodId: string | null = null;
  error: string = '';
  success: string = '';
  loading: boolean = false;

  onMethodSelected(event: string): void {
    this.selectedMethodId = event;
  }

  deleteMethod(): void {
    if (!this.selectedMethodId) {
      this.error = 'Please select a method to delete';
      return;
    }

    this.methodService.deleteMethod(this.selectedMethodId).subscribe({
      next: () => {
        this.success = 'Method id:' + this.selectedMethodId + ' deleted successfully';
        this.error = '';
        this.loading = true;
      },
      error: (err) => {
        this.error = 'Error: ' + (err.error?.message || 'Invalid inputs');
        this.success = '';
      }
    });
  }
}
