import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormArray } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-time-multiselect',
  templateUrl: './time-multiselect.component.html',
  styleUrls: ['./time-multiselect.component.scss']
})
export class TimeMultiselectComponent implements OnInit {

  @Input() timesArray: FormArray;

  constructor(private _formBuilder: FormBuilder) {
  }

  ngOnInit(): void {
  }

  add() {
    this.timesArray.push(this._formBuilder.group({
      id: null,
      time: "12:34"
    }));
  }

  onRemoved(index: number) {
    this.timesArray.removeAt(index);
  }
}
