import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { OnGuardComponent } from './on-guard.component';

const routes: Routes =  [
  {
    path: '',
    component: OnGuardComponent,

  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OnGuardRoutingModule { }
