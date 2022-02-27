import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RoleManagementRoutingModule } from './role-management-routing.module';
import { RoleManagementComponent } from './role-management.component';
import { SharedModule } from '../shared/shared.module';
import { RoleEditComponent } from './role-edit/role-edit.component';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';


@NgModule({
  declarations: [RoleManagementComponent, RoleEditComponent],
  imports: [
    CommonModule,
    SharedModule,
    RoleManagementRoutingModule,
    ConfirmationPopoverModule
  ]
})
export class RoleManagementModule { }
