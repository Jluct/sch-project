var ServiceParametersManager = function (serviceParameters)
{
    var self = this;
    var _serviceParameters = { };

    if (serviceParameters)
    {
        if (serviceParameters.constructor == Array)
        {
            for (var i in serviceParameters)
            {
                var param = serviceParameters[i];
                if (_serviceParameters[param.Name])
                {
                    if (_serviceParameters[param.Name].constructor == Array)
                        _serviceParameters[param.Name].push(param);
                    else
                        _serviceParameters[param.Name] = [_serviceParameters[param.Name], param];
                }
                else
                    _serviceParameters[param.Name] = param;
            }
        }
        else
            _serviceParameters = serviceParameters;
    }

    var convertValue = function (paramInfo)
    {
        switch (paramInfo.ValueType)
        {
            case window.ServiceParameterTypes.string:
                return paramInfo.DefaultValue;
            case window.ServiceParameterTypes.int:
                return parseInt(paramInfo.DefaultValue);
            case window.ServiceParameterTypes.decimal:
                return parseFloat(paramInfo.DefaultValue);
            case window.ServiceParameterTypes.boolean:
                return /^\s*(true|1(.(0)+)?)\s*$/i.test(paramInfo.DefaultValue);
        }
        throw "Not supported parameter type";
    };

    self.getParameterValue = function (paramName, defaultValue)
    {
        return _serviceParameters[paramName] == undefined ? defaultValue : convertValue(_serviceParameters[paramName]);
    };
    
    self.getParameter = function (paramName)
    {
        return _serviceParameters[paramName];
    };

    self.addParameter = function(paramName, valueType, value)
    {
        _serviceParameters[paramName] = { Name: paramName, ValueType: valueType, DefaultValue: value };
    };

    self.getKeys = function ()
    {
        var res = [];
        for (var i in _serviceParameters)
            res.push(i);
        return res;
    };

    self.toDictionary = function ()
    {
        return _serviceParameters;
    };

    return self;
};