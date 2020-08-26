import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Place, PlaceClient } from 'src/app/services/services';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class PlaceService {

  public places: Place[];

  constructor(public placeClient: PlaceClient) {
  }

  public getPlace(placeId: string): Observable<Place> {
    return this.placeClient.getPlace(placeId);
  }



}
