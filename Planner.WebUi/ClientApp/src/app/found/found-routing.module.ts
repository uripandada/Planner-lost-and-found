import { NgModule } from '@angular/core';
import { RouterModule, Routes  } from '@angular/router';
import { CategoryListResolver } from '../core/resolvers/category-list.resolver';
import { WheresListResolver } from '../core/resolvers/wheres-list.resolver';
import { FoundComponent } from './found.component';

const routes: Routes = [
  {
    path: '',
    component: FoundComponent,
    resolve: {
      allWheres: WheresListResolver,
      allCategories: CategoryListResolver
    },
    data: {
      ignoreUnAllocatedReservations: true,
      ignoreBuildingsMap: true,
      ignoreWarehouses: true,            
      ignoreTemporaryRooms: true,            
      ignoreFutureReservations: true,
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FoundRoutingModule { }
