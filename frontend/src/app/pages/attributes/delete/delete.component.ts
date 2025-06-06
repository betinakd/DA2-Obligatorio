import { Component } from '@angular/core';
import { AttributeSelectorTotalComponent } from '../../../components/attribute-selector-total/attribute-selector-total.component';
import { CommonModule } from '@angular/common';
import { AttributeService } from '../../../services/attribute.service';

@Component({
  selector: 'app-delete',
  standalone: true,
  imports: [AttributeSelectorTotalComponent, CommonModule],
  templateUrl: './delete.component.html',
  styleUrl: './delete.component.scss'
})
export class DeleteComponent {

  constructor(private attributeService: AttributeService) { }

  attributeId: string = '';
  error: string = '';
  success: string = '';
  loading: boolean = false;

  onAttributeSelected(attributeId: string): void {
    this.attributeId = attributeId;
  }

  clickDelete(): void {
    this.attributeService.deleteAttribute(this.attributeId)
      .subscribe({
        next: (response) => {
          this.success = `Attribute ${this.attributeId} deleted successfully!`;
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
