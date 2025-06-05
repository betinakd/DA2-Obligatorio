import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClassSelectorComponent } from '../class-selector/class-selector.component';
import { PrivacitySelectorComponent } from '../privacity-selector/privacity-selector.component';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { AttributeRequest } from '../../models/AttributeRequest';

@Component({
  selector: 'app-attribute-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCheckboxModule,
    ClassSelectorComponent,
    PrivacitySelectorComponent
  ],
  templateUrl: './attribute-list.component.html',
  styleUrls: ['./attribute-list.component.scss']
})
export class AttributeListComponent {
  @Input() relatedClassId: string = '';
  @Output() attributesChange = new EventEmitter<AttributeRequest[]>();

  attributes: AttributeRequest[] = [];
  loading: boolean = false;

  addAttribute(): void {
    const newAttribute: AttributeRequest = {
      id: '',
      name: '',
      referenceId: '',
      instanceId: '',
      privacity: 'Public',
      relatedClassId: this.relatedClassId,
      isStatic: false
    };

    this.attributes.push(newAttribute);
    this.attributesChange.emit([...this.attributes]);
  }

  onClassReferenceSelected(classId: string, index: number): void {
    if (this.attributes[index]) {
      this.attributes[index].referenceId = classId;
      this.attributesChange.emit([...this.attributes]);
    }
  }

  onClassInstanceSelected(classId: string, index: number): void {
    if (this.attributes[index]) {
      this.attributes[index].instanceId = classId;
      this.attributesChange.emit([...this.attributes]);
    }
  }

  onPrivacitySelected(value: string, i: number): void {
    if (this.attributes[i]) {
      this.attributes[i].privacity = value;
      this.attributesChange.emit([...this.attributes]);
    }
  }

  onStaticChanged(event: any, index: number): void {
    if (this.attributes[index]) {
      this.attributes[index].isStatic = event.checked;
      this.attributesChange.emit([...this.attributes]);
    }
  }

  onNameChanged(event: Event, index: number): void {
    if (this.attributes[index]) {
      const inputElement = event.target as HTMLInputElement;
      this.attributes[index].name = inputElement.value;
      this.attributesChange.emit([...this.attributes]);
    }
  }

  removeAttribute(index: number): void {
    this.attributes.splice(index, 1);
    this.attributesChange.emit([...this.attributes]);
  }
}