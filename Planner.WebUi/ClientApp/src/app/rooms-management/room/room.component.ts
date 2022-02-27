import { Component, OnInit, Input, ChangeDetectionStrategy } from '@angular/core';
import { FormArray, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RoomComponent implements OnInit {

  @Input() roomIndex: number;
  @Input() isAppartmentRoom: boolean;
  @Input() roomForm: FormGroup;

  public roomTypeDescription: string = "";
  public showBedsSection: boolean = false;

  public showHiddenBedsLabel: boolean = false;
  public numberOfHiddenBeds: number = 0;
  public beds: Array<{ id: string, name: string }> = [];

  get bedsFormArray(): FormArray {
    return this.roomForm.controls.bedsFormArray as FormArray;
  }

  constructor(private _router: Router) { }

  ngOnInit(): void {
    switch (this.roomForm.controls.typeKey.value) {
      case "HOTEL":
        this.roomTypeDescription = "Hotel room";
        break;
      case "HOSTEL":
        this.roomTypeDescription = "Hostel room";
        let numberOfBeds = this.bedsFormArray.length;
        this.showBedsSection = numberOfBeds > 0;

        this.beds = this.bedsFormArray.getRawValue().map(b => { return { id: b.id, name: b.name }; });
        this.numberOfHiddenBeds = 0;

        if (numberOfBeds > 7) {
          this.showHiddenBedsLabel = true;
          this.beds = this.beds.slice(0, 6);
          this.numberOfHiddenBeds = numberOfBeds - 6;
        }
        break;
      case "APPARTMENT":
        this.roomTypeDescription = "Appartment";
        break;
    }
  }

}
