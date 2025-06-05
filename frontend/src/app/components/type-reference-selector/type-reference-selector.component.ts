import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';

@Component({
    selector: 'app-type-reference-selector',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatSelectModule
    ],
    templateUrl: './type-reference-selector.component.html',
    styleUrls: ['./type-reference-selector.component.scss']
})
export class TypeReferenceSelectorComponent implements OnInit {
    @Output() typeSelected = new EventEmitter<string>();
    @Input() labelText = 'Select Reference Type';

    typeControl = new FormControl('This');

    referenceTypes = [
        { value: 'This', label: 'This - Current instance of the class containing this method' },
        { value: 'Base', label: 'Base - Current instance of the class containing this method' },
        { value: 'Attribute', label: 'Attribute - Attribute from where invocation is realised' },
        { value: 'Parameter', label: 'Parameter - Method parameter value' },
        { value: 'LocalVariable', label: 'LocalVariable - Variable declared within method' },
        { value: 'Static', label: 'Static - Class containing statically invoked method' },
        { value: 'StaticAttribute', label: 'StaticAttribute - Static attribute from where invocation is realised' }
    ];

    ngOnInit(): void {
        this.typeSelected.emit(this.typeControl.value || 'This');

        this.typeControl.valueChanges.subscribe(value => {
            this.typeSelected.emit(value || 'This');
        });
    }
}