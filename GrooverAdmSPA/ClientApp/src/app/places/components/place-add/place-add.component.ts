import { Component, OnInit } from '@angular/core';
import { Place } from '../../models/Place';
import { PlaceService } from '../../services/place.service';

@Component({
  selector: 'app-place-add',
  templateUrl: './place-add.component.html',
  styleUrls: ['./place-add.component.scss']
})
export class PlaceAddComponent implements OnInit {

  constructor(private placeService: PlaceService) { }

  ngOnInit(): void {
  }

  public addPlace(place: Place) {
    this.placeService.createPlace(place);
  }

}
