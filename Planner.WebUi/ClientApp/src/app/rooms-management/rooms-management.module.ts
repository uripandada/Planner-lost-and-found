import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { SharedModule } from '../shared/shared.module';
import { BuildingComponent } from './building/building.component';
import { FloorComponent } from './floor/floor.component';
import { RoomComponent } from './room/room.component';
import { RoomsManagementRoutingModule } from './rooms-management-routing.module';
import { RoomsManagementComponent } from './rooms-management.component';
import { RoomDetailsComponent } from '../room/room-details/room-details.component';
import { RoomEditFormComponent } from '../room/room-edit-form/room-edit-form.component';
import { DragToSelectModule } from 'ngx-drag-to-select';
import { RoomImportPreviewComponent } from './room-import-preview/room-import-preview.component';
import { RoomBedsEditComponent } from '../room/room-beds-edit/room-beds-edit.component';
import { TemporaryRoomComponent } from './temporary-room/temporary-room.component';


@NgModule({
  imports: [
    SharedModule,
    RoomsManagementRoutingModule,
    ConfirmationPopoverModule,
    DragToSelectModule
  ],
  declarations: [
    RoomsManagementComponent,
    BuildingComponent,
    FloorComponent,
    RoomComponent,
    RoomDetailsComponent,
    RoomEditFormComponent,
    RoomImportPreviewComponent,
    RoomBedsEditComponent,
    TemporaryRoomComponent,
  ]
})
export class RoomsManagementModule { }
