import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CleaningTimelineItemComponent } from '../cleaning-plan/cleaning-timeline-item/cleaning-timeline-item.component';


@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    CleaningTimelineItemComponent
  ],
  exports: [
    CleaningTimelineItemComponent
  ]
})
export class ComponentsModule {
}
