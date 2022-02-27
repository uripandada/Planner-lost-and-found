import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { VisModule } from 'ngx-vis';
import { SharedModule } from '../shared/shared.module';
import { CleaningCalendarRoutingModule } from './cleaning-calendar-routing.module';
import { CleaningCalendarComponent } from './cleaning-calendar.component';

@NgModule({
  imports: [
    SharedModule,
    CleaningCalendarRoutingModule,
    ConfirmationPopoverModule,
    VisModule
  ],
  declarations: [
    CleaningCalendarComponent,
  ],
})
export class CleaningCalendarModule { }
