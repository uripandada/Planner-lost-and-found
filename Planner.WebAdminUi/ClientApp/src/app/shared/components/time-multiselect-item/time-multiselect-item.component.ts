import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormArray, FormGroup } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-time-multiselect-item',
  templateUrl: './time-multiselect-item.component.html',
  styleUrls: ['./time-multiselect-item.component.scss']
})
export class TimeMultiselectItemComponent implements OnInit {

  @Input() timeForm: FormGroup;
  @Input() timeFormIndex: number;

  @Output() removed: EventEmitter<number> = new EventEmitter<number>();

  mask = [/[0-2]/, /[0-9]/, ':', /[0-5]/, /[0-9]/];

  constructor() {
  }

  ngOnInit(): void {
  }

  remove() {
    this.removed.next(this.timeFormIndex);
  }
}
