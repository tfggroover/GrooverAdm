import { TestBed } from '@angular/core/testing';

import { PlaceService } from './place.service';
import { PlaceClient } from 'src/app/services/services';

describe('PlaceServiceService', () => {
  let service: PlaceService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PlaceClient]
    });
    service = TestBed.get(PlaceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
