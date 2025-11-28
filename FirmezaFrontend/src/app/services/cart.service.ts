import { Injectable, signal, computed } from '@angular/core';
import { Product } from '../models/product.model';

export interface CartItem {
    product: Product;
    quantity: number;
}

@Injectable({
    providedIn: 'root'
})
export class CartService {
    private cartItems = signal<CartItem[]>([]);

    readonly items = this.cartItems.asReadonly();

    readonly count = computed(() => this.cartItems().reduce((acc, item) => acc + item.quantity, 0));

    readonly total = computed(() => this.cartItems().reduce((acc, item) => acc + (item.product.price * item.quantity), 0));

    addToCart(product: Product) {
        this.cartItems.update(items => {
            const existingItem = items.find(i => i.product.id === product.id);
            if (existingItem) {
                return items.map(i => i.product.id === product.id ? { ...i, quantity: i.quantity + 1 } : i);
            }
            return [...items, { product, quantity: 1 }];
        });
    }

    removeFromCart(productId: number) {
        this.cartItems.update(items => items.filter(i => i.product.id !== productId));
    }

    updateQuantity(productId: number, quantity: number) {
        if (quantity <= 0) {
            this.removeFromCart(productId);
            return;
        }
        this.cartItems.update(items => items.map(i => i.product.id === productId ? { ...i, quantity } : i));
    }

    clearCart() {
        this.cartItems.set([]);
    }
}
