import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProductService } from '../../../services/product.service';
import { ClientService } from '../../../services/client.service';
import { SaleService } from '../../../services/sale.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class DashboardComponent implements OnInit {
  totalProducts: number = 0;
  totalClients: number = 0;
  totalSales: number = 0;
  totalRevenue: number = 0;
  loading: boolean = true;

  constructor(
    private productService: ProductService,
    private clientService: ClientService,
    private saleService: SaleService
  ) { }

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    forkJoin({
      products: this.productService.getProducts(),
      clients: this.clientService.getClients(),
      sales: this.saleService.getSales()
    }).subscribe({
      next: (data) => {
        this.totalProducts = data.products.length;
        this.totalClients = data.clients.length;
        this.totalSales = data.sales.length;
        this.totalRevenue = data.sales.reduce((acc, sale) => acc + (sale.grossTotal + sale.ivaTotal), 0);
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading dashboard data', err);
        this.loading = false;
      }
    });
  }
}
