import { Component } from '@angular/core';
import { ClassSelectorComponent } from '../../../components/class-selector/class-selector.component';
import { ClassService } from '../../../services/class.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-delete',
  standalone: true,
  imports: [ClassSelectorComponent, CommonModule],
  templateUrl: './delete.component.html',
  styleUrl: './delete.component.scss'
})
export class DeleteComponent {
  constructor(private classService: ClassService) { }

  idClassDelete = '';
  error = '';
  successMessage = '';
  reload = false;

  onClassSelected(value: string): void {
    this.idClassDelete = value;
  }

  deleteClassClick(): void {
    this.successMessage = '';
    this.error = '';

    if (!this.idClassDelete) {
      this.error = 'Please Select a class to delete.';
      return;
    }

    this.classService.deleteClass(this.idClassDelete).subscribe({
      next: (response) => {
        this.successMessage = 'Class ' + this.idClassDelete + ' deleted successfully.';
        this.reload = true;
      },
      error: (err) => {
        this.error = err.error.message || 'An error occurred while deleting the class.';
      }
    });
  }
}
