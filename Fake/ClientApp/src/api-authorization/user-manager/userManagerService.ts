// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// Adapted for the purposes of this project

import { BehaviorSubject, Observable, of, from, Subscription, Subject } from 'rxjs';
import { Injectable, Inject, EventEmitter } from '@angular/core';
import { UserStorageService } from './userStorageService';
import { AuthenticationResponse, HomeClient, User, IIAuthResponse } from 'src/app/services/services';
import { AngularFireAuth } from '@angular/fire/auth';
import { Log, CordovaPopupNavigator } from 'oidc-client';
import { map } from 'rxjs/operators';
import { storage } from 'firebase';
import { IFrameNavigator } from './IFrameNavigator';
import { PopupNavigator } from './PopupNavigator';


export class AuthUser {
  constructor(name: string, auth: IIAuthResponse) {
    this.name = name;
    this.spotifyAuthentication = auth;
  }
  public name?: string;
  public spotifyAuthentication: IIAuthResponse;
}

export class CompleteUser extends AuthUser {

  constructor(name: string, auth: IIAuthResponse, fire: firebase.User) {
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
    this.closedPopup = this.closedEvent.asObservable();
  }

  private authUrl: string;
  private iframeNavigator: IFrameNavigator;
  private popupNavigator: PopupNavigator;
  private timer;
  private closedEvent: Subject<any> = new Subject();
  public closedPopup: Observable<any>;

  get _popupNavigator() {
    return this.popupNavigator;
  }
  get _iframeNavigator() {
    return this.iframeNavigator;
  }
  get _userStore() {
    return this.userStore;
  }

  public isFirebaseLoggedIn(): Promise<boolean> {
    return Promise.resolve(!!this.fireAuth.auth.currentUser);
  }

  public isSpotifyTokenStored(): Promise<Boolean> {
    return this._loadUser().toPromise().then(user => !!user);
  }

  public saveUser(user: User): AuthUser {
    const currentUser = new AuthUser(this.fireAuth.auth.currentUser.displayName,
      { access_token: user.currentToken, token_type: 'code', expires_in: user.expiresIn, refresh_Token: user.refreshToken });
    this.storeUser(currentUser);
    return currentUser;
  }

  public getUser(): Promise<CompleteUser> {
    return this._loadUser().toPromise().then(async user => {
      if (user) {
        Log.info('UserManager.getUser: user loaded');
        const fireUser = this.fireAuth.auth.currentUser;
        if (!!fireUser) {
          const result = new CompleteUser(fireUser.displayName, user.spotifyAuthentication, fireUser);
          return result;
        }
        return null;
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

  private poll(window: Window) {
    if (window.closed) {
      clearInterval(this.timer);
      this.closedEvent.next(true);
    }
  }


  public signinPopup(args?: any) {
    args = Object.assign({}, args);

    args.request_type = 'si:p';
    const url = this.authUrl;
    if (!url) {
      Log.error('UserManager.signinPopup: No popup_redirect_uri or redirect_uri configured');
      return from(Promise.reject(new Error('No popup_redirect_uri or redirect_uri configured')));
    }
    const win = window.open(url, 'firebaseAuth', 'height=315,width=400');
    this.timer = setInterval(() => this.poll(win), 500);
    return;

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

  signinSilent(): Observable<CompleteUser | null> {

    // first determine if we have a refresh token, or need to use iframe
    return from(this._loadUser().toPromise().then(async user => {
      if (!!user?.spotifyAuthentication?.refresh_Token) {
        const authorizationResponse = await this.homeClient.auth(user.spotifyAuthentication.refresh_Token).toPromise();
        const fireUser = await this.fireAuth.auth.signInWithCustomToken(authorizationResponse.firebase);
        const result = new CompleteUser(fireUser.user.displayName, authorizationResponse.spotify, fireUser.user);
        return result;
      } else {
        throw Error('No token to refresh');
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

  _loadUser(): Observable<AuthUser> {
    return of(this._userStore.getStorageToken());
  }

  storeUser(user: AuthUser): Promise<void> {
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
