import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TransformersService } from '../../../services/transformers.service';

@Component({
  selector: 'app-transformer-get',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './transformer-get.component.html',
  styleUrl: './transformer-get.component.scss'
})
export class TransformerGetComponent implements OnInit {
  transformers: string[] = [];
  loading: boolean = false;
  error: string = '';

  constructor(private transformersService: TransformersService) { }

  ngOnInit(): void {
    this.loadTransformers();
  }

  loadTransformers(): void {
    this.loading = true;
    this.transformersService.getTransformers().subscribe({
      next: (data) => {
        this.transformers = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Error loading transformers: ' + (err.message || 'Unknown error');
        console.error('Error fetching transformers:', err);
        this.loading = false;
      }
    });
  }

  selectTransformer(transformer: string): void {
    console.log('Selected transformer:', transformer);
  }
}