import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RoleListDataResolver } from '../core/resolvers/role-list.resolver';
import { RoleManagementComponent } from './role-management.component';


const routes: Routes = [
  {
    path: '',
    component: RoleManagementComponent,
    resolve: {
      roles: RoleListDataResolver
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RoleManagementRoutingModule { }
