import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AutoHousekeepingUpdateSettingsComponent } from './auto-housekeeping-update-settings.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: AutoHousekeepingUpdateSettingsComponent,
    resolve: {
      //configuration: CleaningPluginsConfigurationResolver,
      //products: ProductListResolver,
    }
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AutoHousekeepingUpdateSettingsRoutingModule { }
