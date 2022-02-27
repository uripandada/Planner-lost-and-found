import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { VisModule } from 'ngx-vis';
import { SharedModule } from '../shared/shared.module';
import { ReservationsCalendarRoutingModule } from './reservations-calendar-routing.module';
import { ReservationsCalendarComponent } from './reservations-calendar.component';

@NgModule({
  imports: [
    SharedModule,
    ReservationsCalendarRoutingModule,
    ConfirmationPopoverModule,
    VisModule
  ],
  declarations: [
    ReservationsCalendarComponent,
  ],
})
export class ReservationsCalendarModule { }
