import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, combineLatest } from 'rxjs';
import { PlaceService } from './place.service';
import { AuthorizeService } from 'src/api-authorization/authorize.service';
import { Place } from 'src/app/services/services';
import { map } from 'rxjs/operators';
import { CompleteUser } from 'src/api-authorization/user-manager/userManagerService';

@Injectable({
  providedIn: 'root'
})
export class OwnerGuard implements CanActivate {

  constructor(public placeService: PlaceService, public auth: AuthorizeService) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    return combineLatest([this.placeService.getPlace(next.params.id), this.auth.getUser()]).pipe(map((p) => this.isUserOwner(p[0], p[1])));
  }

  isUserOwner(place: Place, user: CompleteUser): boolean {
    return place.owners.map(u => u.id).indexOf(user.firebaseAuthentication.uid) !== -1;
  }

}
