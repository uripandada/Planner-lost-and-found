import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExperienceCompensationComponent } from './experience-compensation-management.component';

const routes: Routes = [
  {
    path: '',
    component: ExperienceCompensationComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ExperienceCompensationRoutingModule { }
