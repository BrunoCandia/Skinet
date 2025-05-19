import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Pagination } from '../../shared/models/pagination';
import { Product } from '../../shared/models/product';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = "https://localhost:7130/api/";
  params = new HttpParams();

  constructor(private httpClient: HttpClient) { }

  getProducts(brands?: string[], types?: string[], sort?: string) {
    this.params = new HttpParams();

    if (brands && brands.length > 0) {
      this.params = this.params.append('brands', brands.join(','));
    }

    if (types && types.length > 0) {
      this.params = this.params.append('types', types.join(','));
    }

    if (sort) {
      this.params = this.params.append('sort', sort);
    }

    this.params = this.params.append('pageSize', 20);

    return this.httpClient.get<Pagination<Product>>(this.baseUrl + 'products', { params: this.params});
  }

  getTypes() {
    return this.httpClient.get<string[]>(this.baseUrl + 'products/types');
  }

  getBrands() {
    return this.httpClient.get<string[]>(this.baseUrl + 'products/brands');
  }
}
