import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, ValidationErrors, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { InsertExperienceCategoryCommand, ProcessResponse, ProcessResponseOfGuid, ExperienceCategoryDetailsViewModel, ExperienceCategoryManagementClient, UpdateExperienceCategoryCommand, GetListOfExperienceCategoriesQuery, ExperienceCategoryItemData } from '../../core/autogenerated-clients/api-client';
import { LoadingService } from '../../core/services/loading.service';
import { Observable } from 'rxjs';
import { debounceTime, startWith, switchMap, filter, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-experience-category-edit',
  templateUrl: './experience-category-edit.component.html',
  styleUrls: ['./experience-category-edit.component.scss']
})
export class ExperienceCategoryEditComponent implements OnInit, OnChanges {

  @Input() category: ExperienceCategoryDetailsViewModel;

  @Output() deleted: EventEmitter<string> = new EventEmitter<string>();
  @Output() inserted: EventEmitter<ExperienceCategoryDetailsViewModel> = new EventEmitter<ExperienceCategoryDetailsViewModel>();
  @Output() updated: EventEmitter<ExperienceCategoryDetailsViewModel> = new EventEmitter<ExperienceCategoryDetailsViewModel>();
  @Output() cancelled: EventEmitter<boolean> = new EventEmitter<boolean>();

  isSaving$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isCreateNew$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  categoryForm: FormGroup;
  loading: LoadingService;
  categories: Observable<any[]>;

  constructor(private _router: Router, private _toastr: ToastrService, private _formBuilder: FormBuilder,
    private _CategoryManagementClient: ExperienceCategoryManagementClient,) {
    this.loading = new LoadingService();

  }

  ngOnInit(): void {
    this._setCreateNewStatus();

    this.categoryForm = this._formBuilder.group({
      name: [this.category.name, [Validators.required]],
      experienceName: [this.category.experienceName, [Validators.required]],
    });

    this.categories = this.categoryForm.get('name')!.valueChanges.pipe(
      startWith(''),
      distinctUntilChanged(),
      debounceTime(1000),
      filter((name) => !!name),
      switchMap(name => this._CategoryManagementClient.getList(new GetListOfExperienceCategoriesQuery({ name: name })))
    )

    console.log(this.categories);
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
    this.categoryForm.controls.experienceName.setValue(this.category.experienceName);

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
    let insertRequest: InsertExperienceCategoryCommand = new InsertExperienceCategoryCommand({
      name: formValues.name,
      experienceName: formValues.experienceName
    });

    this.loading.start();

    this.isSaving$.next(true);

    this._CategoryManagementClient.insertExperienceCategory(insertRequest).subscribe(
      (response: ProcessResponseOfGuid) => {
        if (response.hasError) {
          this._toastr.error(response.message);
          this._setFormValidationErrors(response, this.categoryForm);
          this.isSaving$.next(false);
          return;
        }

        this.isSaving$.next(false);

        let CategoryDetails = new ExperienceCategoryDetailsViewModel({
          id: response.data,
          name: insertRequest.name,
          experienceName: insertRequest.experienceName
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
    let updateRequest: UpdateExperienceCategoryCommand = new UpdateExperienceCategoryCommand({
      id: this.category.id,
      name: formValues.name,
      experienceName: formValues.experienceName,
    });

    this.loading.start();
    this.isSaving$.next(true);

    this._CategoryManagementClient.updateExperienceCategory(updateRequest).subscribe(
      (response: ProcessResponse) => {
        if (response.hasError) {
          this._toastr.error(response.message);
          this._setFormValidationErrors(response, this.categoryForm);
          this.isSaving$.next(false);
          return;
        }

        this._toastr.success(response.message);
        this.isSaving$.next(false);

        let CategoryDetails = new ExperienceCategoryDetailsViewModel({
          id: updateRequest.id,
          name: updateRequest.name,
          experienceName: updateRequest.experienceName
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
