// import { CurrencyPipe, Location, NgIf } from '@angular/common';
import { CurrencyPipe, Location } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatIcon } from '@angular/material/icon';
import { MatInput } from '@angular/material/input';
import { RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ShoppingCartService } from '../../../core/services/shopping-cart.service';
import { StripeService } from '../../../core/services/stripe.service';

@Component({
  selector: 'app-order-summary',
  // imports: [RouterLink, MatButton, MatFormField, MatLabel, MatInput, MatIcon, FormsModule, CurrencyPipe, NgIf],
  imports: [RouterLink, MatButton, MatFormField, MatLabel, MatInput, MatIcon, FormsModule, CurrencyPipe],
  templateUrl: './order-summary.component.html',
  styleUrl: './order-summary.component.scss'
})
export class OrderSummaryComponent {

  code?: string;

  get locationPath() {
    return this.location.path();
  }

  get subtotal() {
    return this.shoppingCartService.totals()?.subtotal;
  }

  get discount() {
    return this.shoppingCartService.totals()?.discountValue;
  }

  get shipping() {
    return this.shoppingCartService.totals()?.shipping;
  }

  get total() {
    return this.shoppingCartService.totals()?.total;
  }

  get coupon() {
    return this.shoppingCartService.shoppingCart()?.coupon;
  }

  constructor(private shoppingCartService: ShoppingCartService, private location: Location, private stripeService: StripeService) {}

  async removeCouponCode() {
    const shoppingCart = this.shoppingCartService.shoppingCart();

    if (!shoppingCart) {
      return;
    }
    
    if (shoppingCart.coupon) {
      shoppingCart.coupon = undefined;
    }
    
    await firstValueFrom(this.shoppingCartService.setShoppingCart(shoppingCart));

    if (this.location.path() === '/checkout') {
      await firstValueFrom(this.stripeService.createOrUpdatePaymentIntent());
    }
  }

  applyCouponCode() {    
    console.log('Coupon code: ' + this.code);
    if (!this.code) return;
    this.shoppingCartService.applyDiscount(this.code).subscribe({
      next: async coupon => {
        const cart = this.shoppingCartService.shoppingCart();
        if (cart) {
          cart.coupon = coupon;
          await firstValueFrom(this.shoppingCartService.setShoppingCart(cart));
          this.code = undefined;
          if (this.location.path() === '/checkout') {
            await firstValueFrom(this.stripeService.createOrUpdatePaymentIntent());
          }
        }
      }
    });
  }
}
