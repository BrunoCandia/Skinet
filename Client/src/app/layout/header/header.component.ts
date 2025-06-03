import { Component } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { MatBadge } from '@angular/material/badge';
import { MatProgressBar } from '@angular/material/progress-bar';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { BusyService } from '../../core/services/busy.service';
import { ShoppingCartService } from '../../core/services/shopping-cart.service';
import { AccountService } from '../../core/services/account.service';
import { MatMenu, MatMenuItem, MatMenuTrigger } from '@angular/material/menu';
import { MatDivider } from '@angular/material/divider';
import { IsAdminDirective } from '../../shared/directives/is-admin.directive';

@Component({
  selector: 'app-header',
  imports: [MatIcon, MatButton, MatBadge, RouterLink, RouterLinkActive, MatProgressBar, MatMenuTrigger, MatMenu, MatDivider, MatMenuItem, IsAdminDirective],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  
  get isLoading() {
    return this.busyService.loading();
  }

  get itemCount() {
    return this.shoppingCartService.itemCount();
  }

  get currentUser() {
    return this.accountService.currentUser();
  }
  
  constructor(private busyService: BusyService, private shoppingCartService: ShoppingCartService, private accountService: AccountService, private router: Router) { }

  logout() {
    this.accountService.logout().subscribe({
      next: () => {
        this.accountService.currentUser.set(null);
        this.router.navigateByUrl('/');
      }
    });
  }
}
