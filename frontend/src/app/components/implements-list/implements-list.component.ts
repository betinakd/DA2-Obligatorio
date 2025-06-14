import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { ClassSelectorComponent } from '../class-selector/class-selector.component';
import { Interface } from '../../models/request/Interface';

@Component({
  selector: 'app-implements-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    ClassSelectorComponent
  ],
  templateUrl: './implements-list.component.html',
  styleUrls: ['./implements-list.component.scss']
})

export class ImplementsListComponent {
  @Output() implementsChanged = new EventEmitter<Interface[]>();
  @Input() selectedClassId: string = '';
  interfaces: Interface[] = [];
  index: number = 0;

  addNewInterface(): void {
    const newInterface: Interface = {
      IdInterface: '',
    };
    this.interfaces.push(newInterface);
    this.implementsChanged.emit([...this.interfaces]);
  }

  onInterfaceSelected(value: string, i: number): void {
    this.interfaces[i].IdInterface = value;
    this.implementsChanged.emit([...this.interfaces])
  }

  removeInterface(index: number): void {
    this.interfaces.splice(index, 1);
    this.implementsChanged.emit([...this.interfaces]);
  }
}