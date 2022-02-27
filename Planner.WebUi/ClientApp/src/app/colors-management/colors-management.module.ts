import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { SharedModule } from '../shared/shared.module';
import { ColorsManagementRoutingModule } from './colors-management-routing.module';
import { ColorsManagementComponent } from './colors-management.component';

@NgModule({
  imports: [
    SharedModule,
    ColorsManagementRoutingModule,
    ConfirmationPopoverModule,
  ],
  declarations: [
    ColorsManagementComponent,
  ]
})
export class ColorsManagementModule { }
