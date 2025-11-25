import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SaleService } from '../../../services/sale.service';
import { DatePipe } from '@angular/common';

import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-sales',
  standalone: true,
  imports: [CommonModule, DatePipe, RouterModule],
  templateUrl: './sales.html',
  styleUrls: ['./sales.css']
})
export class SalesComponent implements OnInit {
  sales: any[] = [];
  selectedSale: any | null = null;
  showDetails: boolean = false;

  constructor(private saleService: SaleService) { }

  ngOnInit() {
    this.saleService.getSales().subscribe(data => {
      this.sales = data;
    });
  }

  viewDetails(sale: any) {
    this.selectedSale = sale;
    this.showDetails = true;
  }

  closeDetails() {
    this.showDetails = false;
    this.selectedSale = null;
  }
}
