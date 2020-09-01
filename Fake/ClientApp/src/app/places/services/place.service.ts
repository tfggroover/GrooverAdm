import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Place, PlaceClient } from 'src/app/services/services';
import { Observable, BehaviorSubject, Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';


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

  constructor(private placeService: PlaceService) {
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

    this.placeService.getPlaces(current.page, current.pageSize, current.mineFilter, current.pendingReview)
      .toPromise().then(places => this.processResponse(places, current));
  }

  private processResponse(places: Place[], current: PlaceSearchStatus) {
    const currentNow = current;
    currentNow.places.push(...places);
    currentNow.moreResults = places.length < current.pageSize;

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

}
