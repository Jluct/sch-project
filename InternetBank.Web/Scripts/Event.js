var Event = function ()
{
    var self = this;
    var callbacks = {};

    self.subscribe = function (callback, params)
    {
        if (!callbacks[callback])
        {
            callbacks[callback] = callback;
        }
    };

    self.unsubscribe = function (callback)
    {
        if (callbacks[callback])
            delete callbacks[callback];
    };

    self.invoke = function ()
    {
        var results = [];
        var args = [];
        for (var i = 0; i < arguments.length; i++)
            args.push(arguments[i]);
        for (var callbackId in callbacks)
        {
            var res = callbacks[callbackId].apply(window, args);
            if (res !== undefined)
                results.push(res);
        }
        return results;
    };

    return self;
};