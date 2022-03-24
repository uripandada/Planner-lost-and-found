import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MarketPlaceManagement } from './market-place-management.component';

const routes: Routes = [
  {
    path: '',
    component: MarketPlaceManagement
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MarketPlaceManagementRoutingModule { }
