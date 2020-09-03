import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Place, PlaceClient, PlaceReview } from 'src/app/services/services';
import { Observable, BehaviorSubject, Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { LoadingService } from 'src/app/services/loadingService';


export class PlaceSearchStatus {
  // tslint:disable: no-inferrable-types
  public page: number = 1;
  public pageSize: number = 10;
  public places: Place[] = [];
  public mineFilter: boolean = true;
  public pendingReview: boolean = true;
  public moreResults: boolean = true;
}
@Injectable()
export class PlaceSearchStatusService {


  public readonly placeSearchStatus: Observable<PlaceSearchStatus>;
  private _placeSearchStatus: BehaviorSubject<PlaceSearchStatus> = new BehaviorSubject<PlaceSearchStatus>(new PlaceSearchStatus());
  private readonly onPlaceSearchStatusChange$: Subject<PlaceSearchStatus>;

  constructor(private placeService: PlaceService, private loading: LoadingService) {
    this.placeSearchStatus = this._placeSearchStatus.asObservable();

    this.onPlaceSearchStatusChange$ = new Subject();
    this.onPlaceSearchStatusChange$.pipe(debounceTime(200)).subscribe(this.onPlaceSearchStatusChange.bind(this));
  }

  public get currentPlaceSearchStatus(): PlaceSearchStatus {
    return this._placeSearchStatus.getValue();
  }

  public setFilter(owner: boolean, pendingReview: boolean) {
    const status = new PlaceSearchStatus();
    status.mineFilter = owner;
    status.pendingReview = pendingReview;
    this.notifySubscribers(status);
  }

  public loadMore() {
    const status = this.currentPlaceSearchStatus;
    status.page++;
    this.notifySubscribers(status);
  }

  public reset() {
    this.notifySubscribers(new PlaceSearchStatus());
  }

  public refreshAll() {
    this.notifySubscribers(this.currentPlaceSearchStatus);
  }

  public notifySubscribers(current: PlaceSearchStatus) {
    this._placeSearchStatus.next(current);
    this.onPlaceSearchStatusChange$.next(current);
}

  public onPlaceSearchStatusChange(current: PlaceSearchStatus) {
    this.loading.activate('search');
    this.placeService.getPlaces(current.page, current.pageSize + 1, current.mineFilter, current.pendingReview)
      .toPromise().then(places => this.processResponse(places, current));
  }

  private processResponse(places: Place[], current: PlaceSearchStatus) {
    this.loading.deactivate('search');
    const currentNow = current;
    let insert = places;
    if (places.length > current.pageSize) {
      insert = insert.splice(current.pageSize, 1);
    }
    if (currentNow.page === 1) {
      currentNow.places = insert;
    } else {
      currentNow.places.push(...insert);
    }
    currentNow.places = [...new Set(currentNow.places)];
    currentNow.moreResults = current.pageSize < places.length;

    this._placeSearchStatus.next(currentNow);
  }
}

@Injectable()
export class PlaceService {

  public places: Place[];

  constructor(public placeClient: PlaceClient) {
  }

  public getPlace(placeId: string): Observable<Place> {
    return this.placeClient.getPlace(placeId);
  }

  public getPlaces(page: number, pageSize: number, mine: boolean, pendingReview: boolean): Observable<Place[]> {
    return this.placeClient.getEstablishmentsAll(page, pageSize, mine, pendingReview);
  }

  public createPlace(place: Place) {
    return this.placeClient.createEstablishment(place);
  }

  public updatePlace(place: Place) {
    return this.placeClient.updateEstablishment(place);
  }

  public reviewPlace(review: PlaceReview, id: string) {
    return this.placeClient.reviewPlace(id, review);
  }

}
