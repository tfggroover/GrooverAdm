// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// Adapted for the purposes of this project

import { BehaviorSubject, Observable, of, from } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { UserStorageService } from './userStorageService';
import { AuthenticationResponse, HomeClient, IAuthResponse } from 'src/app/services/services';
import { AngularFireAuth } from '@angular/fire/auth';
import { Log, CordovaPopupNavigator } from 'oidc-client';
import { map } from 'rxjs/operators';
import { storage } from 'firebase';
import { IFrameNavigator } from './IFrameNavigator';
import { PopupNavigator } from './PopupNavigator';


export class User {
  constructor(name: string, auth: IAuthResponse) {
    this.name = name;
    this.spotifyAuthentication = auth;
  }
  public name?: string;
  public spotifyAuthentication: IAuthResponse;
}

export class CompleteUser extends User {

  constructor(name: string, auth: IAuthResponse, fire: firebase.User) {
    super(name, auth);
    this.firebaseAuthentication = fire;
  }

  public firebaseAuthentication: firebase.User;

}
@Injectable({
  providedIn: 'root',
  deps: [UserStorageService, AngularFireAuth]
})
export class UserManager {

  constructor(private userStore: UserStorageService,
    private homeClient: HomeClient,
    private fireAuth: AngularFireAuth,
    @Inject('BASE_URL') baseUrl: string) {

    //this.startSilentRenew();

    this.iframeNavigator = new IFrameNavigator();
    this.popupNavigator = new PopupNavigator();
    this.authUrl = baseUrl + 'home/auth';
  }

  private authUrl: string;
  private iframeNavigator: IFrameNavigator;
  private popupNavigator: PopupNavigator;



  get _popupNavigator() {
    return this.popupNavigator;
  }
  get _iframeNavigator() {
    return this.iframeNavigator;
  }
  get _userStore() {
    return this.userStore;
  }



  public getUser(): Promise<CompleteUser> {
    return this._loadUser().toPromise().then(async user => {
      if (user) {
        Log.info('UserManager.getUser: user loaded');
        const fireUser = await this.fireAuth.user.toPromise();
        const result = new CompleteUser(fireUser.displayName, user.spotifyAuthentication, fireUser);
        return result;
      } else {
        const fireUser = this.fireAuth.auth.currentUser;
        if (!!fireUser) {
          Log.info('UserManager.getUser: Spotify info missing, returning user for API');
          return new CompleteUser(fireUser.displayName, null, fireUser);
        }
        Log.info('UserManager.getUser: user not found in storage');
        return null;
      }
    });
  }

  removeUser() {
    return this.storeUser(null);
  }

  public signinPopup(args: any): Promise<CompleteUser> {
    args = Object.assign({}, args);

    args.request_type = 'si:p';
    const url = this.authUrl;
    if (!url) {
      Log.error('UserManager.signinPopup: No popup_redirect_uri or redirect_uri configured');
      return Promise.reject(new Error('No popup_redirect_uri or redirect_uri configured'));
    }

    args.display = 'popup';

    return this._signin(this._popupNavigator, {
      startUrl: url
    }).then(async user => {
      if (user) {
        Log.info('UserManager.signinPopup: signinPopup successful');
      }

      const fireUser = await this.fireAuth.auth.signInWithCustomToken(user.firebase);
      const result = new CompleteUser(fireUser.user.displayName, user.spotify, fireUser.user);
      return result;
    });
  }
  signinPopupCallback(url) {
    return this._signinCallback(url, this._popupNavigator).then(user => {
      if (user) {
        if (user.profile && user.profile.sub) {
          Log.info('UserManager.signinPopupCallback: successful, signed in sub: ', user.profile.sub);
        } else {
          Log.info('UserManager.signinPopupCallback: no sub');
        }
      }

      return user;
    }).catch(err => {
      Log.error('UserManager.signinPopupCallback error: ' + err && err.message);
    });
  }

  signinSilent(args: any): Observable<CompleteUser | null> {

    args.request_type = 'si:s';
    // first determine if we have a refresh token, or need to use iframe
    return from(this._loadUser().toPromise().then(async user => {
      if (!!user?.spotifyAuthentication?.refresh_Token) {
        const authorizationResponse = await this.homeClient.auth(user.spotifyAuthentication.refresh_Token).toPromise();
        const fireUser = await this.fireAuth.auth.signInWithCustomToken(authorizationResponse.firebase);
        const result = new CompleteUser(fireUser.user.displayName, authorizationResponse, fireUser.user);
        return result;
      } else {
        return this._signinSilentIframe();
      }
    }));
  }

  _signinSilentIframe(): Promise<CompleteUser> {
    return this._signin(this._iframeNavigator, {
      startUrl: this.authUrl
    }).then(async user => {
      if (user) {
        Log.info('UserManager.signinSilent: successful, signed in sub: ', user);
      }
      const fireUser = await this.fireAuth.auth.signInWithCustomToken(user.firebase);
      const result = new CompleteUser(fireUser.user.displayName, user.spotify, fireUser.user);

      return result;
    });
  }

  querySessionStatus(args: any) {
    args = Object.assign({}, args);

    args.request_type = 'si:s'; // this acts like a signin silent
    const url = this.authUrl;
    if (!url) {
      Log.error('UserManager.querySessionStatus: No silent_redirect_uri configured');
      return Promise.reject(new Error('No silent_redirect_uri configured'));
    }

    return this._signinStart(this._iframeNavigator, {
      startUrl: url
    }).then(navResponse => {
      return Promise.resolve(new AuthenticationResponse()).then(signinResponse => {
        Log.debug('UserManager.querySessionStatus: got signin response');

        if (signinResponse.firebase) {
          Log.info('UserManager.querySessionStatus');
          return {
            session_state: signinResponse.spotify
          };
        } else {
          Log.info('querySessionStatus successful, user not authenticated');
        }
      })
        .catch(err => {
          throw err;
        });
    });
  }

  _signin(navigator, navigatorParams = {}): Promise<AuthenticationResponse> {
    return this._signinStart(navigator, navigatorParams).then(navResponse => {
      debugger;
      Log.info('singIn start ended');
      return new AuthenticationResponse();
    });
  }
  _signinStart(navigator, navigatorParams: any) {

    return navigator.prepare(navigatorParams).then(handle => {
      Log.debug('UserManager._signinStart: got navigator window handle');

      navigatorParams.url = this.authUrl;
      navigatorParams.id = 'firebaseAuth';

      return handle.navigate(navigatorParams);
    });
  }

  _signinCallback(url, navigator) {
    Log.debug('UserManager._signinCallback');
    return navigator.callback(url);
  }

  //startSilentRenew() {
  //  this._silentRenewService.start();
  //}
//
  //stopSilentRenew() {
  //  this._silentRenewService.stop();
  //}

  _loadUser(): Observable<User> {
    return of(this._userStore.getStorageToken());
  }

  storeUser(user: User): Promise<void> {
    if (user) {
      Log.debug('UserManager.storeUser: storing user');

      this._userStore.setToken(user);
      return Promise.resolve();
    } else {
      Log.debug('storeUser.storeUser: removing user');
      this._userStore.logOut();
      return Promise.resolve();
    }
  }
}
