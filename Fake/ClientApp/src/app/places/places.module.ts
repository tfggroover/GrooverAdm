import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlaceListComponent } from './components/place-list/place-list.component';
import { PlaceAddComponent } from './components/place-add/place-add.component';
import { PlaceEditComponent } from './components/place-edit/place-edit.component';
import { PlaceService, PlaceSearchStatusService } from './services/place.service';
import { PlaceFormComponent } from './components/shared/place-form/place-form.component';
import { FormsModule } from '@angular/forms';
import { AgmCoreModule } from '@agm/core';
import { firebaseConfig } from '../app.module';
import { MatGoogleMapsAutocompleteModule } from '@angular-material-extensions/google-maps-autocomplete';
import { PlaceRoutingModule } from './place-routing.module';
import { PlaceClient } from '../services/services';



@NgModule({
  declarations: [PlaceListComponent, PlaceAddComponent, PlaceEditComponent, PlaceFormComponent],
  imports: [
    CommonModule,
    FormsModule,
    AgmCoreModule.forRoot({
      apiKey: firebaseConfig.apiKey
    }),
    MatGoogleMapsAutocompleteModule
  ],
  exports: [
    PlaceListComponent,
    PlaceRoutingModule
  ],
  providers: [
    PlaceService,
    PlaceClient,
    PlaceSearchStatusService
  ]
})
export class PlacesModule { }
