import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { NgxEchartsModule } from 'ngx-echarts';
import { SharedModule } from '../shared/shared.module';
import { RoomInsightsRoutingModule } from './room-insights-routing.module';
import { RoomInsightsComponent } from './room-insights.component';



@NgModule({
  declarations: [
    RoomInsightsComponent
  ],
  imports: [
    SharedModule,
    CommonModule,
    RoomInsightsRoutingModule,
    ConfirmationPopoverModule,
    NgxEchartsModule
  ]
})
export class RoomInsightsModule { }
