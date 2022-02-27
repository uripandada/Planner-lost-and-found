export class FilestackFileUploadItemData {
  constructor(
    fileUrl: string,
    isUploaded: boolean,
    fileUploadProgress: number,
  ) {
    this.fileUrl = fileUrl;
    this.isUploaded = isUploaded;
    this.fileUploadProgress = fileUploadProgress;
  }

  fileUrl: string;
  isUploaded: boolean;
  fileUploadProgress: number;
}
