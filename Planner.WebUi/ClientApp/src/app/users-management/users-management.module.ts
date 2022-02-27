import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UsersManagementRoutingModule } from './users-management-routing.module';
import { UsersManagementComponent } from './users-management.component';
import { SharedModule } from '../shared/shared.module';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { GroupComponent } from './group/group.component';
import { SubgroupComponent } from './subgroup/subgroup.component';
import { UserDetailsComponent } from './user-details/user-details.component';
import { UserImportPreviewComponent } from './user-import-preview/user-import-preview.component';


@NgModule({
  declarations: [UsersManagementComponent, GroupComponent, SubgroupComponent, UserDetailsComponent, UserImportPreviewComponent],
  imports: [
    SharedModule,
    CommonModule,
    UsersManagementRoutingModule,
    ConfirmationPopoverModule
  ]
})
export class UsersManagementModule { }
