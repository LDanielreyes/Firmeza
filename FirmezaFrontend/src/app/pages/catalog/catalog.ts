import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Product, ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-catalog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './catalog.html',
  styleUrls: ['./catalog.css']
})
export class CatalogComponent implements OnInit {
  products: Product[] = [];
  searchTerm: string = '';
  selectedType: string = '';

  constructor(
    private productService: ProductService,
    private cartService: CartService
  ) { }

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
    this.productService.getProducts(this.searchTerm, this.selectedType).subscribe(products => {
      this.products = products;
    });
  }

  onSearch() {
    this.loadProducts();
  }

  addToCart(product: Product) {
    this.cartService.addToCart(product);
    alert('Product added to cart!');
  }
}
