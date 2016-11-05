var ElectricServiceViewModel = function (accountId, service, serviceParams)
{
    var self = this;

    self.number = ko.observable(serviceParams.getParameterValue("Number", ""));
    self.numberMask = ko.observable(serviceParams.getParameterValue("Mask"));

    self.prevMeterValue = ko.observable(serviceParams.getParameterValue("PrevMeterValue", 0));
    self.curMeterValue = ko.observable(0);

    var normals = serviceParams.getKeys().map(function (k) { return parseInt(k); }).filter(function (k) { return k > 0; }).sort();
    self.diff = ko.computed(function ()
    {
        var diff = self.curMeterValue() - self.prevMeterValue();
        return diff > 0 && self.curMeterValue() > 0 ? diff : 0;
    });
    self.tariff = ko.computed(function ()
    {
        var prev = 0;
        for (var i = 0; i < normals.length; i++)
        {
            if (prev < self.diff() && self.diff() <= normals[i])
            {
                return serviceParams.getParameterValue(normals[i], 0);
            }
            prev = normals[i];
        }
        return 0;
    });
    self.sum = ko.computed(function ()
    {
        return self.diff() * self.tariff();
    });

    self.onSumbit = function ()
    {
        alert('submit');
    };

    self.getParameters = function ()
    {
        var paramsManager = new ServiceParametersManager();
        paramsManager.addParameter("Number", window.ServiceParameterTypes.string, self.number());
        paramsManager.addParameter("PrevMeterValue", window.ServiceParameterTypes.int, self.curMeterValue());
        return paramsManager;
    };

    self.getDescription = function ()
    {
        return "Лицевой счет: " + self.number() + ", Предыдущее значение: " + self.prevMeterValue() + ", Текущее: " + self.curMeterValue();
    };

    return self;
};