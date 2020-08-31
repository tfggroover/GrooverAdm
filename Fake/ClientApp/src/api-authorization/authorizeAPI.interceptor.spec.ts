import { TestBed, inject } from '@angular/core/testing';

import { AuthorizeAPIInterceptor } from './authorizeAPI.interceptor';

describe('AuthorizeInterceptor', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AuthorizeAPIInterceptor]
    });
  });

  it('should be created', inject([AuthorizeAPIInterceptor], (service: AuthorizeAPIInterceptor) => {
    expect(service).toBeTruthy();
  }));
});
