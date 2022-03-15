import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { SharedModule } from '../shared/shared.module';
import { ExperienceCategoriesRoutingModule } from './experience-categories-management-routing.module';
import { ExperienceCategoriesComponent } from './experience-categories-management.component';
import { ExperienceCategoryEditComponent } from './experience-category-edit/experience-category-edit.component';
import { NgSelect2Module } from "ng-select2"

@NgModule({
  imports: [
    SharedModule,
    NgSelect2Module,
    ExperienceCategoriesRoutingModule,
    ConfirmationPopoverModule,
  ],
  declarations: [
    ExperienceCategoriesComponent,
    ExperienceCategoryEditComponent
  ]
})
export class ExperienceCategoriesModule { }
