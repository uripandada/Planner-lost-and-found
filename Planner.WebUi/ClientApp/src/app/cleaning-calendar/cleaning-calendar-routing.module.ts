import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CleaningCalendarComponent } from './cleaning-calendar.component';

const routes: Routes = [
  {
    path: '',
    component: CleaningCalendarComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CleaningCalendarRoutingModule { }
