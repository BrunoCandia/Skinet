import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatIcon } from '@angular/material/icon';
import { MatInput } from '@angular/material/input';
import { RouterLink } from '@angular/router';
import { ShoppingCartService } from '../../../core/services/shopping-cart.service';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-order-summary',
  imports: [RouterLink, MatButton, MatFormField, MatLabel, MatInput, MatIcon, FormsModule, CurrencyPipe],
  templateUrl: './order-summary.component.html',
  styleUrl: './order-summary.component.scss'
})
export class OrderSummaryComponent {

  get subtotal() {
    return this.shoppingCartService.totals()?.subtotal;
  }

  get discount() {
    return this.shoppingCartService.totals()?.discount;
  }

  get shipping() {
    return this.shoppingCartService.totals()?.shipping;
  }

  get total() {
    return this.shoppingCartService.totals()?.total;
  }

  constructor(private shoppingCartService: ShoppingCartService) {}

  removeCouponCode() {
  throw new Error('Method not implemented.');
  }
  applyCouponCode() {
  throw new Error('Method not implemented.');
  }
}
