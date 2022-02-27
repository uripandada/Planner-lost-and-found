import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UsersManagementRoutingModule } from './users-management-routing.module';
import { UsersManagementComponent } from './users-management.component';
import { SharedModule } from '../shared/shared.module';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { UserDetailsComponent } from './user-details/user-details.component';


@NgModule({
  declarations: [
    UsersManagementComponent,
    UserDetailsComponent
  ],
  imports: [
    SharedModule,
    CommonModule,
    UsersManagementRoutingModule,
    ConfirmationPopoverModule
  ]
})
export class UsersManagementModule { }
