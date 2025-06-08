import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { API_KEYS } from '../shared/constants/api-keys';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageGuard implements CanActivate {

  constructor(private router: Router) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    const requiredKey = route.data['storageKey'] || 'authToken';
    const redirectTo = route.data['redirectTo'] || '/home';

    if (localStorage.getItem(requiredKey) == API_KEYS.KEY1 ||
      localStorage.getItem(requiredKey) == API_KEYS.KEY2) {
      return true;
    }

    return this.router.createUrlTree([redirectTo]);
  }
}