import { NgModule } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ExperienceComponent } from './experience.component';
import { ExperienceRoutingModule } from './experience-routing.module';
import { SharedModule } from '../shared/shared.module';
import { AddEditExperienceComponent } from './add-edit-experience/add-edit-experience.component';
import { NgSelect2Module } from "ng-select2"

@NgModule({
  declarations: [ExperienceComponent, AddEditExperienceComponent],
  imports: [
    CommonModule,
    SharedModule,
    ExperienceRoutingModule,
    NgSelect2Module
  ],
  exports: [ExperienceComponent, AddEditExperienceComponent],
  providers: [CurrencyPipe]
})
export class ExperienceModule { }
