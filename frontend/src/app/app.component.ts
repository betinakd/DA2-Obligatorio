import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RouterModule } from '@angular/router';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { FooterComponent } from './components/footer/footer.component';
import { API_KEYS } from './shared/constants/api-keys';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, SidebarComponent, RouterModule, FooterComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'Simulator';

  ngOnInit(): void {
    localStorage.setItem('authToken', API_KEYS.KEY1);
    console.log('Token set in localStorage start aplication.');
  }
}
