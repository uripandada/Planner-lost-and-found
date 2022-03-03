import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MainComponent } from './main.component';
import { AuthorizeGuard } from '../../api-authorization/authorize.guard';
import { ClaimsKeys, ManagementClaimKeys, SettingsClaimKeys } from 'src/api-authorization/claim-keys';
import { CleaningGeneratorLogsComponent } from '../cleaning-generator-logs/cleaning-generator-logs.component';

const routes: Routes = [
  { path: '',
    component: MainComponent,
    pathMatch: 'full',
    redirectTo: '/dashboards/rooms-view'
  },
  {
    path: 'cleaning-logs',
    component: MainComponent,
    pathMatch: 'full',
    children: [{
      path: '',
      component: CleaningGeneratorLogsComponent,
    }]
  },
  {
    path: 'rooms-management',
    component: MainComponent,
    loadChildren: () => import('../rooms-management/rooms-management.module').then(m => m.RoomsManagementModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [SettingsClaimKeys.Rooms] }
  },
  {
    path: 'assets-management',
    component: MainComponent,
    loadChildren: () => import('../assets-management/assets-management.module').then(m => m.AssetsManagementModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [SettingsClaimKeys.Assets] }
  },
  {
    path: 'users-management',
    component: MainComponent,
    loadChildren: () => import('../users-management/users-management.module').then(m => m.UsersManagementModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [SettingsClaimKeys.Users] }
  },
  {
    path: 'cleaning-plan',
    component: MainComponent,
    loadChildren: () => import('../cleaning-plan/cleaning-plan.module').then(m => m.CleaningPlanModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [ManagementClaimKeys.CleaningPlanner] }
  },
  {
    path: 'hotels-management',
    component: MainComponent,
    loadChildren: () => import('../hotels-management/hotels-management.module').then(m => m.HotelsManagementModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [SettingsClaimKeys.HotelSettings] }
  },
  {
    path: 'tasks-management',
    component: MainComponent,
    loadChildren: () => import('../tasks-management/tasks-management.module').then(m => m.TasksManagementModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [ManagementClaimKeys.Tasks] }
  },
  {
    path: 'task-configurations',
    component: MainComponent,
    loadChildren: () => import('../task-configurations/task-configurations.module').then(m => m.TaskConfigurationsModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [ManagementClaimKeys.Tasks] }
  },
  {
    path: 'role-management',
    component: MainComponent,
    loadChildren: () => import('../role-management/role-management.module').then(m => m.RoleManagementModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [SettingsClaimKeys.RoleManagement] }
  },
  {
    path: 'lost',
    component: MainComponent,
    loadChildren: () => import('../lost-and-found/lost-and-found.module').then(m => m.LostAndFoundModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [ManagementClaimKeys.LostAndFound] }
  },
  {
    path: 'found',
    component: MainComponent,
    loadChildren: () => import('../found/found.module').then(m => m.FoundModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [ManagementClaimKeys.LostAndFound] }
  },
  {
    path: 'on-guard',
    component: MainComponent,
    loadChildren: () => import('../on-guard/on-guard.module').then(m => m.OnGuardModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [ManagementClaimKeys.OnGuard] }
  },
  {
    path: 'room-categories',
    component: MainComponent,
    loadChildren: () => import('../room-categories-management/room-categories-management.module').then(m => m.RoomCategoriesManagementModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [SettingsClaimKeys.RoomCategories] }
  },
  {
    path: 'categories',
    component: MainComponent,
    loadChildren: () => import('../categories-management/categories-management.module').then(m => m.CategoriesManagementModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [SettingsClaimKeys.Categories] }
  },
  {
    path: 'reservations',
    component: MainComponent,
    loadChildren: () => import('../reservations/reservations.module').then(m => m.ReservationsModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [ManagementClaimKeys.Reservations] }
  },
  {
    path: 'room-insights',
    component: MainComponent,
    loadChildren: () => import('../room-insights/room-insights.module').then(m => m.RoomInsightsModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [ManagementClaimKeys.RoomInsights] }

  },
  {
    path: 'user-insights',
    component: MainComponent,
    loadChildren: () => import('../user-insights/user-insights.module').then(m => m.UserInsightsModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [ManagementClaimKeys.UserInsights] }
  },
  {
    path: 'cleaning-calendar',
    component: MainComponent,
    loadChildren: () => import('../cleaning-calendar/cleaning-calendar.module').then(m => m.CleaningCalendarModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [ManagementClaimKeys.CleaningCalendar] }
  },
  {
    path: 'reservations-calendar',
    component: MainComponent,
    loadChildren: () => import('../reservations-calendar/reservations-calendar.module').then(m => m.ReservationsCalendarModule),
    canActivate: [AuthorizeGuard],
    data: { claims: [ManagementClaimKeys.ReservationCalendar] }
  },
  {
    path: 'warehouses',
    component: MainComponent,
    loadChildren: () => import('../warehouses-management/warehouses-management.module').then(m => m.WarehousesManagementModule),
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'dashboards',
    component: MainComponent,
    loadChildren: () => import('../dashboards/dashboards.module').then(m => m.DashboardsModule),
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'colors-management',
    component: MainComponent,
    loadChildren: () => import('../colors-management/colors-management.module').then(m => m.ColorsManagementModule),
    canActivate: [AuthorizeGuard]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MainRoutingModule { }
