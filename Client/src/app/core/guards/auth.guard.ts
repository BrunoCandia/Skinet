import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { inject } from '@angular/core';
import { map, of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);
  
  if (accountService.currentUser()) {
    console.log('User is logged in');
    return true;
  } else {
    router.navigate(['/account/login'], {queryParams: {returnUrl: state.url}});
    console.log('User is not logged in, redirecting to login page');
    return false;
  }

  // if (accountService.currentUser()) {
  //   return of(true);
  // } else {
  //   return accountService.getAuthState().pipe(
  //     map(auth => {
  //       if (auth.isAuthenticated) {
  //         console.log('User is logged in');
  //         return true;
  //       } else {
  //         router.navigate(['/account/login'], {queryParams: {returnUrl: state.url}});
  //         console.log('User is not logged in, redirecting to login page');
  //         return false;
  //       }
  //     })
  //   )
  // }
};
