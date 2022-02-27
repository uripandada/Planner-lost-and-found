import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy, OnChanges, SimpleChanges } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { AvatarUploadData } from './avatar-upload-data';

@Component({
  selector: 'app-avatar-upload',
  templateUrl: './avatar-upload.component.html',
  styleUrls: ['./avatar-upload.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AvatarUploadComponent implements OnInit, OnChanges {

  @Input() initialAvatarUrl: string = null;

  @Output() selected: EventEmitter<AvatarUploadData> = new EventEmitter<AvatarUploadData>();
  @Output() removed: EventEmitter<boolean> = new EventEmitter<boolean>();

  isAvatarSet$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  avatar$: BehaviorSubject<AvatarUploadData> = new BehaviorSubject<AvatarUploadData>(null);
  
  constructor(private _toastr: ToastrService) { }
    

  ngOnInit(): void {
    if (this.initialAvatarUrl) {
      this.avatar$.next({
        file: null,
        fileName: "not-important",
        fileUrl: this.initialAvatarUrl
      });
      this.isAvatarSet$.next(true);
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.initialAvatarUrl) {
      if (this.initialAvatarUrl) {
        this.avatar$.next({
          file: null,
          fileName: "not-important",
          fileUrl: this.initialAvatarUrl
        });
        this.isAvatarSet$.next(true);
      }
      else {
        this.isAvatarSet$.next(false);
        this.avatar$.next(null);
      }
    }
  }

  fileSelected(files: FileList) {
    if (files.length < 1) {
      this._toastr.error("No files selected.");
      return;
    }

    let file: File = files[0];
    let fileUrl: string = URL.createObjectURL(file);
    let fileName: string = file.name;
    //let fileBlob: Blob = new Blob(<any>file);

    let fileReader: FileReader = new FileReader();
    let that = this;

    fileReader.onload = function (event: ProgressEvent) {
      let arrayBuffer: ArrayBuffer = <ArrayBuffer>this.result;
      let array = new Uint8Array(arrayBuffer);
      //let binaryString = String.fromCharCode.apply(null, array);
      let binaryString = that.arrayBufferToBase64(arrayBuffer);

      let avatarUploadData: AvatarUploadData = {
        file: binaryString,
        fileName: fileName,
        fileUrl: fileUrl
      };
      that.avatar$.next(avatarUploadData);
      that.isAvatarSet$.next(true);

      that.selected.next({
        file: binaryString,
        fileName: fileName,
        fileUrl: fileUrl
      });
    };
    fileReader.readAsArrayBuffer(file);
  }

  arrayBufferToBase64(buffer) {
    return window.btoa(this.arrayBufferToBinaryString(buffer));
  }

  arrayBufferToBinaryString(buffer) {
    let binary = '';
    let bytes = new Uint8Array(buffer);
    let len = bytes.byteLength;
    for (let i = 0; i < len; i++) {
      binary += String.fromCharCode(bytes[i]);
    }
    return binary;
  }

  remove(): void {
    this.isAvatarSet$.next(false);
    this.avatar$.next(null);

    this.removed.next(true);
  }
}
