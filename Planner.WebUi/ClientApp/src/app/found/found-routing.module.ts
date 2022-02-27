import { NgModule } from '@angular/core';
import { RouterModule, Routes  } from '@angular/router';
import { WheresListResolver } from '../core/resolvers/wheres-list.resolver';
import { FoundComponent } from './found.component';

const routes: Routes = [
  {
    path: '',
    component: FoundComponent,
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
export class FoundRoutingModule { }
