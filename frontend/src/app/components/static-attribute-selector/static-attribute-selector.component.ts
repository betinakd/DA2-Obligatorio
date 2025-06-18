import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ClassService } from '../../services/class.service';
import { SimClassResponse } from '../../models/response/SimClassResponse';

@Component({
  selector: 'app-static-attribute-selector',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './static-attribute-selector.component.html',
  styleUrls: ['./static-attribute-selector.component.scss']
})
export class StaticAttributeSelectorComponent implements OnInit {
  @Output() attributeSelected = new EventEmitter<string>();
  @Input() labelText: string = 'Select Static Attribute';
  @Input() classId: string = '';
  @Input() reload = false;

  attributeControl = new FormControl('');
  classes: SimClassResponse[] = [];
  attributeOptions: { id: string, displayText: string }[] = [];
  loading = false;
  error: string | null = null;

  constructor(private classService: ClassService) { }

  ngOnInit(): void {
    this.loadClasses();

    this.attributeControl.valueChanges.subscribe(value => {
      this.attributeSelected.emit(value || '');
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
        this.processAttributeOptions();
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.error = 'Error al cargar clases';
        console.error('Error cargando clases:', err);
      }
    });
  }

  private getClassNameById(classId: string): string {
    const classObj = this.classes.find(c => c.id === classId)?.name;
    return classObj ? classObj : 'Unknown Class';
  }

  private processAttributeOptions(): void {
    const filteredClasses = this.classId
      ? this.classes.filter(c => c.id === this.classId)
      : this.classes;

    this.attributeOptions = [];

    filteredClasses.forEach(c => {
      if (c.attributes && c.attributes.length > 0) {
        const staticAttributes = c.attributes.filter(a => a.isStatic);

        staticAttributes.forEach(a => {
          const privacity = a.privacity;
          const attributeType = this.getClassNameById(a.referenceId || '');

          this.attributeOptions.push({
            id: a.id || '',
            displayText: `${c.name} - ${privacity} static ${attributeType} ${a.name}`
          });
        });
      }
    });
  }
}