import { Component, input } from '@angular/core';
import { ShoppingCartItem } from '../../../shared/models/shoppingCart';
import { RouterLink } from '@angular/router';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { CurrencyPipe } from '@angular/common';
import { ShoppingCartService } from '../../../core/services/shopping-cart.service';

@Component({
  selector: 'app-shopping-cart-item',
  imports: [RouterLink, MatButton, MatIcon, CurrencyPipe],
  templateUrl: './shopping-cart-item.component.html',
  styleUrl: './shopping-cart-item.component.scss'
})
export class ShoppingCartItemComponent {

  item = input.required<ShoppingCartItem>();

  constructor(private shoppingCartService: ShoppingCartService) {}

  decrementQuantity() {
    this.shoppingCartService.removeItemFromShoppingCart(this.item().productId);
  }

  incrementQuantity() {
    this.shoppingCartService.addItemToShoppingcart(this.item());
  }

  removeItemFromCart() {
    this.shoppingCartService.removeItemFromShoppingCart(this.item().productId, this.item().quantity);
  }
}
