import { Component, OnInit } from '@angular/core';
import { AngularFirestore } from '@angular/fire/firestore';
import { Observable } from 'rxjs';
import 'firebase/firestore';
import { Router } from '@angular/router';
import { PlaceService, PlaceSearchStatusService, PlaceSearchStatus } from '../../services/place.service';
import { Place } from 'src/app/services/services';

@Component({
  selector: 'app-places',
  templateUrl: './place-list.component.html',
  styleUrls: ['./place-list.component.scss']
})
export class PlaceListComponent implements OnInit {
  items: Place[];
  constructor(placeSearchStatusService: PlaceSearchStatusService,
    private router: Router) {
      placeSearchStatusService.placeSearchStatus.subscribe(this.processSearchStatus.bind(this));
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

  public processSearchStatus(status: PlaceSearchStatus){
    this.items = status.places;
  }

}
