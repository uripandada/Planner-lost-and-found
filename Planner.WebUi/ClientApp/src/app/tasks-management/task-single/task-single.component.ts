import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-task-single',
  templateUrl: './task-single.component.html',
  styleUrls: ['./task-single.component.scss']
})
export class TaskSingleComponent implements OnInit {
  @Input() taskSingleForm: FormGroup;

  constructor() {
  }

  ngOnInit(): void {
  }
}
