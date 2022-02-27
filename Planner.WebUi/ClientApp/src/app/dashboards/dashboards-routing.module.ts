import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoomsViewDashboardComponent } from './rooms-view-dashboard/rooms-view-dashboard.component';

const routes: Routes = [
  {
    path: 'rooms-view',
    component: RoomsViewDashboardComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardsRoutingModule { }
