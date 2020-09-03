import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PlaceFilterComponent } from './place-filter.component';
import { PlaceSearchStatusService } from 'src/app/places/services/place.service';

describe('PlaceFilterComponent', () => {
  let component: PlaceFilterComponent;
  let fixture: ComponentFixture<PlaceFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PlaceFilterComponent ],
      providers: [PlaceSearchStatusService]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PlaceFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
