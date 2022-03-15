import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { SharedModule } from '../shared/shared.module';
import { LostAndFoundCategoriesManagementRoutingModule } from './lost-and-found-categories-management-routing.module';
import { LostAndFoundCategoriesManagementComponent } from './lost-and-found-categories-management.component';
import { LostAndFoundCategoryEditComponent } from './lost-and-found-category-edit/lost-and-found-category-edit.component';


@NgModule({
  imports: [
    SharedModule,
    LostAndFoundCategoriesManagementRoutingModule,
    ConfirmationPopoverModule,
  ],
  declarations: [
    LostAndFoundCategoriesManagementComponent,
    LostAndFoundCategoryEditComponent
  ]
})
export class LostAndFoundCategoriesManagementModule { }
