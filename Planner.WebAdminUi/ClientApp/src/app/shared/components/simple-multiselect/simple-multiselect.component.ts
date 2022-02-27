import { Component, Input } from '@angular/core';
import { FormArray, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-simple-multiselect',
  templateUrl: './simple-multiselect.component.html',
  styleUrls: ['./simple-multiselect.component.scss'],
})
export class SimpleMultiselectComponent {

  @Input() valuesArray: FormArray;
  @Input() inputType: "text"|"number" = "text";

  constructor(private _formBuilder: FormBuilder) {
  }

  ngOnInit(): void {
  }

  add() {
    this.valuesArray.push(this._formBuilder.group({
      id: null,
      value: null
    }));
  }

  remove(valueFormIndex: number) {
    this.valuesArray.removeAt(valueFormIndex);
  }
}
