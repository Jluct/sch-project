window.DataProvider = new function ()
{
    this.GetOperationsMenuData = function (successCallback, errorCallback)
    {
        ajax.get("/Home/GetOperationsMenuData", successCallback, errorCallback);
    };
    this.GetOperationInfo = function (serviceId, successCallback, errorCallback)
    {
        ajax.postJson("/Home/GetOperationInfo", { serviceId: serviceId }, successCallback, errorCallback);
    };
    this.GetServiceInfo = function (accountId, serviceId, successCallback, errorCallback)
    {
        ajax.postJson("/Service/GetServiceInfo", { accountId: accountId, serviceId: serviceId }, successCallback, errorCallback);
    };
    this.GetAccounts = function (successCallback, errorCallback)
    {
        ajax.get("/Home/GetAccounts", successCallback, errorCallback);
    };
    this.GetCards = function (successCallback, errorCallback)
    {
        ajax.get("/Home/GetCards", successCallback, errorCallback);
    };
    this.GetRegionCities = function (regionId, successCallback, errorCallback)
    {
        ajax.postJson("/Home/GetRegionCities", { regionId: regionId }, successCallback, errorCallback);
    };
    this.ChangePassword = function (oldPassword, newPassword, successCallback, errorCallback)
    {
        ajax.postJson("/Home/ChangePassword", { oldPassword: oldPassword, newPassword: newPassword }, successCallback, errorCallback);
    };
    this.ChangeLocality = function (cityId, successCallback, errorCallback)
    {
        ajax.postJson("/Home/ChangeLocality", { cityId: cityId }, successCallback, errorCallback);
    };
    this.GetSessionPasswordNumber = function (successCallback, errorCallback)
    {
        ajax.get("/Home/GetSessionPasswordNumber", successCallback, errorCallback);
    };
    this.GetBalance = function (cardId, successCallback, errorCallback)
    {
        ajax.postJson("/Home/GetBalance", { cardId: cardId }, successCallback, errorCallback);
    };
    this.SetMinBalance = function (cardId, minBalance, successCallback, errorCallback)
    {
        ajax.postJson("/Home/SetMinBalance", { cardId: cardId, minBalance: minBalance }, successCallback, errorCallback);
    };
    this.Pay = function (sessionPassword, cardId, serviceId, parameters, sum, description, successCallback, errorCallback)
    {
        ajax.postJson("/Home/Pay", { sessionPassword: sessionPassword, cardId: cardId, serviceId: serviceId, parameters: parameters, sum: sum, description: description }, successCallback, errorCallback);
    };
    this.Transfer = function (cardIdFrom, cardIdTo, sum, successCallback, errorCallback)
    {
        ajax.postJson("/Home/Transfer", { cardIdFrom: cardIdFrom, cardIdTo: cardIdTo, sum: sum }, successCallback, errorCallback);
    };
};