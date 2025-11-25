import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class HomeComponent {
  isLoggedIn: boolean = false;

  constructor(private authService: AuthService) {
    this.isLoggedIn = this.authService.isLoggedIn();
  }
}
