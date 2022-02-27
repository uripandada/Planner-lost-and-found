import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { NgxEchartsModule } from 'ngx-echarts';
import { SharedModule } from '../shared/shared.module';
import { TaskConfigurationsRoutingModule } from './task-configurations-routing.module';
import { TaskConfigurationsComponent } from './task-configurations.component';

@NgModule({
  imports: [
    SharedModule,
    TaskConfigurationsRoutingModule,
    ConfirmationPopoverModule,
    NgxEchartsModule,
  ],
  declarations: [
    TaskConfigurationsComponent,
  ]
})
export class TaskConfigurationsModule { }
