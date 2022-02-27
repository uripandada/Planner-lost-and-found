import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { DragToSelectModule } from 'ngx-drag-to-select';
import { VisModule } from 'ngx-vis';
import { CleaningStatusToIcon } from '../shared/directives/cleaning-process-status-to-icon.directive';
import { SharedModule } from '../shared/shared.module';
import { ChangeCreditsFormComponent } from './change-credits-form/change-credits-form.component';
import { CleaningPlanRoutingModule } from './cleaning-plan-routing.module';
import { CleaningPlanComponent } from './cleaning-plan.component';
import { CleaningTimelineCleanerItemComponent } from './cleaning-timeline-cleaner-item/cleaning-timeline-cleaner-item.component';
import { CleaningTimelineComponent } from './cleaning-timeline/cleaning-timeline.component';
import { CpsatConfigurationFormComponent } from './cpsat-configuration-form/cpsat-configuration-form.component';
import { CreateCustomCleaningsFormComponent } from './create-custom-cleanings-form/create-custom-cleanings-form.component';
import { ExtendedCpsatConfigurationComponent } from './extended-cpsat-configuration/extended-cpsat-configuration.component';

@NgModule({
  imports: [
    SharedModule,
    CleaningPlanRoutingModule,
    ConfirmationPopoverModule,
    VisModule,
    DragToSelectModule
  ],
  declarations: [
    CleaningPlanComponent,
    CleaningTimelineComponent,
    CleaningTimelineCleanerItemComponent,
    CreateCustomCleaningsFormComponent,
    CpsatConfigurationFormComponent,
    ChangeCreditsFormComponent,
    ExtendedCpsatConfigurationComponent
  ],
})
export class CleaningPlanModule { }
