import { BehaviorSubject, Observable } from "rxjs";
import { Injectable } from "@angular/core";

@Injectable({ providedIn: 'root' })
export class LoadingService {
  private _loadingCounter: number = 0;

  private _isLoading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public isLoading$: Observable<boolean>;

  constructor() {
    this.isLoading$ = this._isLoading$.asObservable();
  }

  public start() {
    this._loadingCounter++;
    this._isLoading$.next(true);
  }

  public stop() {
    this._loadingCounter--;
    if (this._loadingCounter < 0) {
      this._loadingCounter = 0;
    }
    this._isLoading$.next(this._loadingCounter > 0);
  }

  public reset() {
    this._loadingCounter = 0;
    this._isLoading$.next(false);
  }
}
