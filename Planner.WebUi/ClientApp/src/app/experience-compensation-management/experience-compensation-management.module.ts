import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { SharedModule } from '../shared/shared.module';
import { ExperienceCompensationRoutingModule } from './experience-compensation-management-routing.module';
import { ExperienceCompensationComponent } from './experience-compensation-management.component';
import { ExperienceCompensationEditComponent } from './experience-compensation-edit/experience-compensation-edit.component';


@NgModule({
  imports: [
    SharedModule,
    ExperienceCompensationRoutingModule,
    ConfirmationPopoverModule,
  ],
  declarations: [
    ExperienceCompensationComponent,
    ExperienceCompensationEditComponent
  ]
})
export class ExperienceCompensationModule { }
