import { Component } from '@angular/core';
import { ShoppingCartService } from '../../core/services/shopping-cart.service';
import { ShoppingCartItem } from '../../shared/models/shoppingCart';
import { ShoppingCartItemComponent } from "./shopping-cart-item/shopping-cart-item.component";
import { OrderSummaryComponent } from "../../shared/components/order-summary/order-summary.component";
import { EmptyStateComponent } from "../../shared/components/empty-state/empty-state.component";
import { Router } from '@angular/router';

@Component({
  selector: 'app-shopping-cart',
  imports: [ShoppingCartItemComponent, OrderSummaryComponent, EmptyStateComponent],
  templateUrl: './shopping-cart.component.html',
  styleUrl: './shopping-cart.component.scss'
})
export class ShoppingCartComponent {

  get items(): ShoppingCartItem[] | undefined {
    return this.shoppingCartService.shoppingCart()?.items;
  }
  
  constructor(private shoppingCartService: ShoppingCartService, private router: Router) {}
   
  navigate() {
    this.router.navigateByUrl('/shop');
  }
}
