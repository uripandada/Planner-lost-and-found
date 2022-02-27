import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FoundComponent } from './found.component';
import { FoundRoutingModule } from './found-routing.module';
import { SharedModule } from '../shared/shared.module';
import { AddEditFoundComponent } from './add-edit-found/add-edit-found.component';

@NgModule({
  declarations: [FoundComponent, AddEditFoundComponent],
  imports: [
    CommonModule,
    SharedModule,
    FoundRoutingModule
  ],
  exports: [FoundComponent, AddEditFoundComponent]
})
export class FoundModule { }
