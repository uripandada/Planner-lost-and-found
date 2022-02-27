import { NgModule } from '@angular/core';
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MomentDateAdapter } from '@angular/material-moment-adapter/adapter';
import { DateAdapter } from '@angular/material/core';
import { RouterModule, Routes } from '@angular/router';
import { CustomDateAdapter } from '../core/custom-date-adapter';
import { TasksDataResolver } from '../core/resolvers/tasks-data.resolver';
import { TasksManagementComponent } from './tasks-management.component';

const routes: Routes = [
  {
    path: '',
    component: TasksManagementComponent,
    resolve: {
      tasksData: TasksDataResolver
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TasksManagementRoutingModule { }
