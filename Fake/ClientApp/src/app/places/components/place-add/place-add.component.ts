import { Component, OnInit } from '@angular/core';
import { PlaceService } from '../../services/place.service';
import { Router } from '@angular/router';
import { Place } from 'src/app/services/services';

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
    this.placeService.createPlace(place).subscribe(p => {
      if (!!p) {
        this.router.navigateByUrl('/place/list');
      }
    });

    //const result = this.placeService.createPlace(place);
    //if (result) {
    //  this.router.navigateByUrl('places');
    //}
  }

}
