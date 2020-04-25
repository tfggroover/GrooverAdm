import { Component, OnInit } from '@angular/core';
import { Place } from '../../models/Place';
import { PlaceService } from '../../services/place.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-place-add',
  templateUrl: './place-add.component.html',
  styleUrls: ['./place-add.component.scss']
})
export class PlaceAddComponent implements OnInit {

  constructor(private placeService: PlaceService,
    private router: Router) { }

  ngOnInit(): void {
  }

  public addPlace(place: Place) {
    const result = this.placeService.createPlace(place);
    if (result) {
      this.router.navigateByUrl('places');
    }
  }

}
