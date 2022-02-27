import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SidebarMenuComponent } from './components/sidebar-menu/sidebar-menu.component';
import { AngularMaterialModule } from '../core/angular-material.module';
import { PopupComponent } from './components/popup/popup.component';
import { FileUploadComponent } from './components/file-upload/file-upload.component';
import { FileUploadItemComponent } from './components/file-upload/file-upload-item/file-upload-item.component';
import { TagsMultiselectComponent } from './components/tags-multiselect/tags-multiselect.component';
import { TimeIntervalInputComponent } from '../cleaning-plan/time-interval-input/time-interval-input.component';
import { TimeIntervalMultiselectComponent } from '../cleaning-plan/time-interval-multiselect/time-interval-multiselect.component';
import { TextMaskModule } from 'angular2-text-mask';
import { DateTimeComponent } from './components/date-time/date-time.component';
import { DateTimeMultiselectComponent } from './components/date-time-multiselect/date-time-multiselect.component';
import { TimeMultiselectComponent } from './components/time-multiselect/time-multiselect.component';
import { DateTimeMultiselectItemComponent } from './components/date-time-multiselect-item/date-time-multiselect-item.component';
import { TimeMultiselectItemComponent } from './components/time-multiselect-item/time-multiselect-item.component';
import { WeekPickerComponent } from './components/week-picker/week-picker.component';
import { MonthPickerComponent } from './components/month-picker/month-picker.component';
import { AvatarUploadComponent } from './components/avatar-upload/avatar-upload.component';
import { HasClaimDirective } from 'src/api-authorization/has-claim.directive';
import { ComponentsModule } from './components.module';
import { EnumToArrayPipe } from './helpers/enumToArray.pipe';
import { SingleFileUploadComponent } from './components/single-file-upload/single-file-upload.component';
import { WhereMultiselectComponent } from '../tasks-management/where-multiselect/where-multiselect.component';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { FilterMultiselectComponent } from './components/filter-multiselect/filter-multiselect.component';
import { WhereSelectComponent } from '../tasks-management/where-select/where-select.component';
import { CleaningStatusToIcon } from './directives/cleaning-process-status-to-icon.directive';
import { NgxColorsModule } from 'ngx-colors';
import { DrawerComponent } from './components/drawer/drawer.component';
import { QuickCreateAttendantComponent } from './components/quick-create-attendant/quick-create-attendant.component';
import { AffinitiesMultiselectComponent } from '../cleaning-plan/affinities-multiselect/affinities-multiselect.component';
import { MasterFilterMultiselectComponent } from './components/master-filter-multiselect/master-filter-multiselect.component';
import { MessagesFilterMultiselectComponent } from './components/messages-filter-multiselect/messages-filter-multiselect.component';
import { DateMultiselectComponent } from './components/date-multiselect/date-multiselect.component';
import { FilestackFileUploadComponent } from './components/filestack-file-upload/filestack-file-upload.component';
import { FilestackModule } from '@filestack/angular';
import { FilestackFileUploadItemComponent } from './components/filestack-file-upload/filestack-file-upload-item/filestack-file-upload-item.component';
import { CleaningTimelineFilterComponent } from './components/cleaning-timeline-filter/cleaning-timeline-filter.component';
//import { CleaningProcessStatusToIconDirective } from './directives/cleaning-process-status-to-icon.directive';

@NgModule({
  imports: [
  ],
  declarations: [
    CleaningStatusToIcon,
  ],
  exports: [
    CleaningStatusToIcon,
  ]
})
export class SharedDirectivesModule { }

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    AngularMaterialModule,
    ComponentsModule,
    TextMaskModule,
    ConfirmationPopoverModule,
    SharedDirectivesModule,
    NgxColorsModule,
    FilestackModule,
  ],
  declarations: [
    SidebarMenuComponent,
    PopupComponent,
    DrawerComponent,
    FileUploadComponent,
    FileUploadItemComponent,
    SingleFileUploadComponent,
    TagsMultiselectComponent,
    AffinitiesMultiselectComponent,
    TimeIntervalInputComponent,
    TimeIntervalMultiselectComponent,
    DateTimeComponent,
    DateTimeMultiselectComponent,
    DateTimeMultiselectItemComponent,
    TimeMultiselectComponent,
    TimeMultiselectItemComponent,
    WeekPickerComponent,
    MonthPickerComponent,
    AvatarUploadComponent,
    HasClaimDirective,
    EnumToArrayPipe,
    WhereMultiselectComponent,
    WhereSelectComponent,
    FilterMultiselectComponent,
    MasterFilterMultiselectComponent,
    MessagesFilterMultiselectComponent,
    DateMultiselectComponent,
    QuickCreateAttendantComponent,
    FilestackFileUploadComponent,
    FilestackFileUploadItemComponent,
    CleaningTimelineFilterComponent,
    //CleaningStatusToIcon,
  ],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    AngularMaterialModule,
    SharedDirectivesModule,
    NgxColorsModule,
    SidebarMenuComponent,
    PopupComponent,
    DrawerComponent,
    FileUploadComponent,
    FileUploadItemComponent,
    SingleFileUploadComponent,
    TagsMultiselectComponent,
    AffinitiesMultiselectComponent,
    TimeIntervalInputComponent,
    TimeIntervalMultiselectComponent,
    DateTimeComponent,
    DateTimeMultiselectComponent,
    DateTimeMultiselectItemComponent,
    TimeMultiselectComponent,
    TimeMultiselectItemComponent,
    WeekPickerComponent,
    ComponentsModule,
    TextMaskModule,
    MonthPickerComponent,
    AvatarUploadComponent,
    EnumToArrayPipe,
    WhereMultiselectComponent,
    WhereSelectComponent,
    FilterMultiselectComponent,
    MasterFilterMultiselectComponent,
    MessagesFilterMultiselectComponent,
    DateMultiselectComponent,
    QuickCreateAttendantComponent,
    FilestackFileUploadComponent,
    FilestackFileUploadItemComponent,
    CleaningTimelineFilterComponent,
    //CleaningStatusToIcon,
  ]
})
export class SharedModule { }


