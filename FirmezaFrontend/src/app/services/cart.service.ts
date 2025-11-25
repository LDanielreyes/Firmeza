import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Product } from './product.service';

export interface CartItem {
  product: Product;
  quantity: number;
}

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cartKey = 'firmeza_cart';
  private cartItemsSubject = new BehaviorSubject<CartItem[]>(this.getCartItemsFromStorage());

  public cartItems$ = this.cartItemsSubject.asObservable();

  constructor() { }

  addToCart(product: Product, quantity: number = 1) {
    const items = this.getCartItemsFromStorage();
    const existingItem = items.find(i => i.product.id === product.id);

    if (existingItem) {
      existingItem.quantity += quantity;
    } else {
      items.push({ product, quantity });
    }

    this.saveCartItems(items);
  }

  removeFromCart(productId: number) {
    let items = this.getCartItemsFromStorage();
    items = items.filter(i => i.product.id !== productId);
    this.saveCartItems(items);
  }

  updateQuantity(productId: number, quantity: number) {
    const items = this.getCartItemsFromStorage();
    const item = items.find(i => i.product.id === productId);
    if (item) {
      item.quantity = quantity;
      if (item.quantity <= 0) {
        this.removeFromCart(productId);
        return;
      }
      this.saveCartItems(items);
    }
  }

  clearCart() {
    localStorage.removeItem(this.cartKey);
    this.cartItemsSubject.next([]);
  }

  getCartItems(): CartItem[] {
    return this.cartItemsSubject.value;
  }

  private getCartItemsFromStorage(): CartItem[] {
    const items = localStorage.getItem(this.cartKey);
    return items ? JSON.parse(items) : [];
  }

  private saveCartItems(items: CartItem[]) {
    localStorage.setItem(this.cartKey, JSON.stringify(items));
    this.cartItemsSubject.next(items);
  }
}
