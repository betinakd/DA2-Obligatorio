import { Component, Input, OnInit } from '@angular/core';
import { SimClassResponse } from '../../models/response/SimClassResponse';
import { TreefeatureGridComponent } from '../../components/treefeature-grid/treefeature-grid.component';
import { CommonModule } from '@angular/common';
import { ExecutionService } from '../../services/execution.service';
import { ClassService } from '../../services/class.service';
import { MethodExecutionRequest } from '../../models/request/MethodExecutionRequest';

import { AttributeResponse } from '../../models/response/AttributeResponse';
import { MethodResponse } from '../../models/response/MethodResponse';
import { ParameterResponse } from '../../models/response/ParameterResponse.model';
import { VariableResponse } from '../../models/response/VariableResponse';
import { InvocationResponse } from '../../models/response/InvocationResponse.model';
import { MatFormField, MatLabel, MatOption, MatSelect } from '@angular/material/select';

@Component({
  selector: 'app-pattern-examples',
  imports: [TreefeatureGridComponent, CommonModule, MatSelect, MatFormField, MatLabel, MatOption],
  templateUrl: './pattern-examples.component.html',
  styleUrl: './pattern-examples.component.scss'
})
export class PatternExamplesComponent implements OnInit {
  @Input() patternName = '';
  @Input() patternIdClasses: string[] = [];
  @Input() executionInfo: MethodExecutionRequest[] = [{
    methodName: '',
    parameters: [],
    idReferenceType: '',
    idInstanceType: '',
    idReturnType: ''
  }];
  @Input() executionsOptions: string[] = [];

  classList: any[] = [];
  class: SimClassResponse | null = null;
  methodList: any[] = [];

  executionOutput = 'Execution is not available yet.';

  classes: SimClassResponse[] = [];


  constructor(private classService: ClassService, private executionService: ExecutionService) { }
  ngOnInit(): void {
    this.loadClasses().then(() => {
      this.getExecutionOutput();
      for (let i = 0; i < this.patternIdClasses.length; i++) {
        this.getPatternClasses(this.patternIdClasses[i]);
      }
    });
  }

  loadClasses(): Promise<void> {
    return new Promise((resolve) => {
      this.classService.getAllClasses().subscribe({
        next: (data) => {
          this.classes = data;
          resolve();
        },
        error: () => resolve()
      });
    });
}

  getExecutionOutput(index: number = 0): void {
    const info = this.executionInfo[index];
    const request: MethodExecutionRequest = {
      methodName: info.methodName,
      parameters: info.parameters,
      idReferenceType: info.idReferenceType,
      idInstanceType: info.idInstanceType,
      idReturnType: info.idReturnType
    };
    this.executionService.executeMethod(info).subscribe({
      next: (response) => {
      this.executionOutput = String(response.execution);
      },
      error: (error) => {
        this.executionOutput = 'Execution is not available yet.';
    }
    });
  }

  /*loadClasses(): void {
    this.classService.getAllClasses().subscribe({
      next: (data) => {
        this.classes = data;
      },
    });
  }*/
  private getClassNameById(classId: string): string {
    const classObj = this.classes.find(c => c.id === classId)?.name;
    return classObj ? classObj : '';
  }

  getPatternClasses(id : string): void {
    this.classService.getClass(id).subscribe({
      next: (data) => {
        this.class = data;
        for (const method of this.class.methods || []) {
          const processedMethod = this.processSimMethod(method, data.name ?? 'Unknown Class');
          this.methodList.push(processedMethod);
        }
        var parsedClass = this.processSimClass(this.class);
        this.classList.push(parsedClass);
      }
    });
  }

  private processSimClass(sc: SimClassResponse): any {
    const name = sc.name || 'Unknown Class';
    const baseClass = sc.idBaseClass
      ? this.getClassNameById(sc.idBaseClass)
      : 'Unknown Base Class';

    const attributes = sc.attributes.map(attr => ({
      name: attr.name,
      reference: attr.referenceId
        ? this.getClassNameById(attr.referenceId)
        : 'Unknown Type',
      instance: attr.instanceId
        ? this.getClassNameById(attr.instanceId)
        : 'Unknown Instance',
      privacity: attr.privacity,
      static: attr.isStatic ? 'static' : ''
    }));

    const methods = sc.methods.map(m => ({
        name: m.name
    }));

    const _implements = sc.implements.map(i => i.name);

    return {
      name: name,
      baseClass: baseClass,
      attributes: attributes,
      methods: methods,
      implements: _implements
    };
  }

  private processSimMethod(m: MethodResponse, className : string): any {
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

    const isStatic = m.isStatic;
    const isVirtual = m.isVirtual;
    const isOverride = m.isOverride;
    const priv = m.privacity;
    const acc = m.accesibility;
    const prefixes = [
      priv,
      acc,
      isStatic ? 'static' : null,
      isVirtual ? 'virtual' : null,
      isOverride ? 'override' : null
    ].filter(x => !!x).join(' ');

    const variables = (m.variables || []).map(v => ({
      name: v.name,
      reference: v.referenceId
        ? this.getClassNameById(v.referenceId)
        : 'Unknown Type',
      instance: v.instanceId
        ? this.getClassNameById(v.instanceId)
        : 'Unknown Instance'
    }));

    const rawInvs = m.invocations || [];

    const invocations = rawInvs.map(inv => {
      const invParams = (inv.parameters || [])
        .map(p => {
          const pt = p.referenceId
            ? this.getClassNameById(p.referenceId)
            : 'Unknown Type';
          const pi = p.instanceId
            ? this.getClassNameById(p.instanceId)
            : '';
          return `${p.name}: ${pt}${pi ? ' ' + pi : ''}`;
        })
        .join(', ');

      const refName = this.getInvocationReferenceName(inv);
      return `${refName}.${inv.methodName}(${invParams})`;
    });

    return {
      class: className,
      name: m.name,
      returnType,
      parameters,
      isStatic,
      isVirtual,
      isOverride,
      displayText: `${prefixes} ${m.name}(${paramsText}): ${returnType}`.trim(),
      invocations,
      variables
    };
  }

  private getInvocationReferenceName(inv: any): string {
    switch (inv.typeReference) {
      case 'Attribute':
      case 'StaticAttribute': {
        for (const cls of this.classes) {
          const attr = (cls.attributes || []).find((a: any) => a.id === inv.idReference);
          if (attr?.name) return attr.name;
        }
        return 'Unknown Attribute';
      }
      case 'Parameter': {
        for (const cls of this.classes) {
          for (const m of cls.methods || []) {
            const param = (m.parameters || []).find((p: any) => p.id === inv.idReference);
            if (param?.name) return param.name;
          }
        }
        return 'Unknown Parameter';
      }
      case 'LocalVariable': {
        for (const cls of this.classes) {
          for (const m of cls.methods || []) {
            const variable = (m.variables || []).find((v: any) => v.id === inv.idReference);
            if (variable?.name) return variable.name;
          }
        }
        return 'Unknown Variable';
      }
      case 'Base':
      case 'This':
      case 'Static': {
        const className = this.getClassNameById(inv.idReference);
        return className || 'Unknown Class';
      }
      default:
        return 'Unknown Reference';
    }
  }
}
