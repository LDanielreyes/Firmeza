import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { SaleService } from '../../services/sale.service';
import { AuthService } from '../../services/auth.service';
import { CreateSaleDto } from '../../models/sale.model';

@Component({
  selector: 'app-cart',
  imports: [CommonModule, RouterLink],
  templateUrl: './cart.html',
  styleUrl: './cart.css',
})
export class CartComponent {
  cartService: CartService;
  loading = signal(false);
  error = signal<string | null>(null);
  success = signal(false);

  constructor(
    cartService: CartService,
    private saleService: SaleService,
    private authService: AuthService,
    private router: Router
  ) {
    this.cartService = cartService;
  }

  get items() {
    return this.cartService.items();
  }

  get total() {
    return this.cartService.total();
  }

  get count() {
    return this.cartService.count();
  }

  updateQuantity(productId: number, quantity: number) {
    this.cartService.updateQuantity(productId, quantity);
  }

  removeItem(productId: number) {
    this.cartService.removeFromCart(productId);
  }

  checkout() {
    if (this.items.length === 0) {
      this.error.set('El carrito está vacío');
      return;
    }

    const user = this.authService.getCurrentUser();
    if (!user || !user.id) {
      this.router.navigate(['/login']);
      return;
    }

    const createSaleDto: CreateSaleDto = {
      clientId: user.id,
      items: this.items.map(item => ({
        productId: item.product.id,
        quantity: item.quantity
      }))
    };

    this.loading.set(true);
    this.error.set(null);

    this.saleService.createSale(createSaleDto).subscribe({
      next: () => {
        this.success.set(true);
        this.cartService.clearCart();
        setTimeout(() => {
          this.router.navigate(['/my-purchases']);
        }, 2000);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.message || 'Error al procesar la compra. Por favor, inténtelo de nuevo.');
        console.error(err);
      }
    });
  }

  continueShopping() {
    this.router.navigate(['/catalog']);
  }
}
