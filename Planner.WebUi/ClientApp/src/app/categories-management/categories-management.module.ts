import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { SharedModule } from '../shared/shared.module';
import { CategoriesManagementRoutingModule } from './categories-management-routing.module';
import { CategoriesManagementComponent } from './categories-management.component';
import { CategoryEditComponent } from './category-edit/category-edit.component';


@NgModule({
  imports: [
    SharedModule,
    CategoriesManagementRoutingModule,
    ConfirmationPopoverModule,
  ],
  declarations: [
    CategoriesManagementComponent,
    CategoryEditComponent
  ]
})
export class CategoriesManagementModule { }
