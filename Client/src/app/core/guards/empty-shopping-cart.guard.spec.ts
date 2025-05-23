import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { emptyShoppingCartGuard } from './empty-shopping-cart.guard';

describe('emptyShoppingCartGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => emptyShoppingCartGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
