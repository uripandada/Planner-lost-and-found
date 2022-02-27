import { Component, OnInit } from '@angular/core';
import { AuthorizeService, IUser } from '../../../../api-authorization/authorize.service';

@Component({
  selector: 'app-sidebar-menu',
  templateUrl: './sidebar-menu.component.html',
  styleUrls: ['./sidebar-menu.component.scss']
})
export class SidebarMenuComponent implements OnInit {
  userFullName: string;
  userInitials: string;

  constructor(private authService: AuthorizeService) { }

  ngOnInit(): void {
    this.authService.getUser().subscribe((user: IUser) => {
      if (user) {
        this.userFullName = `${user.name}`;
        this.userInitials = user.name[0];
      } else {
      }
    });
  }

  logout(): void {
    const state = { returnUrl: '' };
    this.authService.signOut(state);
  }
}
