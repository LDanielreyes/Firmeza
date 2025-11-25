import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.css']
})
export class NavbarComponent implements OnInit {
  isLoggedIn$: Observable<boolean> | undefined; // Using Observable for reactive updates if needed, but AuthService currently doesn't expose isLoggedIn as observable directly, but userRole$ can be used.
  userRole$: Observable<string | null>;

  constructor(public authService: AuthService) {
    this.userRole$ = this.authService.userRole$;
  }

  ngOnInit() {
    // Logic to initialize if needed
  }

  logout() {
    this.authService.logout();
  }
}
