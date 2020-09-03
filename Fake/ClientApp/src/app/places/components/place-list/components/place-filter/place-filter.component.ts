import { Component, OnInit } from '@angular/core';
import { PlaceSearchStatusService } from 'src/app/places/services/place.service';

@Component({
  selector: 'app-place-filter',
  templateUrl: './place-filter.component.html',
  styleUrls: ['./place-filter.component.scss']
})
export class PlaceFilterComponent implements OnInit {

  public pendingReview: boolean = true;
  public owner: boolean = true;

  constructor(private placeSearchStatusService: PlaceSearchStatusService) { }

  ngOnInit(): void {
  }


  public reset() {
    this.pendingReview = true;
    this.owner = true;
    this.placeSearchStatusService.reset();
  }

  public doSearch() {
    this.placeSearchStatusService.setFilter(this.owner, this.pendingReview);
  }

  public loadMore() {
    this.placeSearchStatusService.loadMore();
  }

}
