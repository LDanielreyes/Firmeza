import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../services/product.service';
import { AuthService } from '../../../services/auth.service';
import { ImportModalComponent } from '../../../components/import-modal/import-modal.component';
import { ImportExportService } from '../../../services/import-export.service';
import { ToastService } from '../../../services/toast.service';
import { ToastComponent } from '../../../components/toast/toast.component';
import { ProductModalComponent, ProductFormData } from '../../../components/product-modal/product-modal.component';

export interface Product {
    id: number;
    name: string;
    description?: string;
    type?: string;
    price: number;
    stock: number;
    category?: string;
    imageUrl?: string;
}

@Component({
    selector: 'app-products',
    standalone: true,
    imports: [CommonModule, RouterLink, FormsModule, ImportModalComponent, ToastComponent, ProductModalComponent],
    templateUrl: './products.component.html',
    styleUrls: ['./products.component.css']
})
export class ProductsComponent implements OnInit {
    products: Product[] = [];
    loading: boolean = true;
    searchTerm: string = '';

    showImportModal = false;
    showProductModal = false;
    selectedProduct: ProductFormData | null = null;
    isEditMode = false;

    constructor(
        private productService: ProductService,
        private authService: AuthService,
        private router: Router,
        private importExportService: ImportExportService,
        private toastService: ToastService
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

    openAddProductModal() {
        this.selectedProduct = null;
        this.isEditMode = false;
        this.showProductModal = true;
    }

    editProduct(product: Product) {
        this.selectedProduct = {
            ...product,
            type: product.type || 'Product'
        };
        this.isEditMode = true;
        this.showProductModal = true;
    }

    closeProductModal() {
        this.showProductModal = false;
        this.selectedProduct = null;
    }

    saveProduct(productData: ProductFormData) {
        if (this.isEditMode && productData.id) {
            // Update existing product
            this.productService.updateProduct(productData.id, productData).subscribe({
                next: () => {
                    this.loadProducts();
                    this.toastService.success('Product updated successfully!');
                    this.closeProductModal();
                },
                error: (err) => {
                    console.error('Error updating product:', err);
                    this.toastService.error('Failed to update product. Please try again.');
                }
            });
        } else {
            // Create new product
            this.productService.createProduct(productData).subscribe({
                next: () => {
                    this.loadProducts();
                    this.toastService.success('Product created successfully!');
                    this.closeProductModal();
                },
                error: (err) => {
                    console.error('Error creating product:', err);
                    this.toastService.error('Failed to create product. Please try again.');
                }
            });
        }
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
