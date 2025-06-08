import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NamespaceService } from '../../services/namespace.service';
import { Namespace } from '../../models/namespace.model';
import { ErrorResponse } from '../../models/ErrorResponse.model';
import { NamespaceCardComponent } from '../../components/namespace-card/namespace-card.component';
import { ClassService } from '../../services/class.service';
import { SimClassResponse } from '../../models/SimClassResponse';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule,
    NamespaceCardComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  namespaces: Namespace[] = [];
  classes: SimClassResponse[] = [];
  loading = false;
  error: string = '';
  // Agregando propiedades faltantes
  classId: string | null = null;
  methodOptions: { id: string, displayText: string }[] = [];
  // Para mostrar la información de namespaces de forma amigable
  processedNamespaces: any[] = [];

  constructor(private namespaceService: NamespaceService, private classService: ClassService) { }

  ngOnInit(): void {
    this.getNamespaces();
    this.loadClasses();
  }

  getNamespaces(): void {
    this.loading = true;
    this.error = '';
    this.namespaceService.fetchNamespaces().subscribe({
      next: (data) => {
        this.namespaces = data;
        this.processNamespaceData(); // Procesar datos para mostrar de forma amigable
        this.loading = false;
      },
      error: (err) => {
        const apiError = err.error as ErrorResponse;
        this.loading = false;
        console.error('Complete error:', err);
      }
    });
  }

  loadClasses(): void {
    this.loading = true;
    this.error = '';

    this.classService.getAllClasses().subscribe({
      next: (data) => {
        this.classes = data;
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.error = 'Error al cargar métodos';
        console.error('Error cargando clases:', err);
      }
    });
  }

  private getClassNameById(classId: string): string {
    const classObj = this.classes.find(c => c.id === classId)?.name;
    return classObj ? classObj : 'Unknown Class';
  }

  private processNamespaceData(): void {
    // Reiniciar
    this.processedNamespaces = [];

    // Mapa rápido de id→namespace name (si lo necesitas)
    const nsMap = new Map<string, string>();
    this.namespaces.forEach(ns => nsMap.set(ns.id, ns.name));

    for (const ns of this.namespaces) {
      this.processedNamespaces.push({
        id: ns.id,
        name: ns.name,
        baseNamespace: ns.baseNamespaceId
          ? nsMap.get(ns.baseNamespaceId) || 'Desconocido'
          : 'Ninguno',
        classes: ns.elements.map(el => this.processSimClass(el))
      });
    }
  }

  // Convierte un SimClassResponse en un objeto amigable
  private processSimClass(sc: SimClassResponse): any {
    // Nombre de la clase base
    const baseClass = sc.idBaseClass
      ? this.getClassNameById(sc.idBaseClass)
      : null;

    // Atributos: si tienen referenceId, sustituye por nombre
    const attributes = sc.attributes.map(attr => ({
      name: attr.name,
      type: attr.referenceId
        ? this.getClassNameById(attr.referenceId)
        : 'Unknown Type'
    }));

    // Métodos: parámetros y tipo de retorno con nombres
    const methods = sc.methods.map(m => {
      const returnType = m.returnTypeId
        ? this.getClassNameById(m.returnTypeId)
        : 'void';

      const parameters = (m.parameters || []).map(p => ({
        name: p.name,
        type: p.referenceId
          ? this.getClassNameById(p.referenceId)
          : 'Unknown Type'
      }));

      const paramsText = parameters
        .map(p => `${p.name}: ${p.type}`)
        .join(', ');

      return {
        name: m.name,
        returnType,
        parameters,
        displayText: `${m.name}(${paramsText}): ${returnType}`
      };
    });

    // Interfaces implementadas
    const impls = sc.implements.map(i => i.name);

    return {
      id: sc.id,
      name: sc.name,
      baseClass,
      attributes,
      methods,
      implements: impls
    };
  }
}
