import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SaleService } from '../../../services/sale.service';
import { AuthService } from '../../../services/auth.service';

export interface Sale {
    id: number;
    clientName?: string;
    saleDate: string;
    grossTotal: number;
    ivaTotal: number;
    products?: any[];
}

@Component({
    selector: 'app-sales',
    standalone: true,
    imports: [CommonModule, RouterLink, FormsModule],
    templateUrl: './sales.component.html',
    styleUrls: ['./sales.component.css']
})
export class SalesComponent implements OnInit {
    sales: Sale[] = [];
    loading: boolean = true;
    searchTerm: string = '';

    constructor(
        private saleService: SaleService,
        private authService: AuthService,
        private router: Router
    ) { }

    ngOnInit() {
        this.loadSales();
    }

    loadSales() {
        this.loading = true;
        this.saleService.getSales().subscribe({
            next: (data) => {
                this.sales = data;
                this.loading = false;
            },
            error: (err) => {
                console.error('Error loading sales:', err);
                this.loading = false;
            }
        });
    }

    get filteredSales() {
        if (!this.searchTerm) {
            return this.sales;
        }
        return this.sales.filter(sale =>
            sale.id.toString().includes(this.searchTerm) ||
            (sale.clientName && sale.clientName.toLowerCase().includes(this.searchTerm.toLowerCase()))
        );
    }

    getTotalAmount(sale: Sale): number {
        return sale.grossTotal + sale.ivaTotal;
    }

    formatDate(date: string): string {
        return new Date(date).toLocaleDateString();
    }

    getTotalSalesCount(): number {
        return this.sales.length;
    }

    getSubtotal(): number {
        return this.sales.reduce((sum, sale) => sum + sale.grossTotal, 0);
    }

    getTotalRevenue(): number {
        return this.sales.reduce((sum, sale) => sum + this.getTotalAmount(sale), 0);
    }

    logout() {
        this.authService.logout();
    }
}
