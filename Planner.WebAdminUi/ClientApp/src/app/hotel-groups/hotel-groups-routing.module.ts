import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TimeZoneListResolver } from '../core/resolvers/time-zone-list.resolver';
import { HotelGroupsComponent } from './hotel-groups.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: HotelGroupsComponent,
    resolve: {
      timeZones: TimeZoneListResolver
    }
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HotelGroupsRoutingModule { }
