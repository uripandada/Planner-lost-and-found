import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { SharedModule } from '../shared/shared.module';
import { HotelGroupDetailsComponent } from './hotel-group-details/hotel-group-details.component';
import { HotelGroupsRoutingModule } from './hotel-groups-routing.module';
import { HotelGroupsComponent } from './hotel-groups.component';

@NgModule({
  declarations: [
    HotelGroupsComponent,
    HotelGroupDetailsComponent
  ],
  imports: [
    SharedModule,
    CommonModule,
    HotelGroupsRoutingModule,
    ConfirmationPopoverModule
  ]
})
export class HotelGroupsModule { }
