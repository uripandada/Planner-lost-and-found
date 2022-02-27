import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { SharedModule } from '../shared/shared.module';
import { CleaningGeneratorLogsRoutingModule } from './cleaning-generator-logs-routing.module';
import { CleaningGeneratorLogsComponent } from './cleaning-generator-logs.component';

@NgModule({
  imports: [
    SharedModule,
    CleaningGeneratorLogsRoutingModule,
    ConfirmationPopoverModule,
  ],
  declarations: [
   CleaningGeneratorLogsComponent,
  ]
})
export class CleaningGeneratorLogsModule { }
