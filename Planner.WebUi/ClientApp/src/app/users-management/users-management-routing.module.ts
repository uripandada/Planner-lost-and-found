import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FullGroupHierarchyDataResolver } from '../core/resolvers/full-group-hierarchy-data.resolver';
import { GroupsSubGroupsDataResolver } from '../core/resolvers/groups-subgroups-data.resolver';
import { HotelListResolver } from '../core/resolvers/hotel-list.resolver';
import { RoleListDataResolver } from '../core/resolvers/role-list.resolver';
import { UserDetailsDataResolver } from '../core/resolvers/user-details.resolver';
import { UserDetailsComponent } from './user-details/user-details.component';
import { UserImportPreviewComponent } from './user-import-preview/user-import-preview.component';
import { UsersManagementComponent } from './users-management.component';


const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: UsersManagementComponent,
    resolve: {
      hierarchy: FullGroupHierarchyDataResolver
    }
  },
  {
    path: 'new-user',
    component: UserDetailsComponent,
    resolve: {
      groups: GroupsSubGroupsDataResolver,
      roles: RoleListDataResolver
     },
    data: {
      isCreateNewUser: true
    }
  },
  {
    path: 'user-details',
    component: UserDetailsComponent,
    resolve: {
      userDetails: UserDetailsDataResolver,
      groups: GroupsSubGroupsDataResolver,
      roles: RoleListDataResolver
    },
    data: {
      isCreateNewUser: false
    }
  },
  {
    path: 'user-import-preview',
    component: UserImportPreviewComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsersManagementRoutingModule { }
