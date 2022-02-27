import { Component, Input, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-key-value-multiselect-item',
  templateUrl: './key-value-multiselect-item.component.html',
  styleUrls: ['./key-value-multiselect-item.component.scss']
})
export class KeyValueMultiselectItemComponent implements OnInit {
  @Input() keyControl: FormControl = new FormControl("");
  @Input() valueControl: FormControl = new FormControl("");

  constructor() { }

  ngOnInit(): void {
  }
}
