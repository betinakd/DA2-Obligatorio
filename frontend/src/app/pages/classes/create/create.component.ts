import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormClassComponent } from '../../../components/form-create-class/form-class.component';
import { ClassService } from '../../../services/class.service';
import { SimClass } from '../../../models/SimClass.model';
import { ErrorResponse } from '../../../models/ErrorResponse.model';
import { ClassCardComponent } from '../../../components/class-card/class-card.component';

@Component({
  selector: 'app-create',
  standalone: true,
  imports: [CommonModule, FormClassComponent,ClassCardComponent], // Remove ClassService from imports
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss'
})
export class CreateComponent implements OnInit {
  loading = false;
  error: ErrorResponse | null = null;
  createdClass: SimClass | null = null;
  name = '';
  baseClassId = '';
  baseNamespaceId = '';
  state = '';

  constructor(private classService: ClassService) {}
  
  ngOnInit(): void {
    
  }

  handleButtonClick() {
    console.log('Button was clicked!');
    
    const classData = {
      name: this.name,
      idBaseClass: this.baseClassId,
      idBaseNamespace: this.baseNamespaceId,
      state: this.state
    };
    
    console.log('Sending class data:', classData);
    this.loading = true;
    
    this.classService.createClass(classData).subscribe({
      next: (response) => {
        console.log('Class created successfully:', response);
        this.loading = false;
        this.createdClass = response;
      },
      error: (error) => {
        console.error('Error creating class:', error);
        this.error = error;
        this.loading = false;
      }
    });
  }
}