import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { PlaceService } from '../../services/place.service';
import { Subscription } from 'rxjs';
import { Place, PlaceReview } from 'src/app/services/services';
import { LoadingService } from 'src/app/services/loadingService';
import { AuthorizeService } from 'src/api-authorization/authorize.service';


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
    private router: Router,
    private loading: LoadingService,
    private auth: AuthorizeService
  ) {
    this.paramsSuscripption();
  }

  ngOnInit(): void {
  }

  public paramsSuscripption(): any {
    this.paramsSub = this.activatedRoute.params.subscribe((params) => {
      const place: string = params['id'];
      this.placeId = place;
      this.loadPlace(place);
    });
  }

  public loadPlace(placeId: string) {
    this.placeService.getPlace(placeId).subscribe(p => this.place = p);
  }

  public updatePlace(place: Place) {
    this.loading.activate('update');
    this.auth.userDataChanged.subscribe(u => {
      place.id = this.placeId;
      if (u.admin) {
        const review = new PlaceReview({
          approved: place.approved,
          reviewComment: place.reviewComment
        });
        this.placeService.reviewPlace(review, this.placeId).subscribe(p => {
          this.loading.deactivate('update');
          if (!!p) {
            this.router.navigate(['/place']);
          }
        });
      } else {
        this.placeService.updatePlace(place).subscribe(p => {
          this.loading.deactivate('update');
          if (!!p) {
            this.router.navigate(['/place']);
          }
        });
      }
    });
  }
}
