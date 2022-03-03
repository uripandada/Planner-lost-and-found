import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { LoadingService } from '../core/services/loading.service';
import { ToastrService } from 'ngx-toastr';
import { HotelService } from '../core/services/hotel.service';
import { BehaviorSubject } from 'rxjs';
import { GetPageOfCategoriesQuery, GetCategoryDetailsQuery, InsertCategoryCommand, PageOfOfCategoryGridItemViewModel, ProcessResponse, ProcessResponseOfGuid, CategoryDetailsViewModel, CategoryGridItemViewModel, CategoryManagementClient, UpdateCategoryCommand } from '../core/autogenerated-clients/api-client';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-categories-management',
  templateUrl: './categories-management.component.html',
  styleUrls: ['./categories-management.component.scss']
})
export class CategoriesManagementComponent implements OnInit {
  sorts = [
    { key: 'NAME_ASC', value: 'Name A to Z' },
    { key: 'NAME_DESC', value: 'Name Z to A' },
    { key: 'CREATED_AT_DESC', value: 'Newest first' },
    { key: 'CREATED_AT_ASC', value: 'Oldest first' },
    { key: 'EXPIRATION_DAYS_ASC', value: 'Expiration days High to Low' },
    { key: 'EXPIRATION_DAYS_DESC', value: 'Expiration days Low to High' },
  ];

  isCategoryLoaded$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isLoadingCategoryDetails$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  showLoadMore$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  loadedNumberOfCategories$: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  totalNumberOfCategories$: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  selectedCategoryId$: BehaviorSubject<string> = new BehaviorSubject<string>(null);
  Categories$: BehaviorSubject<CategoryGridItemViewModel[]> = new BehaviorSubject<CategoryGridItemViewModel[]>([]);


  areDetailsDisplayed$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  selectedCategoryDetails$: BehaviorSubject<CategoryDetailsViewModel> = new BehaviorSubject<CategoryDetailsViewModel>(new CategoryDetailsViewModel({
    id: null,
    name: null,
    expirationDays: 0
  }));

  filterForm: FormGroup;

  constructor(
    public loading: LoadingService,
    private _formBuilder: FormBuilder,
    private _CategoryManagementClient: CategoryManagementClient,
    private _route: ActivatedRoute,
    private _toastr: ToastrService) {
  }

  ngOnInit(): void {
    this.loading.start();

    this.filterForm = this._formBuilder.group({
      sortKey: ['NAME_ASC'],
      keywords: [''],
    });

    this.filterForm.valueChanges.pipe(
      debounceTime(300)
    ).subscribe(
      (formValues: any) => { this._loadCategories(0); },
      (error: Error) => { },
      () => { }
    );

    this._loadCategories(0);
  }

  selectCategory(category: CategoryGridItemViewModel) {
    console.log("rrrrrrrrrrrrrrr");
    this.isLoadingCategoryDetails$.next(true);
    this.selectedCategoryId$.next(category.id);
    this.loading.start();
    let request = new GetCategoryDetailsQuery({ id: category.id });
    console.log(request);

    this._CategoryManagementClient.getCategoryDetails(request).subscribe(
      (categoryDetails: CategoryDetailsViewModel) => {
        this.selectedCategoryDetails$.next(categoryDetails);
        this.isCategoryLoaded$.next(true);
        this.areDetailsDisplayed$.next(true);
      },
      (error: Error) => {
        this._toastr.error(error.message);
      },
      () => {
        this.isLoadingCategoryDetails$.next(false);
        this.loading.stop();
      }
    );
  }

  loadMoreCategories() {
    this._loadCategories(this.loadedNumberOfCategories$.value);
  }

  newCategoryDetails() {
    this.selectedCategoryId$.next(null);
    console.log("dddddddddddddddddd");
    this.selectedCategoryDetails$.next(this._createNewCategoryDetails());
    this.isCategoryLoaded$.next(true);

    this.areDetailsDisplayed$.next(true);
  }

  onCategoryInserted(category: CategoryDetailsViewModel) {
    this.selectedCategoryDetails$.next(category);
    this.selectedCategoryId$.next(category.id);
    this.Categories$.next([...this.Categories$.value, new CategoryGridItemViewModel(category)]);
    this.loadedNumberOfCategories$.next(this.loadedNumberOfCategories$.value + 1);
    this.totalNumberOfCategories$.next(this.totalNumberOfCategories$.value + 1);
  }

  onCategoryUpdated(category: CategoryDetailsViewModel) {
    this.selectedCategoryDetails$.next(category);

    let Categories = [...this.Categories$.value];
    let categoryItem = Categories.find(c => c.id === category.id);
    if (categoryItem) {
      categoryItem.name = category.name;
      categoryItem.expirationDays = category.expirationDays;
      this.Categories$.next(Categories);
    }
  }

  onCategoryCancel() {
    this.areDetailsDisplayed$.next(false);
    this.selectedCategoryId$.next(null);
  }

  private _createNewCategoryDetails(): CategoryDetailsViewModel {
    return new CategoryDetailsViewModel({
      id: null,
      name: null,
      expirationDays: 0
    });
  }

  private _loadCategories(skip: number) {
    this.loading.start();

    let query: GetPageOfCategoriesQuery = new GetPageOfCategoriesQuery({
      skip: skip,
      take: 20,
      keywords: this.filterForm.controls.keywords.value,
      sortKey: this.filterForm.controls.sortKey.value
    });

    this._CategoryManagementClient.getPageOfCategories(query).subscribe(
      (response: PageOfOfCategoryGridItemViewModel) => {
        if (skip === 0) {
          this.Categories$.next(response.items);
        } else {
          this.Categories$.next([...this.Categories$.value, ...response.items]);
        }
        this.totalNumberOfCategories$.next(response.totalNumberOfItems);
        this.loadedNumberOfCategories$.next(this.Categories$.value.length);
        this.showLoadMore$.next(this.loadedNumberOfCategories$.value < this.totalNumberOfCategories$.value);
      },
      (error: Error) => {
        this._toastr.error(error.message);
      },
      () => {
        this.loading.stop();
      },
    );
  }
}
