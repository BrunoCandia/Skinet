import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../core/services/order.service';
import { Order } from '../../shared/models/order';
import { RouterLink } from '@angular/router';
import { CurrencyPipe, DatePipe } from '@angular/common';

@Component({
  selector: 'app-order',
  imports: [RouterLink, CurrencyPipe, DatePipe],
  templateUrl: './order.component.html',
  styleUrl: './order.component.scss'
})
export class OrderComponent implements OnInit {
  
  orders: Order[] = [];

  constructor(private orderService: OrderService) {}
  
  ngOnInit(): void {
    this.orderService.getOrdersForUser().subscribe({
      next: orders => {
        this.orders = orders,
        console.log(orders);
      },
      error: error => console.log(error)
    });
  }
}
