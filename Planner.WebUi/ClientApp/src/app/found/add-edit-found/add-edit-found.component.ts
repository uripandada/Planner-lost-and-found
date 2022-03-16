import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, Subscription } from 'rxjs';
import {
  LostAndFoundCategoryGridItemViewModel,
  ExtendedWhereData,
  HotelItemData,
  InsertLostAndFoundCommand,
  LostAndFoundClient,
  LostAndFoundFilesUploadedData,
  LostAndFoundModel,
  FoundStatus,
  GuestStatus,
  DeliveryStatus,
  OtherStatus,
  TaskWhereData,
  TypeOfLoss,
  UpdateLostAndFoundCommand
} from 'src/app/core/autogenerated-clients/api-client';
import { FileDetails, FilesChangedData } from 'src/app/shared/components/file-upload/file-upload.component';
import moment from 'moment';
import { HotelService } from '../../core/services/hotel.service';

@Component({
  selector: 'app-add-edit-found',
  templateUrl: './add-edit-found.component.html',
  styleUrls: ['./add-edit-found.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AddEditFoundComponent implements OnInit {

  @Input() item: LostAndFoundModel;
  @Input() allWheres: Array<TaskWhereData> = [];
  @Input() allCategories: Array<LostAndFoundCategoryGridItemViewModel> = [];
  @Input() currentlyUploadingFiles: Array<FileDetails> = [];
  @Input() temporaryUploadedFiles: Array<FileDetails> = [];
  @Input() uploadedFiles: Array<FileDetails> = [];

  @Output() reloadList: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() cancelled: EventEmitter<boolean> = new EventEmitter<boolean>();


  selectedFiles: Array<FilesChangedData> = [];
  currentlyUploadingFiles$: BehaviorSubject<Array<FileDetails>> = new BehaviorSubject<Array<FileDetails>>([]);
  temporaryUploadedFiles$: BehaviorSubject<Array<FileDetails>> = new BehaviorSubject<Array<FileDetails>>([]);
  uploadedFiles$: BehaviorSubject<Array<FileDetails>> = new BehaviorSubject<Array<FileDetails>>([]);

  foundForm: FormGroup;

  typesOfLoss: Array<{ key: TypeOfLoss, value: string }> = [];
  foundStatuses: Array<{ key: FoundStatus, value: string }> = [];
  guestStatuses: Array<{ key: GuestStatus, value: string }> = [];
  deliveryStatuses: Array<{ key: DeliveryStatus, value: string }> = [];
  otherStatuses: Array<{ key: OtherStatus, value: string }> = [];

  foundStatusMappings: { [index: number]: string } = {};
  guestStatusMappings: { [index: number]: string } = {};
  deliveryStatusMappings: { [index: number]: string } = {};
  otherStatusMappings: { [index: number]: string } = {};

  statusChange$: Subscription;
  statusFlag: number;

  selectedFoundStatus: string;
  selectedGuestStatus: string;

  isFoundStatus: boolean;
  isGuestStatus: boolean;
  isDeliveryStatus: boolean;
  isOtherStatus: boolean;

  hotels: HotelItemData[] = [];

  public isCreateNew = true;
  constructor(
    private formBuilder: FormBuilder,
    private toastr: ToastrService,
    private _route: ActivatedRoute,
    private lostAndFoundClient: LostAndFoundClient,
    public hotelService: HotelService,
  ) {
    this.typesOfLoss.push({ key: TypeOfLoss.Employee, value: "Employee" });
    this.typesOfLoss.push({ key: TypeOfLoss.Customer, value: "Customer" });

    this.foundStatuses.push({ key: FoundStatus.WaitingRoomMaid, value: "Waiting Room Maid" });
    this.foundStatuses.push({ key: FoundStatus.Received, value: "Received" });

    this.guestStatuses.push({ key: GuestStatus.Unclaimed, value: "Unclaimed" });
    this.guestStatuses.push({ key: GuestStatus.ClientContactedByEmail, value: "Contact by email" });
    this.guestStatuses.push({ key: GuestStatus.ClientContactedByPhone, value: "Contact by phone" });
    this.guestStatuses.push({ key: GuestStatus.ClientUndecided, value: "Client Undecided" });
    this.guestStatuses.push({ key: GuestStatus.WaitingForClientReturn, value: "Waiting For Client Return" });

    this.deliveryStatuses.push({ key: DeliveryStatus.None, value: "None" });
    this.deliveryStatuses.push({ key: DeliveryStatus.WaitingForHandDelivered, value: "Waiting For Hand-Delivered" });
    this.deliveryStatuses.push({ key: DeliveryStatus.WaitingForShipment, value: "Waiting For Shipment" });
    this.deliveryStatuses.push({ key: DeliveryStatus.OTShipped, value: "OT Shipped" });
    this.deliveryStatuses.push({ key: DeliveryStatus.HandDelivered, value: "Hand Delivered" });

    this.otherStatuses.push({ key: OtherStatus.None, value: "None" });
    this.otherStatuses.push({ key: OtherStatus.Expired, value: "Expired" });
    this.otherStatuses.push({ key: OtherStatus.RefusedByTheClient, value: "Refused By The Client" });
    this.otherStatuses.push({ key: OtherStatus.BadReferencing, value: "Bad Referencing" });
    this.otherStatuses.push({ key: OtherStatus.Destroy, value: "Detruit" });
    this.otherStatuses.push({ key: OtherStatus.ReturnedToInventor, value: "Rendu a linventeur" });
    this.otherStatuses.push({ key: OtherStatus.GivenToAnotherPerson, value: "Donne a une autre personne" });
    this.otherStatuses.push({ key: OtherStatus.DisappearedOrLost, value: "Disparu/Perdu" });

    this.hotels = hotelService.getHotels();
  }


  ngOnInit(): void {
    this.allWheres = this._route.snapshot.data.allWheres;
    this.allCategories = this._route.snapshot.data.allCategories;
    this.initForm();

    this.statusFlag = GuestStatus.Unclaimed;

    this.statusChange$ = this.foundForm.controls['guestStatus'].valueChanges.subscribe((value: number) => {
      if (value !== GuestStatus.Unclaimed) {
        this.addClientFormControls();
      } else {
        this.removeClientFormControls();
      }
    })
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.item.id) {
      this.isCreateNew = false;

    } else {
      this.isCreateNew = true;
    }

    if (!changes.item.firstChange) {
      this.setFormData();
    }
  }

  foundStatusSelectChanged() {
    this.isFoundStatus = true;
    this.isGuestStatus = false;
    this.isDeliveryStatus = false;
    this.isOtherStatus = false;
  }

  guestStatusSelectChanged() {
    this.isFoundStatus = true;
    this.isGuestStatus = true;
    this.isDeliveryStatus = false;
    this.isOtherStatus = false;
  }

  deliveryStatusSelectChanged(value:any) {
    if ( value == this.deliveryStatuses[0].key) {
      this.isFoundStatus = true;
      this.isGuestStatus = true;
      this.isDeliveryStatus = false;
      this.isOtherStatus = false;  
    }else {
      this.isFoundStatus = true;
      this.isGuestStatus = true;
      this.isDeliveryStatus = true;
      this.isOtherStatus = false;
    }
  }

  otherStatusSelectChanged(val:any) {
    if (val.value == this.otherStatuses[0].key) {
      this.isFoundStatus = true;
      this.isGuestStatus = true;
      this.isDeliveryStatus = false;
      this.isOtherStatus = false;
    }else {
      this.foundForm.controls.foundStatus.setValue(this.foundStatuses[0].key);
      // this.foundForm.controls.guestStatus.setValue(this.guestStatuses[0].key);
      this.foundForm.controls.deliveryStatus.setValue(this.deliveryStatuses[0].key);
      this.isFoundStatus = false;
      this.isGuestStatus = false;
      this.isDeliveryStatus = false;
      this.isOtherStatus = true;
      
    }
  }

  initForm() {
    let where: TaskWhereData;
    if (this.item.reservationId) {
      where = this.allWheres.find(x => x.referenceId == this.item.reservationId);
    }
    else if (this.item.roomId) {
      where = this.allWheres.find(x => x.referenceId == this.item.roomId);
    }
    
    this.isFoundStatus = true;
    this.isGuestStatus = true;

    if (this.item.deliveryStatus != DeliveryStatus.None) {
      this.isDeliveryStatus = true;
    } else {
      this.isDeliveryStatus = false;
    }

    if (this.item.otherStatus != OtherStatus.None) {
      this.isOtherStatus = true;
      this.isFoundStatus = false;
      this.isGuestStatus = false;
      this.isDeliveryStatus = false;
    } 

    this.foundForm = this.formBuilder.group({
      hotelId: [this.item.hotelId],
      name: [this.item.reservation?.guestName],
      phoneNumber: [this.item.phoneNumber],
      email: [this.item.email],
      address: [this.item.address],
      city: [this.item.city],
      postalCode: [this.item.postalCode],
      referenceNumber: [this.item.referenceNumber],
      street: "",
      country: [this.item.country],
      clientName: [this.item.reservation?.guestName],
      description: [this.item.description, Validators.required],
      foundOn: [this.item.lostOn?.format('yyyy-MM-DD'), Validators.required],
      notes: [this.item.notes],
      typeOfLoss: [this.item.typeOfLoss],
      foundStatus: [this.item.foundStatus, Validators.required],
      guestStatus: [this.item.guestStatus, Validators.required],
      deliveryStatus: [this.item.deliveryStatus],
      otherStatus: [this.item.otherStatus],
      storage: [''],
      category: [this.item.lostAndFoundCategoryId, Validators.required],
      whereFrom: [this.item.room?.name || this.item.reservation?.roomName, Validators.required],
      placeOfStorage: [this.item.placeOfStorage],
      foundByNumber: [''],

      founderName: [this.item.founderName],
      founderEmail: [this.item.founderEmail],
      founderPhoneNumber: [this.item.founderPhoneNumber],
    });

    if (this.item.files) {
      const files = this.item.files.map(x => ({
        id: x.id,
        isImage: x.isImage,
        imageUrl: x.url,
        fileName: x.name,
        displayText: ''
      }) as FileDetails);
      this.uploadedFiles$.next(files);
    } else {
      this.uploadedFiles$.next([]);
    }
  }

  setFormData() {
    let where: TaskWhereData;
    if (this.item.reservationId) {
      where = this.allWheres.find(x => x.referenceId == this.item.reservationId);
    }
    else if (this.item.roomId) {
      where = this.allWheres.find(x => x.referenceId == this.item.roomId);
    }

    this.isFoundStatus = true;
    this.isGuestStatus = true;

    if (this.item.deliveryStatus != DeliveryStatus.None) {
      this.isDeliveryStatus = true;
    } else {
      this.isDeliveryStatus = false;
    }

    if (this.item.otherStatus != OtherStatus.None) {
      this.isOtherStatus = true;
      this.isFoundStatus = false;
      this.isGuestStatus = false;
      this.isDeliveryStatus = false;
    } else {
      this.isOtherStatus = false;
    }

    this.foundForm.controls.hotelId.setValue(this.item.hotelId);
    this.foundForm.controls.description.setValue(this.item.description);
    this.foundForm.controls.foundOn.setValue(this.item.lostOn?.format('yyyy-MM-DD'));
    this.foundForm.controls.notes.setValue(this.item.notes);
    this.foundForm.controls.typeOfLoss.setValue(this.item.typeOfLoss);
    this.foundForm.controls.foundStatus.setValue(this.item.foundStatus);
    this.foundForm.controls.guestStatus.setValue(this.item.guestStatus);
    this.foundForm.controls.deliveryStatus.setValue(this.item.deliveryStatus);
    this.foundForm.controls.otherStatus.setValue(this.item.otherStatus);
    this.foundForm.controls.whereFrom.setValue(where);
    this.foundForm.controls.storage.setValue(this.item.storageRoomId);
    this.foundForm.controls.placeOfStorage.setValue(this.item.placeOfStorage);
    this.foundForm.controls.foundByNumber.setValue('');
    this.foundForm.controls.clientName.setValue(this.item.reservation?.guestName);
    this.foundForm.controls.founderName.setValue(this.item.founderName);
    this.foundForm.controls.founderEmail.setValue(this.item.founderEmail);
    this.foundForm.controls.founderPhoneNumber.setValue(this.item.founderPhoneNumber);
    this.foundForm.controls.category.setValue(this.item.lostAndFoundCategoryId);

    if (this.item.guestStatus != GuestStatus.Unclaimed) {
      this.foundForm.controls.name.setValue(this.item.name);
      this.foundForm.controls.phoneNumber.setValue(this.item.phoneNumber);
      this.foundForm.controls.email.setValue(this.item.email);
      this.foundForm.controls.address.setValue(this.item.address);
      this.foundForm.controls.city.setValue(this.item.city);
      this.foundForm.controls.postalCode.setValue(this.item.postalCode);
      this.foundForm.controls.country.setValue(this.item.country);
    }

    this.foundForm.markAsUntouched({ onlySelf: false });
    this.foundForm.markAsPristine({ onlySelf: false });

    this.foundForm.updateValueAndValidity({ onlySelf: false, emitEvent: false });

    if (this.item.files) {
      const files = this.item.files.map(x => ({
        id: x.id,
        isImage: x.isImage,
        imageUrl: x.url,
        fileName: x.name,
        displayText: ''
      }) as FileDetails);
      this.uploadedFiles$.next(files);
    } else {
      this.uploadedFiles$.next([]);
    }
  }

  get f() {
    return this.foundForm.controls;
  }

  save() {
    this.foundForm.markAsTouched({ onlySelf: false });
    if (this.foundForm?.invalid) {
      this.toastr.error("You have to fix invalid form fields before you can continue.");
      return;
    }

    let formValues = this.foundForm.getRawValue();

    let insertRequest = new InsertLostAndFoundCommand({
      hotelId: formValues.hotelId,
      description: formValues.description,
      lostOn: moment.utc(formValues.foundOn),
      foundStatus: formValues.foundStatus,
      guestStatus: formValues.guestStatus,
      deliveryStatus: formValues.deliveryStatus,
      otherStatus: formValues.otherStatus,
      typeOfLoss: formValues.typeOfLoss,
      address: formValues.address,
      postalCode: formValues.postalCode,
      city: formValues.city,
      country: formValues.country,
      notes: formValues.notes,
      name: formValues.name,
      phoneNumber: formValues.phoneNumber,
      email: formValues.email,
      whereData: formValues.whereFrom,
      placeOfStorage: formValues.placeOfStorage,
      referenceNumber: null,
      isLostItem: false,
      reservationId: null,
      roomId: null,
      trackingNumber: null,
      clientName: formValues.clientName,
      founderName: formValues.founderName,
      founderEmail: formValues.founderEmail,
      founderPhoneNumber: formValues.founderPhoneNumber,
      lostAndFoundCategoryId: formValues.category,
      storageRoomId: null,
      files: this.selectedFiles.map(x => new LostAndFoundFilesUploadedData({
        fileName: x.fileName,
        id: x.id
      }))
    });

    if (this.item.id === null) {

      this.lostAndFoundClient.insert(insertRequest).subscribe(
        response => {
          if (response.isSuccess) {
            this.toastr.success(response.message);
            this.reloadList.next(true);
          } else {
            this.toastr.error(response.message);
          }
        },
        error => {
          this.toastr.error(error);
        }
      );
    } else {
      let updateRequest = null;
      updateRequest = new UpdateLostAndFoundCommand({
        id: this.item.id,
        ...insertRequest
      });
      this.lostAndFoundClient.update(updateRequest).subscribe(
        response => {
          if (response.isSuccess) {
            this.toastr.success(response.message);
            this.reloadList.next(true);
          } else {
            this.toastr.error(response.message);
          }
        },
        error => {
          this.toastr.error(error);
        }
      );
    }

  }

  private addClientFormControls() {
    this.foundForm.addControl('name', new FormControl('', [Validators.required]));
    this.foundForm.controls.name.setValue(this.item.reservation?.guestName);
    this.foundForm.addControl('phoneNumber', new FormControl(''));
    this.foundForm.addControl('email', new FormControl('', [Validators.required]));
    this.foundForm.addControl('street', new FormControl(''));
    this.foundForm.addControl('city', new FormControl(''));
    this.foundForm.addControl('postalCode', new FormControl(''));
    this.foundForm.addControl('country', new FormControl(''));
    this.foundForm.addControl('referenceNumber', new FormControl(''));
  }

  private removeClientFormControls() {
    this.foundForm.removeControl('name');
    this.foundForm.removeControl('phoneNumber');
    this.foundForm.removeControl('email');
    this.foundForm.removeControl('street');
    this.foundForm.removeControl('city');
    this.foundForm.removeControl('postalCode');
    this.foundForm.removeControl('country');
    this.foundForm.removeControl('referenceNumber');
  }

  public uploadedFilesChanged(fileChanges: Array<FilesChangedData>) {
    this.selectedFiles = fileChanges;
  }

  cancel() {
    this.cancelled.next(true);
  }

  public canShowErrorMessage(control: string,): boolean {
    const foundForm = this.foundForm as FormGroup;
    if (foundForm.controls[control]) {
      return !!(
        (foundForm.controls[control].touched || foundForm.touched) &&
        foundForm.controls[control].errors
      );
    } else {
      return false;
    }
  }

  getSelection(data: ExtendedWhereData) {
    this.foundForm.controls.whereFrom.setValue(data.roomName);
    this.foundForm.controls.clientName.setValue(data.guestName);
    this.foundForm.controls.name.setValue(data.guestName);
  }

  ngOnDestroy(): void {
    if (this.statusChange$) this.statusChange$.unsubscribe();
  }

}
