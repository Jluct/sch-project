window.AdminDataProvider = new function ()
{
    this.GetAllOperationsMenuData = function (successCallback, errorCallback)
    {
        ajax.get("/Admin/GetOperationsMenuData", successCallback, errorCallback);
    };
    this.MoveOperation = function (operationId, parentOperationId, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/MoveOperation", { operationId: operationId, parentOperationId: parentOperationId }, successCallback, errorCallback);
    };
    this.MoveService = function (serviceId, parentOperationId, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/MoveService", { serviceId: serviceId, parentOperationId: parentOperationId }, successCallback, errorCallback);
    };
    this.SaveOperation = function (operation, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/SaveOperation", { operation: operation }, successCallback, errorCallback);
    };
    this.DeleteOperation = function (operationId, isService, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/DeleteOperation", { operationId: operationId, isService: isService }, successCallback, errorCallback);
    };

    this.GetUsers = function (successCallback, errorCallback)
    {
        ajax.postJson("/Admin/GetUsers", {}, successCallback, errorCallback);
    };
    this.SaveUser = function (user, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/SaveUser", { user: user }, successCallback, errorCallback);
    };
    this.DeleteUser = function (userId, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/DeleteUser", { userId: userId }, successCallback, errorCallback);
    };


    this.GetCards = function (accountId, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/GetCards", { accountId: accountId }, successCallback, errorCallback);
    };
    this.SaveCard = function (card, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/SaveCard", { card: card }, successCallback, errorCallback);
    };
    this.DeleteCard = function (cardId, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/DeleteCard", { cardId: cardId }, successCallback, errorCallback);
    };
    this.GetCardTypes = function (successCallback, errorCallback)
    {
        ajax.get("/Admin/GetCardTypes", successCallback, errorCallback);
    };
    this.SaveCardType = function (cardType, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/SaveCardType", { cardType: cardType }, successCallback, errorCallback);
    };
    this.DeleteCardType = function (cardTypeId, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/DeleteCardType", { cardTypeId: cardTypeId }, successCallback, errorCallback);
    };


    this.GetAccounts = function (userId, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/GetAccounts", { userId: userId }, successCallback, errorCallback);
    };
    this.SaveAccount = function (account, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/SaveAccount", { account: account }, successCallback, errorCallback);
    };
    this.DeleteAccount = function (accountId, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/DeleteAccount", { accountId: accountId }, successCallback, errorCallback);
    };


    this.GetServiceParameters = function (serviceId, accountId, successCallback, errorCallback)
    {
        var data = { serviceId: serviceId };
        if (accountId)
            data.accountId = accountId;
        ajax.postJson("/Admin/GetServiceParameters", data, successCallback, errorCallback);
    };
    this.SaveServiceParameter = function (param, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/SaveServiceParameter", { parameter: param }, successCallback, errorCallback);
    };
    this.DeleteServiceParameter = function (paramId, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/DeleteServiceParameter", { serviceParamId: paramId }, successCallback, errorCallback);
    };


    this.GetRegions = function (successCallback, errorCallback)
    {
        ajax.postJson("/Admin/GetRegions", {}, successCallback, errorCallback);
    };
    this.GetCities = function (regionId, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/GetCities", { regionId: regionId }, successCallback, errorCallback);
    };
    this.SaveRegion = function (region, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/SaveRegion", { region: region }, successCallback, errorCallback);
    };
    this.SaveCity = function (city, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/SaveCity", { city: city }, successCallback, errorCallback);
    };
    this.DeleteCity = function (cityId, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/DeleteCity", { cityId: cityId }, successCallback, errorCallback);
    };
    this.DeleteRegion = function (regionId, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/DeleteRegion", { regionId: regionId }, successCallback, errorCallback);
    };


    this.GetSessionPasswords = function (userId, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/GetSessionPasswords", { userId: userId }, successCallback, errorCallback);
    };
    this.SaveSessionPassword = function (sessionPassword, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/SaveSessionPassword", { sessionPassword: sessionPassword }, successCallback, errorCallback);
    };
    this.DeleteSessionPassword = function (sessionPasswordId, successCallback, errorCallback)
    {
        ajax.postJson("/Admin/DeleteSessionPassword", { sessionPasswordId: sessionPasswordId }, successCallback, errorCallback);
    };
};