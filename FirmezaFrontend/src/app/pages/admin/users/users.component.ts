import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../../services/user.service';
import { AuthService } from '../../../services/auth.service';

export interface User {
    id: number;
    email: string;
    firstName?: string;
    lastName?: string;
    role?: string;
}

@Component({
    selector: 'app-users',
    standalone: true,
    imports: [CommonModule, RouterLink, FormsModule],
    templateUrl: './users.component.html',
    styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {
    users: User[] = [];
    loading: boolean = true;
    searchTerm: string = '';

    constructor(
        private userService: UserService,
        private authService: AuthService,
        private router: Router
    ) { }

    ngOnInit() {
        this.loadUsers();
    }

    loadUsers() {
        this.loading = true;
        this.userService.getUsers().subscribe({
            next: (data) => {
                this.users = data;
                this.loading = false;
            },
            error: (err) => {
                console.error('Error loading users:', err);
                this.loading = false;
            }
        });
    }

    get filteredUsers() {
        if (!this.searchTerm) {
            return this.users;
        }
        return this.users.filter(user =>
            user.email.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
            (user.firstName && user.firstName.toLowerCase().includes(this.searchTerm.toLowerCase())) ||
            (user.lastName && user.lastName.toLowerCase().includes(this.searchTerm.toLowerCase()))
        );
    }

    deleteUser(id: number) {
        if (confirm('Are you sure you want to delete this user?')) {
            this.userService.deleteUser(id).subscribe({
                next: () => {
                    this.loadUsers();
                },
                error: (err) => {
                    console.error('Error deleting user:', err);
                    alert('Failed to delete user');
                }
            });
        }
    }

    logout() {
        this.authService.logout();
    }
}
