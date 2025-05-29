import { Component, OnDestroy } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { SignalrService } from '../../../core/services/signalr.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CurrencyPipe, DatePipe, NgIf } from '@angular/common';
import { AddressPipe } from '../../../shared/pipes/address.pipe';
import { PaymentCardPipe } from '../../../shared/pipes/payment-card.pipe';
import { OrderService } from '../../../core/services/order.service';

@Component({
  selector: 'app-checkout-success',
  imports: [MatButton, RouterLink, MatProgressSpinnerModule, DatePipe, CurrencyPipe, AddressPipe, PaymentCardPipe, NgIf],
  templateUrl: './checkout-success.component.html',
  styleUrl: './checkout-success.component.scss'
})
export class CheckoutSuccessComponent implements OnDestroy {
  
  get order() {
    return this.signalrService.orderSignal();
  }  

  constructor(private signalrService: SignalrService, private orderService: OrderService) {}
  
  ngOnDestroy(): void {
    this.orderService.orderComplete = false;
    this.signalrService.orderSignal.set(null);
  }
}
