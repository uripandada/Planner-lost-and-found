import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { OnGuardRoutingModule } from './on-guard-routing.module';
import { OnGuardComponent } from './on-guard.component';
import { OnGuardEditComponent } from './on-guard-edit/on-guard-edit.component';
import { SharedModule } from '../shared/shared.module';


@NgModule({
  declarations: [OnGuardComponent, OnGuardEditComponent],
  imports: [
    CommonModule,
    SharedModule,
    OnGuardRoutingModule
  ]
})
export class OnGuardModule { }
