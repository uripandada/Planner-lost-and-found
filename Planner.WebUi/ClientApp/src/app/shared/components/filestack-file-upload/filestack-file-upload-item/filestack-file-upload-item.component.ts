import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-filestack-file-upload-item',
  templateUrl: './filestack-file-upload-item.component.html'
})
export class FilestackFileUploadItemComponent implements OnInit, OnChanges {

  @Input() fileUrl: string;
  @Input() isUploaded: boolean;
  //@Input() localTemporaryFileUrl: string;
  @Input() fileUploadProgress: number;

  @Output() fileRemoved: EventEmitter<string> = new EventEmitter<string>();

  public isUploaded$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public progress$: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  public fileUrl$: BehaviorSubject<SafeResourceUrl> = new BehaviorSubject<SafeResourceUrl>("");

  constructor(private _sanitizer: DomSanitizer) {
  }

  ngOnInit() {
    let fileUrl: SafeResourceUrl = this._sanitizer.bypassSecurityTrustResourceUrl(this.fileUrl);
    this.fileUrl$.next(fileUrl);
    this.isUploaded$.next(this.isUploaded);
    if (this.isUploaded) {
      this.progress$.next(100);
    }
    else {
      this.progress$.next(0);
    }
  }

  ngOnChanges(changes: SimpleChanges): void {

    //if (changes.isUploaded && !changes.isUploaded.firstChange) {
    //  this.isUploaded$.next(changes.isUploaded.currentValue);
    //}
    //if (changes.fileUrl && !changes.fileUrl.firstChange) {
    //  if (this.isUploaded) {
    //    this.fileUrl$.next(changes.fileUrl.currentValue);
    //  }
    //}
    if (changes.fileUploadProgress && !changes.fileUploadProgress.firstChange) {
      this.progress$.next(changes.fileUploadProgress.currentValue);
    }
  }

  remove() {
    this.fileRemoved.next(this.fileUrl);
  }
}

