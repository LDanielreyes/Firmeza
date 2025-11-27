import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../services/product.service';
import { AuthService } from '../../../services/auth.service';
import { ImportModalComponent } from '../../../components/import-modal/import-modal.component';
import { ImportExportService } from '../../../services/import-export.service';

export interface Product {
    id: number;
    name: string;
    description?: string;
    price: number;
    stock: number;
    category?: string;
}

@Component({
    selector: 'app-products',
    standalone: true,
    imports: [CommonModule, RouterLink, FormsModule, ImportModalComponent],
    templateUrl: './products.component.html',
    styleUrls: ['./products.component.css']
})
export class ProductsComponent implements OnInit {
    products: Product[] = [];
    loading: boolean = true;
    searchTerm: string = '';

    showImportModal = false;

    constructor(
        private productService: ProductService,
        private authService: AuthService,
        private router: Router,
        private importExportService: ImportExportService
    ) { }

    ngOnInit() {
        this.loadProducts();
    }

    loadProducts() {
        this.loading = true;
        this.productService.getProducts().subscribe({
            next: (data) => {
                this.products = data;
                this.loading = false;
            },
            error: (err) => {
                console.error('Error loading products:', err);
                this.loading = false;
            }
        });
    }

    get filteredProducts() {
        if (!this.searchTerm) {
            return this.products;
        }
        return this.products.filter(product =>
            product.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
            (product.description && product.description.toLowerCase().includes(this.searchTerm.toLowerCase())) ||
            (product.category && product.category.toLowerCase().includes(this.searchTerm.toLowerCase()))
        );
    }

    deleteProduct(id: number) {
        if (confirm('Are you sure you want to delete this product?')) {
            this.productService.deleteProduct(id).subscribe({
                next: () => {
                    this.loadProducts();
                },
                error: (err) => {
                    console.error('Error deleting product:', err);
                    alert('Failed to delete product');
                }
            });
        }
    }

    getStockStatus(stock: number): string {
        if (stock === 0) return 'Out of Stock';
        if (stock <= 10) return 'Low Stock';
        return 'In Stock';
    }

    getStockClass(stock: number): string {
        if (stock === 0) return 'bg-red-100 text-red-800';
        if (stock <= 10) return 'bg-yellow-100 text-yellow-800';
        return 'bg-green-100 text-green-800';
    }

    logout() {
        this.authService.logout();
    }

    openImportModal() {
        this.showImportModal = true;
    }

    closeImportModal() {
        this.showImportModal = false;
    }

    onImported() {
        this.closeImportModal();
        this.loadProducts();
        // Optional: Show success toast
    }

    exportExcel() {
        this.importExportService.exportExcel('Products');
    }

    exportPdf() {
        this.importExportService.exportPdf('Products');
    }
}
