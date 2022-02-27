import { ChangeDetectionStrategy, Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-cleaning-timeline-cleaner-item',
  templateUrl: './cleaning-timeline-cleaner-item.component.html',
  styleUrls: ['./cleaning-timeline-cleaner-item.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CleaningTimelineCleanerItemComponent implements OnInit {

  @Input() cleanerFormGroup: FormGroup;

  constructor() {
  }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
  }
}
