import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { SharedModule } from '../shared/shared.module';
import { AutoHousekeepingSettingsDetailsComponent } from './auto-housekeeping-settings-details/auto-housekeeping-settings-details.component';
import { AutoHousekeepingUpdateSettingsRoutingModule } from './auto-housekeeping-update-settings-routing.module';
import { AutoHousekeepingUpdateSettingsComponent } from './auto-housekeeping-update-settings.component';

@NgModule({
  declarations: [
    AutoHousekeepingUpdateSettingsComponent,
    AutoHousekeepingSettingsDetailsComponent,
  ],
  imports: [
    SharedModule,
    CommonModule,
    AutoHousekeepingUpdateSettingsRoutingModule,
    ConfirmationPopoverModule
  ]
})
export class AutoHousekeepingUpdateSettingsModule { }
