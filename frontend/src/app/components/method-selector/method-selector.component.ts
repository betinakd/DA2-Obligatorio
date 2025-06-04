import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';

import { Method } from '../../models/method.model';
import { DataService } from '../../services/data.service';

@Component({
    selector: 'app-method-selector',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatSelectModule,
        MatProgressSpinnerModule,
        MatButtonModule
    ],
    template: `
    <div class="selector-container">
      <div *ngIf="loading" class="loading-indicator">
        <mat-spinner diameter="24"></mat-spinner>
        <span>Cargando métodos...</span>
      </div>
      
      <div *ngIf="error && !loading" class="error-message">
        <p>{{ error }}</p>
        <button mat-raised-button color="primary" (click)="loadMethods()">
          Intentar de nuevo
        </button>
      </div>
      
      <mat-form-field *ngIf="!loading && !error" appearance="outline" class="full-width">
        <mat-label>{{ labelText }}</mat-label>
        <mat-select [formControl]="methodControl">
          <mat-option value="">Seleccionar Método</mat-option>
          <mat-option *ngFor="let method of methods" [value]="method.id">
            {{ method.name }}
          </mat-option>
        </mat-select>
      </mat-form-field>
    </div>
  `,
    styles: [`
    .selector-container {
      background-color: rgb(4, 4, 4);
      border-radius: 8px;
      padding: 16px;
    }
    
    .loading-indicator {
      display: flex;
      align-items: center;
      gap: 10px;
      color: #c59dd5;
      padding: 8px 0;
    }
    
    .error-message {
      color: #f44336;
      padding: 8px 0;
    }
    
    .full-width {
      width: 100%;
    }
  `]
})
export class MethodSelectorComponent implements OnInit, OnChanges {
    @Output() methodSelected = new EventEmitter<string>();
    @Input() labelText: string = 'Seleccionar Método';
    @Input() classId: string = '';

    methodControl = new FormControl('');
    methods: Method[] = [];
    loading = false;
    error: string | null = null;

    constructor(private dataService: DataService) { }

    ngOnInit(): void {
        this.methodControl.valueChanges.subscribe(value => {
            this.methodSelected.emit(value || '');
        });
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes['classId'] &&
            changes['classId'].currentValue !== changes['classId'].previousValue) {
            this.methodControl.reset('');
            this.loadMethods();
        }
    }

    loadMethods(): void {
        if (!this.classId) {
            this.methods = [];
            return;
        }

        this.loading = true;
        this.error = null;

        this.dataService.loadAllData().subscribe({
            next: () => {
                const methodsForClass = this.dataService.getMethodsByClassId(this.classId);

                this.methods = methodsForClass;
                this.loading = false;

                console.log(`⚡ Métodos para la clase ${this.classId}:`, this.methods);

                if (this.methods.length === 0) {
                    this.error = 'No se encontraron métodos para esta clase';
                }
            },
            error: (err) => {
                this.error = 'Error cargando métodos: ' + (err.message || 'Error desconocido');
                this.loading = false;
                console.error('Error:', err);
            }
        });
    }
}