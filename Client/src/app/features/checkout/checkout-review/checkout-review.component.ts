import { Component, Input } from '@angular/core';
import { ShoppingCartService } from '../../../core/services/shopping-cart.service';
import { CurrencyPipe } from '@angular/common';
import { ConfirmationToken } from '@stripe/stripe-js';
import { AddressPipe } from "../../../shared/pipes/address.pipe";
import { PaymentCardPipe } from "../../../shared/pipes/payment-card.pipe";

@Component({
  selector: 'app-checkout-review',
  imports: [CurrencyPipe, AddressPipe, PaymentCardPipe],
  templateUrl: './checkout-review.component.html',
  styleUrl: './checkout-review.component.scss'
})
export class CheckoutReviewComponent {

  @Input() confirmationToken?: ConfirmationToken;

  get items() {
    return this.shoppingCartService.shoppingCart()?.items;
  }

  constructor(private shoppingCartService: ShoppingCartService) {}
}
