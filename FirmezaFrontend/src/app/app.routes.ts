import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home';
import { LoginComponent } from './pages/login/login';
import { RegisterComponent } from './pages/register/register';
import { CatalogComponent } from './pages/catalog/catalog';
import { CartComponent } from './pages/cart/cart';
import { PurchasesComponent } from './pages/purchases/purchases';
import { DashboardComponent } from './pages/admin/dashboard/dashboard';
import { ProductsComponent } from './pages/admin/products/products';
import { ClientsComponent } from './pages/admin/clients/clients';
import { SalesComponent } from './pages/admin/sales/sales';
import { authGuard } from './guards/auth-guard';
import { adminGuard } from './guards/admin-guard';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },

    // Client Routes
    { path: 'catalog', component: CatalogComponent, canActivate: [authGuard] },
    { path: 'cart', component: CartComponent, canActivate: [authGuard] },
    { path: 'purchases', component: PurchasesComponent, canActivate: [authGuard] },

    // Admin Routes
    { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard, adminGuard] },
    { path: 'admin/products', component: ProductsComponent, canActivate: [authGuard, adminGuard] },
    { path: 'admin/clients', component: ClientsComponent, canActivate: [authGuard, adminGuard] },
    { path: 'admin/sales', component: SalesComponent, canActivate: [authGuard, adminGuard] },

    { path: '**', redirectTo: '' }
];
