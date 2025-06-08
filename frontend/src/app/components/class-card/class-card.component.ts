import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-class-card',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './class-card.component.html',
    styleUrl: './class-card.component.scss'
})
export class ClassCardComponent implements OnInit {
    @Input() classData: any;
    @Input() cardColor: string = '#7d1db5';
    @Input() allClasses: any[] = [];
    @Input() allNamespaces: any[] = [];

    showMethods: boolean = true;
    showAttributes: boolean = true;
    showMethodDetails: { [methodId: string]: boolean } = {};

    private classIdToNameMap: Map<string, string> = new Map();
    private namespaceIdToNameMap: Map<string, string> = new Map();

    constructor() { }

    ngOnInit(): void {
        if (this.classData?.methods) {
            this.classData.methods.forEach((method: any) => {
                this.showMethodDetails[method.id] = false;
            });
        }
        // Opcional: precargar mapas para rendimiento
        this.allClasses.forEach(cls => this.classIdToNameMap.set(cls.id, cls.name));
        this.allNamespaces.forEach(ns => this.namespaceIdToNameMap.set(ns.id, ns.name));
    }

    // Devuelve la firma UML de un método: nombre(param1: Tipo1, param2: Tipo2): TipoRetorno
    getMethodUMLSignature(method: any): string {
        const params = (method.parameters || [])
            .map((p: any) => `${p.name}: ${this.getTypeName(p.type)}`)
            .join(', ');
        const returnType = this.getTypeName(method.returnType);
        return `${method.name}(${params}): ${returnType}`;
    }

    // Devuelve los atributos de la clase en formato UML: nombre: Tipo
    getAttributeUML(attribute: any): string {
        return `${attribute.name}: ${this.getTypeName(attribute.type)}`;
    }

    // Devuelve el nombre del tipo, buscando en allClasses y allNamespaces si es un ID
    getTypeName(typeIdOrName: string): string {
        if (!typeIdOrName) return 'void';
        if (this.classIdToNameMap.has(typeIdOrName)) {
            return this.classIdToNameMap.get(typeIdOrName)!;
        }
        if (this.namespaceIdToNameMap.has(typeIdOrName)) {
            return this.namespaceIdToNameMap.get(typeIdOrName)!;
        }
        return typeIdOrName;
    }
}