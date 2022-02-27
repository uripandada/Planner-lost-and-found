import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { SharedModule } from '../shared/shared.module';
import { ReservationsRoutingModule } from './reservations-routing.module';
import { ReservationsComponent } from './reservations.component';

@NgModule({
  imports: [
    SharedModule,
    ReservationsRoutingModule,
    ConfirmationPopoverModule,
  ],
  declarations: [
    ReservationsComponent,
  ]
})
export class ReservationsModule { }
