import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormControl } from '@angular/forms';
import { MatAutocomplete, MatAutocompleteSelectedEvent, MatAutocompleteTrigger } from '@angular/material/autocomplete';
import { BehaviorSubject, Observable } from 'rxjs';
import { debounceTime, map, startWith } from 'rxjs/operators';

export class CleaningTimelineFilterGroup {
  constructor(groupKey: string, groupName: string) {
    this.groupKey = groupKey;
    this.groupName = groupName;
    this.options = [];
  }
  groupKey: string;
  groupName: string;
  options: Array<CleaningTimelineFilterOption>;
}

export class CleaningTimelineFilterOption {
  constructor(groupKey: string, value: string, description: string, id: string) {
    this.id = id;
    this.groupKey = groupKey;
    this.value = value;
    this.description = description;
  }

  groupKey: string;
  value: string;
  description: string;
  id: string;
}

@Component({
  selector: 'app-cleaning-timeline-filter',
  templateUrl: './cleaning-timeline-filter.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CleaningTimelineFilterComponent implements OnInit {

  @Input() filterGroups: Array<CleaningTimelineFilterGroup> = [];

  @Output() changed: EventEmitter<CleaningTimelineFilterOption[]> = new EventEmitter<CleaningTimelineFilterOption[]>();

  keywordsControl: FormControl;

  isLoadingFilterValues$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  filteredFilterGroups$: Observable<Array<CleaningTimelineFilterGroup>>;
  selectedFilterValues$: BehaviorSubject<Array<CleaningTimelineFilterOption>> = new BehaviorSubject<Array<CleaningTimelineFilterOption>>([]);

  elementId: string = "cleaning-timeline-filter";

  constructor() {
  }

  ngOnInit(): void {
    this.isLoadingFilterValues$.next(true);

    this.keywordsControl = new FormControl({ value: '', disabled: false });

    this.filteredFilterGroups$ = this.keywordsControl.valueChanges
      .pipe(
        startWith(''),
        map(value => this._filter(value))
      );
  }

  select(eventData: MatAutocompleteSelectedEvent) {
    let itemId = eventData.option.value;
    let groupName = eventData.option.group.label;
    let group = this.filterGroups.find(g => g.groupName === groupName);

    if (!this._isItemAlreadyAdded(itemId, group.groupKey)) {
      let item = group.options.find(a => a.value === itemId);
      var selectedFilterValues = [...this.selectedFilterValues$.value, new CleaningTimelineFilterOption(group.groupKey, item.value, item.description, item.id)];
      this.selectedFilterValues$.next(selectedFilterValues);

      this.changed.next(this.selectedFilterValues$.value);
    }

    this.keywordsControl.setValue('');
    (<any>document.getElementById(this.elementId)).blur();
  }

  remove(index: number) {
    var selectedFilterValues = [...this.selectedFilterValues$.value];
    selectedFilterValues.splice(index, 1);
    this.selectedFilterValues$.next(selectedFilterValues);

    this.changed.next(this.selectedFilterValues$.value);
  }

  private _filter(value: any): CleaningTimelineFilterGroup[] {
    if (!value) {
      return this.filterGroups;
    }

    if (typeof value === "string") {
      let valueParameter = value.toLowerCase();

      let groups: CleaningTimelineFilterGroup[] = [];
      for (let group of this.filterGroups) {
        let grItems = group.options.filter(a => a.value.toLowerCase().indexOf(valueParameter) >= 0);
        if (grItems.length > 0) {
          let gr = new CleaningTimelineFilterGroup(group.groupKey, group.groupName);
          gr.options = grItems;
          groups.push(gr);
        }
      }

      return groups;
    }

    return this.filterGroups;
  }

  private _isItemAlreadyAdded(itemId: string, groupKey: string): boolean {
    return !!this.selectedFilterValues$.value.find(a => a.value === itemId && a.groupKey === groupKey);
  }





  ////@Input() filterOptionGroups: Array<CleaningTimelineFilterGroup> = [];

  ////@Output() changed: EventEmitter<Array<CleaningTimelineFilterOption>> = new EventEmitter<Array<CleaningTimelineFilterOption>>();

  ////@ViewChild('autoCompleteInput', { read: MatAutocompleteTrigger }) autoComplete: MatAutocompleteTrigger;

  ////filterControl: FormControl;

  ////optionGroups$: BehaviorSubject<Array<CleaningTimelineFilterGroup>> = new BehaviorSubject<Array<CleaningTimelineFilterGroup>>([]);
  ////filteredOptions$: Observable<Array<CleaningTimelineFilterGroup>>;

  
  ////selectedOptions$: BehaviorSubject<Array<CleaningTimelineFilterOption>> = new BehaviorSubject<Array<CleaningTimelineFilterOption>>([]);

  ////constructor() { }

  ////ngOnInit(): void {

  ////  //let options: Array<SpecificFilterOption> = [];

  ////  //if (this.addSearchAllOption) {
  ////  //  options.push(this._searchEverywhereOption);

  ////  //  //options.push(new SpecificFilterOption("BUILDING", "Filter by building", "Building"));
  ////  //  //options.push(new SpecificFilterOption("FLOOR", "Filter by floor", "Floor"));
  ////  //}

  ////  //options.push(...this.filterOptions);
  ////  //this.options$.next(options);

  ////  this.optionGroups$.next(this.filterOptionGroups);

  ////  this.filterControl = new FormControl("");

  ////  this.filteredOptions$ = this.filterControl.valueChanges
  ////    .pipe(
  ////      //startWith(''),
  ////      debounceTime(250),
  ////      map(value => this._filter(value)),
  ////    );
  ////}

  ////close(eventData) {
  ////  if (eventData && eventData.target) {
  ////    eventData.target.blur();
  ////  }
  ////  this.autoComplete.closePanel();
  ////}

  ////private _filter(value) {
  ////  if (typeof (value) === "string") {
  ////    let options = [];
  ////    var selectedFilterValues = [...this.selectedOptions$.value];

  ////    if (value) {
  ////      options = this.options$.value.map(o => { o.filterValue = (!!value) ? value : ""; return o; });
  ////      selectedFilterValues.push(new SpecificFilterValue(this._searchEverywhereOption.key, this._searchEverywhereOption.label, value));
  ////    }

  ////    this.changed.next(selectedFilterValues);

  ////    return options;
  ////  }
  ////  else {
  ////    if (!value) { // - null value - nothing/empty string is selected!
  ////      // RAISE FILTERING CHANGED!
  ////      var selectedFilterValues = [...this.selectedOptions$.value];
  ////      this.changed.next(selectedFilterValues);

  ////      return [];
  ////    }
  ////    else {
  ////      //let resetOptions: boolean = false;
  ////      if (typeof (value) === "object") {
  ////        let options = [...this.options$.value];

  ////        if (value.key !== this._searchEverywhereOption.key) {
  ////          this._selectOption(value);

  ////          var selectedFilterValues = [...this.selectedOptions$.value];
  ////          this.changed.next(selectedFilterValues);

  ////          //for (let o of options) {
  ////          //  o.filterValue = "";
  ////          //}
  ////          return [];
  ////        }
  ////        else {
  ////          return options;
  ////        }
  ////      }
  ////      else {
  ////        return this.options$.value;
  ////      }
  ////    }

      
  ////  }
  ////}

  ////private _selectOption(option: SpecificFilterOption) {
  ////  let selectedOptions = this.selectedOptions$.value;
  ////  if (selectedOptions.find(o => o.key === option.key && o.value === option.filterValue)) {
  ////    return;
  ////  }

  ////  this.selectedOptions$.next([...this.selectedOptions$.value, new SpecificFilterValue(option.key, option.label, option.filterValue)])
  ////}

  ////onFilterFocus() {

  ////}

  ////offFilterFocus() {

  ////}

  ////onFilterEnter() {

  ////}

  ////remove(selectedIndex: number) {
  ////  var options = [...this.selectedOptions$.value];
  ////  options.splice(selectedIndex, 1);

  ////  this.selectedOptions$.next(options);

  ////  if (this.filterControl.value && typeof (this.filterControl.value) === "string" && this.filterControl.value) {
  ////    this.changed.next([...options, new SpecificFilterValue(this._searchEverywhereOption.key, this._searchEverywhereOption.label, this.filterControl.value)]);
  ////  }
  ////  else {
  ////    this.changed.next([...options]);
  ////  }
  ////}

  ////displayFilterName(filter) {
  ////  if (filter && typeof (filter) === "object" && filter.key === "__ANYTHING__") {
  ////    return filter.filterValue;
  ////  }
  ////  return "";
  ////}
}
