import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CleaningCalendarTestsComponent } from './cleaning-calendar-tests.component';

const routes: Routes = [
  {
    path: '',
    component: CleaningCalendarTestsComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CleaningCalendarTestsRoutingModule { }
