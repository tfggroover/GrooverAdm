import { Component, OnInit } from '@angular/core';
import { Place, User } from 'src/app/services/services';

@Component({
  selector: 'app-place-detail',
  templateUrl: './place-detail.component.html',
  styleUrls: ['./place-detail.component.css']
})
export class PlaceDetailComponent implements OnInit {

  public place: Place = new Place();
  public userLocation: Coordinates;
  public marker: {lat: number, lng: number};
  public zoom: number;
  public user: User;

  constructor() { }

  ngOnInit(): void {
  }

}
