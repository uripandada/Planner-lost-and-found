import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { NgxEchartsModule } from 'ngx-echarts';
import { SharedModule } from '../shared/shared.module';
import { TaskBalancedComponent } from './task-balanced/task-balanced.component';
import { TaskChatComponent } from './task-chat/task-chat.component';
import { TaskConfigurationComponent } from './task-configuration/task-configuration.component';
import { TaskEditFormComponent } from './task-edit-form/task-edit-form.component';
import { TaskEventComponent } from './task-event/task-event.component';
import { TaskHistoryComponent } from './task-history/task-history.component';
import { TaskRecurringDailyComponent } from './task-recurring-daily/task-recurring-daily.component';
import { TaskRecurringEveryComponent } from './task-recurring-every/task-recurring-every.component';
import { TaskRecurringMonthlyItemComponent } from './task-recurring-monthly-item/task-recurring-monthly-item.component';
import { TaskRecurringMonthlyComponent } from './task-recurring-monthly/task-recurring-monthly.component';
import { TaskRecurringSpecificTimeComponent } from './task-recurring-specific-time/task-recurring-specific-time.component';
import { TaskRecurringWeeklyComponent } from './task-recurring-weekly/task-recurring-weekly.component';
import { TaskRecurringComponent } from './task-recurring/task-recurring.component';
import { TaskSingleComponent } from './task-single/task-single.component';
import { TaskComponent } from './task/task.component';
import { TasksListViewComponent } from './tasks-list-view/tasks-list-view.component';
import { TasksManagementRoutingModule } from './tasks-management-routing.module';
import { TasksManagementComponent } from './tasks-management.component';
import { TasksMonthlyGraphsComponent } from './tasks-monthly-graphs/tasks-monthly-graphs.component';
import { TasksMonthlyViewComponent } from './tasks-monthly-view/tasks-monthly-view.component';
import { TasksPreviewListComponent } from './tasks-preview-list/tasks-preview-list.component';
import { TasksWeeklyViewComponent } from './tasks-weekly-view/tasks-weekly-view.component';
import { WhatMultiselectComponent } from './what-multiselect/what-multiselect.component';
//import { WhereMultiselectComponent } from './where-multiselect/where-multiselect.component';
//import { WhereSelectComponent } from './where-select/where-select.component';
import { WhoMultiselectComponent } from './who-multiselect/who-multiselect.component';


@NgModule({
  imports: [
    SharedModule,
    TasksManagementRoutingModule,
    ConfirmationPopoverModule,
    NgxEchartsModule,
  ],
  declarations: [
    TasksManagementComponent,
    TaskComponent,
    TaskConfigurationComponent,
    TasksListViewComponent,
    TasksWeeklyViewComponent,
    TasksMonthlyViewComponent,
    WhoMultiselectComponent,
    //WhereMultiselectComponent,
    TaskEventComponent,
    TaskRecurringComponent,
    TaskSingleComponent,
    TaskRecurringDailyComponent,
    TaskRecurringMonthlyComponent,
    TaskRecurringSpecificTimeComponent,
    TaskRecurringWeeklyComponent,
    TaskRecurringMonthlyItemComponent,
    TasksPreviewListComponent,
    TaskHistoryComponent,
    TaskChatComponent,
    TaskEditFormComponent,
    TasksMonthlyGraphsComponent,
    TaskBalancedComponent,
    WhatMultiselectComponent,
    //WhereSelectComponent,
    TaskRecurringEveryComponent,
  ]
})
export class TasksManagementModule { }
