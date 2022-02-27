import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthorizeService } from './authorize.service';
import { tap } from 'rxjs/operators';
import { ApplicationPaths, QueryParameterNames } from './api-authorization.constants';

@Injectable({
  providedIn: 'root'
})
export class AuthorizeGuard implements CanActivate {
  constructor(
    private authorize: AuthorizeService,
    private router: Router) {
  }
  canActivate(
    _next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {

    return this.authorize
      .isAuthenticated()
      .pipe(tap(isAuthenticated => {
        if (isAuthenticated) {
          const claims = _next.data.claims;
          if (claims !== undefined) {
            const hasClaim = this.authorize.hasClaim(claims);
            if (hasClaim) {
              return true;
            }
          } else {
            return true;
          }
        }
        this.handleAuthorization(false, state);
        return false;
      }));
  }

  private handleAuthorization(isAuthenticated: boolean, state: RouterStateSnapshot) {
    if (!isAuthenticated) {
      this.router.navigate([ApplicationPaths.Login], {
        // queryParams: {
        //   [QueryParameterNames.ReturnUrl]: state.url
        // }
      });
    }
  }
}
