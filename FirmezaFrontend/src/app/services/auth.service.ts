import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { environment } from '../../environments/environment';
import { Router } from '@angular/router';

@Injectable({
    providedIn: 'root',
})
export class AuthService {
    private apiUrl = `${environment.apiUrl}/Auth`;
    private tokenKey = 'firmeza_token';
    private userRoleSubject = new BehaviorSubject<string | null>(this.getRoleFromToken());

    public userRole$ = this.userRoleSubject.asObservable();

    constructor(private http: HttpClient, private router: Router) { }

    register(data: any): Observable<any> {
        return this.http.post(`${this.apiUrl}/register`, data);
    }

    login(credentials: any): Observable<any> {
        return this.http.post(`${this.apiUrl}/login`, credentials).pipe(
            tap((response: any) => {
                if (response.token) {
                    this.saveToken(response.token);
                    this.userRoleSubject.next(this.getRoleFromToken());
                }
            })
        );
    }

    logout() {
        localStorage.removeItem(this.tokenKey);
        this.userRoleSubject.next(null);
        this.router.navigate(['/landing']);
    }

    getToken(): string | null {
        return localStorage.getItem(this.tokenKey);
    }

    private saveToken(token: string) {
        localStorage.setItem(this.tokenKey, token);
    }

    isLoggedIn(): boolean {
        const token = this.getToken();
        if (!token) return false;

        try {
            const decoded: any = jwtDecode(token);
            const currentTime = Date.now() / 1000;
            if (decoded.exp < currentTime) {
                this.logout();
                return false;
            }
            return true;
        } catch (e) {
            return false;
        }
    }

    getUserRole(): string | null {
        return this.userRoleSubject.value;
    }

    isAdmin(): boolean {
        const role = this.getUserRole();
        return role === 'Admin' || role === 'Administrador';
    }

    private getRoleFromToken(): string | null {
        const token = this.getToken();
        if (!token) return null;
        try {
            const decoded: any = jwtDecode(token);
            let role = decoded.role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;

            // Handle role as array (e.g., ["Administrador", "Admin"])
            if (Array.isArray(role)) {
                // Prioritize "Admin" role if it exists in the array
                if (role.includes('Admin')) {
                    return 'Admin';
                }
                // Otherwise return the first role
                return role.length > 0 ? role[0] : null;
            }

            return role;
        } catch (e) {
            return null;
        }
    }

    getCurrentUser(): { id: number; email: string; role: string } | null {
        const token = this.getToken();
        if (!token) return null;
        try {
            const decoded: any = jwtDecode(token);
            return {
                id: parseInt(decoded.sub || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']),
                email: decoded.email || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
                role: this.getRoleFromToken() || ''
            };
        } catch (e) {
            return null;
        }
    }
}
