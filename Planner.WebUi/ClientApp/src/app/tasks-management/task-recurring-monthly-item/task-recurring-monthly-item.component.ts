import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-task-recurring-monthly-item',
  templateUrl: './task-recurring-monthly-item.component.html',
  styleUrls: ['./task-recurring-monthly-item.component.scss']
})
export class TaskRecurringMonthlyItemComponent implements OnInit {
  @Input() startsAtMonthDayForm: FormGroup;
  @Input() startsAtMonthDayFormIndex: number;

  mask = [/[0-2]/, /[0-9]/, ':', /[0-5]/, /[0-9]/];

  monthDays: Array<{ id: number, text: string }> = [
    { id: 1, text:  "<span class=\"month-day-item\">1<span>st</span></span>" },
    { id: 2, text:  "<span class=\"month-day-item\">2<span>nd</span></span>" },
    { id: 3, text:  "<span class=\"month-day-item\">3<span>rd</span></span>" },
    { id: 4, text:  "<span class=\"month-day-item\">4<span>th</span></span>" },
    { id: 5, text:  "<span class=\"month-day-item\">5<span>th</span></span>" },
    { id: 6, text:  "<span class=\"month-day-item\">6<span>th</span></span>" },
    { id: 7, text:  "<span class=\"month-day-item\">7<span>th</span></span>" },
    { id: 8, text:  "<span class=\"month-day-item\">8<span>th</span></span>" },
    { id: 9, text:  "<span class=\"month-day-item\">9<span>th</span></span>" },
    { id: 10, text: "<span class=\"month-day-item\">10<span>th</span></span>" },
    { id: 11, text: "<span class=\"month-day-item\">11<span>th</span></span>" },
    { id: 12, text: "<span class=\"month-day-item\">12<span>th</span></span>" },
    { id: 13, text: "<span class=\"month-day-item\">13<span>th</span></span>" },
    { id: 14, text: "<span class=\"month-day-item\">14<span>th</span></span>" },
    { id: 15, text: "<span class=\"month-day-item\">15<span>th</span></span>" },
    { id: 16, text: "<span class=\"month-day-item\">16<span>th</span></span>" },
    { id: 17, text: "<span class=\"month-day-item\">17<span>th</span></span>" },
    { id: 18, text: "<span class=\"month-day-item\">18<span>th</span></span>" },
    { id: 19, text: "<span class=\"month-day-item\">19<span>th</span></span>" },
    { id: 20, text: "<span class=\"month-day-item\">20<span>th</span></span>" },
    { id: 21, text: "<span class=\"month-day-item\">21<span>st</span></span>" },
    { id: 22, text: "<span class=\"month-day-item\">22<span>nd</span></span>" },
    { id: 23, text: "<span class=\"month-day-item\">23<span>rd</span></span>" },
    { id: 24, text: "<span class=\"month-day-item\">24<span>th</span></span>" },
    { id: 25, text: "<span class=\"month-day-item\">25<span>th</span></span>" },
    { id: 26, text: "<span class=\"month-day-item\">26<span>th</span></span>" },
    { id: 27, text: "<span class=\"month-day-item\">27<span>th</span></span>" },
    { id: 28, text: "<span class=\"month-day-item\">28<span>th</span></span>" },
    { id: 29, text: "<span class=\"month-day-item\">29<span>th</span></span>" },
    { id: 30, text: "<span class=\"month-day-item\">30<span>th</span></span>" },
    { id: 31, text: "<span class=\"month-day-item\">31<span>st</span></span>" },
  ];

  constructor() {
  }

  ngOnInit(): void {
  }

  remove(): void {

  }
}
