import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NamespaceService } from '../../../services/namespace.service';
import { NamespaceSelectorComponent } from '../../../components/namespace-selector/namespace-selector.component';
import { NamespaceRequest } from '../../../models/request/NamespaceRequest.model';
import { CreatedNamespaceResponse } from '../../../models/response/CreatedNamespaceResponse';

@Component({
  selector: 'app-create',
  standalone: true,
  imports: [CommonModule, NamespaceSelectorComponent],
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss'
})
export class CreateComponent {
  loading = false;
  error = '';
  name = '';
  baseNamespaceId = '';
  successMessage = '';
  createdNamespace: CreatedNamespaceResponse | null = null;

  constructor(private namespaceService: NamespaceService) { }


  onNamespaceSelected(value: string): void {
    this.baseNamespaceId = value;
  }

  handleButtonClick() {
    const namespaceData: NamespaceRequest = {
      name: this.name,
      baseNamespaceId: this.baseNamespaceId || null
    };

    this.successMessage = '';

    this.namespaceService.createNamespace(namespaceData).subscribe({
      next: (response) => {
        this.successMessage =
          response.message +
          ' - ID: ' + response.namespaceResponse.id +
          ' - Name: ' + response.namespaceResponse.name +
          (response.namespaceResponse.baseNamespaceId ? ' - Base Namespace ID: ' + response.namespaceResponse.baseNamespaceId : '');
        this.loading = true;
        this.createdNamespace = response;
        this.error = '';
        this.name = '';
      },
      error: (err) => {
        this.error = 'Error: ' + (err.error?.message || 'Unknown error');
        this.loading = false;
        this.successMessage = '';
      }
    });
  }
}