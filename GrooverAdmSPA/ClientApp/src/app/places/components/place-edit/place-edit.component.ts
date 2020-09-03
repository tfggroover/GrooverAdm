import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Place } from '../../models/Place';
import { PlaceService } from '../../services/place.service';


@Component({
  selector: 'app-place-edit',
  templateUrl: './place-edit.component.html',
  styleUrls: ['./place-edit.component.scss']
})
export class PlaceEditComponent implements OnInit {

  public place: Place;

  constructor(public firestore: PlaceService,
    private router: Router) { }

  ngOnInit(): void {
    const placeId = window.localStorage.getItem('editPlaceId');

  }

}
