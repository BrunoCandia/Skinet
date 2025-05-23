import { Injectable } from '@angular/core';
import { ShoppingCartService } from './shopping-cart.service';
import { forkJoin, of } from 'rxjs';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class InitService {

  constructor(private shoppingCartService: ShoppingCartService, private accountService: AccountService) { }

  init() {
    const shoppingCartId = localStorage.getItem('shoppingCartId');
    const shoppingCart$ = shoppingCartId ? this.shoppingCartService.getShoppingCart(shoppingCartId) : of(null)

    return forkJoin({
      shoppingCart: shoppingCart$,
      user: this.accountService.getUserInfo()
    });
  }
}
