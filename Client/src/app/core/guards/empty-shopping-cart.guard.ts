import { CanActivateFn, Router } from '@angular/router';
import { ShoppingCartService } from '../services/shopping-cart.service';
import { inject } from '@angular/core';
import { SnackbarService } from '../services/snackbar.service';

export const emptyShoppingCartGuard: CanActivateFn = (route, state) => {
  const shoppingCartService = inject(ShoppingCartService);
  const router = inject(Router);
  const snackService = inject(SnackbarService);

  if (shoppingCartService.shoppingCart()?.items.length === 0) {
    snackService.error('Yout shopping cart is empty');
    router.navigateByUrl('/shopping-cart');
    return false;
  }
  
  return true;
};
