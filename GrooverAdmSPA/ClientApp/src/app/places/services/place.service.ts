import { Injectable } from '@angular/core';
import { AngularFirestore } from '@angular/fire/firestore';
import 'firebase/firestore';
import { Place } from '../models/Place';


@Injectable({
  providedIn: 'root'
})
export class PlaceService {

  constructor(private firestore: AngularFirestore) {}

  public getPlaces() {
    return this.firestore.collection('places').snapshotChanges();
  }

  public getPlace(placeId: string) {
    return this.firestore.doc('places/' + placeId).get();
  }

  public createPlace(place: Place) {
    return this.firestore.collection('places').add(place);
  }

  public updatePlace(place: Place) {
    this.firestore.doc('places/' + place.id).set(place);
  }

  public deletePlace(place: Place) {
    this.firestore.doc('places/' + place.id).delete();
  }
}
