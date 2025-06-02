import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ConfirmationToken, loadStripe, Stripe, StripeAddressElement, StripeAddressElementOptions, StripeElements, StripePaymentElement } from '@stripe/stripe-js';
import { firstValueFrom, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ShoppingCart } from '../../shared/models/shoppingCart';
import { AccountService } from './account.service';
import { ShoppingCartService } from './shopping-cart.service';

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
      throw new Error('Stripe not available in createConfirmationToken');
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
    console.log('Client secret in confirmPayment: ' + clientSecret);

    if (stripe && clientSecret) {
      return await stripe.confirmPayment({
        clientSecret: clientSecret,
        confirmParams: {
          confirmation_token: confirmationToken.id
        },
        redirect: 'if_required'
      });
    } else {
      throw new Error('Stripe not available in confirmPayment');
    }
  }

  createOrUpdatePaymentIntent() {
    const shoppingCart = this.shoppingCartService.shoppingCart();
    const hasClientSecret = !!shoppingCart?.clientSecret;

    console.log('Client secret in createOrUpdatePaymentIntent: ' + shoppingCart?.clientSecret);

    if (!shoppingCart) throw new Error('Problem with shopping cart');

    return this.httpClient.post<ShoppingCart>(this.baseUrl + 'payments/' + shoppingCart.id, {})
      .pipe(
        map(async shoppingCart => {
          if (hasClientSecret) {
            await firstValueFrom(this.shoppingCartService.setShoppingCart(shoppingCart));
            //this.shoppingCartService.shoppingCart.set(shoppingCart);
            return shoppingCart;
          }
          
          return shoppingCart;
        })
      );

    //  const cart = this.cartService.cart();
    // const hasClientSecret = !!cart?.clientSecret;
    // if (!cart) throw new Error('Problem with cart');
    // return this.http.post<Cart>(this.baseUrl + 'payments/' + cart.id, {}).pipe(
    //   map(async cart => {
    //     if (!hasClientSecret) {
    //       await firstValueFrom(this.cartService.setCart(cart));
    //       return cart;
    //     }
    //     return cart;
    //   }))
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
