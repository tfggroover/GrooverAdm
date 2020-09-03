import { Injectable, Inject } from '@angular/core';
import { Observable ,  BehaviorSubject } from 'rxjs';
import { HttpXhrBackend } from '@angular/common/http';
import { GrooverXHRBackend } from './grooverHttpXhrBackend';

@Injectable({
    providedIn: 'root'
})
export class LoadingService {
    private readonly debounceDesactivateTime: number = 1000;
    public readonly loadingStatus: Observable<boolean>;
    public _loadingStatus: BehaviorSubject<boolean>
        = new BehaviorSubject(false);
    private isActive: boolean = false;
    private activeLoadings: string[] = [];

    constructor(
        @Inject(HttpXhrBackend) httpXHRBackend: GrooverXHRBackend
    ) {
        this.loadingStatus = this._loadingStatus.asObservable();

        httpXHRBackend.onError.subscribe(e => this.deactivate());
    }

    public activate(identifier: string = null) {
        if (!identifier) {
            this.internalActivate();
            return;
        }

        const alreadyActive = this.activeLoadings.some(al => al === identifier);
        if (alreadyActive) {
            return;
        }

        this.activeLoadings.push(identifier);
        this.internalActivate();
    }

    private internalActivate() {
        this.isActive = true;
        this.notifySubscribers(this.isActive);
    }

    public deactivate(identifier: string = null, debounce: boolean = true) {
        if (!identifier) {
            this.debounceDesactivate();
            return;
        }

        const index = this.activeLoadings.indexOf(identifier);
        if (index === -1) {
            return;
        }

        this.activeLoadings.splice(index, 1);
        if (debounce) {
            this.debounceDesactivate();
        } else {
            this.noDebounceDesactivate();
        }
    }

    public deactivateAll(debounce: boolean = true) {
      for (const item of this.activeLoadings) {
        this.deactivate(item, debounce);
      }
    }

    private noDebounceDesactivate() {
        this.isActive = this.activeLoadings.length !== 0;
        this.notifySubscribers(this.isActive);
    }

    private debounceDesactivate() {
        setTimeout(() => {
            this.noDebounceDesactivate();
        }, this.debounceDesactivateTime);
    }

    private notifySubscribers(current: boolean) {
        this._loadingStatus.next(current);
    }
}
