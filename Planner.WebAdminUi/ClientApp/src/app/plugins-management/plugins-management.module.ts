import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { SharedModule } from '../shared/shared.module';
import { PluginDetailsComponent } from './plugin-details/plugin-details.component';
import { PluginsManagementRoutingModule } from './plugins-management-routing.module';
import { PluginsManagementComponent } from './plugins-management.component';



@NgModule({
  declarations: [
    PluginsManagementComponent,
    PluginDetailsComponent
  ],
  imports: [
    SharedModule,
    CommonModule,
    PluginsManagementRoutingModule,
    ConfirmationPopoverModule
  ]
})
export class PluginsManagementModule { }
