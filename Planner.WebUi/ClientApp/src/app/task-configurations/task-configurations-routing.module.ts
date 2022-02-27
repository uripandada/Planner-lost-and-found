import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TaskConfigurationsComponent } from './task-configurations.component';

const routes: Routes = [
  {
    path: '',
    component: TaskConfigurationsComponent,
    resolve: {
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TaskConfigurationsRoutingModule { }
