import { Injectable } from '@angular/core';
import { forkJoin, map, Observable } from 'rxjs';
import { UserService } from './user.service';
import { ProductService } from './product.service';
import { SaleService } from './sale.service';

export interface DashboardStats {
    totalRevenue: number;
    totalSales: number;
    totalUsers: number;
    totalProducts: number;
    recentSales: any[];
    lowStockProducts: any[];
}

@Injectable({
    providedIn: 'root'
})
export class DashboardService {

    constructor(
        private userService: UserService,
        private productService: ProductService,
        private saleService: SaleService
    ) { }

    getDashboardStats(): Observable<DashboardStats> {
        return forkJoin({
            users: this.userService.getUsers(),
            products: this.productService.getProducts(),
            sales: this.saleService.getSales()
        }).pipe(
            map(({ users, products, sales }) => {
                const totalRevenue = sales.reduce((sum, sale) => sum + (sale.grossTotal + sale.ivaTotal), 0);
                const lowStockProducts = products.filter(p => p.stock < 10);
                const recentSales = sales.slice(0, 5);

                return {
                    totalRevenue,
                    totalSales: sales.length,
                    totalUsers: users.length,
                    totalProducts: products.length,
                    recentSales,
                    lowStockProducts
                };
            })
        );
    }
}
