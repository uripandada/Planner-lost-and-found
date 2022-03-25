import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AlexaServiceComponent } from './alexa-service/alexa-service.component';
import { BacnetServiceComponent } from './bacnet-service/bacnet-service.component';
import { BowoServiceComponent } from './bowo-service/bowo-service.component';
import { ConciergeServiceComponent } from './concierge-service/concierge-service.component';
import { DuveServiceComponent } from './duve-service/duve-service.component';
import { LoungeServiceComponent } from './lounge-service/lounge-service.component';
import { MarketPlaceManagement } from './market-place-management.component';
import { MqttServiceComponent } from './mqtt-service/mqtt-service.component';
import { NikoServiceComponent } from './niko-service/niko-service.component';
import { NocoreServiceComponent } from './nocore-service/nocore-service.component';
import { PeekinServiceComponent } from './peekin-service/peekin-service.component';
import { QuicktestServiceComponent } from './quicktest-service/quicktest-service.component';

const routes: Routes = [
  {
    path: '',
    component: MarketPlaceManagement
  },
  {
    path: 'peekin-service',
    component: PeekinServiceComponent
  },
  {
    path: 'bacnet-service',
    component: BacnetServiceComponent
  },
  {
    path: 'bowo-service',
    component: BowoServiceComponent
  },
  {
    path: 'concierge-service',
    component: ConciergeServiceComponent
  },
  {
    path: 'duve-service',
    component: DuveServiceComponent
  },
  {
    path: 'lounge-service',
    component: LoungeServiceComponent
  },
  {
    path: 'quicktest-service',
    component: QuicktestServiceComponent
  },
  {
    path: 'nocore-service',
    component: NocoreServiceComponent
  },
  {
    path: 'niko-service',
    component: NikoServiceComponent
  },
  {
    path: 'mqtt-service',
    component: MqttServiceComponent
  },
  {
    path: 'alexa-service',
    component: AlexaServiceComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MarketPlaceManagementRoutingModule { }
