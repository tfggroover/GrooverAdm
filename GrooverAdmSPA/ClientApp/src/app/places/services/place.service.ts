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

  public async createPlace(place: Place) {
    const promise = this.firestore.collection('places').add(place.toJson());
    if (place.playlist) {
      const ref = await promise;
      ref.collection('placeMusic').doc('mainPlaylist').set(place.playlist.toJson());
    }
  }

  public updatePlace(place: Place) {
    this.firestore.doc('places/' + place.id).set(place.toJson());
  }

  public deletePlace(place: Place) {
    this.firestore.doc('places/' + place.id).delete();
  }
}
