import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlacesComponent } from './places/places.component';



@NgModule({
  declarations: [PlacesComponent],
  imports: [
    CommonModule
  ],
  exports: [
    PlacesComponent
  ]
})
export class PlacesModule { }
