import { Component, OnInit, output } from '@angular/core';
import { MatRadioModule } from '@angular/material/radio';
import { CheckoutService } from '../../../core/services/checkout.service';
import { DeliveryMethod } from '../../../shared/models/deliveryMethod';
import { CurrencyPipe } from '@angular/common';
import { ShoppingCartService } from '../../../core/services/shopping-cart.service';

@Component({
  selector: 'app-checkout-delivery',
  imports: [MatRadioModule, CurrencyPipe],
  templateUrl: './checkout-delivery.component.html',
  styleUrl: './checkout-delivery.component.scss'
})
export class CheckoutDeliveryComponent implements OnInit {

  deliveryMethods: DeliveryMethod[] = [];
  deliveryComplete = output<boolean>(); 

  get selectedMethodDelivery() {
    return this.shoppingCartService.selectedDeliveryMethod();
  }
  
  constructor(private checkoutService: CheckoutService, private shoppingCartService: ShoppingCartService) {}
  
  ngOnInit(): void {
    //this.checkoutService.getDeliveryMethods().subscribe();
    this.checkoutService.getDeliveryMethods().subscribe({
      next: deliveryMethods => {
        this.deliveryMethods = deliveryMethods;

        if (this.shoppingCartService.shoppingCart()?.deliveryMethodId) {
          const deliveryMethod = deliveryMethods.find(x => x.id === this.shoppingCartService.shoppingCart()?.deliveryMethodId)

          if (deliveryMethod) {
            this.shoppingCartService.selectedDeliveryMethod.set(deliveryMethod);
            this.deliveryComplete.emit(true);
          }
        }
      },
      error: error => console.log(error)
    });
  }

  updateDeliveryMethod(deliveryMethod: DeliveryMethod) {
    this.shoppingCartService.selectedDeliveryMethod.set(deliveryMethod);

    const shoppingCart = this.shoppingCartService.shoppingCart();
    if (shoppingCart) {
      shoppingCart.deliveryMethodId = deliveryMethod.id;
      this.shoppingCartService.setShoppingCart(shoppingCart);
      this.deliveryComplete.emit(true);
    }
  }

}
