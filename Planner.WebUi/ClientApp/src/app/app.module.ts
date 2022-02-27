import { DragDropModule } from '@angular/cdk/drag-drop';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { TextMaskModule } from 'angular2-text-mask';
import { NgxEchartsModule } from 'ngx-echarts';
import { ToastrModule } from 'ngx-toastr';
import { VisModule } from 'ngx-vis';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CleaningTimelineGroupComponent } from './cleaning-plan/cleaning-timeline-group/cleaning-timeline-group.component';
import { CleaningTimelineItemTooltipComponent } from './cleaning-plan/cleaning-timeline-item-tooltip/cleaning-timeline-item-tooltip.component';
import { CleaningTimelineItemComponent } from './cleaning-plan/cleaning-timeline-item/cleaning-timeline-item.component';
import { CoreModule } from './core/core.module';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { DragToSelectModule } from 'ngx-drag-to-select';
import { ComponentsModule } from './shared/components.module';
import { PlannableCleaningTooltipComponent } from './cleaning-plan/plannable-cleaning-tooltip/plannable-cleaning-tooltip.component';
import { SharedDirectivesModule } from './shared/shared.module';
import { FilestackModule } from '@filestack/angular';
import { CleaningGeneratorLogsComponent } from './cleaning-generator-logs/cleaning-generator-logs.component';
import { AngularMaterialModule } from './core/angular-material.module';


@NgModule({
  declarations: [
    AppComponent,
    PageNotFoundComponent,
    CleaningTimelineItemTooltipComponent,
    PlannableCleaningTooltipComponent,
    CleaningTimelineGroupComponent,
    CleaningGeneratorLogsComponent,
  ],
  imports: [
    CoreModule.forRoot(),
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    DragDropModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    ApiAuthorizationModule,
    ToastrModule.forRoot(),
    VisModule,
    ComponentsModule,
    TextMaskModule,
    DragToSelectModule.forRoot(),
    SharedDirectivesModule,
    AngularMaterialModule,
    ConfirmationPopoverModule.forRoot({
      confirmButtonType: 'danger',
      cancelText: 'No',
      confirmText: 'Yes'
    }),
    FilestackModule.forRoot({
      //apikey: 'AwMlkjOdcTp2fmqSd0KPDz'
      apikey: 'A2xpVzAeShanofZW4Ep4Cz'
    }),
    NgxEchartsModule.forRoot({
      /**
       * This will import all modules from echarts.
       * If you only need custom modules,
       * please refer to [Custom Build] section.
       */
      echarts: () => import('echarts'), // or import('./path-to-my-custom-echarts')
    }),
  ],
  exports: [
    TextMaskModule,
    SharedDirectivesModule,
    AngularMaterialModule,
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthorizeInterceptor,
      multi: true
    },
  ],
  bootstrap: [AppComponent],
  entryComponents: [
    CleaningTimelineItemTooltipComponent,
    PlannableCleaningTooltipComponent,
    CleaningTimelineGroupComponent,
    CleaningTimelineItemComponent
  ]
})
export class AppModule { }
