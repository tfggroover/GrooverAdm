import { Component, OnInit } from '@angular/core';
import { PlaceService } from '../../services/place.service';
import { Router } from '@angular/router';
import { Place } from 'src/app/services/services';
import { LoadingService } from 'src/app/services/loadingService';

@Component({
  selector: 'app-place-add',
  templateUrl: './place-add.component.html',
  styleUrls: ['./place-add.component.scss']
})
export class PlaceAddComponent implements OnInit {

  constructor(private placeService: PlaceService,
    private router: Router, private loading: LoadingService) { }

  ngOnInit(): void {
  }

  public addPlace(place: Place) {
    this.loading.activate('addPlace');
    this.placeService.createPlace(place).subscribe(p => {
      this.loading.deactivate('addPlace');
      if (!!p) {
        this.router.navigate(['../']);
      }
    });

    //const result = this.placeService.createPlace(place);
    //if (result) {
    //  this.router.navigateByUrl('places');
    //}
  }

}
