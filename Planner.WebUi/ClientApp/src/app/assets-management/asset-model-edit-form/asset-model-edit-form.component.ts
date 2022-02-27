import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FileDetails, FilesChangedData } from '../../shared/components/file-upload/file-upload.component';

@Component({
  selector: 'app-asset-model-edit-form',
  templateUrl: './asset-model-edit-form.component.html',
  styleUrls: ['./asset-model-edit-form.component.scss']
})
export class AssetModelEditFormComponent implements OnInit {
  @Input() assetModelForm: FormGroup;
  @Input() assetModelFormIndex: number;
  @Input() currentlyUploadingFiles: Array<FileDetails> = [];
  @Input() temporaryUploadedFiles: Array<FileDetails> = [];
  @Input() uploadedFiles: Array<FileDetails> = [];

  constructor() { }

  ngOnInit(): void {
  }

  public uploadedFilesChanged(filesChanges: Array<FilesChangedData>) {
  }
}
