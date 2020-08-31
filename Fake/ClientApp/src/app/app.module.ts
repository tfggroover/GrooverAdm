import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER, Injector } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { AngularFireModule } from '@angular/fire';
import { AngularFireAuthModule } from '@angular/fire/auth';
import { AuthorizeAPIInterceptor } from 'src/api-authorization/authorizeAPI.interceptor';
import { AgmCoreModule } from '@agm/core';
import { MatGoogleMapsAutocompleteModule } from '@angular-material-extensions/google-maps-autocomplete';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AuthorizeSpotifyInterceptor } from 'src/api-authorization/authorize-spotify.interceptor';

export const firebaseConfig = {
  apiKey: 'AIzaSyASCdn-XXWI2uQmRDvTHJauYN0Qca07-oE',
  authDomain: 'groover-3b82a.firebaseapp.com',
  databaseURL: 'https://groover-3b82a.firebaseio.com',
  projectId: 'groover-3b82a',
  storageBucket: 'groover-3b82a.appspot.com',
  messagingSenderId: '427397483542',
  appId: '1:427397483542:web:9468e5b824c4f97a'
};



@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ApiAuthorizationModule,
    AngularFireModule.initializeApp(firebaseConfig),
    AngularFireAuthModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      {
        path: 'place',
        loadChildren: () => import('./places/places.module').then(m => m.PlacesModule)
      }
    ]),
    MatGoogleMapsAutocompleteModule,
    AgmCoreModule.forRoot(),
    BrowserAnimationsModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizeAPIInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizeSpotifyInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
