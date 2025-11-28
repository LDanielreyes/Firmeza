import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, RouterLink],
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent {
    loginForm: FormGroup;
    errorMessage: string = '';
    loading: boolean = false;

    constructor(
        private fb: FormBuilder,
        private authService: AuthService,
        private router: Router
    ) {
        this.loginForm = this.fb.group({
            email: ['', [Validators.required, Validators.email]],
            password: ['', [Validators.required, Validators.minLength(6)]]
        });
    }

    onSubmit() {
        if (this.loginForm.valid) {
            this.loading = true;
            this.errorMessage = '';

            this.authService.login({
                email: this.loginForm.value.email,
                password: this.loginForm.value.password
            }).subscribe({
                next: () => {
                    if (this.authService.isAdmin()) {
                        this.router.navigate(['/admin/dashboard']);
                    } else {
                        this.router.navigate(['/catalog']);
                    }
                },
                error: (err) => {
                    this.errorMessage = 'Invalid credentials. Please try again.';
                    this.loading = false;
                }
            });
        }
    }
}
