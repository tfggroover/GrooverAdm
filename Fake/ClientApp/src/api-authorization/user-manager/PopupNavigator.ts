// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


import { PopupWindow } from './PopupWindow';
import { Log } from 'oidc-client';

export class PopupNavigator {

    prepare(params) {
        const popup = new PopupWindow(params);
        return Promise.resolve(popup);
    }

    callback(url: string, keepOpen: boolean, delimiter: string) {
        Log.debug('PopupNavigator.callback');

        try {
            PopupWindow.notifyOpener(url, keepOpen, delimiter);
            return Promise.resolve();
        } catch (e) {
            return Promise.reject(e);
        }
    }
}
