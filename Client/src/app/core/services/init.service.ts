import { Injectable } from '@angular/core';
import { ShoppingCartService } from './shopping-cart.service';
import { forkJoin, of, tap } from 'rxjs';
import { AccountService } from './account.service';
import { SignalrService } from './signalr.service';

@Injectable({
  providedIn: 'root'
})
export class InitService {

  constructor(private shoppingCartService: ShoppingCartService, private accountService: AccountService, private signalrService: SignalrService) { }

  init() {
    const shoppingCartId = localStorage.getItem('shoppingCartId');
    const shoppingCart$ = shoppingCartId ? this.shoppingCartService.getShoppingCart(shoppingCartId) : of(null)

    return forkJoin({
      shoppingCart: shoppingCart$,
      user: this.accountService.getUserInfo()
              .pipe(
                tap(user => {
                  if (user) {
                    this.signalrService.createHubConnection();
                  }
                })
              )
    });
  }
}
