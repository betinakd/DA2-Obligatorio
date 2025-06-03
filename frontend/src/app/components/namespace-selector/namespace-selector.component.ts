import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { NamespaceService } from '../../services/namespace.service';
import { Namespace } from '../../models/namespace.model';

@Component({
  selector: 'app-namespace-selector',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './namespace-selector.component.html',
  styleUrls: ['./namespace-selector.component.scss']
})
export class NamespaceSelectorComponent implements OnInit {
  @Output() namespaceSelected = new EventEmitter<string | null>();
  @Input() labelText = 'Seleccionar Namespace';
  
  namespaceControl = new FormControl('');
  namespaces: Namespace[] = [];
  loading = false;
  error: string | null = null;
  
  constructor(private namespaceService: NamespaceService) {}
  
  ngOnInit(): void {
    this.loadNamespaces();
    
    this.namespaceControl.valueChanges.subscribe(value => {
      this.namespaceSelected.emit(value);
    });
  }
  
  loadNamespaces(): void {
    this.loading = true;
    this.error = null;
    
    this.namespaceService.fetchNamespaces().subscribe({
      next: (data) => {
        this.namespaces = data;
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.error = 'Error al cargar namespaces';
        console.error('Error cargando namespaces:', err);
      }
    });
  }
}