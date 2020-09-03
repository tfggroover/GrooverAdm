import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { Marker } from '@agm/core';
import { Location, Appearance, MatGoogleMapsAutocompleteComponent } from '@angular-material-extensions/google-maps-autocomplete';
import PlaceResult = google.maps.places.PlaceResult;
import { Place, User, Geolocation, DayOfWeek, Timetable, Schedule, Playlist } from 'src/app/services/services';
import { AuthorizeService } from 'src/api-authorization/authorize.service';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray, ValidationErrors } from '@angular/forms';

export class Day {
  number: number;
  text: string;
}

@Component({
  selector: 'app-place-form',
  templateUrl: './place-form.component.html',
  styleUrls: ['./place-form.component.scss']
})
export class PlaceFormComponent implements OnInit {

  appearance = Appearance;
  days: Day[] = [];
  _place: Place;
  forceAutocomplete: boolean = false;

  @Input()
  public set place(value: Place) {
    if (!!value && !!value.mainPlaylist && !(new RegExp('spotify:playlist:\\w+').test(value.mainPlaylist.id))) {
      value.mainPlaylist.id = 'spotify:playlist:' + value.mainPlaylist.id;
    }
    this.buildForm(value);
    if (value) {
      this.marker = {lat: value.location.latitude, lng: value.location.longitude, place: true};
    }
    this._place = value;
  }
  @ViewChild('autocomplete') autocomplete: MatGoogleMapsAutocompleteComponent;

  public get place(): Place {
    return this._place;
  }

  @Output()
  public placeUpdateListener: EventEmitter<Place> = new EventEmitter();

  public userLocation: AgmMarker = {lat: 0, lng: 0 };
  public marker: AgmMarker = {lat: 0, lng: 0, place: false};
  public zoom: number;
  public user: User;
  public form: FormGroup;

  constructor(private authService: AuthorizeService, private fb: FormBuilder) {
    this.authService.userDataChanged.subscribe(u => this.user = u);
    for (const key in DayOfWeek) {
      if (Object.prototype.hasOwnProperty.call(DayOfWeek, key)) {
        const value = DayOfWeek[key];
        if (typeof value === 'number') {
          this.days.push({ number: +value, text: key });
        }

      }
    }
    this.buildForm();
  }

  ngOnInit(): void {
    if ('geolocation' in navigator) {
      navigator.geolocation.getCurrentPosition(s => {
        this.userLocation = {lat: s.coords.latitude, lng: s.coords.longitude };
        if (!this.marker.place) {
          this.marker = { lat: this.userLocation.lat, lng: this.userLocation.lng , place: false};
        }
      });
    }
    this.zoom = 13;
  }

  forceAuto() {
    this.forceAutocomplete = true;
  }

  public buildForm(data?: Place) {
    let timetablesInitial = [];
    if (!!data && !!data.timetables && data.timetables.length > 0) {
      timetablesInitial = data.timetables.map(t => {
        let schedulesInitial = [];
        if (!!t.schedules && t.schedules.length > 0) {
          schedulesInitial = t.schedules.map(s => this.fb.group({
            start: this.fb.control(s.start, [Validators.required]),
            end: this.fb.control(s.end, [Validators.required])
          }));
        }
        this.fb.group({
          dayOfWeek: this.fb.control(t.day, [Validators.required]),
          schedules: this.fb.array(schedulesInitial)
        });
      });
    }

    this.form = this.fb.group({
      name: this.fb.control(data?.displayName || '', [Validators.required]),
      phone: this.fb.control(data?.phone || ''),
      address: this.fb.control(data?.address || '', [Validators.required]),
      latitude: this.fb.control(data?.location?.latitude || '', [Validators.required, Validators.min(-90), Validators.max(90)]),
      longitude: this.fb.control(data?.location?.longitude || '', [Validators.required, Validators.min(-180), Validators.max(180)]),
      approved: this.fb.control(data?.approved || false),
      reviewComment: this.fb.control(data?.reviewComment || ''),
      timetables: this.fb.array(timetablesInitial),
      playlist: this.fb.control(data?.mainPlaylist?.id || '', [Validators.required, Validators.pattern('spotify:playlist:\\w+')])
    });
  }

