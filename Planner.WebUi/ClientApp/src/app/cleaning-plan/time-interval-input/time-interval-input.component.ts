import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-time-interval-input',
  templateUrl: './time-interval-input.component.html',
  styleUrls: ['./time-interval-input.component.scss']
})
export class TimeIntervalInputComponent implements OnInit {
  @Input() intervalFormIndex: number;
  @Input() intervalFormGroup: FormGroup;

  @Output() removed: EventEmitter<number> = new EventEmitter<number>();

  mask = [/[0-2]/, /[0-9]/, ':', /[0-5]/, /[0-9]/];

  constructor() {
  }

  ngOnInit(): void {

  }

  remove() {
    this.removed.next(this.intervalFormIndex);
  }
}
