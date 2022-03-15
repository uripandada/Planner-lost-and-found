import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ExperienceComponent } from './experience.component';
import { ExperienceRoutingModule } from './experience-routing.module';
import { SharedModule } from '../shared/shared.module';
import { AddEditExperienceComponent } from './add-edit-experience/add-edit-experience.component';

@NgModule({
  declarations: [ExperienceComponent, AddEditExperienceComponent],
  imports: [
    CommonModule,
    SharedModule,
    ExperienceRoutingModule
  ],
  exports: [ExperienceComponent, AddEditExperienceComponent]
})
export class ExperienceModule { }
