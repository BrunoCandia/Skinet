import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { OrderSummaryComponent } from "../../shared/components/order-summary/order-summary.component";
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { MatButton } from '@angular/material/button';
import { Router, RouterLink } from '@angular/router';
import { StripeService } from '../../core/services/stripe.service';
import { ConfirmationToken, StripeAddressElement, StripeAddressElementChangeEvent, StripePaymentElement, StripePaymentElementChangeEvent } from '@stripe/stripe-js';
import { SnackbarService } from '../../core/services/snackbar.service';
import { MatCheckboxChange, MatCheckboxModule } from '@angular/material/checkbox'
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { Address } from '../../shared/models/address';
import { AccountService } from '../../core/services/account.service';
import { firstValueFrom } from 'rxjs';
import { CheckoutDeliveryComponent } from "./checkout-delivery/checkout-delivery.component";
import { CheckoutReviewComponent } from "./checkout-review/checkout-review.component";
import { ShoppingCartService } from '../../core/services/shopping-cart.service';
import { CurrencyPipe, JsonPipe } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner'

@Component({
  selector: 'app-checkout',
  imports: [OrderSummaryComponent, MatStepperModule, MatButton, RouterLink, MatCheckboxModule, CheckoutDeliveryComponent, CheckoutReviewComponent, CurrencyPipe, JsonPipe, MatProgressSpinnerModule],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss'
})
export class CheckoutComponent implements OnInit, OnDestroy {
  isSavedAsDefaultAddress = false;
  addressElement?: StripeAddressElement;
  paymentElement?: StripePaymentElement;
  completionStatus = signal<{address: boolean, delivery: boolean, card: boolean}>(
    {address: false, delivery: false, card: false}
  );
  confirmationToken?: ConfirmationToken;
  loading = false;

  get total() {
    return this.shoppingCartService.totals()?.total;
  }

  constructor(private stripeService: StripeService, private snackbarService: SnackbarService, private accountService: AccountService, private shoppingCartService: ShoppingCartService, private router: Router) {
    // Option 1, manually bind in the constructor the function to the class
    //this.handleAddressChange = this.handleAddressChange.bind(this);
  }    
  
  async ngOnInit(): Promise<void> {
    try {
      this.addressElement = await this.stripeService.createAddressElement();
      this.addressElement.mount('#address-element');
      this.addressElement.on('change', this.handleAddressChange);

      this.paymentElement = await this.stripeService.createPaymentElement();
      this.paymentElement.mount('#payment-element');
      this.paymentElement.on('change', this.handlePaymentChange);
    } catch (error: any) {
      this.snackbarService.error(error.message);
    }
  }

  // Option 2, bind using an arrow function
  handleAddressChange = (event: StripeAddressElementChangeEvent) => {
    this.completionStatus.update(state => {
      state.address = event.complete;
      return state;
    });
  }

  // handleAddressChange(event: StripeAddressElementChangeEvent) {
  //   this.completionStatus.update(state => {
  //     state.address = event.complete;
  //     return state;
  //   });
  // }

  handlePaymentChange = (event: StripePaymentElementChangeEvent) => {
    this.completionStatus.update(state => {
      state.card = event.complete;
      return state;
    });
  }

  handleDeliveryChange(event: boolean) {
    this.completionStatus.update(state => {
      state.delivery = event;
      return state;
    });
  }

  onSaveAddressCheckboxChange(event: MatCheckboxChange) {
    this.isSavedAsDefaultAddress = event.checked;
  }

  async onStepChange(event: StepperSelectionEvent) {
    if (event.selectedIndex === 1) {
      if (this.isSavedAsDefaultAddress) {
        //const address = await this.addressElement?.getValue();
        const address = await this.getAddressFormStripeAddress();
        
        address && firstValueFrom(this.accountService.updateAddress(address))

        // if (address){
        //   firstValueFrom(this.accountService.updateAddress(address));
        // }
      }
    }

    if (event.selectedIndex === 2) {
      await firstValueFrom(this.stripeService.createOrUpdatePaymentIntent());
    }

    if (event.selectedIndex === 3) {
      await this.getConfirmationToken();
    }
  }

  async confirmPayment(stepper: MatStepper) {
    this.loading = true;

    try {
      if (this.confirmationToken) {
        const result = await this.stripeService.confirmPayment(this.confirmationToken);

        if (result.error) {
          throw new Error(result.error.message);
        } else {
          this.shoppingCartService.deleteShoppingCart();
          this.shoppingCartService.selectedDeliveryMethod.set(null);
          this.router.navigateByUrl('checkout/success');
        }
      }
    } catch (error: any) {
      this.snackbarService.error(error.message || 'Something wnet wrong');
      stepper.previous();
    } finally {
      this.loading = false;
    }
  }

  async getConfirmationToken() {
    try {
      if (Object.values(this.completionStatus()).every(status => status === true)) {
        const result = await this.stripeService.createConfirmationToken();

        if (result.error) {
          throw new Error(result.error.message);
        }

        this.confirmationToken = result.confirmationToken;

        console.log(this.confirmationToken);
      } 
    } catch (error: any) {
      this.snackbarService.error(error.message);
    }    
  }

  private async getAddressFormStripeAddress(): Promise<Address | null> {
    const result = await this.addressElement?.getValue();
    const address = result?.value.address;

    if (address) {
      return {
        line1: address.line1,
        line2: address.line2 || undefined,
        city: address.city,
        state: address.state,
        country: address.country,
        postalCode: address.postal_code
      }
    } else {
      return null;
    }
  }

  ngOnDestroy(): void {
    this.stripeService.disposeElements();
  }
}
