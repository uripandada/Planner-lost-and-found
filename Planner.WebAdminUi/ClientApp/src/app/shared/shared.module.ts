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
import { HotelGroupPickerComponent } from './components/hotel-group-picker/hotel-group-picker.component';
import { HotelPickerComponent } from './components/hotel-picker/hotel-picker.component';
import { SimpleMultiselectComponent } from './components/simple-multiselect/simple-multiselect.component';
import { RoomMultiselectComponent } from './components/room-multiselect/room-multiselect.component';
import { KeyValueMultiselectComponent } from './components/key-value-multiselect/key-value-multiselect.component';
import { KeyValueMultiselectItemComponent } from './components/key-value-multiselect-item/key-value-multiselect-item.component';
import { RoomCreditsMultiselectComponent } from './components/room-credits-multiselect/room-credits-multiselect.component';
import { BasedOnProductsMultiselectComponent } from './components/based-on-products-multiselect/based-on-products-multiselect.component';
import { BasedOnOtherPropertiesMultiselectComponent } from './components/based-on-other-properties-multiselect/based-on-other-properties-multiselect.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    AngularMaterialModule,
    TextMaskModule,
  ],
  declarations: [
    SidebarMenuComponent,
    PopupComponent,
    FileUploadComponent,
    FileUploadItemComponent,
    TagsMultiselectComponent,
    DateTimeComponent,
    DateTimeMultiselectComponent,
    DateTimeMultiselectItemComponent,
    TimeMultiselectComponent,
    TimeMultiselectItemComponent,
    WeekPickerComponent,
    MonthPickerComponent,
    AvatarUploadComponent,
    HasClaimDirective,
    HotelGroupPickerComponent,
    HotelPickerComponent,
    SimpleMultiselectComponent,
    BasedOnProductsMultiselectComponent,
    BasedOnOtherPropertiesMultiselectComponent,
    RoomMultiselectComponent,
    KeyValueMultiselectComponent,
    KeyValueMultiselectItemComponent,
    RoomCreditsMultiselectComponent,
  ],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    AngularMaterialModule,
    SidebarMenuComponent,
    PopupComponent,
    FileUploadComponent,
    FileUploadItemComponent,
    TagsMultiselectComponent,
    DateTimeComponent,
    DateTimeMultiselectComponent,
    DateTimeMultiselectItemComponent,
    TimeMultiselectComponent,
    TimeMultiselectItemComponent,
    WeekPickerComponent,
    TextMaskModule,
    MonthPickerComponent,
    AvatarUploadComponent,
    HotelGroupPickerComponent,
    HotelPickerComponent,
    SimpleMultiselectComponent,
    BasedOnProductsMultiselectComponent,
    BasedOnOtherPropertiesMultiselectComponent,
    RoomMultiselectComponent,
    KeyValueMultiselectComponent,
    KeyValueMultiselectItemComponent,
    RoomCreditsMultiselectComponent,
  ]
})
export class SharedModule { }


