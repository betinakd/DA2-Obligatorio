import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClassSelectorComponent } from '../../../components/class-selector/class-selector.component';
import { AttributeItemComponent } from '../../../components/attribute-item/attribute-item.component';
import { AttributeRequest } from '../../../models/request/AttributeRequest';
import { AttributeService } from '../../../services/attribute.service';

@Component({
  selector: 'app-create',
  imports: [ClassSelectorComponent, CommonModule, AttributeItemComponent
  ],
  standalone: true,
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss'
})
export class CreateComponent {

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

  onAttributeChanged(attribute: AttributeRequest): void {
    this.myAttribute = attribute;
  }

  onClassSelected(value: string): void {
    this.myAttribute.idRelatedClass = value;
  }

  clickCreate(): void {
    this.attributeService.createAttribute(this.myAttribute.idRelatedClass, this.myAttribute)
      .subscribe({
        next: (response) => {
          this.success = `Attribute created successfully!\n\n${JSON.stringify(response.attribute, null, 2)}`;
          this.error = '';
          this.resetForm();
        },
        error: (err) => {
          this.error = 'Error: ' + (err.error?.message || 'Wrong data format. Please check the input fields.');
          this.loading = false;
          this.success = '';
        }
      });
  }

  resetForm(): void {
    this.myAttribute = {
      id: '',
      name: '',
      idReference: '',
      idInstance: '',
      privacity: 'Public',
      idRelatedClass: '',
      isStatic: false
    };
  }

}
