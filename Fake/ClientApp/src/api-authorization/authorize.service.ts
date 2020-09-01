import { Injectable } from '@angular/core';
import { WebStorageStateStore } from 'oidc-client';
import { BehaviorSubject, concat, from, Observable } from 'rxjs';
import { filter, map, mergeMap, take, tap } from 'rxjs/operators';
import { ApplicationPaths, ApplicationName } from './api-authorization.constants';
import { AngularFireAuth } from '@angular/fire/auth';
import { AuthUser, UserManager, CompleteUser } from './user-manager/userManagerService';
import { UserClient } from 'src/app/services/services';

export type IAuthenticationResult =
  SuccessAuthenticationResult |
  FailureAuthenticationResult |
  RedirectAuthenticationResult;

export interface SuccessAuthenticationResult {
  status: AuthenticationResultStatus.Success;
  state: any;
}

export interface FailureAuthenticationResult {
  status: AuthenticationResultStatus.Fail;
  message: string;
}

export interface RedirectAuthenticationResult {
  status: AuthenticationResultStatus.Redirect;
}

export enum AuthenticationResultStatus {
  Success,
  Redirect,
  Fail
}


@Injectable({
  providedIn: 'root'
})
export class AuthorizeService {
  // By default pop ups are disabled because they don't work properly on Edge.
  // If you want to enable pop up authentication simply set this flag to false.


  private popUpDisabled = true;
  private userSubject: BehaviorSubject<AuthUser | null> = new BehaviorSubject(null);
  public userChanged: Observable<AuthUser>;

  constructor(public auth: AngularFireAuth, private userManager: UserManager, private userClient: UserClient) {
    this.userChanged = this.userSubject.asObservable();
   }
  public isAuthenticated(): Observable<boolean> {
    return this.getUser().pipe(map(u => !!u));
  }

  public getUser(): Observable<AuthUser | null> {
    return concat(
      this.userSubject.pipe(take(1), filter(u => !!u)),
      this.getUserFromStorage().pipe(filter(u => !!u), tap(u => this.userSubject.next(u))),
      this.userSubject.asObservable());
  }

  public getFirebaseAccessToken(): Observable<string> {
    return from(this.ensureUserManagerInitialized())
      .pipe(mergeMap(() => from(this.userManager.getUser())),
        mergeMap(user => user && user.firebaseAuthentication.getIdToken()));
  }

  public getSpotifyAccessToken(): Observable<string> {
    return from(this.ensureUserManagerInitialized())
      .pipe(mergeMap(() => from(this.userManager.getUser())),
        map(user => user && user.spotifyAuthentication.access_token));
  }

  public setCurrentUser() {
    this.userClient.getCurrentUser().subscribe(user => {
      const current = this.userManager.saveUser(user);
      this.userSubject.next(current);
    });
  }

  public async trySignInSilent() {
    await this.ensureUserManagerInitialized();
    let user: CompleteUser = null;
    try {
      user = await this.userManager.signinSilent().toPromise();
      this.userSubject.next(user);
    } catch (silentError) {
      // User might not be authenticated, fallback to popup authentication
      console.log('Silent authentication error: No token to refresh');
    }
  }

  // We try to authenticate the user in three different ways:
  // 1) We try to see if we can authenticate the user silently.
  // 2) We try to authenticate the user using a PopUp Window. This might fail if there is a
  //    Pop-Up blocker or the user has disabled PopUps.
  public async signIn(state?: any): Promise<IAuthenticationResult> {
    await this.ensureUserManagerInitialized();
    let user: CompleteUser = null;
    try {
      user = await this.userManager.signinSilent().toPromise();
      this.userSubject.next(user);
      return this.success(state);
    } catch (silentError) {
      // User might not be authenticated, fallback to popup authentication
      console.log('Silent authentication error: No token to refresh');

      try {
        this.userManager.signinPopup();
        this.userManager.closedPopup.subscribe(() => {
          this.setCurrentUser();
          return this.success(state);
        });
      } catch (popupError) {
        if (popupError.message === 'Popup window closed') {
          // The user explicitly cancelled the login action by closing an opened popup.
          return this.error('The user closed the window.');
        } else if (!this.popUpDisabled) {
          console.log('Popup authentication error: ', popupError);
        }
      }
    }
  }

  public async completeSignIn(url: string): Promise<IAuthenticationResult> {
    try {
      await this.ensureUserManagerInitialized();
      //const user = await this.userManager.signinCallback(url);
      this.userSubject.next(null);
      return this.success(null);
    } catch (error) {
      console.log('There was an error signing in: ', error);
      return this.error('There was an error signing in.');
    }
  }

  public async signOut(state: any): Promise<IAuthenticationResult> {
    try {
      if (this.popUpDisabled) {
        throw new Error('Popup disabled. Change \'authorize.service.ts:AuthorizeService.popupDisabled\' to false to enable it.');
      }

      await this.ensureUserManagerInitialized();
      //this.userManager.signOut();

      this.userSubject.next(null);
      return this.success(state);
    } catch (popupSignOutError) {
      console.log('Popup signout error: ', popupSignOutError);
      try {
        return this.redirect();
      } catch (redirectSignOutError) {
        console.log('Redirect signout error: ', popupSignOutError);
        return this.error(redirectSignOutError);
      }
    }
  }

  public async completeSignOut(url: string): Promise<IAuthenticationResult> {
    await this.ensureUserManagerInitialized();
    try {
      //const response = await this.userManager.signoutCallback(url);
      this.userSubject.next(null);
      return this.success(null);
    } catch (error) {
      console.log(`There was an error trying to log out '${error}'.`);
      return this.error(error);
    }
  }

  private createArguments(state?: any): any {
    return { useReplaceToNavigate: true, data: state };
  }

  private error(message: string): IAuthenticationResult {
    return { status: AuthenticationResultStatus.Fail, message };
  }

  private success(state: any): IAuthenticationResult {
    return { status: AuthenticationResultStatus.Success, state };
  }

  private redirect(): IAuthenticationResult {
    return { status: AuthenticationResultStatus.Redirect };
  }

  private async ensureUserManagerInitialized(): Promise<void> {
    if (this.userManager !== undefined) {
      return;
    }

    this.userSubject.next(null);
  }

  private getUserFromStorage(): Observable<AuthUser> {
    return from(this.ensureUserManagerInitialized())
      .pipe(
        mergeMap(() => this.userManager.getUser()),
        map(u => u));
  }
}
