import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormClassComponent } from '../../../components/form-create-class/form-class.component';
import { ClassService } from '../../../services/class.service';
import { SimClass } from '../../../models/SimClass.model';
import { ErrorResponse } from '../../../models/ErrorResponse.model';

@Component({
  selector: 'app-create',
  standalone: true,
  imports: [CommonModule, FormClassComponent],
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss'
})
export class CreateComponent implements OnInit {
  classes: SimClass[] = [];
  loading = false;
  error: ErrorResponse | null = null;

  constructor(private classService: ClassService) {}
  
  ngOnInit(): void {
    this.loadClasses();
  }
  
  loadClasses(): void {
    this.loading = true;
    this.error = null;
    
    this.classService.getAllClasses().subscribe({
      next: (data) => {
        this.classes = data;
        this.loading = false;
      },
      error: (err) => {
        const apiError = err.error as ErrorResponse;
        
        if (apiError?.innerCode !== undefined && apiError?.message) {
          this.error = apiError;
        } else {
          this.error = {
            innerCode: err.status || 0,
            message: err.message || 'Unexpected error loading classes'
          };
        }
        
        this.loading = false;
      }
    });
  }
}