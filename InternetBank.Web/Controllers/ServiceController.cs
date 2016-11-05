using Common;
using InternetBank.Dal;
using InternetBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InternetBank.Controllers
{
    [Authorize]
    public class ServiceController : Controller
    {
        IDataProvider _dataProvider = DataProviderFactory.Get();
        private IMapper<ServiceParameter, ServiceParameterModel> _serviceParamMapper = new Mapper<ServiceParameter, ServiceParameterModel>();

        [HttpPost]
        public JsonResult GetServiceInfo(int accountId, int serviceId)
        {
            var service = _dataProvider.GetService(serviceId);
            var serviceParams = _dataProvider.GetServiceParameters(serviceId);
            return Json(new { service = new OperationMenuItemModel(service), serviceParams = _serviceParamMapper.MapToModelList(serviceParams) });
        }

        [HttpPost]
        public JsonResult PayForService(int cardId, int serviceId, decimal sum, IEnumerable<ServiceParameterModel> parameters)
        {
            return Json(new string[0]);
        }
    }
}
