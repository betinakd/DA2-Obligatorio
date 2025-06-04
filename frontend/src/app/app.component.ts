import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RouterModule } from '@angular/router';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { DataService } from './services/data.service';
import { HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, SidebarComponent, RouterModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'frontend';
  loading = true;
  error: string | null = null;
  apiUrl = 'http://localhost:5128/api/v1';

  constructor(private dataService: DataService, private http: HttpClient) { }

  ngOnInit(): void {
    console.log('Inicializando aplicación...');

    this.http.get(this.apiUrl + '/health-check').pipe(
      catchError(() => of({ status: 'API posiblemente no disponible' }))
    ).subscribe(result => {
      console.log('Estado del backend:', result);
    });

    this.dataService.loadAllData().subscribe({
      next: () => {
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Error cargando datos: ' + (err.message || 'Error desconocido');
        this.loading = false;
        console.error('Error cargando datos:', err);
      }
    });

    console.log('Iniciando diagnóstico de carga de datos...');

    this.http.get('http://localhost:5128/api/v1/namespaces').subscribe({
      next: data => console.log('/namespaces funciona:', data),
      error: err => console.error('/namespaces error:', err)
    });

    this.http.get('http://localhost:5128/api/v1/simulator/namespaces').subscribe({
      next: data => console.log('/simulator/namespaces funciona:', data),
      error: err => console.error('/simulator/namespaces error:', err)
    });

    this.http.get('http://localhost:5128/api/v1/classes').subscribe({
      next: data => console.log('/classes funciona:', data),
      error: err => console.error('/classes error:', err)
    });
  }
}
