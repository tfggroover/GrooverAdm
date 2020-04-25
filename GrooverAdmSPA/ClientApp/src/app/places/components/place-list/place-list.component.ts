import { Component, OnInit } from '@angular/core';
import { AngularFirestore } from '@angular/fire/firestore';
import { Observable } from 'rxjs';
import 'firebase/firestore';
import { Router } from '@angular/router';

@Component({
  selector: 'app-places',
  templateUrl: './place-list.component.html',
  styleUrls: ['./place-list.component.scss']
})
export class PlaceListComponent implements OnInit {
  items: Observable<any[]>;
  constructor(firestore: AngularFirestore,
    private router: Router) {
      this.items = firestore.collection('places').valueChanges({idField: 'id'});
   }

  ngOnInit(): void {
  }

  public editPlace(place: any) {
    window.localStorage.removeItem('editPlaceId');
    window.localStorage.setItem('editPlaceId', place.id.toString());
    this.router.navigate(['places/edit']);
  }

  public addPlace() {
    this.router.navigate(['places/add']);
  }

}
