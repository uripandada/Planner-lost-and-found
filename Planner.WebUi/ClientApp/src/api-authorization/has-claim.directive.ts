import { Directive, ElementRef, Input, TemplateRef, ViewContainerRef } from '@angular/core';
//import { ConsoleReporter } from 'jasmine';
import { AuthorizeService } from 'src/api-authorization/authorize.service';

@Directive({
  selector: '[hasClaim]'
})
export class HasClaimDirective {

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private authorizeService: AuthorizeService) { }

  @Input() set hasClaim(claimType: any) {
    if (this.authorizeService
      .hasClaim(claimType)) {
      // Add template to DOM
      this.viewContainer.
        createEmbeddedView(this.templateRef);
    } else {
      // Remove template from DOM
      this.viewContainer.clear();
    }
  }


}
