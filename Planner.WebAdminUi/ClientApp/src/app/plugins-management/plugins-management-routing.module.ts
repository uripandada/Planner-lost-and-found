import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CleaningPluginsConfigurationResolver } from '../core/resolvers/cleaning-plugins-configuration.resolver';
import { ProductListResolver } from '../core/resolvers/product-list.resolver';
import { PluginsManagementComponent } from './plugins-management.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: PluginsManagementComponent,
    resolve: {
      configuration: CleaningPluginsConfigurationResolver,
      //products: ProductListResolver,
    }
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PluginsManagementRoutingModule { }
