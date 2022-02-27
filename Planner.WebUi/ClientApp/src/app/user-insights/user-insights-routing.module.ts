import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserInsightsComponent } from './user-insights.component';


const routes: Routes = [
  {
    path: '',
    component: UserInsightsComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserInsightsRoutingModule { }
