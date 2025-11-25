import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CartService, CartItem } from '../../services/cart.service';
import { SaleService } from '../../services/sale.service';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cart.html',
  styleUrls: ['./cart.css']
})
export class CartComponent implements OnInit {
  cartItems: CartItem[] = [];
  total: number = 0;

  constructor(
    private cartService: CartService,
    private saleService: SaleService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit() {
    this.cartService.cartItems$.subscribe(items => {
      this.cartItems = items;
      this.calculateTotal();
    });
  }

  calculateTotal() {
    this.total = this.cartItems.reduce((acc, item) => acc + (item.product.price * item.quantity), 0);
  }

  updateQuantity(productId: number, quantity: number) {
    this.cartService.updateQuantity(productId, quantity);
  }

  removeItem(productId: number) {
    this.cartService.removeFromCart(productId);
  }

  checkout() {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }

    const token = this.authService.getToken();
    let clientId = 0;
    if (token) {
      const decoded: any = jwtDecode(token);
      clientId = parseInt(decoded.sub || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']);
    }

    const saleData = {
      clientId: clientId, // Ideally backend gets this from token, but DTO might require it. Check DTO.
      items: this.cartItems.map(item => ({
        productId: item.product.id,
        quantity: item.quantity
      }))
    };

    // Wait, DTO requires ClientId? Let's check CreateSaleDto.
    // If backend uses User.Identity to get ID, we might not need to send it, or we send it if the endpoint expects it.
    // The controller says: var client = await _context.People.OfType<Client>().FirstOrDefaultAsync(c => c.Id == createSaleDto.ClientId);
    // So it expects ClientId in the body.
    // I need to extract ID from token. The token has 'sub' which is the ID.

    this.saleService.createSale(saleData).subscribe({
      next: (receipt) => {
        alert('Purchase successful!');
        this.cartService.clearCart();
        // Redirect to receipt or purchases
        this.router.navigate(['/purchases']);
      },
      error: (err) => {
        alert('Purchase failed: ' + (err.error?.message || err.message));
      }
    });
  }
}
