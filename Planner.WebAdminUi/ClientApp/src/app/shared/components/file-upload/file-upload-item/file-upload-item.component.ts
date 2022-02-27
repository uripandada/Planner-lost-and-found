import { HttpClient, HttpEvent, HttpEventType } from '@angular/common/http';
import { Component, EventEmitter, Inject, Input, OnInit, Optional, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { FileDetails } from '../file-upload.component';


@Component({
  selector: 'app-file-upload-item',
  templateUrl: './file-upload-item.component.html',
  styleUrls: ['./file-upload-item.component.scss']
})
export class FileUploadItemComponent implements OnInit {
  
  @Input() fileIndex: number;
  @Input() file: FileDetails;
  @Input() isUploadInProgress: boolean;

  @Output() fileRemoved: EventEmitter<number> = new EventEmitter<number>();


  constructor() {
  }

  ngOnInit(): void {

  }

  delete() {
    this.fileRemoved.next(this.fileIndex);
  }
}

