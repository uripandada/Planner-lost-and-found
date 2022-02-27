import { Component, Input, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-date-time',
  templateUrl: './date-time.component.html',
  styleUrls: ['./date-time.component.scss']
})
export class DateTimeComponent implements OnInit {
  @Input() dateControl: FormControl = new FormControl(new Date());
  @Input() timeControl: FormControl = new FormControl("00:00");

  mask = [/[0-2]/, /[0-9]/, ':', /[0-5]/, /[0-9]/];

  constructor() { }

  ngOnInit(): void {
  }
}
