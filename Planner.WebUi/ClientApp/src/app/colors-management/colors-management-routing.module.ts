import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RccHousekeepingStatusColorListResolver } from '../core/resolvers/rcc-housekeeping-status-color-list.resolver';
import { ColorsManagementComponent } from './colors-management.component';

const routes: Routes = [
  {
    path: '',
    component: ColorsManagementComponent,
    resolve: {
      colors: RccHousekeepingStatusColorListResolver,
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ColorsManagementRoutingModule { }
