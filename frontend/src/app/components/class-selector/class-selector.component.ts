import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { SimClass } from '../../models/SimClass.model';
import { DataService } from '../../services/data.service';

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
  classes: SimClass[] = [];
  loading = false;
  error: string | null = null;

  constructor(private dataService: DataService) { }

  ngOnInit(): void {
    this.loadClasses();

    this.classControl.valueChanges.subscribe(value => {
      this.classSelected.emit(value || '');
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['reload'] && changes['reload'].currentValue === true) {
      this.loadClasses();
    }
  }

  loadClasses(): void {
    this.loading = true;
    this.error = null;
    console.log('ClassSelector: Intentando cargar clases...');

    this.dataService.loadAllData().subscribe({
      next: () => {
        const classes = this.dataService.getAllClasses();
        this.classes = classes;
        this.loading = false;
        console.log(`${classes.length} clases cargadas en selector`);

        if (classes.length === 0) {
          this.error = 'No se encontraron clases';
        }
      },
      error: (err) => {
        this.error = 'Error cargando clases: ' + (err.message || 'Error desconocido');
        this.loading = false;
        console.error('Error en ClassSelector:', err);
      }
    });
  }
}