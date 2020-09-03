import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthorizeService } from './authorize.service';
import { mergeMap } from 'rxjs/operators';
import { UserClient } from 'src/app/services/services';

@Injectable()
export class AuthorizeSpotifyInterceptor implements HttpInterceptor {

  constructor(private authorize: AuthorizeService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    if (this.isToSpotify(request)) {
      return this.authorize.getSpotifyAccessToken()
      .pipe(mergeMap(token => this.processRequestWithToken(token, request, next)));
    }
      return next.handle(request);

  }

  private processRequestWithToken(token: string, req: HttpRequest<any>, next: HttpHandler) {
    if (!!token && this.isToSpotify(req)) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }


    return next.handle(req);
  }

  private isToSpotify(req: HttpRequest<any>) {
    // It's an absolute url with the same origin.
    if (req.url.startsWith(`https://api.spotify.com/`)) {
      return true;
    }

    // It's an absolute or protocol relative url that
    // doesn't have the same origin.
    return false;
  }
}
