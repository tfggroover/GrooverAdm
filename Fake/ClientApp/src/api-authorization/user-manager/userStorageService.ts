import {Injectable} from '@angular/core';
import { User } from './userManagerService';

@Injectable({
    providedIn: 'root'
})
export class UserStorageService {

    public spotifyKey: string = 'access';
    public token: User;

    public getToken(): User {
        return this.token == null ? this.getStorageToken() : this.token;
    }

    public setToken(token: User): void {
        this.token = token;
        this.saveStorageToken();
    }

    public isToken(): boolean {
        return this.token != null || this.getStorageToken() != null;
    }

    public logOut(): void {
        sessionStorage.setItem(this.spotifyKey, null);
        this.token = null;
    }

    public saveStorageToken() {
        sessionStorage.setItem(this.spotifyKey, JSON.stringify(this.token));
    }

    public getStorageToken(): User {
        return JSON.parse(sessionStorage.getItem(this.spotifyKey));
    }
}

export class Token {

    public access_token: string;
    public expires_in: string;
    public token_type: string;

    constructor(token: any) {
        this.access_token = token.access_token;
        this.expires_in = token.expires_in;
        this.token_type = token.token_type;
    }
}
