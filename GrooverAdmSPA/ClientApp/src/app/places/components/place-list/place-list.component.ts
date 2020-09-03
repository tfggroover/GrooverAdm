import { Component, OnInit } from '@angular/core';
import { AngularFirestore } from '@angular/fire/firestore';
import { Observable } from 'rxjs';
import 'firebase/firestore';
import { Router } from '@angular/router';
import { PlaceService } from '../../services/place.service';
import { Place } from '../../models/Place';

@Component({
  selector: 'app-places',
  templateUrl: './place-list.component.html',
  styleUrls: ['./place-list.component.scss']
})
export class PlaceListComponent implements OnInit {
  items: Place[];
  constructor(firestore: AngularFirestore,
    private router: Router,
    private placeService: PlaceService) {
      this.placeService.getPlaces().subscribe(places => this.items = places);
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
