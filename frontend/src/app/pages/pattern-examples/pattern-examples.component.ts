import { Component, Input, OnInit } from '@angular/core';
import { SimClassResponse } from '../../models/SimClassResponse';
import { TreefeatureGridComponent } from '../../components/treefeature-grid/treefeature-grid.component';
import { CommonModule } from '@angular/common';
import { ExecutionService } from '../../services/execution.service';
import { ClassService } from '../../services/class.service';
import { MethodExecutionRequest } from '../../models/request/MethodExecutionRequest';
import { AttributeResponse } from '../../models/attribute-response.model';
import { MethodResponse } from '../../models/method-response.model';
import { ParameterResponse } from '../../models/parameter-response.model';
import { VariableResponse } from '../../models/VariableResponse';
import { InvocationResponse } from '../../models/invocation-response.model';

@Component({
  selector: 'app-pattern-examples',
  imports: [TreefeatureGridComponent, CommonModule],
  templateUrl: './pattern-examples.component.html',
  styleUrl: './pattern-examples.component.scss'
})
export class PatternExamplesComponent implements OnInit {
  @Input() patternName = '';
  @Input() patternIdClasses: string[] = [];
  @Input() executionInfo: MethodExecutionRequest = {
    methodName: '',
    parameters: [],
    idReferenceType: '',
    idInstanceType: '',
    idReturnType: ''
  };

  classList: any[] = [];
  class: SimClassResponse | null = null;
  methodList: any[] = [];

  executionOutput = 'empty execution output';

  typesList: SimClassResponse[] = [];


  constructor(private classService: ClassService, private executionService: ExecutionService) { }
  ngOnInit(): void {
    this.getExecutionOutput();
    for (let i = 0; i < this.patternIdClasses.length; i++) {
      this.getClasses(this.patternIdClasses[i]);
    }
    this.GetTypes();
  }

  getExecutionOutput() {
    this.executionService.executeMethod(this.executionInfo).subscribe({
      next: (response) => {
        this.executionOutput = String(response.execution);
      },
      error: (error) => {
        this.executionOutput = 'Error loading execution information. Message: ' + error.message;
      }
    });
  }

  getClasses(objectId: string): void {
    this.classService.getClass(objectId).subscribe({
      next: (data) => {
        const classData =
        {
          "id": data.id,
          "name": data.name,
          "idBaseClass": data.idBaseClass,
          "state": data.state,
          "methods": data.methods.map(method => method.name).filter((name): name is string => typeof name === 'string'),
          "attributes": this.parseAttributesData(data.attributes),
          "implements": data.implements.map((iface) => {
            return { name: iface.name, id: iface.id };
          }),
        }
        this.classList.push(classData);
        data.methods.forEach((method) => {
          var parsedMethod = this.parseMethodData(method, data.name || 'Unknown Class');
          this.methodList.push(parsedMethod);
        });

      }
    });
  }
  parseMethodData(rawMehtod: MethodResponse, name: string): string {
    const parsedMethod: any = {
      name: rawMehtod.name || 'Unknown Method',
      methodScript: '',
      class: '',
      localVariables: [],
      invocations: []
    }

    if (rawMehtod) {
      const returnType = this.parseTypeData(rawMehtod.returnTypeId ?? ' ');
      const parameters = this.parsedParametersData(rawMehtod.parameters);
      const modifier = this.parseModifier(rawMehtod);

      parsedMethod.methodScript = `${rawMehtod.privacity} ${rawMehtod.accesibility} ${modifier}${rawMehtod.name}(${parameters}): ${returnType}`;
    }
    parsedMethod.localVariables = this.parseLocalVariablesData(rawMehtod.variables);
    parsedMethod.invocations = this.parseMethodInvocationData(rawMehtod.invocations);
    parsedMethod.class = name || 'Unknown Class';
    return parsedMethod;

  }

  parseMethodInvocationData(methodInvocations: InvocationResponse[]): any[] {
    let parsedInvocations: any[] = [];
    if (!methodInvocations || methodInvocations.length === 0) {
      parsedInvocations = ['No method invocations found'];
    }
    for (let i = 0; i < methodInvocations.length; i++) {
      const invocation = methodInvocations[i];
      const parameters = this.parsedParametersData(invocation.parameters || []);
      const invocationInfo = invocation.typeReference + '.' + invocation.methodName + '(' + parameters + ')';
      parsedInvocations.push(invocationInfo);
    }
    return parsedInvocations;
  }

  parseLocalVariablesData(localVariables: VariableResponse[]): string[] {
    let parsedLocalVariables: string[] = [];
    if (!localVariables || localVariables.length === 0) {
      parsedLocalVariables = ['No local variables found'];
    }
    for (let i = 0; i < localVariables.length; i++) {
      const localVariable = localVariables[i];
      const type = this.parseTypeData(localVariable.referenceId ?? 'Unknown Type');
      const name = localVariable.name || 'Unknown Variable';
      const localVar = `${name}: ${type}`;
      parsedLocalVariables.push(localVar);
    }
    return parsedLocalVariables;
  }

  parseModifier(rawMethod: MethodResponse): string {
    let modifier = '';
    if (rawMethod.isStatic) {
      modifier = 'static';
    }
    if (rawMethod.isVirtual) {
      if (rawMethod.isOverride) {
        modifier = 'override';
      } else {
        modifier = 'virtual';
      }
    }
    return modifier;
  }

  parsedParametersData(parameters: ParameterResponse[]): string {
    let returnInfo = [];
    if (parameters != null) {
      for (let i = 0; i < parameters.length; i++) {
        const param = parameters[i];
        const type = this.parseTypeData(param.referenceId ?? 'Unknown Type');
        const name = param.name || 'Unknown Parameter';
        returnInfo.push(` ${name}: ${type}`);
      }
    }
    return returnInfo.join(',');
  }

  GetTypes(): void {
    this.classService.getAllClassesForPatterns().subscribe({
      next: (data) => {
        this.typesList = data;
      },
    }
    );
  }

  parseTypeData(typeId: string | undefined): string {
    if (!typeId) {
      return 'Unknown Type';
    }
    const type = this.typesList.filter(item => item.id === typeId)[0];
    return type && type.name ? String(type.name) : 'Unknown Type';
  }

  parseAttributesData(attributes: AttributeResponse[]): string[] {
    var parsedAttr: string[] = [];
    if (!attributes || attributes.length === 0) {
      var emptyAttr: string = 'No attributes found';
      return parsedAttr;
    }
    for (let i = 0; i < attributes.length; i++) {
      const attr = attributes[i];

      const reference = this.parseTypeData(attr.referenceId ?? 'Unknown Type');
      const instance = this.parseTypeData(attr.instanceId ?? 'Unknown Type');
      const isStatic = attr.isStatic ? 'static' : '';

      const parsedAttrI = '' + attr?.privacity + ' ' + isStatic + ' ' + reference + ' ' + attr?.name + ': ' + instance;
      parsedAttr.push(parsedAttrI);
    }
    return parsedAttr
  }

}
