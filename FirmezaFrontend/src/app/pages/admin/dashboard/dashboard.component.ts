import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { DashboardService, DashboardStats } from '../../../services/dashboard.service';
import { AuthService } from '../../../services/auth.service';

@Component({
    selector: 'app-dashboard',
    standalone: true,
    imports: [CommonModule, RouterLink],
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
    stats: DashboardStats | null = null;
    loading: boolean = true;

    constructor(
        private dashboardService: DashboardService,
        private authService: AuthService,
        private router: Router
    ) { }

    ngOnInit() {
        this.loadStats();
    }

    loadStats() {
        this.dashboardService.getDashboardStats().subscribe({
            next: (data) => {
                this.stats = data;
                this.loading = false;
            },
            error: (err) => {
                console.error('Error loading stats:', err);
                this.loading = false;
            }
        });
    }

    logout() {
        this.authService.logout();
    }
}
