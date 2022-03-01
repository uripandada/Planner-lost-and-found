import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoleListDataResolver } from '../core/resolvers/role-list.resolver';
import { WheresListWithReservationsWithRoomsResolver } from '../core/resolvers/wheres-list.resolver';
import { CleaningPlanComponent } from './cleaning-plan.component';

const routes: Routes = [
  {
    path: '',
    component: CleaningPlanComponent,
    resolve: {
      allWheres: WheresListWithReservationsWithRoomsResolver,
      roles: RoleListDataResolver
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CleaningPlanRoutingModule { }
