import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HotelsManagementRoutingModule } from './hotels-management-routing.module';
import { HotelsManagementComponent } from './hotels-management.component';
import { SharedModule } from '../shared/shared.module';
import { HotelSettingsComponent } from './hotel-settings/hotel-settings.component';


@NgModule({
  declarations: [HotelsManagementComponent, HotelSettingsComponent],
  imports: [
    CommonModule,
    SharedModule,
    HotelsManagementRoutingModule,
  ]
})
export class HotelsManagementModule { }
