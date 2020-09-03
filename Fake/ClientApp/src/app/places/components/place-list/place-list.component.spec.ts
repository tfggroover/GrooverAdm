import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PlaceListComponent } from './place-list.component';

describe('PlacesComponent', () => {
  let component: PlaceListComponent;
  let fixture: ComponentFixture<PlaceListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PlaceListComponent ],
      providers: []
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PlaceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