  public onSubmit() {
    if (this.form.invalid) {
      return;
    }
    this.place = new Place({
      address: this.form.value.address,
      approved: false,
      displayName: this.form.value.name,
      location: new Geolocation({
        latitude: this.form.value.latitude,
        longitude: this.form.value.longitude
      }),
      mainPlaylist: new Playlist({
        id: this.form.value.playlist
      }),
      pendingReview: true,
      phone: this.form.value.phone,
      timetables: this.form.value.timetables.map(t =>
        new Timetable({
          day: t.dayOfWeek.number,
          schedules: t.schedules.map(s =>
            new Schedule({
              start: this.setDateTime(new Date(), s.start),
              end: this.setDateTime(new Date(), s.end)
            }))
        })
      )
    });
    this.placeUpdateListener.next(this.place);
  }

  public updatePlace() {
    this.form.controls['latitude'].setValue(this.marker.lat);
    this.form.controls['longitude'].setValue(this.marker.lng);
  }

  public changeMarker(lat: number, lng: number) {
    this.marker.lat = lat;
    this.marker.lng = lng;
    this.marker.place = true;
    this.updatePlace();
  }

  displayFn(day: Day): string {
    return day && day.text ? day.text : '';
  }

  public addRow() {

    const control = <FormArray>this.form.controls['timetables'];
    const timetable = this.fb.group({
      dayOfWeek: this.fb.control('', [Validators.required]),
      schedules: this.fb.array([])
    });
    control.push(timetable);
    this.addSchedule(timetable);
  }

  public addSchedule(timetable: FormGroup) {
    const control = <FormArray>timetable.controls['schedules'];

    control.push(this.fb.group({
      start: this.fb.control('', [Validators.required]),
      end: this.fb.control('', [Validators.required])
    }));
  }

  onAutocompleteSelected(result: PlaceResult) {
    console.log('onAutocompleteSelected: ', result);
    this.form.controls['address'].setValue(result.formatted_address);
  }

  onLocationSelected(location: Location) {
    console.log('onLocationSelected: ', location);
    this.changeMarker(location.latitude, location.longitude);
  }

  public onTimeChanged(schedule: Schedule, start: boolean, value: string) {
    if (start) {
      schedule.start = this.setDateTime(new Date(), value);
    } else {
      schedule.end = this.setDateTime(new Date(), value);
    }
  }

  private setDateTime(date: Date, time: string) {
    const index = time.indexOf('.'); // replace with ":" for differently displayed time.
    const index2 = time.indexOf(' ');

    let hours = time.substring(0, index);
    const minutes = time.substring(index + 1, index2);

    const mer = time.substring(index2 + 1, time.length);
    if (mer == 'PM') {
      hours = hours + 12;
    }


    date.setHours(+hours);
    date.setMinutes(+minutes);
    date.setSeconds(0);

    return date;
  }



  //timetablesValidator(control: FormArray): ValidationErrors | null {
  //  const values: FormGroup[] = control.value;
  //  return values.map(v => v['dayOfWeek']).some((v, i) => values.map(z => z['dayOfWeek']).indexOf(v) !== i) ? { duplicateDay: true } : null;
  //}

  public hasError = (controlName: string[], errorName: string) => {
    let reference: any = this.form;
    controlName.forEach(n => reference = reference.controls[n]);
    return reference.hasError(errorName);
  }

  removeSchedule(schedule: number, timetable: FormGroup) {
    (<FormArray>timetable.controls['schedules']).removeAt(schedule);
  }

  removeTimetable(timetable: number) {
    (<FormArray>this.form.controls['timetables']).removeAt(timetable);
  }


}

interface AgmMarker {
  lat?: number;
  lng?: number;
  icn?: string;
  place?: boolean;
}
