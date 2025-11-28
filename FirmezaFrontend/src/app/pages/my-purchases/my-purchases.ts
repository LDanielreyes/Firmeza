import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { SaleService } from '../../services/sale.service';
import { Receipt } from '../../models/sale.model';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';

@Component({
  selector: 'app-my-purchases',
  imports: [CommonModule, RouterLink],
  templateUrl: './my-purchases.html',
  styleUrl: './my-purchases.css',
})
export class MyPurchasesComponent implements OnInit {
  purchases = signal<Receipt[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  constructor(
    private saleService: SaleService,
    private router: Router
  ) { }

  ngOnInit() {
    this.loadPurchases();
  }

  loadPurchases() {
    this.loading.set(true);
    this.saleService.getSales().subscribe({
      next: (data) => {
        this.purchases.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Error al cargar las compras');
        this.loading.set(false);
        console.error(err);
      }
    });
  }

  downloadPDF(receipt: Receipt) {
    const doc = new jsPDF();

    // Header
    doc.setFontSize(20);
    doc.setTextColor(102, 126, 234);
    doc.text('FIRMEZA', 105, 20, { align: 'center' });

    doc.setFontSize(16);
    doc.setTextColor(0, 0, 0);
    doc.text('Recibo de Compra', 105, 30, { align: 'center' });

    // Receipt info
    doc.setFontSize(10);
    doc.text(`Recibo #${receipt.id}`, 20, 45);
    doc.text(`Fecha: ${new Date(receipt.receiptDate).toLocaleDateString()}`, 20, 52);

    // Items table
    const tableData = receipt.saleLines.map(line => [
      line.product?.name || 'Producto',
      line.quantity.toString(),
      `$${line.pricePerUnit.toFixed(2)}`,
      `$${line.netTotal.toFixed(2)}`
    ]);

    autoTable(doc, {
      startY: 60,
      head: [['Producto', 'Cantidad', 'Precio Unitario', 'Subtotal']],
      body: tableData,
      theme: 'grid',
      headStyles: {
        fillColor: [102, 126, 234],
        textColor: [255, 255, 255],
        fontStyle: 'bold'
      },
      styles: {
        fontSize: 10,
        cellPadding: 5
      }
    });

    // Totals
    const finalY = (doc as any).lastAutoTable.finalY || 60;

    doc.setFontSize(10);
    doc.text(`Subtotal:`, 130, finalY + 15);
    doc.text(`$${receipt.grossTotal.toFixed(2)}`, 180, finalY + 15, { align: 'right' });

    doc.text(`IVA (19%):`, 130, finalY + 22);
    doc.text(`$${receipt.ivaTotal.toFixed(2)}`, 180, finalY + 22, { align: 'right' });

    doc.setFontSize(12);
    doc.setFont('helvetica', 'bold');
    doc.text(`Total:`, 130, finalY + 32);
    doc.text(`$${(receipt.grossTotal + receipt.ivaTotal).toFixed(2)}`, 180, finalY + 32, { align: 'right' });

    // Footer
    doc.setFontSize(8);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(128, 128, 128);
    doc.text('Gracias por su compra', 105, 280, { align: 'center' });

    // Save
    doc.save(`recibo-${receipt.id}.pdf`);
  }

  goToCatalog() {
    this.router.navigate(['/catalog']);
  }
}
