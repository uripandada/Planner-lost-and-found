import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LostAndFoundRoutingModule } from './lost-and-found-routing.module';
import { LostAndFoundComponent } from './lost-and-found.component';
import { SharedModule } from '../shared/shared.module';
import { LostAndFoundEditComponent } from './lost-and-found-edit/lost-and-found-edit.component';
//import { WhereSelectComponent } from '../tasks-management/where-select/where-select.component';


@NgModule({
  declarations: [
    LostAndFoundComponent,
    LostAndFoundEditComponent,
    //WhereSelectComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    LostAndFoundRoutingModule
  ]
})
export class LostAndFoundModule { }
