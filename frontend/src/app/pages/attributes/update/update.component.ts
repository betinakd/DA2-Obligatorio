import { Component } from '@angular/core';
import { AttributeService } from '../../../services/attribute.service';
import { CommonModule } from '@angular/common';
import { AttributeItemComponent } from '../../../components/attribute-item/attribute-item.component';
import { AttributeRequest } from '../../../models/request/AttributeRequest';
import { AttributeSelectorTotalComponent } from '../../../components/attribute-selector-total/attribute-selector-total.component';

@Component({
  selector: 'app-update',
  standalone: true,
  imports: [AttributeSelectorTotalComponent, CommonModule, AttributeItemComponent],
  templateUrl: './update.component.html',
  styleUrl: './update.component.scss'
})
export class UpdateComponent {

  constructor(private attributeService: AttributeService) { }

  myAttribute: AttributeRequest = {
    id: '',
    name: '',
    idReference: '',
    idInstance: '',
    privacity: 'Public',
    idRelatedClass: '',
    isStatic: false
  };
  error: string = '';
  success: string = '';
  loading: boolean = false;

  onAttributeSelected(attributeId: string): void {
    this.myAttribute.id = attributeId;
  }

  onAttributeChanged(attribute: AttributeRequest): void {
    this.myAttribute.name = attribute.name;
    this.myAttribute.idReference = attribute.idReference;
    this.myAttribute.idInstance = attribute.idInstance;
    this.myAttribute.privacity = attribute.privacity;
    this.myAttribute.isStatic = attribute.isStatic;
  }

  onClassIdSelected(classId: string): void {
    this.myAttribute.idRelatedClass = classId;
  }

  clickUpdate() {
    this.loading = true;
    this.error = '';
    this.success = '';
    console.log('Updating attribute:', this.myAttribute);
    this.attributeService.updateAttribute(this.myAttribute)
      .subscribe({
        next: (response) => {
          this.success = `Attribute ${response.attribute?.name} updated successfully!`;
          this.error = '';
        },
        error: (err) => {
          this.error = 'Error: ' + (err.error?.message || err.message || 'Unknown error');
          this.loading = false;
          this.success = '';
        }
      });
  }
}
