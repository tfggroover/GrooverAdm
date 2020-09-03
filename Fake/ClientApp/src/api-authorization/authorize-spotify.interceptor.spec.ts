import { TestBed } from '@angular/core/testing';

import { AuthorizeSpotifyInterceptor } from './authorize-spotify.interceptor';

describe('AuthorizeSpotifyInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [
      AuthorizeSpotifyInterceptor
      ]
  }));

  it('should be created', () => {
    const interceptor: AuthorizeSpotifyInterceptor = TestBed.inject(AuthorizeSpotifyInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
