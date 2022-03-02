import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoomCategoriesManagementComponent } from './room-categories-management.component';

const routes: Routes = [
  {
    path: '',
    component: RoomCategoriesManagementComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RoomCategoriesManagementRoutingModule { }
