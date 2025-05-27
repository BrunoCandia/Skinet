import { Injectable } from '@angular/core';
import { ConfirmationToken, loadStripe, Stripe, StripeAddressElement, StripeAddressElementOptions, StripeElements, StripePaymentElement } from '@stripe/stripe-js';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { ShoppingCartService } from './shopping-cart.service';
import { ShoppingCart } from '../../shared/models/shoppingCart';
import { firstValueFrom, map } from 'rxjs';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class StripeService {
  baseUrl = environment.apiUrl;
  private stripePromise: Promise<Stripe | null>;
  private elements?: StripeElements;
  private addressElement?: StripeAddressElement;
  private paymentElement?: StripePaymentElement;

  constructor(private httpClient: HttpClient, private shoppingCartService: ShoppingCartService, private accountService: AccountService) {
    this.stripePromise = loadStripe(environment.stripePublicKey);
  }

  getStripeInstance() {    
    return this.stripePromise;
  }

  async initializeElements(): Promise<StripeElements> {
    if (!this.elements) {
      const stripe = await this.getStripeInstance();
      if (stripe) {
        const shoppingCart = await firstValueFrom(this.createOrUpdatePaymentIntent());
        this.elements = stripe.elements(
          {
            clientSecret: shoppingCart.clientSecret,
            appearance: {labels: 'floating'}
          }
        );
      } else {
        throw new Error('Stripe has not been loaded');
      }
    }

    return this.elements;
  }

  async createAddressElement(): Promise<StripeAddressElement> {
    if (!this.addressElement) {
      const elements = await this.initializeElements();

      if (elements) {
        const user = this.accountService.currentUser();
        let defaultValues: StripeAddressElementOptions['defaultValues'] = {};

        if (user) {
          defaultValues.name = user.firstName + ' ' + user.lastName;
          // defaultValues.firstName = user.firstName;
          // defaultValues.lastName = user.lastName;          
        }

        if (user?.address) {
          defaultValues.address = {
            line1: user.address.line1,
            line2: user.address.line2,
            city: user.address.city,
            state: user.address.state,
            country: user.address.country,
            postal_code: user.address.postalCode
          }
        }

        const options: StripeAddressElementOptions = {
          mode: 'shipping',
          defaultValues: defaultValues
        };

        this.addressElement = elements.create('address', options);
      } else {
        throw new Error('Elements instance has not been loaded');
      }
    }

    return this.addressElement;
  }

  async createConfirmationToken() {
    const stripe = await this.getStripeInstance();
    const elements = await this.initializeElements();
    const result = await elements.submit();

    if (result.error) {
      throw new Error(result.error.message);
    }

    if (stripe) {
      return await stripe.createConfirmationToken({elements});
    } else {
      throw new Error('Stripe not available');
    }
  }

  async confirmPayment(confirmationToken: ConfirmationToken) {
    const stripe = await this.getStripeInstance();
    const elements = await this.initializeElements();
    const result = await elements.submit();

    if (result.error) {
      throw new Error(result.error.message);
    }

    const clientSecret = this.shoppingCartService.shoppingCart()?.clientSecret;

    if (stripe && clientSecret) {
      return await stripe.confirmPayment({
        clientSecret: clientSecret,
        confirmParams: {
          confirmation_token: confirmationToken.id
        },
        redirect: 'if_required'
      });
    } else {
      throw new Error('Stripe not available');
    }
  }

  createOrUpdatePaymentIntent() {
    const shoppingCart = this.shoppingCartService.shoppingCart();

    if (!shoppingCart) throw new Error('Problem with shopping cart');

    return this.httpClient.post<ShoppingCart>(this.baseUrl + 'payments/' + shoppingCart.id, {})
      .pipe(
        map(shoppingCart => {
          this.shoppingCartService.setShoppingCart(shoppingCart);
          //this.shoppingCartService.shoppingCart.set(shoppingCart);
          return shoppingCart;
        })
      );
  }

  async createPaymentElement() {
    if (!this.paymentElement) {
      const elements = await this.initializeElements();

      if (elements) {
        this.paymentElement = elements.create('payment');
      } else {
        throw new Error('Elements instance has not been loaded');
      }
    }

    return this.paymentElement;
  }

  disposeElements() {
    this.elements = undefined;
    this.addressElement = undefined;
    this.paymentElement = undefined;
  }
}
