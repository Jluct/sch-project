window.MainViewModel = function (resources)
{
    var self = this;

    self.accountList = ko.observableArray([]);
    self.accountId = ko.observable();

    self.operations = ko.observableArray([]);
    self.operationFormTemplate = ko.observable();
    self.operationFormViewModel = ko.observable();
    self.operationName = ko.observable('');

    self.paymentErrorMesssage = ko.observable('');

    var serviceId = 0;

    self.sessionPasswordNumber = ko.observable();
    self.sessionPassword = ko.observable('');

    self.onSubmit = function ()
    {
        self.paymentErrorMesssage('');
        if (!self.operationFormViewModel())
            return;
        var paramsManager = self.operationFormViewModel().getParameters();
        var description = self.operationFormViewModel().getDescription();
        var sum = self.operationFormViewModel().sum();
        window.DataProvider.Pay(
            self.sessionPassword(),
            self.accountId(),
            serviceId,
            paramsManager.toDictionary(),
            sum,
            description,
            function (errors)
            {
                if (errors.length > 0)
                    self.paymentErrorMesssage(errors.join('<BR />'));
                else
                {
                    self.sessionPasswordNumber(undefined);
                    self.operationFormTemplate(undefined);
                    self.operationFormViewModel(undefined);
                    alert('Оплата произведена успешно');
                }
            },
            function ()
            {
                alert('error');
            }
        );
    };

    self.onServiceClick = function (service)
    {
        if (self.accountId() > 0)
        {
            window.DataProvider.GetServiceInfo(
                self.accountId(),
                service.Id,
                function (data)
                {
                    self.sessionPasswordNumber('');
                    self.sessionPassword('');
                    if (window[data.service.ScriptName] != undefined)
                    {
                        window.DataProvider.GetSessionPasswordNumber(
                            function (number)
                            {
                                self.sessionPasswordNumber(number);
                            },
                            function ()
                            {
                                alert('error');
                            }
                        );
                        self.operationFormTemplate(undefined);
                        self.operationName(data.service.Name);
                        self.operationFormViewModel(new window[data.service.ScriptName](self.accountId(), data.service, new ServiceParametersManager(data.serviceParams)));
                        self.operationFormTemplate(data.service.TemplateName);
                        serviceId = service.Id;
                    }
                },
                function ()
                {
                    alert('error');
                }
            );
        }
    };

    var hideAccountNumber = function (number)
    {
        return number[0] + "*** **** **** " + number.substr(number.length - 4);
    };

    var init = function ()
    {
        window.DataProvider.GetOperationsMenuData(
            function (data)
            {
                data = JSON.parse(data);
                self.operations(data);
            },
            function ()
            {
                alert('error');
            }
        );
        window.DataProvider.GetAccounts(
            function (data)
            {
                data = JSON.parse(data);
                for (var i in data)
                {
                    data[i].Number = hideAccountNumber(data[i].Number) + ' ' + data[i].CardType.Name;
                }
                self.accountList(data);
                //if (data.length > 0)
                //    self.accountId(data[0].Id);
            },
            function ()
            {
                alert('error');
            }
        );
    };

    init();

    return self;
};