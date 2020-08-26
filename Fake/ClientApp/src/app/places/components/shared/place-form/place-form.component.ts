import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Marker } from '@agm/core';
import {Location, Appearance} from '@angular-material-extensions/google-maps-autocomplete';
import PlaceResult = google.maps.places.PlaceResult;
import { Place } from 'src/app/services/services';

@Component({
  selector: 'app-place-form',
  templateUrl: './place-form.component.html',
  styleUrls: ['./place-form.component.scss']
})
export class PlaceFormComponent implements OnInit {

  @Input()
  public place: Place = new Place();


  @Output()
  public placeUpdateListener: EventEmitter<Place> = new EventEmitter();

  public userLocation: Coordinates;
  public marker: {lat: number, lng: number};
  public zoom: number;

  constructor() {}

  ngOnInit(): void {
    if ('geolocation' in navigator) {
      navigator.geolocation.getCurrentPosition(s => {
        this.userLocation =  s.coords;
        this.marker = {lat: this.userLocation.latitude, lng: this.userLocation.longitude};
      });
    }
    this.zoom = 10;
  }

  public onSubmit() {
    this.placeUpdateListener.next(this.place);
  }

  public updatePlace() {
    this.place.location.latitude = this.marker.lat;
    this.place.location.longitude = this.marker.lng;
  }

  public changeMarker(lat: number, lng: number) {
    this.marker.lat = lat;
    this.marker.lng = lng;
    this.updatePlace();
  }

  onAutocompleteSelected(result: PlaceResult) {
    console.log('onAutocompleteSelected: ', result);
  }

  onLocationSelected(location: Location) {
    console.log('onLocationSelected: ', location);
    this.marker.lat = location.latitude;
    this.marker.lng = location.longitude;
  }

}
