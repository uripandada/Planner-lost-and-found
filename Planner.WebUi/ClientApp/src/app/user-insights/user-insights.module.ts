import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { NgxEchartsModule } from 'ngx-echarts';
import { SharedModule } from '../shared/shared.module';
import { UserInsightsRoutingModule } from './user-insights-routing.module';
import { UserInsightsComponent } from './user-insights.component';



@NgModule({
  declarations: [
    UserInsightsComponent
  ],
  imports: [
    SharedModule,
    CommonModule,
    UserInsightsRoutingModule,
    ConfirmationPopoverModule,
    NgxEchartsModule
  ]
})
export class UserInsightsModule { }
