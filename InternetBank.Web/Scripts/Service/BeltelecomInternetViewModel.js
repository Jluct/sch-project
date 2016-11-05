var BeltelecomInternetViewModel = function (accountId, service, serviceParams)
{
    var self = this;

    self.number = ko.observable(serviceParams.getParameterValue("Number", ""));
    self.sum = ko.observable(0);
    self.numberMask = ko.observable(serviceParams.getParameterValue("Mask"));

    self.onSumbit = function ()
    {
        alert('submit');
    };

    self.getParameters = function ()
    {
        var paramsManager = new ServiceParametersManager();
        paramsManager.addParameter("Number", window.ServiceParameterTypes.string, self.number());
        return paramsManager;
    };

    self.getDescription = function ()
    {
        return "Лицевой счет: " + self.number();
    };

    return self;
};