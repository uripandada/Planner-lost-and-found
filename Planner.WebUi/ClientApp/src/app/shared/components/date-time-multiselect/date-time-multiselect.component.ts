import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, Validators } from '@angular/forms';
import { DateHelper } from '../../helpers/date.helper';

@Component({
  selector: 'app-date-time-multiselect',
  templateUrl: './date-time-multiselect.component.html',
  styleUrls: ['./date-time-multiselect.component.scss']
})
export class DateTimeMultiselectComponent implements OnInit {
  // array of FormGroups of which each has a date and time FormControl properties.
  @Input() dateTimesFormArray: FormArray;

  constructor(private _formBuilder: FormBuilder) {
  }

  ngOnInit(): void {
  }

  add(): void {
    let currentDate: Date = new Date();
    this.dateTimesFormArray.push(this._formBuilder.group({
      id: null,
      date: [currentDate, Validators.required],
      time: [DateHelper.getTime(currentDate), Validators.required]
    }));
  }
}
