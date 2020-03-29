import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Place } from 'src/app/places/models/Place';

@Component({
  selector: 'app-place-form',
  templateUrl: './place-form.component.html',
  styleUrls: ['./place-form.component.scss']
})
export class PlaceFormComponent implements OnInit {

  @Input()
  public place: Place = new Place();


  @Output()
  public submit: EventEmitter<Place> = new EventEmitter();

  constructor() {   }

  ngOnInit(): void {
  }

  public onSubmit() {
    this.submit.next(this.place);
  }

}
