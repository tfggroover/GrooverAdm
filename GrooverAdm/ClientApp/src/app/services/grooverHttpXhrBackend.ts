import {
  HttpXhrBackend, HttpRequest,
  HttpResponse, HttpClientXsrfModule, XhrFactory
} from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';


import { Injectable, Injector, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';


@Injectable()
export class GrooverXHRBackend extends HttpXhrBackend {

  public onError: EventEmitter<any>;

  constructor(
      _xhrStrategy: XhrFactory,
      public injector: Injector
  ) {
      super(_xhrStrategy);
      this.onError = new EventEmitter();
  }

  public handle(request: HttpRequest<any>): any {
      const connection = super.handle(request);

      connection
          .pipe(catchError( (resp, caught) => {
              this.processResponseError(request, resp);

              return of(resp);
          }));

      return connection;
  }

  public processResponseError(request: HttpRequest<any>, response: HttpResponse<any>): Observable<HttpResponse<any>> {
      this.onError.emit();
      if (response.status === 401 || response.status === 403) {

          const redirectService: Router = this.injector.get(Router);

          redirectService.navigate(['/']);
      } else {

      }

      return of(response);
  }
}
