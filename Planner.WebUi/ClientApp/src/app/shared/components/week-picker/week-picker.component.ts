import { ChangeDetectionStrategy, Component, EventEmitter, Injectable, Input, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import {
  MatDateRangeSelectionStrategy,
  DateRange,
  MAT_DATE_RANGE_SELECTION_STRATEGY,
} from '@angular/material/datepicker';
import { DateHelper } from '../../helpers/date.helper';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
// tslint:disable-next-line:no-duplicate-imports
import { default as _rollupMoment, Moment } from 'moment';
import { MAT_MOMENT_DATE_FORMATS } from '@angular/material-moment-adapter/adapter/moment-date-formats';
import { MomentDateAdapter } from '@angular/material-moment-adapter/adapter';

const moment = _rollupMoment || _moment;

@Injectable()
export class WeekRangeSelectionStrategy implements MatDateRangeSelectionStrategy<any> {
  constructor(private _dateAdapter: DateAdapter<Moment>) { }

  selectionFinished(date: Moment): DateRange<Moment> {
    return this._createWeekRange(date);
  }

  createPreview(activeDate: Moment): DateRange<Moment> {
    return this._createWeekRange(activeDate);
  }

  private _createWeekRange(date: Moment): DateRange<Moment> {
    if (date) {
      let weekDate = DateHelper.getWeekMomentDates(date);
      return new DateRange(weekDate.startOfTheWeek, weekDate.endOfTheWeek);
    }

    return new DateRange(null, null);
  }

  //private _getWeekDates(date: D | null): { startOfTheWeek: D, endOfTheWeek: D } {
  //  var dt = <Moment>(date || new Moment()); 
  //  var currentWeekDay = dt.getDay();
  //  var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1;
  //  var wkStart = new Moment(new Moment(dt).setDate(dt.getDate() - lessDays));
  //  var wkEnd = new Moment(new Moment(wkStart).setDate(wkStart.getDate() + 6));

  //  return {
  //    endOfTheWeek: <D><any>wkEnd,
  //    startOfTheWeek: <D><any>wkStart
  //  };
  //}
}
@Component({
  selector: 'app-week-picker',
  templateUrl: './week-picker.component.html',
  styleUrls: ['./week-picker.component.scss'],
  providers: [{
    provide: MAT_DATE_RANGE_SELECTION_STRATEGY,
    useClass: WeekRangeSelectionStrategy
  },
    //{ provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
    //{ provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class WeekPickerComponent implements OnInit {
  @Input() startDateControl: FormControl;
  @Input() endDateControl: FormControl;

  @Output() weekChanged: EventEmitter<{ startDate: Moment, endDate: Moment }> = new EventEmitter<{ startDate: Moment, endDate: Moment }>();

  constructor() { }

  ngOnInit(): void {
  }

  private _startDate: Moment;
  private _endDate: Moment;

  startDateChanged(eventData) {
  }
  endDateChanged(eventData) {
    this.weekChanged.next({ startDate: this.startDateControl.value, endDate: this.endDateControl.value });
  }
}
