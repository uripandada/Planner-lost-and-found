import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, ValidationErrors, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { InsertExperienceCompensationCommand, ProcessResponse, ProcessResponseOfGuid, ExperienceCompensationDetailsViewModel, ExperienceCompensationManagementClient, UpdateExperienceCompensationCommand } from '../../core/autogenerated-clients/api-client';
import { LoadingService } from '../../core/services/loading.service';

@Component({
  selector: 'app-experience-compensation-edit',
  templateUrl: './experience-compensation-edit.component.html',
  styleUrls: ['./experience-compensation-edit.component.scss']
})
export class ExperienceCompensationEditComponent implements OnInit, OnChanges {

  @Input() category: ExperienceCompensationDetailsViewModel;

  @Output() deleted: EventEmitter<string> = new EventEmitter<string>(); 
  @Output() inserted: EventEmitter<ExperienceCompensationDetailsViewModel> = new EventEmitter<ExperienceCompensationDetailsViewModel>();
  @Output() updated: EventEmitter<ExperienceCompensationDetailsViewModel> = new EventEmitter<ExperienceCompensationDetailsViewModel>();
  @Output() cancelled: EventEmitter<boolean> = new EventEmitter<boolean>();

  isSaving$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isCreateNew$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  categoryForm: FormGroup;
  loading: LoadingService;

  constructor(private _router: Router, private _toastr: ToastrService, private _formBuilder: FormBuilder,
    private _CategoryManagementClient: ExperienceCompensationManagementClient,) {
    this.loading = new LoadingService();
  }

  ngOnInit(): void {
    this._setCreateNewStatus();

    this.categoryForm = this._formBuilder.group({
      name: [this.category.name, [Validators.required]],
      price: [this.category.price, [Validators.required]],
      currency: [this.category.currency, [Validators.required]],
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.category && !changes.category.firstChange) {
      this._setCreateNewStatus(); 
      this._setCategoryFormData();
    }
  }

  cancel() {
    this.cancelled.next(true);
  }

  private _setCreateNewStatus() {
    if (this.category.id) {
      this.isCreateNew$.next(false);
    }
    else {
      this.isCreateNew$.next(true);
    }
  }

  private _setCategoryFormData() {
    this.categoryForm.controls.name.setValue(this.category.name);
    this.categoryForm.controls.price.setValue(this.category.price);
    this.categoryForm.controls.currency.setValue(this.category.currency);

    this.categoryForm.updateValueAndValidity({ onlySelf: false });

    this.categoryForm.markAsUntouched({ onlySelf: false });
    this.categoryForm.markAsPristine({ onlySelf: false });
  }

  save() {
    if (!this.categoryForm.valid) {
      this.categoryForm.markAllAsTouched();
      this.categoryForm.markAsDirty({ onlySelf: false });
      this._toastr.error("You have to fix form errors before you can continue.");
      return;
    }

    let formValues = this.categoryForm.getRawValue();

    if (this.isCreateNew$.value) {
      this._insertCategory();
    }
    else {
      this._updateCategory();
    }
  }

  private _insertCategory() {
    let formValues = this.categoryForm.getRawValue();
    let insertRequest: InsertExperienceCompensationCommand = new InsertExperienceCompensationCommand({
      name: formValues.name,
      price: formValues.price,
      currency: formValues.currency
    });

    this.loading.start();

    this.isSaving$.next(true);

    this._CategoryManagementClient.insertExperienceCompensation(insertRequest).subscribe(
      (response: ProcessResponseOfGuid) => {
        if (response.hasError) {
          this._toastr.error(response.message);
          this._setFormValidationErrors(response, this.categoryForm);
          this.isSaving$.next(false);
          return;
        }

        this.isSaving$.next(false);

        let CategoryDetails = new ExperienceCompensationDetailsViewModel({
          id: response.data,
          name: insertRequest.name,
          price: insertRequest.price,
          currency: insertRequest.currency
        });

        this.inserted.next(CategoryDetails);
      },
      (error: Error) => {
        this._toastr.error(error.message);
        this.isSaving$.next(false);
      },
      () => {
        this.loading.stop();
        this.isSaving$.next(false);
      },
    );
  }
  private _updateCategory() {
    let formValues = this.categoryForm.getRawValue();
    let updateRequest: UpdateExperienceCompensationCommand = new UpdateExperienceCompensationCommand({
      id: this.category.id,
      name: formValues.name,
      price: formValues.price,
      currency: formValues.currency
    });

    this.loading.start();
    this.isSaving$.next(true);

    this._CategoryManagementClient.updateExperienceCompensation(updateRequest).subscribe(
      (response: ProcessResponse) => {
        if (response.hasError) {
          this._toastr.error(response.message);
          this._setFormValidationErrors(response, this.categoryForm);
          this.isSaving$.next(false);
          return;
        }

        this._toastr.success(response.message);
        this.isSaving$.next(false);

        let CategoryDetails = new ExperienceCompensationDetailsViewModel({
          id: updateRequest.id,
          name: updateRequest.name,
          price: updateRequest.price,
          currency: updateRequest.currency
        });

        this.updated.next(CategoryDetails);
      },
      (error: Error) => {
        this._toastr.error(error.message);
        this.isSaving$.next(false);
      },
      () => {
        this.loading.stop();
        this.isSaving$.next(false);
      },
    );
  }
  private _setFormValidationErrors(response: ProcessResponse, form: FormGroup) {
    for (let error of response.modelErrors) {
      let control = form.get(error.key);

      if (control) {
        var errors: ValidationErrors = {};
        errors[error.validatorKey] = true;

        control.setErrors(errors)
        control.markAsDirty();
        control.markAsTouched();
      }
    }
  }
}
