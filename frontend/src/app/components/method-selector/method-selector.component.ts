import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MethodService } from '../../services/method.service';
import { ClassService } from '../../services/class.service';
import { SimClassResponse } from '../../models/response/SimClassResponse';
import { MethodResponse } from '../../models/response/MethodResponse';

@Component({
  selector: 'app-method-selector',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './method-selector.component.html',
  styleUrls: ['./method-selector.component.scss']
})
export class MethodSelectorComponent implements OnInit {
  @Output() methodSelected = new EventEmitter<string>();
  @Input() labelText: string = 'Select Method';
  @Input() classId: string = '';
  @Input() reload = false;

  methodControl = new FormControl('');
  classes: SimClassResponse[] = [];
  methodOptions: { id: string, displayText: string }[] = [];
  loading = false;
  error: string | null = null;

  constructor(
    private methodService: MethodService,
    private classService: ClassService
  ) { }

  ngOnInit(): void {
    this.loadClasses();

    this.methodControl.valueChanges.subscribe(value => {
      this.methodSelected.emit(value || '');
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if ((changes['reload'] && changes['reload'].currentValue === true) ||
      (changes['classId'] && changes['classId'].currentValue)) {
      this.loadClasses();
    }
  }

  loadClasses(): void {
    this.loading = true;
    this.error = null;

    this.classService.getAllClasses().subscribe({
      next: (data) => {
        this.classes = data;
        this.processMethodOptions();
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.error = 'Error al cargar métodos';
        console.error('Error cargando clases:', err);
      }
    });
  }

  private getClassNameById(classId: string): string {
    const classObj = this.classes.find(c => c.id === classId)?.name;
    return classObj ? classObj : 'Unknown Class';
  }

  private processMethodOptions(): void {
    const filteredClasses = this.classId
      ? this.classes.filter(c => c.id === this.classId)
      : this.classes;

    this.methodOptions = [];

    filteredClasses.forEach(c => {
      if (c.methods && c.methods.length > 0) {
        c.methods.forEach(m => {
          const privacity = m.privacity;
          const returnType = this.getClassNameById(m.returnTypeId || '');
          const paramList = m.parameters.map(p => {
            const paramClassName = this.getClassNameById(p.referenceId || '');
            return `${paramClassName} ${p.name}`;
          }).join(', ');

          this.methodOptions.push({
            id: m.id || '',
            displayText: `${c.name} - ${privacity} ${returnType} ${m.name}(${paramList})`
          });
        });
      }
    });
  }
}