import { Directive, effect, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../../core/services/account.service';

@Directive({
  selector: '[appIsAdmin]'  // *appIsAdmin
})
export class IsAdminDirective implements OnInit {

  constructor(private accountService: AccountService, private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>) { 
    effect(() => {
      if (this.accountService.isAdmin()) {
        this.viewContainerRef.createEmbeddedView(this.templateRef);
      } else {
        this.viewContainerRef.clear();
      }
    });
  }
  
  ngOnInit(): void {

    // This does not work!!!

    // if (this.accountService.isAdmin()) {
    //   this.viewContainerRef.createEmbeddedView(this.templateRef);
    // } else {
    //   this.viewContainerRef.clear();
    // }
  }

}
