import { Component } from '@angular/core';
import { ShoppingCartService } from '../../core/services/shopping-cart.service';
import { ShoppingCartItem } from '../../shared/models/shoppingCart';
import { ShoppingCartItemComponent } from "./shopping-cart-item/shopping-cart-item.component";
import { OrderSummaryComponent } from "../../shared/components/order-summary/order-summary.component";

@Component({
  selector: 'app-shopping-cart',
  imports: [ShoppingCartItemComponent, OrderSummaryComponent],
  templateUrl: './shopping-cart.component.html',
  styleUrl: './shopping-cart.component.scss'
})
export class ShoppingCartComponent {

  get items(): ShoppingCartItem[] | undefined {
    return this.shoppingCartService.shoppingCart()?.items;
  }
  
  constructor(private shoppingCartService: ShoppingCartService) {}
   
}
