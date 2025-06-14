import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ClassService } from '../../services/class.service';
import { MethodService } from '../../services/method.service';
import { SimClassResponse } from '../../models/response/SimClassResponse';

@Component({
  selector: 'app-variable-selector',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './variable-selector.component.html',
  styleUrls: ['./variable-selector.component.scss']
})
export class VariableSelectorComponent implements OnInit {
  @Output() variableSelected = new EventEmitter<string>();
  @Input() labelText: string = 'Select Variable';
  @Input() methodId: string = '';
  @Input() reload = false;

  variableControl = new FormControl('');
  classes: SimClassResponse[] = [];
  variableOptions: { id: string, displayText: string }[] = [];
  loading = false;
  error: string | null = null;

  constructor(
    private classService: ClassService,
    private methodService: MethodService
  ) { }

  ngOnInit(): void {
    this.loadClasses();

    this.variableControl.valueChanges.subscribe(value => {
      this.variableSelected.emit(value || '');
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
        this.processVariableOptions();
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.error = 'Error al cargar variables';
        console.error('Error cargando clases:', err);
      }
    });
  }

  private getClassNameById(classId: string): string {
    const classObj = this.classes.find(c => c.id === classId)?.name;
    return classObj ? classObj : 'Unknown Class';
  }

  private processVariableOptions(): void {
    this.variableOptions = [];

    this.classes.forEach(c => {
      if (c.methods && c.methods.length > 0) {
        const filteredMethods = this.methodId
          ? c.methods.filter(m => m.id === this.methodId)
          : c.methods;

        filteredMethods.forEach(m => {
          if (m.variables && m.variables.length > 0) {
            const returnTypeName = this.getClassNameById(m.returnTypeId || '');

            const paramList = m.parameters?.map(p => {
              const paramType = this.getClassNameById(p.referenceId || '');
              return `${paramType} ${p.name}`;
            }).join(', ') || '';

            const methodSignature = `${returnTypeName} ${m.name}(${paramList})`;

            m.variables.forEach(v => {
              const variableType = this.getClassNameById(v.referenceId || '');

              this.variableOptions.push({
                id: v.id || '',
                displayText: `${c.name} - ${methodSignature} - ${variableType} ${v.name}`
              });
            });
          }
        });
      }
    });
  }
}