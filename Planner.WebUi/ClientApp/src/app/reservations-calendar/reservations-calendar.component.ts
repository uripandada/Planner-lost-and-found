import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MAT_MOMENT_DATE_FORMATS } from '@angular/material-moment-adapter';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import moment from 'moment';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { CustomDateAdapter } from '../core/custom-date-adapter';
//import { CleaningPlanService } from './_services/cleaning-plan-service';
import { HotelService } from '../core/services/hotel.service';
import { LoadingService } from '../core/services/loading.service';

@Component({
  selector: 'app-reservations-calendar',
  templateUrl: './reservations-calendar.component.html',
  styleUrls: ['./reservations-calendar.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    // `MomentDateAdapter` and `MAT_MOMENT_DATE_FORMATS` can be automatically provided by importing
    // `MatMomentDateModule` in your applications root module. We provide it at the component level
    // here, due to limitations of our example generation script.
    { provide: DateAdapter, useClass: CustomDateAdapter, deps: [MAT_DATE_LOCALE] },
    { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    { provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: { useUtc: true } },
  ],
})
export class ReservationsCalendarComponent implements OnInit {
  isCalendarLoading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor(
    private _toastr: ToastrService,
    private _formBuilder: FormBuilder,
    public loading: LoadingService,
    public hotelService: HotelService) {
  }

  selectCalendarForm: FormGroup;

  ngOnInit(): void {
    this.loading.reset();
    this.selectCalendarForm = this._createSelectPlanForm();
  }

  private _createSelectPlanForm(): FormGroup {
    let form: FormGroup = this._formBuilder.group({
      dateFrom: [moment().startOf('day')],
      dateTo: [moment().startOf('day').add(7, 'day')],
      hotelId: [this.hotelService.getSelectedHotelId()]
    });

    form.controls.hotelId.valueChanges.subscribe((hotelId: string) => {
      this.hotelService.selectHotelId(hotelId);
    });

    //form.controls.date.valueChanges.subscribe(() => {
    //});

    return form;
  }
}
