import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatAutocomplete, MatAutocompleteTrigger } from '@angular/material/autocomplete';
import { BehaviorSubject, Observable } from 'rxjs';
import { debounceTime, map, startWith } from 'rxjs/operators';

export class SpecificFilterOption {
  constructor(key: string, description: string, label: string) {
    this.key = key;
    this.description = description;
    this.label = label;
    this.filterValue = "";
  }

  key: string; // e.g. "FLOOR"
  description: string; // e.g. "Search floors"
  label: string; // e.g. "Floor"
  filterValue: string;
}

export class SpecificFilterValue {
  constructor(key: string, label: string, value: string) {
    this.key = key;
    this.label = label;
    this.value = value;
  }

  key: string;
  label: string;
  value: string;
}


@Component({
  selector: 'app-filter-multiselect',
  templateUrl: './filter-multiselect.component.html',
  styleUrls: ['./filter-multiselect.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FilterMultiselectComponent implements OnInit {
  @Input() addSearchAllOption: boolean = true;
  @Input() filterOptions: Array<SpecificFilterOption> = [];

  @Output() changed: EventEmitter<Array<SpecificFilterValue>> = new EventEmitter<Array<SpecificFilterValue>>();

  @ViewChild('autoCompleteInput', { read: MatAutocompleteTrigger }) autoComplete: MatAutocompleteTrigger;

  filterControl: FormControl;

  options$: BehaviorSubject<Array<SpecificFilterOption>> = new BehaviorSubject<Array<SpecificFilterOption>>([]);
  filteredOptions$: Observable<Array<SpecificFilterOption>>;

  
  selectedOptions$: BehaviorSubject<Array<SpecificFilterValue>> = new BehaviorSubject<Array<SpecificFilterValue>>([]);

  private _searchEverywhereOption = new SpecificFilterOption("__ANYTHING__", "Everywhere", "All options");

  constructor() { }

  ngOnInit(): void {

    let options: Array<SpecificFilterOption> = [];

    if (this.addSearchAllOption) {
      options.push(this._searchEverywhereOption);

      //options.push(new SpecificFilterOption("BUILDING", "Filter by building", "Building"));
      //options.push(new SpecificFilterOption("FLOOR", "Filter by floor", "Floor"));
    }

    options.push(...this.filterOptions);
    this.options$.next(options);

    this.filterControl = new FormControl("");

    this.filteredOptions$ = this.filterControl.valueChanges
      .pipe(
        //startWith(''),
        debounceTime(250),
        map(value => this._filter(value)),
      );
  }

  close(eventData) {
    if (eventData && eventData.target) {
      eventData.target.blur();
    }
    this.autoComplete.closePanel();
  }

  private _filter(value) {
    if (typeof (value) === "string") {
      let options = [];
      var selectedFilterValues = [...this.selectedOptions$.value];

      if (value) {
        options = this.options$.value.map(o => { o.filterValue = (!!value) ? value : ""; return o; });
        selectedFilterValues.push(new SpecificFilterValue(this._searchEverywhereOption.key, this._searchEverywhereOption.label, value));
      }

      this.changed.next(selectedFilterValues);

      return options;
    }
    else {
      if (!value) { // - null value - nothing/empty string is selected!
        // RAISE FILTERING CHANGED!
        var selectedFilterValues = [...this.selectedOptions$.value];
        this.changed.next(selectedFilterValues);

        return [];
      }
      else {
        //let resetOptions: boolean = false;
        if (typeof (value) === "object") {
          let options = [...this.options$.value];

          if (value.key !== this._searchEverywhereOption.key) {
            this._selectOption(value);

            var selectedFilterValues = [...this.selectedOptions$.value];
            this.changed.next(selectedFilterValues);

            //for (let o of options) {
            //  o.filterValue = "";
            //}
            return [];
          }
          else {
            return options;
          }
        }
        else {
          return this.options$.value;
        }
      }

      
    }
  }

  private _selectOption(option: SpecificFilterOption) {
    let selectedOptions = this.selectedOptions$.value;
    if (selectedOptions.find(o => o.key === option.key && o.value === option.filterValue)) {
      return;
    }

    this.selectedOptions$.next([...this.selectedOptions$.value, new SpecificFilterValue(option.key, option.label, option.filterValue)])
  }

  onFilterFocus() {

  }

  offFilterFocus() {

  }

  onFilterEnter() {

  }

  remove(selectedIndex: number) {
    var options = [...this.selectedOptions$.value];
    options.splice(selectedIndex, 1);

    this.selectedOptions$.next(options);

    if (this.filterControl.value && typeof (this.filterControl.value) === "string" && this.filterControl.value) {
      this.changed.next([...options, new SpecificFilterValue(this._searchEverywhereOption.key, this._searchEverywhereOption.label, this.filterControl.value)]);
    }
    else {
      this.changed.next([...options]);
    }
  }

  displayFilterName(filter) {
    if (filter && typeof (filter) === "object" && filter.key === "__ANYTHING__") {
      return filter.filterValue;
    }
    return "";
  }
}
