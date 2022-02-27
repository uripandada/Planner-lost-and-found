import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { AuthorizeService, IUser } from '../../../../api-authorization/authorize.service';

@Component({
  selector: 'app-drawer',
  templateUrl: './drawer.component.html'
})
export class DrawerComponent implements OnInit {
  @Input() isOpen: boolean;

  @Output() closed: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor() { }

  ngOnInit(): void {
  }

  close() {
    this.closed.next(true);
  }
}
