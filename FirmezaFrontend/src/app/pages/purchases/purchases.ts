import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SaleService } from '../../services/sale.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-purchases',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './purchases.html',
  styleUrls: ['./purchases.css']
})
export class PurchasesComponent implements OnInit {
  purchases: any[] = [];

  constructor(private saleService: SaleService) { }

  ngOnInit() {
    this.saleService.getSales().subscribe(data => {
      this.purchases = data;
    });
  }

  printReceipt(purchase: any) {
    // Simple print implementation: Open a new window with receipt details
    const printWindow = window.open('', '_blank');
    if (printWindow) {
      printWindow.document.write(`
        <html>
          <head>
            <title>Receipt #${purchase.id}</title>
            <style>
              body { font-family: sans-serif; padding: 20px; }
              table { width: 100%; border-collapse: collapse; margin-top: 20px; }
              th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
              th { background-color: #f2f2f2; }
              .total { margin-top: 20px; text-align: right; font-weight: bold; }
            </style>
          </head>
          <body>
            <h1>Receipt #${purchase.id}</h1>
            <p>Date: ${new Date(purchase.receiptDate).toLocaleString()}</p>
            <p>Client: ${purchase.clientName}</p>
            
            <table>
              <thead>
                <tr>
                  <th>Product</th>
                  <th>Quantity</th>
                  <th>Price</th>
                  <th>Total</th>
                </tr>
              </thead>
              <tbody>
                ${purchase.items.map((item: any) => `
                  <tr>
                    <td>${item.productName}</td>
                    <td>${item.quantity}</td>
                    <td>$${item.pricePerUnit}</td>
                    <td>$${item.netTotal}</td>
                  </tr>
                `).join('')}
              </tbody>
            </table>
            
            <div class="total">
              <p>Gross Total: $${purchase.grossTotal}</p>
              <p>IVA (19%): $${purchase.ivaTotal}</p>
              <p>Net Total: $${purchase.grossTotal + purchase.ivaTotal}</p>
            </div>
            
            <script>
              window.print();
            </script>
          </body>
        </html>
      `);
      printWindow.document.close();
    }
  }
}
