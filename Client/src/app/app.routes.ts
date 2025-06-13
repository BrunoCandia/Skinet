import { Routes } from '@angular/router';
// import { adminGuard } from './core/guards/admin.guard';
// import { authGuard } from './core/guards/auth.guard';
// import { emptyShoppingCartGuard } from './core/guards/empty-shopping-cart.guard';
// import { orderCompleteGuard } from './core/guards/order-complete.guard';
// import { LoginComponent } from './features/account/login/login.component';
// import { RegisterComponent } from './features/account/register/register.component';
// import { AdminComponent } from './features/admin/admin.component';
// import { CheckoutSuccessComponent } from './features/checkout/checkout-success/checkout-success.component';
// import { CheckoutComponent } from './features/checkout/checkout.component';
import { HomeComponent } from './features/home/home.component';
// import { OrderDetailsComponent } from './features/orders/order-details/order-details.component';
// import { OrderComponent } from './features/orders/order.component';
import { ProductDetailsComponent } from './features/shop/product-details/product-details.component';
import { ShopComponent } from './features/shop/shop.component';
import { ShoppingCartComponent } from './features/shopping-cart/shopping-cart.component';
import { TestErrorComponent } from './features/test-error/test-error.component';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';
import { ServerErrorComponent } from './shared/components/server-error/server-error.component';

export const routes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'shop', component: ShopComponent},
    {path: 'shop/:id', component: ProductDetailsComponent},
    {path: 'shopping-cart', component: ShoppingCartComponent},
    {path: 'checkout', loadChildren: () => import('./features/checkout/routes').then(routes => routes.checkoutRoutes)},
    // {path: 'checkout', component: CheckoutComponent, canActivate: [authGuard, emptyShoppingCartGuard]},
    // {path: 'checkout/success', component: CheckoutSuccessComponent, canActivate: [authGuard, orderCompleteGuard]},
    {path: 'orders', loadChildren: () => import('./features/orders/routes').then(routes => routes.orderRoutes)},
    // {path: 'orders', component: OrderComponent, canActivate: [authGuard]},
    // {path: 'orders/:id', component: OrderDetailsComponent, canActivate: [authGuard]},
    {path: 'account', loadChildren: () => import('./features/account/routes').then(routes => routes.accountRoutes)},
    // {path: 'account/login', component: LoginComponent},
    // {path: 'account/register', component: RegisterComponent},
    {path: 'test-error', component: TestErrorComponent},
    {path: 'not-found', component: NotFoundComponent},
    {path: 'server-error', component: ServerErrorComponent},
    {path: 'admin', loadChildren: () => import('./features/admin/routes').then(routes => routes.adminRoutes)},
    // {path: 'admin', component: AdminComponent, canActivate: [authGuard, adminGuard]},
    {path: '**', redirectTo: 'not-found', pathMatch: 'full'},
];
