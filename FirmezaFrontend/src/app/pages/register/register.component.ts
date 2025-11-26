import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-register',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, RouterLink],
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.css']
})
export class RegisterComponent {
    registerForm: FormGroup;
    errorMessage: string = '';
    loading: boolean = false;

    constructor(
        private fb: FormBuilder,
        private authService: AuthService,
        private router: Router
    ) {
        this.registerForm = this.fb.group({
            name: ['', [Validators.required]],
            lastName: ['', [Validators.required]],
            email: ['', [Validators.required, Validators.email]],
            document: ['', [Validators.required]],
            phone: ['', [Validators.required]],
            password: ['', [Validators.required, Validators.minLength(6)]],
            confirmPassword: ['', [Validators.required]]
        });
    }

    onSubmit() {
        if (this.registerForm.valid) {
            if (this.registerForm.value.password !== this.registerForm.value.confirmPassword) {
                this.errorMessage = 'Passwords do not match';
                return;
            }

            this.loading = true;
            this.errorMessage = '';

            this.authService.register(this.registerForm.value).subscribe({
                next: () => {
                    alert('Registration successful! Please login.');
                    this.router.navigate(['/login']);
                },
                error: (err) => {
                    this.errorMessage = err.error?.message || 'Registration failed. Please try again.';
                    this.loading = false;
                }
            });
        }
    }
}
