import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoomInsightsComponent } from './room-insights.component';


const routes: Routes = [
  {
    path: '',
    component: RoomInsightsComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RoomInsightsRoutingModule { }
