import { Injectable, Inject } from '@angular/core';
import { AngularFirestore } from '@angular/fire/firestore';
import 'firebase/firestore';
import { Place } from '../models/Place';
import { HttpClient } from '@angular/common/http';


@Injectable({
  providedIn: 'root'
})
export class PlaceService {

  public places: Place[];

  constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public getPlaces() {
    return this.http.get<Place[]>(this.baseUrl + 'api/place?lat=31&lon=6&distance=100');
  }

}
