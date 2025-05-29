import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Order, OrderToCreate } from '../../shared/models/order';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  baseUrl = environment.apiUrl;
  orderComplete = false;

  constructor(private httpClient: HttpClient) { }

  createOrder(orderToCreate: OrderToCreate) {
    return this.httpClient.post<Order>(this.baseUrl + 'orders', orderToCreate);
  }

  getOrdersForUser() {
    return this.httpClient.get<Order[]>(this.baseUrl + 'orders');
  }

  getOrderForUser(id: string) {
    return this.httpClient.get<Order>(this.baseUrl + 'orders/' + id);
  }
}
