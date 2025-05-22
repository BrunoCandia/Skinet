import { Component, OnInit } from '@angular/core';
import { ShopService } from '../../../core/services/shop.service';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../../shared/models/product';
import { CurrencyPipe } from '@angular/common';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatDivider } from '@angular/material/divider';
import { ShoppingCartService } from '../../../core/services/shopping-cart.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-product-details',
  imports: [CurrencyPipe, MatIcon, MatButton, MatFormField, MatInput, MatLabel, MatDivider, FormsModule],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss'
})
export class ProductDetailsComponent implements OnInit {
  
  product?: Product;
  quantityInShoppingCart = 0;
  quantity = 1;

  constructor(private shopService: ShopService, private activatedRoute: ActivatedRoute, private shoppingCartService: ShoppingCartService) { }

  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');

    if (id) {
      this.shopService.getProduct(id).subscribe({
        next: product => {
          this.product = product
          this.updateQuantityInShoppingCart();
        },
        error: error => console.log(error)
      });
    }
  }

  updateShoppingCart() {
    if (!this.product) {
      console.error('Product is not defined');
      return;
    } else {
      if (this.quantity > this.quantityInShoppingCart){
        const itemsToAdd = this.quantity - this.quantityInShoppingCart;
        this.quantityInShoppingCart += itemsToAdd;
        this.shoppingCartService.addItemToShoppingcart(this.product, itemsToAdd);
      } else {
        const itemsToRemove = this.quantityInShoppingCart - this.quantity;
        this.quantityInShoppingCart += itemsToRemove;
        this.shoppingCartService.removeItemFromShoppingCart(this.product.id, itemsToRemove);
      }
    }
  }

  getButtonText() {
    return this.quantityInShoppingCart > 0 ? 'Update Shopping Cart' : 'Add to Shopping Cart';
  }

  private updateQuantityInShoppingCart(){
    this.quantityInShoppingCart = this.shoppingCartService.shoppingCart()?.items
                                      .find(item => item.productId === this.product?.id)?.quantity || 0;

    this.quantity = this.quantityInShoppingCart || 1;
  }  
}
