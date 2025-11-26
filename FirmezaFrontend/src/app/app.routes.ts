import { Routes } from '@angular/router';
import { LandingComponent } from './pages/landing/landing.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
    { path: '', redirectTo: '/landing', pathMatch: 'full' },
    { path: 'landing', component: LandingComponent },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },

    // Admin routes
    {
        path: 'admin',
        canActivate: [authGuard, adminGuard],
        children: [
            {
                path: 'dashboard',
                loadComponent: () => import('./pages/admin/dashboard/dashboard.component').then(m => m.DashboardComponent)
            },
            {
                path: 'users',
                loadComponent: () => import('./pages/admin/users/users.component').then(m => m.UsersComponent)
            },
            {
                path: 'products',
                loadComponent: () => import('./pages/admin/products/products.component').then(m => m.ProductsComponent)
            },
            {
                path: 'sales',
                loadComponent: () => import('./pages/admin/sales/sales.component').then(m => m.SalesComponent)
            }
        ]
    },

    { path: '**', redirectTo: '/landing' }
];
