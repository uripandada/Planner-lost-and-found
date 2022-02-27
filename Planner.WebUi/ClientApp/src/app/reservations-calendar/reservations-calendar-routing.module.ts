import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReservationsCalendarComponent } from './reservations-calendar.component';

const routes: Routes = [
  {
    path: '',
    component: ReservationsCalendarComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReservationsCalendarRoutingModule { }
