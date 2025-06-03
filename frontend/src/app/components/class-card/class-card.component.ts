import { Component, Input } from '@angular/core';
import { SimClass } from '../../models/SimClass.model';
import { ErrorResponse } from '../../models/ErrorResponse.model';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';

@Component({
  standalone: true,
  imports: [CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule],
  templateUrl: './class-card.component.html',
  styleUrls: ['./class-card.component.scss'],
  selector: 'app-class-card',
})

export class ClassCardComponent {
  @Input() simClass?: SimClass;
  @Input() error?: ErrorResponse;
  
  get isError(): boolean {
    return !!this.error;
  }
}