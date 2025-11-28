import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../../services/user.service';
import { AuthService } from '../../../services/auth.service';
import { ImportModalComponent } from '../../../components/import-modal/import-modal.component';
import { ImportExportService } from '../../../services/import-export.service';
import { ToastService } from '../../../services/toast.service';
import { ToastComponent } from '../../../components/toast/toast.component';
import { UserModalComponent, UserFormData } from '../../../components/user-modal/user-modal.component';

export interface User {
    id: number;
    email: string;
    fullName: string;
    phone: string;
    document: string;
    address: string;
    age: number;
    registerDate: string;
    role?: string;
}

@Component({
    selector: 'app-users',
    standalone: true,
    imports: [CommonModule, RouterLink, FormsModule, ImportModalComponent, ToastComponent, UserModalComponent],
    templateUrl: './users.component.html',
    styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {
    users: User[] = [];
    loading: boolean = true;
    searchTerm: string = '';

    showImportModal = false;
    showUserModal = false;
    selectedUser: UserFormData | null = null;
    isEditMode = false;

    constructor(
        private userService: UserService,
        private authService: AuthService,
        private router: Router,
        private importExportService: ImportExportService,
        private toastService: ToastService
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
            (user.fullName && user.fullName.toLowerCase().includes(this.searchTerm.toLowerCase()))
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

    openAddUserModal() {
        this.selectedUser = null;
        this.isEditMode = false;
        this.showUserModal = true;
    }

    editUser(user: User) {
        this.selectedUser = { ...user };
        this.isEditMode = true;
        this.showUserModal = true;
    }

    closeUserModal() {
        this.showUserModal = false;
        this.selectedUser = null;
    }

    saveUser(userData: UserFormData) {
        if (this.isEditMode && userData.id) {
            // Update existing user
            this.userService.updateUser(userData.id, userData).subscribe({
                next: () => {
                    this.loadUsers();
                    this.toastService.success('User updated successfully!');
                    this.closeUserModal();
                },
                error: (err) => {
                    console.error('Error updating user:', err);
                    this.toastService.error('Failed to update user. Please try again.');
                }
            });
        } else {
            // For creating users, we would typically use Auth/Register endpoint
            // This is a simplified approach - adjust based on your backend requirements
            this.toastService.info('Please use the Auth/Register endpoint to create new users');
            this.closeUserModal();
        }
    }

    logout() {
        this.authService.logout();
    }

    openImportModal() {
        this.showImportModal = true;
    }

    closeImportModal() {
        this.showImportModal = false;
    }

    onImported() {
        this.closeImportModal();
        this.loadUsers();
        // Optional: Show success toast
    }

    exportExcel() {
        this.importExportService.exportExcel('Clients');
    }

    exportPdf() {
        this.importExportService.exportPdf('Clients');
    }
}
