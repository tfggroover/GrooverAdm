// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

import { IFrameWindow } from './IFrameWindow';
import { Log } from 'oidc-client';

export class IFrameNavigator {

    prepare(params) {
        const frame = new IFrameWindow(params);
        return Promise.resolve(frame);
    }

    callback(url) {
        Log.debug('IFrameNavigator.callback');

        try {
            IFrameWindow.notifyParent(url);
            return Promise.resolve();
        } catch (e) {
            return Promise.reject(e);
        }
    }
}
