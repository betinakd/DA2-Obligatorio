import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormClassComponent } from '../../../components/form-create-class/form-class.component';

@Component({
  selector: 'app-create',
  imports: [CommonModule, FormClassComponent],
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss'
})
export class CreateComponent {

}
