import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExperienceCategoriesComponent } from './experience-categories-management.component';

const routes: Routes = [
  {
    path: '',
    component: ExperienceCategoriesComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ExperienceCategoriesRoutingModule { }
