import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoomsManagementComponent } from './rooms-management.component';
import { RoomDetailsComponent } from '../room/room-details/room-details.component';
import { BuildingSimpleDataResolver } from '../core/resolvers/building-simple.resolver';
import { RoomDetailsDataResolver } from '../core/resolvers/room-details-data.resolver';
import { RoomCategoryListResolver } from '../core/resolvers/room-category-list.resolver';
import { RoomImportPreviewComponent } from './room-import-preview/room-import-preview.component';

const routes: Routes = [
  {
    path: '',
    component: RoomsManagementComponent,
    resolve: {
      roomCategories: RoomCategoryListResolver
    },
  },
  {
    path: 'new-room',
    component: RoomDetailsComponent,
    resolve: {
      building: BuildingSimpleDataResolver,
      roomCategories: RoomCategoryListResolver
    },
    data: {
      isCreateNewRoom: true
    }
  },
  {
    path: 'room-details',
    component: RoomDetailsComponent,
    resolve: {
      roomDetails: RoomDetailsDataResolver,
      roomCategories: RoomCategoryListResolver
    },
    data: {
      isCreateNewRoom: false
    }
  },
  {
    path: 'room-import-preview',
    component: RoomImportPreviewComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RoomsManagementRoutingModule { }
