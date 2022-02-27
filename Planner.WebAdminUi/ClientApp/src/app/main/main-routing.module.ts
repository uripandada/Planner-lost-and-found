import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthorizeGuard } from '../../api-authorization/authorize.guard';
import { MainComponent } from './main.component';

const routes: Routes = [
  {
    path: '',
    component: MainComponent,
    pathMatch: 'full',
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'users-management',
    component: MainComponent,
    loadChildren: () => import('../users-management/users-management.module').then(m => m.UsersManagementModule),
    canActivate: [AuthorizeGuard],
  },
  {
    path: 'cleaning-plugins',
    component: MainComponent,
    loadChildren: () => import('../plugins-management/plugins-management.module').then(m => m.PluginsManagementModule),
    canActivate: [AuthorizeGuard],
  },
  {
    path: 'cleaning-calendar',
    component: MainComponent,
    loadChildren: () => import('../cleaning-calendar/cleaning-calendar.module').then(m => m.CleaningCalendarModule),
    canActivate: [AuthorizeGuard],
  },
  {
    path: 'cleaning-calendar-tests',
    component: MainComponent,
    loadChildren: () => import('../cleaning-calendar-tests/cleaning-calendar-tests.module').then(m => m.CleaningCalendarTestsModule),
    canActivate: [AuthorizeGuard],
  },
  {
    path: 'auto-housekeeping-update-settings',
    component: MainComponent,
    loadChildren: () => import('../auto-housekeeping-settings/auto-housekeeping-update-settings.module').then(m => m.AutoHousekeepingUpdateSettingsModule),
    canActivate: [AuthorizeGuard],
  },
  {
    path: 'hotel-groups',
    component: MainComponent,
    loadChildren: () => import('../hotel-groups/hotel-groups.module').then(m => m.HotelGroupsModule),
    canActivate: [AuthorizeGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MainRoutingModule { }
