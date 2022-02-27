import { Component, Input, OnInit } from '@angular/core';
import { CleaningTimelineGroup } from '../../core/services/cleaning-timeline.service';
import { EditCleaningGroupService } from '../_services/edit-cleaning-group.service';

@Component({
  selector: 'app-cleaning-timeline-group',
  templateUrl: './cleaning-timeline-group.component.html',
  styleUrls: ['./cleaning-timeline-group.component.scss']
})
export class CleaningTimelineGroupComponent implements OnInit {

  @Input() data: CleaningTimelineGroup;

  constructor(private _editCleaningGroupService: EditCleaningGroupService) {
  }

  ngOnInit(): void {
  }

  edit() {
    this._editCleaningGroupService.showEditCleanerPopup(this.data.groupData);
  }
}
