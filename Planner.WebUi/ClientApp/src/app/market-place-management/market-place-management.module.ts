import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { SharedModule } from '../shared/shared.module';
import { MarketPlaceManagementRoutingModule } from './market-place-management-routing.module';
import { MarketPlaceManagement } from './market-place-management.component';
import { MarketPlaceServicesComponent } from './market-place-services/market-place-services.component';

@NgModule({
  imports: [
    SharedModule,
    MarketPlaceManagementRoutingModule,
    ConfirmationPopoverModule,
  ],
  declarations: [
    MarketPlaceManagement,
    MarketPlaceServicesComponent,
  ]
})
export class MarketPlaceManagementModule { }
