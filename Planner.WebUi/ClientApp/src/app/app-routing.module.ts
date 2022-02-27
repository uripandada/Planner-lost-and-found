import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { AuthorizeGuard } from '../api-authorization/authorize.guard';
import { HotelListResolver } from './core/resolvers/hotel-list.resolver';

const routes: Routes = [
  {
    path: '',
    loadChildren: () => import('./main/main.module').then(m => m.MainModule),
    canActivate: [AuthorizeGuard],
    resolve: {
      hotels: HotelListResolver // Resolver is called only to initialize hotels service with data from the server.
    }
  },
  {
    path: '404',
    component: PageNotFoundComponent
  },
  //{
  //  path: '**',
  //  redirectTo: '/404'
  //}
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
