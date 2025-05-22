import { computed, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { ShoppingCart, ShoppingCartItem } from '../../shared/models/shoppingCart';
import { Product } from '../../shared/models/product';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ShoppingCartService {

  baseUrl = environment.apiUrl;
  shoppingCart = signal<ShoppingCart | null>(null);
  itemCount = computed(() => {
    return this.shoppingCart()?.items.reduce((sum, item) => sum + item.quantity, 0)
  });
  totals = computed(() => {
    const shoppingCart = this.shoppingCart();
    if (!shoppingCart) { 
      return null;
    } else {
      const subtotal = shoppingCart.items.reduce((sum, item) => sum + item.price * item.quantity, 0);
      const shipping = 0;
      const discount = 0;
      const total = subtotal + shipping - discount;
      return {
        subtotal,
        shipping,
        discount,
        total
      };
    }
  });

  constructor(private httpClient: HttpClient) { }

  getShoppingCart(id: string) {
    return this.httpClient.get<ShoppingCart>(this.baseUrl + 'shoppingCart?id=' + id)
      .pipe(
        map(shoppingCart => {
          this.shoppingCart.set(shoppingCart);
          return shoppingCart;
        })
      );
  }

  // getShoppingCart(id: string) {
  //   return this.httpClient.get<ShoppingCart>(this.baseUrl + 'shoppingCart?id=' + id).subscribe({
  //     next: shoppingCart => this.shoppingCart.set(shoppingCart),
  //     error: error => console.log(error)
  //   });
  // }

  // TODO: Review if the subscription can be done in the component
  setShoppingCart(shoppingCart: ShoppingCart) {
    return this.httpClient.post<ShoppingCart>(this.baseUrl + 'shoppingCart', shoppingCart).subscribe({
      next: shoppingCart => this.shoppingCart.set(shoppingCart),
      error: error => console.log(error)
    });
  }

  // TODO: Review if the subscription can be done in the component
  deleteShoppingCart() {
    this.httpClient.delete(this.baseUrl + 'shoppingCart?id=' + this.shoppingCart()?.id).subscribe({
      next: () => {
        localStorage.removeItem('shoppingCartId');
        this.shoppingCart.set(null);
      }
    });
  }

  addItemToShoppingcart(item: ShoppingCartItem | Product, quantity = 1) {
    const shoppingCart = this.shoppingCart() ?? this.createShoppingCart();

    if (this.isProduct(item)) {
      item = this.mapProductToShoppingCartItem(item);
    }

    shoppingCart.items = this.addOrUpdateItem(shoppingCart.items, item, quantity);

    this.setShoppingCart(shoppingCart);
  }

  removeItemFromShoppingCart(productId: string, quantity = 1) {
    const shoppingCart = this.shoppingCart();

    if (!shoppingCart) {
      console.log('No shopping cart found');
      return;
    }

    const index = shoppingCart.items.findIndex(x => x.productId === productId);

    if (index !== -1) {
      if (shoppingCart.items[index].quantity > quantity) {
        shoppingCart.items[index].quantity -= quantity;
      } else {
        shoppingCart.items.splice(index, 1);
      }

      if (shoppingCart.items.length === 0) {
        this.deleteShoppingCart();
      } else {
        this.setShoppingCart(shoppingCart);
      }
    } else {
      console.log('Item not found in shopping cart');
    }
  }

  private addOrUpdateItem(items: ShoppingCartItem[], item: ShoppingCartItem, quantity: number): ShoppingCartItem[] {
    const index = items.findIndex(x => x.productId === item.productId);

    if (index === -1) {
      item.quantity = quantity;
      items.push(item);
    } else {
      items[index].quantity += quantity;
    }

    return items;
  }

  private mapProductToShoppingCartItem(item: Product): ShoppingCartItem {
    return {
      productId: item.id,
      productName: item.name,
      price: item.price,
      quantity: 0,
      pictureUrl: item.pictureUrl,
      brand: item.brand,
      type: item.type
    };
  }

  private isProduct(item: ShoppingCartItem | Product): item is Product {
    return (item as Product).id !== undefined
  }

  private createShoppingCart(): ShoppingCart {
    const shoppingCart = new ShoppingCart();

    localStorage.setItem('shoppingCartId', shoppingCart.id);

    return shoppingCart;
  }
}
