import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClassSelectorComponent } from '../class-selector/class-selector.component';
import { PrivacitySelectorComponent } from '../privacity-selector/privacity-selector.component';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { AttributeRequest } from '../../models/request/AttributeRequest';

@Component({
  selector: 'app-attribute-item',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCheckboxModule,
    ClassSelectorComponent,
    PrivacitySelectorComponent
  ],
  templateUrl: './attribute-item.component.html',
  styleUrls: ['./attribute-item.component.scss']
})
export class AttributeItemComponent {
  @Input() relatedClassId: string = '';
  @Input() attribute: AttributeRequest = {
    id: '',
    name: '',
    idReference: '',
    idInstance: '',
    privacity: 'Public',
    idRelatedClass: '',
    isStatic: false
  };
  @Output() attributeChange = new EventEmitter<AttributeRequest>();

  loading: boolean = false;

  onNameChanged(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    this.attribute.name = inputElement.value;
    this.emitChange();
  }

  onClassReferenceSelected(classId: string): void {
    this.attribute.idReference = classId;
    this.emitChange();
  }

  onClassInstanceSelected(classId: string): void {
    this.attribute.idInstance = classId;
    this.emitChange();
  }

  onPrivacitySelected(value: string): void {
    this.attribute.privacity = value;
    this.emitChange();
  }

  onStaticChanged(event: any): void {
    this.attribute.isStatic = event.checked;
    this.emitChange();
  }

  private emitChange(): void {
    this.attributeChange.emit({ ...this.attribute });
  }
}