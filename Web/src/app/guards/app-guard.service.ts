import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  CanLoad,
  Router,
  RouterStateSnapshot,
} from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AppGuardService implements CanActivate, CanLoad {
  constructor(private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const currentUser =
      sessionStorage.getItem('userData') &&
      sessionStorage.getItem('userData') !== 'undefined'
        ? JSON.parse(sessionStorage.getItem('userData'))
        : {};
    if (currentUser && currentUser.token) {
      // logged in so return true
      return true;
    }
    // not logged in so redirect to login page with the return url
    this.router.navigate(['/login']); 
  }

  canLoad() {
    return !!sessionStorage.getItem('userData');
  }
}
