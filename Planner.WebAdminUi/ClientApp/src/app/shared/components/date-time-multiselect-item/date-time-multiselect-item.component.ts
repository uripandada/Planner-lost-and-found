import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-date-time-multiselect-item',
  templateUrl: './date-time-multiselect-item.component.html',
  styleUrls: ['./date-time-multiselect-item.component.scss']
})
export class DateTimeMultiselectItemComponent implements OnInit {
  // array of FormGroups of which each has a date and time FormControl properties.
  @Input() dateTimeForm: FormGroup;
  @Input() dateTimeFormIndex: number;

  mask = [/[0-2]/, /[0-9]/, ':', /[0-5]/, /[0-9]/];

  constructor() {
  }

  ngOnInit(): void {
  }

  remove(): void {

  }
}
