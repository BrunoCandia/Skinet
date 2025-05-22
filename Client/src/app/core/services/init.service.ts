import { Injectable } from '@angular/core';
import { ShoppingCartService } from './shopping-cart.service';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InitService {

  constructor(private shoppingCartService: ShoppingCartService) { }

  init() {
    const shoppingCartId = localStorage.getItem('shoppingCartId');
    const shoppingCart$ = shoppingCartId ? this.shoppingCartService.getShoppingCart(shoppingCartId) : of(null)

    return shoppingCart$;
  }
}
