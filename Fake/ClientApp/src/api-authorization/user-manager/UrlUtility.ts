// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

import { Global } from './Global.js';
import { Log } from 'oidc-client';

export class UrlUtility {
    static addQueryParam(url, name, value) {
        if (url.indexOf('?') < 0) {
            url += '?';
        }

        if (url[url.length - 1] !== '?') {
            url += '&';
        }

        url += encodeURIComponent(name);
        url += '=';
        url += encodeURIComponent(value);

        return url;
    }

    static parseUrlFragment(value: string, delimiter = '#', global = Global): any {
        if (typeof value !== 'string') {
            value = global.location.href;
        }

        let idx = value.lastIndexOf(delimiter);
        if (idx >= 0) {
            value = value.substr(idx + 1);
        }

        if (delimiter === '?') {
            // if we're doing query, then strip off hash fragment before we parse
            idx = value.indexOf('#');
            if (idx >= 0) {
                value = value.substr(0, idx);
            }
        }

        const params = {},
            regex = /([^&=]+)=([^&]*)/g;
        let m;

        let counter = 0;
        while (m = regex.exec(value)) {
            params[decodeURIComponent(m[1])] = decodeURIComponent(m[2].replace(/\+/g, ' '));
            if (counter++ > 50) {
                Log.error('UrlUtility.parseUrlFragment: response exceeded expected number of parameters', value);
                return {
                    error: 'Response exceeded expected number of parameters'
                };
            }
        }

        for (let prop in params) {
            return params;
        }

        return {};
    }
}
