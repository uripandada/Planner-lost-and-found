import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LostAndFoundCategoriesManagementComponent } from './lost-and-found-categories-management.component';

const routes: Routes = [
  {
    path: '',
    component: LostAndFoundCategoriesManagementComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LostAndFoundCategoriesManagementRoutingModule { }
