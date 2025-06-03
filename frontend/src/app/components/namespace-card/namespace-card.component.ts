import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

import { Namespace } from '../../models/namespace.model';
import { ErrorResponse } from '../../models/ErrorResponse.model';

@Component({
  selector: 'app-namespace-card',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule
  ],
  templateUrl: './namespace-card.component.html',
  styleUrls: ['./namespace-card.component.scss']
})
export class NamespaceCardComponent {
  @Input() namespace?: Namespace;
  @Input() error?: ErrorResponse;
  
  get isError(): boolean {
    return !!this.error;
  }
  
  get hasParentNamespace(): boolean {
    return !!this.namespace?.baseNamespaceId;
  }
}