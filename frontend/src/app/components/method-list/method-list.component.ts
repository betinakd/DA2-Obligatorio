import { Component, EventEmitter, Output } from '@angular/core';
import { MethodCreateComponent } from '../method-create/method-create.component';
import { CommonModule } from '@angular/common';
import { MethodRequest } from '../../models/request/MethodRequest';

@Component({
  selector: 'app-method-list',
  standalone: true,
  imports: [MethodCreateComponent, CommonModule],
  templateUrl: './method-list.component.html',
  styleUrl: './method-list.component.scss'
})
export class MethodListComponent {
  @Output() methodsChange = new EventEmitter<MethodRequest[]>();
  
  methods: MethodRequest[] = [];

  addMethod(): void {
    console.log('addMethod called, antes:', this.methods.length);
    this.methods.push(new MethodRequest());
    console.log('después:', this.methods.length);
    this.emitChange();
  }

  removeMethod(index: number): void {
    this.methods.splice(index, 1);
    this.emitChange();
  }

  onMethodChange(updated: MethodRequest, index: number): void {
    this.methods[index] = updated;
    this.emitChange();
  }

  private emitChange(): void {
    this.methodsChange.emit(this.methods);
  }
}