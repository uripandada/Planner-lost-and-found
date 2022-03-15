import { NgModule } from '@angular/core';
import { RouterModule, Routes  } from '@angular/router';
import { CategoryListResolver } from '../core/resolvers/category-list.resolver';
import { WheresListResolver } from '../core/resolvers/wheres-list.resolver';
import { ExperienceComponent } from './experience.component';

const routes: Routes = [
  {
    path: '',
    component: ExperienceComponent,
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
export class ExperienceRoutingModule { }
