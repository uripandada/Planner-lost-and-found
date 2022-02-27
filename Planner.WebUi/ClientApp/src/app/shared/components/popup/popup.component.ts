import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { AuthorizeService, IUser } from '../../../../api-authorization/authorize.service';

@Component({
  selector: 'app-popup',
  templateUrl: './popup.component.html',
  styleUrls: ['./popup.component.scss']
})
export class PopupComponent implements OnInit {
  @Input() isOpen: boolean;
  @Input() isNonButtonCloseEnabled: boolean = true;

  @Output() closed: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor() { }

  ngOnInit(): void {
    //this.authService.getUser().subscribe((user: IUser) => {
    //  if (user) {
    //    this.userFullName = `${user.name}`;
    //  }
    //  else {
    //  }
    //});
  }

  close() {
    this.closed.next(true);
  }

  nonButtonClose() {
    if (this.isNonButtonCloseEnabled) {
      this.closed.next(true);
    }
  }
}
