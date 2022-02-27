import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UsersManagementComponent } from './users-management.component';


const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: UsersManagementComponent,
  },
//  {
//    path: 'new-user',
//    component: UserDetailsComponent,
//    resolve: {
//      groups: GroupsSubGroupsDataResolver,
//      roles: RoleListDataResolver
//     },
//    data: {
//      isCreateNewUser: true
//    }
//  },
//  {
//    path: 'user-details',
//    component: UserDetailsComponent,
//    resolve: {
//      userDetails: UserDetailsDataResolver,
//      groups: GroupsSubGroupsDataResolver,
//      roles: RoleListDataResolver
//    },
//    data: {
//      isCreateNewUser: false
//    }
//  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsersManagementRoutingModule { }
