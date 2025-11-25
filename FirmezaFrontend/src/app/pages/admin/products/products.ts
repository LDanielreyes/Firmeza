import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Product, ProductService } from '../../../services/product.service';

import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule],
  templateUrl: './products.html',
  styleUrls: ['./products.css']
})
export class ProductsComponent implements OnInit {
  products: Product[] = [];
  productForm: FormGroup;
  isEditing: boolean = false;
  showForm: boolean = false;
  showDetails: boolean = false;
  selectedProduct: Product | null = null;
  currentProductId: number | null = null;

  constructor(
    private productService: ProductService,
    private fb: FormBuilder
  ) {
    this.productForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      type: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      stock: [0, [Validators.required, Validators.min(0)]],
      imageUrl: ['']
    });
  }

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
    this.productService.getProducts().subscribe(data => {
      this.products = data;
    });
  }

  onSubmit() {
    if (this.productForm.valid) {
      if (this.isEditing && this.currentProductId) {
        this.productService.updateProduct(this.currentProductId, this.productForm.value).subscribe(() => {
          this.loadProducts();
          this.resetForm();
        });
      } else {
        this.productService.createProduct(this.productForm.value).subscribe(() => {
          this.loadProducts();
          this.resetForm();
        });
      }
    }
  }

  editProduct(product: Product) {
    this.isEditing = true;
    this.currentProductId = product.id;
    this.productForm.patchValue(product);
    this.showForm = true;
  }

  deleteProduct(id: number) {
    if (confirm('Are you sure you want to delete this product?')) {
      this.productService.deleteProduct(id).subscribe(() => {
        this.loadProducts();
      });
    }
  }

  viewDetails(product: Product) {
    this.selectedProduct = product;
    this.showDetails = true;
    this.showForm = false;
  }

  closeDetails() {
    this.showDetails = false;
    this.selectedProduct = null;
  }

  resetForm() {
    this.isEditing = false;
    this.currentProductId = null;
    this.productForm.reset();
    this.showForm = false;
    this.showDetails = false;
  }

  toggleForm() {
    this.showForm = !this.showForm;
    this.showDetails = false;
    if (!this.showForm) {
      this.resetForm();
    }
  }
}
