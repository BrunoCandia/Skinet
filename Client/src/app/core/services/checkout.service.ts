import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { DeliveryMethod } from '../../shared/models/deliveryMethod';

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {

  baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  getDeliveryMethods() {
    return this.httpClient.get<DeliveryMethod[]>(this.baseUrl + 'payments/delivery-methods');
  }
}
