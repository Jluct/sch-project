var TelephoneViewModel = function (accountId, service, serviceParams)
{
    var self = this;

    self.telephone = ko.observable(serviceParams.getParameterValue("Telephone", ""));
    self.sum = ko.observable(0);
    self.maskDefs = ko.observable({});
    self.telCodes = ko.observableArray(serviceParams.getKeys().map(function (k) { return parseInt(k); }).filter(function (k) { return k > 0; }));
    self.code = ko.observable(serviceParams.getParameterValue("Code", self.telCodes().length > 0 ? self.telCodes()[0] : undefined));
    self.telephoneMask = ko.computed(function ()
    {
        var mask = serviceParams.getParameterValue(self.code(), "");
        if (mask.indexOf('[') >= 0)
        {
            var def = mask.substring(mask.indexOf('['), mask.indexOf(']') + 1);
            mask = mask.replace(def, 'R');
            self.maskDefs({ R: def });
        }
        return mask;
    });

    self.onSumbit = function ()
    {
        alert('submit');
    };

    self.getParameters = function ()
    {
        var paramsManager = new ServiceParametersManager();
        paramsManager.addParameter("Telephone", window.ServiceParameterTypes.string, self.telephone());
        paramsManager.addParameter("Code", window.ServiceParameterTypes.string, self.code());
        return paramsManager;
    };

    self.getDescription = function ()
    {
        return "Номер телефона: " + self.code() + self.telephone();
    };

    return self;
};