import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-room-beds-edit',
  templateUrl: './room-beds-edit.component.html'
})
export class RoomBedsEditComponent implements OnInit {
  @Input() roomBedsFormArray: FormArray;

  constructor(private _formBuilder: FormBuilder) {

  }

  ngOnInit(): void {
  }

  addBed() {
    this.roomBedsFormArray.push(this._createRoomBedFormGroup(null, null));
  }

  removeBed(index: number) {
    this.roomBedsFormArray.removeAt(index);
  }

  private _createRoomBedFormGroup(id: string, name: string) {
    return this._formBuilder.group({
      id: [null],
      name: [null, [Validators.required]],
    });
  }
}
