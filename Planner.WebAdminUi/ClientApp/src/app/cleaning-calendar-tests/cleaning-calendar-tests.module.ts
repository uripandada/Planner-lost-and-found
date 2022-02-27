import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { VisModule } from 'ngx-vis';
import { SharedModule } from '../shared/shared.module';
import { CleaningCalendarTestsRoutingModule } from './cleaning-calendar-tests-routing.module';
import { CleaningCalendarTestsComponent } from './cleaning-calendar-tests.component';

@NgModule({
  imports: [
    SharedModule,
    CleaningCalendarTestsRoutingModule,
    ConfirmationPopoverModule,
    VisModule
  ],
  declarations: [
    CleaningCalendarTestsComponent,
  ],
})
export class CleaningCalendarTestsModule { }
