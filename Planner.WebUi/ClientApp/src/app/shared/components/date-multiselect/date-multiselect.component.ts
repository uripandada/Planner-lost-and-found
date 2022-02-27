import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, Validators } from '@angular/forms';
import { DateHelper } from '../../helpers/date.helper';

@Component({
  selector: 'app-date-multiselect',
  templateUrl: './date-multiselect.component.html',
  styleUrls: ['./date-multiselect.component.scss']
})
export class DateMultiselectComponent implements OnInit {
  @Input() datesFormArray: FormArray;

  constructor(private _formBuilder: FormBuilder) {
  }

  ngOnInit(): void {
  }

  add(): void {
    let currentDate: Date = new Date();
    this.datesFormArray.push(this._formBuilder.control(currentDate, Validators.required));
  }

  remove(index: number) {
    this.datesFormArray.removeAt(index);
  }
}
