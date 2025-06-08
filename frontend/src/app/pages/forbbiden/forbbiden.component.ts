import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-forbbiden',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './forbbiden.component.html',
  styleUrl: './forbbiden.component.scss'
})
export class ForbbidenComponent {
  // No hay corrección en el nombre del componente para mantener compatibilidad
  // pero vale la pena notar que "forbbiden" debería ser "forbidden"
}