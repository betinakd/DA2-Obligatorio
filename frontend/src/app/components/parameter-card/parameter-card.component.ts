import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';

import { Parameter } from '../../models/parameter.model';
import { ErrorResponse } from '../../models/ErrorResponse.model';

@Component({
  selector: 'app-parameter-card',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatChipsModule,
    MatTooltipModule
  ],
  templateUrl: './parameter-card.component.html',
  styleUrls: ['./parameter-card.component.scss']
})
export class ParameterCardComponent {
  @Input() parameter?: Parameter;
  @Input() error?: ErrorResponse;
  
  get isError(): boolean {
    return !!this.error;
  }
  
  get hasReferenceType(): boolean {
    return !!this.parameter?.referenceId;
  }
  
  get shortTypeName(): string {
    if (!this.parameter?.referenceId) return 'any';
    
    const parts = this.parameter.referenceId.split('.');
    return parts[parts.length - 1];
  }
}