import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ClaimsKeys, ManagementClaimKeys, SettingsClaimKeys } from 'src/api-authorization/claim-keys';
import { AuthorizeService, IUser } from '../../../../api-authorization/authorize.service';

@Component({
  selector: 'app-sidebar-menu',
  templateUrl: './sidebar-menu.component.html',
  styleUrls: ['./sidebar-menu.component.scss']
})
export class SidebarMenuComponent implements OnInit {

  SETTINGS = 'SETTINGS';
  MANAGEMENT = 'MANAGEMENT';

  settingsClaimKeys: SettingsClaimKeys;
  managementClaimKeys: ManagementClaimKeys;

  userFullName: string;
  userInitials: string;
  selectedSubMenuKey: string = this.MANAGEMENT;

  constructor(private authService: AuthorizeService, private _router: Router) { }

  ngOnInit(): void {
    this.authService.getUser().subscribe((user: IUser) => {
      if (user) {
        this.userFullName = `${user.name}`;
        this.userInitials = user.name[0];
      } else {
      }
    });

    this._initializeSubMenuKey();
  }

  showSettings(): void {
    this.selectedSubMenuKey = this.SETTINGS;
    this._saveSelectedSubMenu(this.SETTINGS);
  }

  showManagement(): void {
    this.selectedSubMenuKey = this.MANAGEMENT;
    this._saveSelectedSubMenu(this.MANAGEMENT);
  }

  logout(): void {
    const state = { returnUrl: '' };
    this.authService.signOut(state);
  }

  navigateToHome(): void {
    this._router.navigate(['/']);
  }

  private _initializeSubMenuKey(): void {
    let subMenuKey = this._loadSelectedSubMenu();
    if (!subMenuKey) {

      let settingsRoutes: string[] = [
        '/rooms-management',
        '/assets-management',
        '/users-management',
        '/reservations',
        '/role-management',
        '/categories',
        '/room-categories',
        '/hotels-management',
        '/colors-management',
      ];

      for (let route of settingsRoutes) {
        if (this._router.url.indexOf(route) > -1) {
          subMenuKey = this.SETTINGS;
        }
      }

      if (!subMenuKey) {
        subMenuKey = this.MANAGEMENT;
      }

      this._saveSelectedSubMenu(subMenuKey);
    }

    this.selectedSubMenuKey = subMenuKey;
  }

  private _loadSelectedSubMenu(): string {
    return localStorage.getItem('__#SMKey');
  }

  private _saveSelectedSubMenu(subMenuKey: string): void {
    localStorage.setItem('__#SMKey', subMenuKey);
  }
}
