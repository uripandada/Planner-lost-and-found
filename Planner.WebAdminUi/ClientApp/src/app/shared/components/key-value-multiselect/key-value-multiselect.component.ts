import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-key-value-multiselect',
  templateUrl: './key-value-multiselect.component.html',
  styleUrls: ['./key-value-multiselect.component.scss']
})
export class KeyValueMultiselectComponent implements OnInit {
  // Key value form array must be consisted of an array
  // of FormGroup objects each with 'key' and 'value' FormControl
  @Input() keyValueFormArray: FormArray = new FormArray([]);

  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
  }

  deleteItem(index: number) {
    this.keyValueFormArray.removeAt(index);
  }

  addItem() {
    this.keyValueFormArray.push(this._formBuilder.group({
      key: [''],
      value: ['']
    }));
  }
}
