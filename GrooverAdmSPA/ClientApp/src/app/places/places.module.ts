import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlaceListComponent } from './components/place-list/place-list.component';
import { PlaceAddComponent } from './components/place-add/place-add.component';
import { PlaceEditComponent } from './components/place-edit/place-edit.component';
import { PlaceService } from './services/place.service';
import { PlaceFormComponent } from './components/shared/place-form/place-form.component';
import { FormsModule } from '@angular/forms';



@NgModule({
  declarations: [PlaceListComponent, PlaceAddComponent, PlaceEditComponent, PlaceFormComponent],
  imports: [
    CommonModule,
    FormsModule
  ],
  exports: [
    PlaceListComponent
  ],
  providers: [
    PlaceService
  ]
})
export class PlacesModule { }
