import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ClassService } from '../../services/class.service';
import { SimClassResponse } from '../../models/response/SimClassResponse';

@Component({
  selector: 'app-parameter-selector',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './parameter-selector.component.html',
  styleUrls: ['./parameter-selector.component.scss']
})
export class ParameterSelectorComponent implements OnInit {
  @Output() parameterSelected = new EventEmitter<string>();
  @Input() labelText: string = 'Select Parameter';
  @Input() methodId: string = '';
  @Input() reload = false;

  parameterControl = new FormControl('');
  classes: SimClassResponse[] = [];
  parameterOptions: { id: string, displayText: string }[] = [];
  loading = false;
  error: string | null = null;

  constructor(private classService: ClassService) { }

  ngOnInit(): void {
    this.loadClasses();

    this.parameterControl.valueChanges.subscribe(value => {
      this.parameterSelected.emit(value || '');
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if ((changes['reload'] && changes['reload'].currentValue === true) ||
      (changes['methodId'] && changes['methodId'].currentValue)) {
      this.loadClasses();
    }
  }

  loadClasses(): void {
    this.loading = true;
    this.error = null;

    this.classService.getAllClasses().subscribe({
      next: (data) => {
        this.classes = data;
        this.processParameterOptions();
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.error = 'Error al cargar parámetros';
        console.error('Error cargando clases:', err);
      }
    });
  }

  private getClassNameById(classId: string): string {
    const classObj = this.classes.find(c => c.id === classId)?.name;
    return classObj ? classObj : 'Unknown Type';
  }

  private processParameterOptions(): void {
    this.parameterOptions = [];

    this.classes.forEach(c => {
      if (c.methods && c.methods.length > 0) {
        const filteredMethods = this.methodId
          ? c.methods.filter(m => m.id === this.methodId)
          : c.methods;

        filteredMethods.forEach(m => {
          if (m.parameters && m.parameters.length > 0) {
            const returnTypeName = this.getClassNameById(m.returnTypeId || '');

            const paramList = m.parameters?.map(p => {
              const paramType = this.getClassNameById(p.referenceId || '');
              return `${paramType} ${p.name}`;
            }).join(', ') || '';

            const methodSignature = `${returnTypeName} ${m.name}(${paramList})`;

            m.parameters.forEach(p => {
              const paramType = this.getClassNameById(p.referenceId || '');

              this.parameterOptions.push({
                id: p.id || '',
                displayText: `${c.name} - ${methodSignature} - Parameter: ${paramType} ${p.name}`
              });
            });
          }
        });
      }
    });
  }
}