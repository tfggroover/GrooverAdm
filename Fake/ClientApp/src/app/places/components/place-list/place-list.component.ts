import { Component, OnInit } from '@angular/core';
import { AngularFirestore } from '@angular/fire/firestore';
import { Observable } from 'rxjs';
import 'firebase/firestore';
import { Router } from '@angular/router';
import { PlaceService, PlaceSearchStatusService, PlaceSearchStatus } from '../../services/place.service';
import { Place } from 'src/app/services/services';
import { PlaceViewModel } from '../../models/PlaceViewModel';
import { AuthorizeService } from 'src/api-authorization/authorize.service';
import { CompleteUser } from 'src/api-authorization/user-manager/userManagerService';

@Component({
  selector: 'app-places',
  templateUrl: './place-list.component.html',
  styleUrls: ['./place-list.component.scss']
})
export class PlaceListComponent implements OnInit {
  items: PlaceViewModel[];
  currentUser: CompleteUser;
  showMoreButton: boolean;
  constructor(private placeSearchStatusService: PlaceSearchStatusService,
    private router: Router, private authService: AuthorizeService) {
      placeSearchStatusService.placeSearchStatus.subscribe(this.processSearchStatus.bind(this));
      placeSearchStatusService.refreshAll();
      authService.getUser().subscribe(u => {
        this.currentUser = u;
        if (!!this.items && this.items.length > 0) {
          this.items = this.items.map(p => {
            p.checkUserOwner(this.currentUser.firebaseAuthentication.uid);
            return p;
          });
        }
      });
   }

  ngOnInit(): void {
  }

  public processSearchStatus(status: PlaceSearchStatus) {
    this.items = status.places.map(p => {
      const res = new PlaceViewModel(p);
      if (!!this.currentUser) {
        res.checkUserOwner(this.currentUser.firebaseAuthentication.uid);
      }
      return res;
    });
    this.showMoreButton = status.moreResults;
  }

  public showMore() {
    this.placeSearchStatusService.loadMore();
  }

}
