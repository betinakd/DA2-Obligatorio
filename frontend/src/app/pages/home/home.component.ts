import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NamespaceService } from '../../services/namespace.service';
import { Namespace } from '../../models/namespace.model';
import { ErrorResponse } from '../../models/ErrorResponse.model';
import { NamespaceCardComponent } from '../../components/namespace-card/namespace-card.component';

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
  loading = false;
  error: ErrorResponse | null = null;

  constructor(private namespaceService: NamespaceService) {}
  
  ngOnInit(): void {
    this.getNamespaces();
  }

  getNamespaces(): void {
    this.loading = true;
    this.error = null;
    
    this.namespaceService.fetchNamespaces().subscribe({
      next: (data) => {
        this.namespaces = data;
        this.loading = false;
      },
      error: (err) => {
        const apiError = err.error as ErrorResponse;
        
        if (apiError?.innerCode !== undefined && apiError?.message) {
          this.error = apiError;
        } else {
          this.error = {
            innerCode: err.status || 0,
            message: err.message || 'unespected error charging namespaces'
          };
        }
        
        this.loading = false;
        console.error('Complete error:', err);
      }
    });
  }
}