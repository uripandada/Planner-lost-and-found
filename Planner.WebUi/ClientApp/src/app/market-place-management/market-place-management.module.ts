import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { SharedModule } from '../shared/shared.module';
import { MarketPlaceManagementRoutingModule } from './market-place-management-routing.module';
import { MarketPlaceManagement } from './market-place-management.component';
import { MarketPlaceServicesComponent } from './market-place-services/market-place-services.component';
import { PeekinServiceComponent } from './peekin-service/peekin-service.component';
import { AlexaServiceComponent } from './alexa-service/alexa-service.component';
import { MqttServiceComponent } from './mqtt-service/mqtt-service.component';
import { BacnetServiceComponent } from './bacnet-service/bacnet-service.component';
import { LoungeServiceComponent } from './lounge-service/lounge-service.component';
import { QuicktestServiceComponent } from './quicktest-service/quicktest-service.component';
import { NocoreServiceComponent } from './nocore-service/nocore-service.component';
import { ConciergeServiceComponent } from './concierge-service/concierge-service.component';
import { NikoServiceComponent } from './niko-service/niko-service.component';
import { BowoServiceComponent } from './bowo-service/bowo-service.component';
import { DuveServiceComponent } from './duve-service/duve-service.component';
import { MarketPlaceHeaderComponent } from './market-place-header/market-place-header.component';

@NgModule({
  imports: [
    SharedModule,
    MarketPlaceManagementRoutingModule,
    ConfirmationPopoverModule,
  ],
  declarations: [
    MarketPlaceManagement,
    MarketPlaceServicesComponent,
    PeekinServiceComponent,
    AlexaServiceComponent,
    MqttServiceComponent,
    BacnetServiceComponent,
    LoungeServiceComponent,
    QuicktestServiceComponent,
    NocoreServiceComponent,
    ConciergeServiceComponent,
    NikoServiceComponent,
    BowoServiceComponent,
    DuveServiceComponent,
    MarketPlaceHeaderComponent,
  ]
})
export class MarketPlaceManagementModule { }
