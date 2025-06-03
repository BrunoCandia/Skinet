import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../../core/services/order.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Order } from '../../../shared/models/order';
import { MatCardModule } from '@angular/material/card';
import { MatButton } from '@angular/material/button';
import { AddressPipe } from "../../../shared/pipes/address.pipe";
import { PaymentCardPipe } from "../../../shared/pipes/payment-card.pipe";
import { CurrencyPipe, DatePipe } from '@angular/common';
import { AccountService } from '../../../core/services/account.service';
import { AdminService } from '../../../core/services/admin.service';

@Component({
  selector: 'app-order-details',
  imports: [MatCardModule, MatButton, AddressPipe, PaymentCardPipe, DatePipe, CurrencyPipe],
  templateUrl: './order-details.component.html',
  styleUrl: './order-details.component.scss'
})
export class OrderDetailsComponent implements OnInit {

  order?: Order;
  buttonText: string = '';
  //buttonText = this.accountService.isAdmin() ? 'Return to Admin' : 'Return to orders';

  constructor(private orderService: OrderService, private activatedRoute: ActivatedRoute, private router: Router, private accountService: AccountService, private adminService: AdminService) {}
  
  ngOnInit(): void {
    this.buttonText = this.accountService.isAdmin() ? 'Return to Admin' : 'Return to orders';

    this.loadOrder();
  }

  loadOrder() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');

    if (id) {
      
      const loadOrderData = this.accountService.isAdmin()
      ? this.adminService.getOrder(id)
      : this.orderService.getOrderForUser(id);

      loadOrderData.subscribe({
        next: order => this.order = order,
        error: error => console.log(error)
      });
    }    

    // if (id) {
    //   this.orderService.getOrderForUser(id).subscribe({
    //     next: order => this.order = order,
    //     error: error => console.log(error)        
    //   })
    // }
  }

  onReturnClick() {

    this.accountService.isAdmin()
      ? this.router.navigateByUrl('/admin')
      : this.router.navigateByUrl('/orders');

    //this.router.navigateByUrl('/orders');
  }
}
