import { NgModule } from '@angular/core';
import { RouterModule, Routes  } from '@angular/router';
import { WheresListResolver } from '../core/resolvers/wheres-list.resolver';
import { LostAndFoundComponent } from './lost-and-found.component';

const routes: Routes = [
  {
    path: '',
    component: LostAndFoundComponent,
    resolve: {
      allWheres: WheresListResolver
    },
    data: {
      ignoreUnAllocatedReservations: true,
      ignoreBuildingsMap: true,
      ignoreWarehouses: true
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LostAndFoundRoutingModule { }
