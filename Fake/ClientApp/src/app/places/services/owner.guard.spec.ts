import { TestBed, async, inject } from '@angular/core/testing';

import { OwnerGuard } from './owner.guard';
import { PlaceService } from './place.service';
import { AuthorizeService } from 'src/api-authorization/authorize.service';

describe('OwnerGuard', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [OwnerGuard, PlaceService, AuthorizeService],
      imports: []
    });
  });

  it('should ...', inject([OwnerGuard], (guard: OwnerGuard) => {
    expect(guard).toBeTruthy();
  }));
});
