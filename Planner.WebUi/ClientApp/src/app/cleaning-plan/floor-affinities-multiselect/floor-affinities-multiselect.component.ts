import { Component } from '@angular/core';

export class FloorAffinitiesSelectedFloor {
  id: string;
  name: string;
  number: number;
  formArrayIndex: number;
}
export class FloorAffinitiesSelectedBuilding {
  id: string;
  name: string;
  floors: Array<FloorAffinitiesSelectedFloor> = [];
}

@Component({
  selector: 'app-floor-affinities-multiselect',
  templateUrl: './floor-affinities-multiselect.component.html',
  styleUrls: ['./floor-affinities-multiselect.component.scss']
})
export class FloorAffinitiesMultiselectComponent {

  //////floorId: a.floorId,
  //////floorNumber: a.floorNumber,
  //////floorName: a.floorName,
  //////buildingName: a.buildingName,
  //////buildingId: a.buildingId
  ////@Input() floorAffinitiesFormArray: FormArray;

  //filterAffinitiesForm: FormGroup;

  //selectedBuildings$: BehaviorSubject<Array<FloorAffinitiesSelectedBuilding>> = new BehaviorSubject<Array<FloorAffinitiesSelectedBuilding>>([]);

  //@Input() elementId: string = "default-affinities-input-1";
  //@Input() allFloorAffinities: Array<FloorAffinityData> = [];
  //@Input() floorAffinitiesFormArray: FormArray;

  //public filteredFloorAffinities$: Observable<Array<IFloorAffinityData>>;

  //constructor(private _formBuilder: FormBuilder, private _toastr: ToastrService) {
  //}

  //ngOnInit(): void {

  //  this.filterAffinitiesForm = this._formBuilder.group({
  //    keywords: ['']
  //  });

  //  let buildings = [];

  //  let formArrayIndex = 0;
  //  for (let affinity of this.floorAffinitiesFormArray.value) {

  //    let building = buildings.find(b => b.id === affinity.buildingId);

  //    if (!building) {
  //      building = <FloorAffinitiesSelectedBuilding>{
  //        floors: [],
  //        id: affinity.buildingId,
  //        name: affinity.buildingName
  //      };
  //      buildings.push(building);
  //    }

  //    let floor = building.floors.find(f => f.id === affinity.floorId);
  //    if (floor) {
  //      this._toastr.info("Floor already selected");
  //    }
  //    else {
  //      building.floors.push(<FloorAffinitiesSelectedFloor>{
  //        id: affinity.floorId,
  //        name: affinity.floorName,
  //        number: affinity.floorNumber,
  //        formArrayIndex: formArrayIndex 
  //      });

  //      this.selectedBuildings$.next(buildings);

  //      this.filterAffinitiesForm.controls.keywords.setValue('');

  //    }

  //    formArrayIndex++;
  //  }

  //  this.filteredFloorAffinities$ = this.filterAffinitiesForm.controls.keywords.valueChanges
  //    .pipe(
  //      startWith(''),
  //      map(value => this._filter(value))
  //    );
  //}

  //remove(buildingId: string, floorId: string) {
  //  let buildings = [...this.selectedBuildings$.value];
  //  let building: FloorAffinitiesSelectedBuilding = null;
  //  let buildingIndex = -1;
  //  for (let b of buildings) {
  //    buildingIndex++;
  //    if (b.id === buildingId) {
  //      building = b;
  //      break;
  //    }
  //  }

  //  if (!building) {
  //    this._toastr.error("Unable to remove. Building not found.");
  //    return;
  //  }

  //  let floor: FloorAffinitiesSelectedFloor = null;
  //  let floorIndex = -1;
  //  for (let f of building.floors) {
  //    floorIndex++;
  //    if (f.id === floorId) {
  //      floor = f;
  //      break;
  //    }
  //  }

  //  if (!floor) {
  //    this._toastr.error("Unable to remove. Floor not found.");
  //    return;
  //  }

  //  this.floorAffinitiesFormArray.removeAt(floor.formArrayIndex);
  //  building.floors.splice(floorIndex, 1);
  //  if (building.floors.length === 0) {
  //    buildings.splice(buildingIndex, 1);
  //  }

  //  this.selectedBuildings$.next(buildings);
  //}

  //private _filter(value: any): IFloorAffinityData[] {
  //  if (!value) {
  //    return this.allFloorAffinities;
  //  }

  //  if (typeof value === "string") {
  //    let valueParameter = value.toLowerCase();
  //    return this.allFloorAffinities.filter(a => a.buildingName.toLowerCase().indexOf(valueParameter) >= 0 || a.floorName.toLowerCase().indexOf(valueParameter) >= 0 || a.floorNumber.toString().indexOf(valueParameter) === 0);
  //  }

  //  return this.allFloorAffinities;
  //}

  //floorSelected(eventData: MatAutocompleteSelectedEvent) {
  //  let value = eventData.option.value;

  //  let buildings = this.selectedBuildings$.value;
  //  let building = buildings.find(b => b.id === value.buildingId);

  //  if (!building) {
  //    building = <FloorAffinitiesSelectedBuilding>{
  //      floors: [],
  //      id: value.buildingId,
  //      name: value.buildingName
  //    };
  //    buildings.push(building);
  //  }

  //  let floor = building.floors.find(f => f.id === value.floorId);
  //  if (floor) {
  //    this._toastr.info("Floor already selected");
  //  }
  //  else {
  //    this.floorAffinitiesFormArray.push(this._formBuilder.group({
  //      buildingId: [value.buildingId],
  //      floorId: [value.floorId],
  //      buildingName: [value.buildingName],
  //      floorName: [value.floorName],
  //      floorNumber: [value.floorNumber]
  //    }));

  //    building.floors.push(<FloorAffinitiesSelectedFloor>{
  //      id: value.floorId,
  //      name: value.floorName,
  //      number: value.floorNumber,
  //      formArrayIndex: this.floorAffinitiesFormArray.length - 1
  //    });

  //    this.selectedBuildings$.next(buildings);

  //    this.filterAffinitiesForm.controls.keywords.setValue('');

  //  }

  //  (<any>document.getElementById(this.elementId)).blur();
  //}
}
