import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { PlaceService } from '../../services/place.service';
import { Subscription } from 'rxjs';
import { Place } from 'src/app/services/services';


@Component({
  selector: 'app-place-edit',
  templateUrl: './place-edit.component.html',
  styleUrls: ['./place-edit.component.scss']
})
export class PlaceEditComponent implements OnInit {

  public place: Place;
  public paramsSub: Subscription;
  public placeId: string;

  constructor(public placeService: PlaceService,
    public activatedRoute: ActivatedRoute,
  ) { }

  ngOnInit(): void {
  }

  public paramsSuscripption(): any {
    this.place = new Place();
    this.paramsSub = this.activatedRoute.params.subscribe((params) => {
      const place: string = params['id'];
      this.placeId = place;
      this.loadPlace(place);
    });
  }

  public loadPlace(placeId: string) {
    this.placeService.getPlace(placeId).subscribe(p => this.place = p);
  }
}
