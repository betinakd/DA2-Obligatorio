import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ClassService } from '../../services/class.service';
import { SimClassResponse } from '../../models/response/SimClassResponse';

@Component({
  selector: 'app-class-selector',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './class-selector.component.html',
  styleUrls: ['./class-selector.component.scss']
})

export class ClassSelectorComponent implements OnInit {
  @Output() classSelected = new EventEmitter<string>();
  @Input() labelText: string = 'Select Class';
  @Input() reload = false;

  classControl = new FormControl('');
  classes: SimClassResponse[] = [];
  loading = false;
  error: string | null = null;

  constructor(private classService: ClassService) { }

  ngOnInit(): void {
    this.loadClasses();

    this.classControl.valueChanges.subscribe(value => {
      this.classSelected.emit(value || '');
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['reload'] && changes['reload'].currentValue === true) {
      this.loadClasses();
      this.reload = false;
    }
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
        this.loading = false;
        this.error = 'Error al cargar clases';
        console.error('Error cargando clases:', err);
      }
    });
  }
}