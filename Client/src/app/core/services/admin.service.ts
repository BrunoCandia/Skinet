import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Order } from '../../shared/models/order';
import { OrderParams } from '../../shared/models/orderParams';
import { Pagination } from '../../shared/models/pagination';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

  
  constructor(private httpClient: HttpClient) { }

  getOrders(orderParams: OrderParams) {
    let params = new HttpParams();

    if (orderParams.filter && orderParams.filter !== 'All') {
      params = params.append('status', orderParams.filter);
    }

    params = params.append('pageIndex', orderParams.pageIndex);
    params = params.append('pageSize', orderParams.pageSize);

    return this.httpClient.get<Pagination<Order>>(this.baseUrl + 'admin/orders', {params});
  }

  getOrder(id: string) {
    return this.httpClient.get<Order>(this.baseUrl + 'admin/orders/' + id);
  }

  refundOrder(id: string) {
    return this.httpClient.post<Order>(this.baseUrl + 'admin/orders/refund/' + id, {});
  }
}
