import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { ExperienceClientRelationStatus, ExperienceDetailsViewModel, ExperienceGridItemViewModel, ExperienceManagementClient, ExperienceResolutionStatus, ExperienceTicketStatus, GetExperienceDetailsQuery, GetExperienceListQuery, PageOfOfExperienceGridItemViewModel } from '../core/autogenerated-clients/api-client';
import * as moment from 'moment';
import { LoadingService } from '../core/services/loading.service';
import { ToastrService } from 'ngx-toastr';
import { HotelItemData, } from 'src/app/core/autogenerated-clients/api-client';
import { HotelService } from '../core/services/hotel.service';
import { data } from 'jquery';

@Component({
  selector: 'app-experience',
  templateUrl: './experience.component.html',
  styleUrls: ['./experience.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ExperienceComponent implements OnInit {

  filterForm: FormGroup;
  itemsList: BehaviorSubject<ExperienceGridItemViewModel[]> = new BehaviorSubject<ExperienceGridItemViewModel[]>(null);
  selectedItem: BehaviorSubject<ExperienceGridItemViewModel> = new BehaviorSubject<ExperienceGridItemViewModel>(null);
  areDetailsDisplayed$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  loadedNumber$: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  totalNumber$: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  showLoadMore$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  hotels: HotelItemData[] = [];

  public experienceTicketStatuses: any;
  public experienceClientRelationStatuses: any;
  public experienceResolutionStatuses: any;

  experienceTicketStatusMappings: { [index: number]: string } = {};
  experienceClientRelationStatusMappings: { [index: number]: string } = {};
  experienceResolutionStatusMappings: { [index: number]: string } = {};

  constructor(
    private formBuilder: FormBuilder,
    public loading: LoadingService,
    private ExperienceManagementClient: ExperienceManagementClient,
    private toastr: ToastrService,
    public hotelService: HotelService,
  ) {
    this.hotels = hotelService.getHotels();
  }

  ngOnInit(): void {

    this.filterForm = this.formBuilder.group({
      keywords: [''],
      dateFrom: [''],
      dateTo: [''],
      hotelId: [],
    });

    this.filterForm.controls.hotelId.setValue(this.hotels[0].id);

    this.filterForm.valueChanges.subscribe(
      value => {
        this.reloadList(true);
      }
    );

    this.reloadList(true);

    this.experienceTicketStatusMappings = {};
    this.experienceTicketStatusMappings[ExperienceTicketStatus.Pending] = "Pending";
    this.experienceTicketStatuses = ExperienceTicketStatus;

    this.experienceClientRelationStatusMappings = {};
    this.experienceClientRelationStatusMappings[ExperienceClientRelationStatus.NoClientAction] = "No client action";
    this.experienceClientRelationStatusMappings[ExperienceClientRelationStatus.MeetWithClient] = "Meet with client";
    this.experienceClientRelationStatusMappings[ExperienceClientRelationStatus.MeetWithClientAtCO] = "Meet with Client at C/O(avecArchiveiuto)";
    this.experienceClientRelationStatuses = ExperienceClientRelationStatus;
    
    this.experienceResolutionStatusMappings = {};
    this.experienceResolutionStatusMappings[ExperienceResolutionStatus.None] = "None";
    this.experienceResolutionStatusMappings[ExperienceResolutionStatus.Resolved] = "Resolved";
    this.experienceResolutionStatusMappings[ExperienceResolutionStatus.Closed] = "Closed";
    this.experienceResolutionStatuses = ExperienceResolutionStatus;
  }

  createNewFound() {
    const item = new ExperienceDetailsViewModel({
      id: null,
      roomName: '',
      guestName: '',
      checkIn: null,
      checkOut: null,
      reservationId: '',
      vip: null,
      email: null,
      phoneNumber: null,
      type!: 0,
      description: null,
      actions: null,
      group: null,
      internalFollowUp: null,
      experienceCategoryId!: '',
      experienceCompensationId!: '',
      experienceTicketStatus: ExperienceTicketStatus.Pending,
      experienceClientRelationStatus: ExperienceClientRelationStatus.NoClientAction,
      experienceResolutionStatus: ExperienceResolutionStatus.None
    });
    this.selectedItem.next(item);
    this.areDetailsDisplayed$.next(false);
    this.areDetailsDisplayed$.next(true);
  }

  reloadList(reload: boolean) {
    if (reload) {
      this.loadedNumber$.next(0);
    }

    this.areDetailsDisplayed$.next(false);

    this.loading.start();
    this.ExperienceManagementClient.getList(new GetExperienceListQuery({
      skip: this.loadedNumber$.value,
      take: 20,
      dateFrom: this.filterForm.controls.dateFrom.value,
      dateTo: this.filterForm.controls.dateTo.value,
      keywords: this.filterForm.controls.keywords.value,
    })).subscribe((response) => {
      console.log(response.items);
      this.itemsList.next(response.items);
      this.totalNumber$.next(response.totalNumberOfItems);
      this.loadedNumber$.next(this.loadedNumber$.value + 20);
      this.showLoadMore$.next(this.loadedNumber$.value < this.totalNumber$.value);
      this.loading.stop();
    },
      (error) => {
        this.toastr.error(error);
        this.loading.stop();
      });
  }

  selectItem(item: ExperienceDetailsViewModel) {
    this.ExperienceManagementClient.getById(new GetExperienceDetailsQuery({ id: item.id })).subscribe(response => {
      this.selectedItem.next(response);
      this.areDetailsDisplayed$.next(false);
      this.areDetailsDisplayed$.next(true);
    })
  }

  onItemEditCancelled() {
    this.areDetailsDisplayed$.next(false);
    this.selectedItem.next(null);
  }

}
