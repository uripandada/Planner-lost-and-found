import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { DashboardsRoutingModule } from './dashboards-routing.module';
import { RoomMessageListItemComponent } from './room-message-list-item/room-message-list-item.component';
import { RoomMessageListComponent } from './room-message-list/room-message-list.component';
import { RoomMessageSendComplexFormComponent } from './room-message-send-complex-form/room-message-send-complex-form.component';
import { RoomMessageSendSimpleFormComponent } from './room-message-send-simple-form/room-message-send-simple-form.component';
import { RoomsViewDashboardReservationsComponent } from './rooms-view-dashboard-reservations/rooms-view-dashboard-reservations.component';
import { RoomsViewDashboardRoomNameActionsComponent } from './rooms-view-dashboard-room-name-actions/rooms-view-dashboard-room-name-actions.component';
import { RoomsViewDashboardRoomComponent } from './rooms-view-dashboard-room/rooms-view-dashboard-room.component';
import { RoomsViewDashboardWorkersComponent } from './rooms-view-dashboard-workers/rooms-view-dashboard-workers.component';
import { RoomsViewDashboardComponent } from './rooms-view-dashboard/rooms-view-dashboard.component';


@NgModule({
  imports: [
    SharedModule,
    DashboardsRoutingModule,
  ],
  declarations: [
    RoomsViewDashboardComponent,
    RoomsViewDashboardReservationsComponent,
    RoomsViewDashboardRoomComponent,
    RoomsViewDashboardWorkersComponent,
    RoomsViewDashboardRoomNameActionsComponent,
    RoomMessageListComponent,
    RoomMessageListItemComponent,
    RoomMessageSendComplexFormComponent,
    RoomMessageSendSimpleFormComponent,
  ]
})
export class DashboardsModule { }
