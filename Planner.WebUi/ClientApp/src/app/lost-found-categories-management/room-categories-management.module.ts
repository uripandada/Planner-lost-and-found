import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { SharedModule } from '../shared/shared.module';
import { RoomCategoriesManagementRoutingModule } from './room-categories-management-routing.module';
import { RoomCategoriesManagementComponent } from './room-categories-management.component';
import { RoomCategoryEditComponent } from './room-category-edit/room-category-edit.component';


@NgModule({
  imports: [
    SharedModule,
    RoomCategoriesManagementRoutingModule,
    ConfirmationPopoverModule,
  ],
  declarations: [
    RoomCategoriesManagementComponent,
    RoomCategoryEditComponent
  ]
})
export class RoomCategoriesManagementModule { }
