// get: url, done, error, data
// postJson: url, data, done, error, dontDecodeResponse
window.ajax = {
    types: {
        cnt: 'Content-Type',
        json: 'application/json;charset=UTF-8',
        text: 'text/plain;charset=UTF-8'
    },
    headers: {
        xrw: 'X-Requested-With',
        xhr: 'XMLHttpRequest'
    },
    methods: {
        get: 'GET',
        post: 'POST'
    },
    serializers: {
        json: window.JSON,
        urlEncoder: window.jQuery.param
    },
    _xhr: XMLHttpRequest,
    _date: Date,
    postJson: function (url, data, done, error, dontDecode) {
        var r = new this._xhr;
        r.open(this.methods.post, url);
        r.overrideMimeType && r.overrideMimeType(this.types.text);
        r.setRequestHeader(this.types.cnt, this.types.json);
        r.setRequestHeader(this.headers.xrw, this.headers.xhr);
        if (done || error)
            r.onreadystatechange = function () {
                if (this.readyState == 4) {
                    this.onreadystatechange = null;
                    if (this.status == 200 && done) {
                        var t = this.response || this.responseText;
                        done(dontDecode ? t : JSON.parse(t));
                    }
                    else if (error)
                        error(this.status);
                }
            };
        r.send(this.serializers.json.stringify(data));
        return r;
    },
    get: function (url, done, error, data) {
        var r = new this._xhr,
            t = this._date.now();
        data ? data._ = t : (data = { _: t });
        r.open(this.methods.get, url + '?' + this.serializers.urlEncoder(data));
        r.overrideMimeType && r.overrideMimeType(this.types.text);
        r.onreadystatechange = function () {
            if (this.readyState == 4) {
                this.onreadystatechange = null;
                if (this.status == 200 && done)
                    done(this.response || this.responseText);
                else if (error)
                    error(this.status);
            }
        };
        r.send(null);
        return r;
    }
};
Object.freeze && Object.freeze(window.ajax);

window.Service = (function () {
    var contexts = {}, globalOptions = {},
        ajaxOptions = { type: 'POST', contentType: 'application/json', async: true },
        eventsList = ['done', 'fail', 'always', 'afterSend'];

    function invoke(url, data, serviceOptions) {
        // Ensure data
        data = data || {};

        // Ensure options exist
        serviceOptions = serviceOptions || {};

        // Get context
        var context = serviceOptions.context, request = contexts[context], stateHandler = serviceOptions.stateHandler;

        // Abort last request
        if (request) {
            request.abort();
        }

        // Raise after send event
        setTimeout(function () {
            request.afterSend._invoke(request);
        }, 0);

        var initializeRequest = function (xhr) {
            xhr.url = url;
            xhr.data = data;
        };

        // Perform request
        request = $.ajax({ type: this.type, url: url, data: JSON.stringify(data), contentType: this.contentType, async: this.async, beforeSend: initializeRequest });
        request.afterSend = new invocationCollection();

        if (stateHandler) {
            stateHandler.enter();
            request.always(function () {
                stateHandler.leave();
            });
        }

        // Check if context was specified
        if (context) {
            // Add context
            contexts[context] = request.always(function () {
                // Remove context
                delete contexts[context];
            });
        }

        // Extend request
        extendRequest(request, globalOptions);
        extendRequest(request, serviceOptions);

        // Return current request for configuration purposes
        return request;
    }

    function extendRequest(request, options) {
        for (var index = 0, length = eventsList.length; index < length; ++index) {
            // Get event name
            var name = eventsList[index];

            // Check if item exist
            if (options[name] == null) {
                continue;
            }

            // Subscribe
            $.map([].concat(options[name]), function (callback) {
                request[name].call(request, callback);
            });
        }
    }

    function addSubscription(event, callback) {
        var collection = globalOptions[event];
        if (collection == null) {
            globalOptions[event] = collection = [];
        }
        collection.push(callback);
        return new DisposableObject(function () {
            Utils.array.remove(collection, callback);
        });
    }

    function register(path, url, options) {
        Utils.extend(Service, path, invoke.bind($.extend({}, ajaxOptions, options), url), false);
    }

    function abort(items) {
        items = [].concat(items);

        for (var index = items.length - 1, item; index >= 0; --index) {
            if (contexts[item = items[index]] != null) {
                contexts[item].abort();
            }
        }
    }

    function invocationCollection() {
        var listeners = [];

        function subscribe(callback) {
            // Subscribe
            listeners.push(callback);

            // Fluent interface
            return this;
        }

        // Invokation interface
        //obfuscator fix
        subscribe._invoke = Utils['function'].invoke.bind(null, listeners);

        // Return subscribing interface
        return subscribe;
    }

    return { register: register, abort: abort, subscribe: addSubscription };
})();

window.ServiceMonitor = new function () {

    var thisRef = this, monitors = {};

    thisRef.listen = function () {
        var guid = GuidGenerator.create(), invocations = [];

        var subscription = Service.subscribe('afterSend', function () {
            invocations.push({ url: this.url, data: this.data });
        });

        monitors[guid] = { invocations: invocations, subscription: subscription };

        return guid;
    };

    thisRef.release = function (guid) {
        var monitor = monitors[guid];
        if (monitor == null) {
            return [];
        }

        monitor.subscription.dispose();
        delete monitors[guid];
        return monitor.invocations;
    };
};