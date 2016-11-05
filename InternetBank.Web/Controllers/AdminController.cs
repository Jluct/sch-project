using Common;
using InternetBank.Dal;
using InternetBank.Mappers;
using InternetBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InternetBank.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IUserManager _userManager = UserManagerFactory.Get();
        private IDataProvider _dataProvider = DataProviderFactory.Get();
        private IMapper<User, UserModel> _userMapper = new Mapper<User, UserModel>();
        private CardMapper _cardMapper = new CardMapper();
        private AccountMapper _accountMapper = new AccountMapper();
        private IMapper<ServiceParameter, ServiceParameterModel> _serviceParamMapper = new Mapper<ServiceParameter, ServiceParameterModel>();
        private IMapper<Region, RegionModel> _regionMapper = new Mapper<Region, RegionModel>();
        private IMapper<City, CityModel> _cityMapper = new Mapper<City, CityModel>();
        private IMapper<CardType, CardTypeModel> _cardTypeMapper = new Mapper<CardType, CardTypeModel>();
        private IMapper<SessionPassword, SessionPasswordModel> _sessionPasswordMapper = new Mapper<SessionPassword, SessionPasswordModel>();

        #region Views

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UsersView()
        {
            return View();
        }

        public ActionResult LocalitiesView()
        {
            return View();
        }

        public ActionResult ServicesView()
        {
            return View();
        }

        public ActionResult CardTypesView()
        {
            return View();
        }

        #endregion

        #region Users

        [HttpPost]
        public JsonResult GetUsers()
        {
            return Json(_userMapper.MapToModelList(_userManager.GetUsers()));
        }
        [HttpPost]
        public JsonResult SaveUser(UserModel user)
        {
            var dbUser = _userManager.GetById(user.Id) ?? new User();
            if (dbUser.IsBlocked != user.IsBlocked)
                dbUser.LoginAttempts = 0;
            var entity = _userMapper.MapToEntity(user, ref dbUser);
            _userManager.SaveUser(entity);
            return Json(entity.Id);
        }
        [HttpPost]
        public JsonResult DeleteUser(int userId)
        {
            _userManager.DeleteUser(userId);
            return Json(new object[1]);
        }

        #endregion

        #region Cards

        [HttpPost]
        public JsonResult GetCards(int accountId)
        {
            return Json(_cardMapper.MapToModelList(_dataProvider.GetCards(accountId)));
        }
        [HttpPost]
        public JsonResult SaveCard(CardModel card)
        {
            var dbCard = _dataProvider.GetCard(card.Id) ?? new Card();
            var entity = _cardMapper.MapToEntity(card, ref dbCard);
            _dataProvider.SaveCard(entity);
            return Json(entity.Id);
        }
        [HttpPost]
        public JsonResult DeleteCard(int cardId)
        {
            _dataProvider.DeleteCard(cardId);
            return Json(new object[1]);
        }

        public JsonResult GetCardTypes()
        {
            return Json(_cardTypeMapper.MapToModelList(_dataProvider.GetCardTypes()), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveCardType(CardTypeModel cardType)
        {
            var dbCardType = _dataProvider.GetCardType(cardType.Id) ?? new CardType();
            var entity = _cardTypeMapper.MapToEntity(cardType, ref dbCardType);
            _dataProvider.SaveCardType(entity);
            return Json(entity.Id);
        }
        [HttpPost]
        public JsonResult DeleteCardType(int cardTypeId)
        {
            _dataProvider.DeleteCardType(cardTypeId);
            return Json(new object[1]);
        }

        #endregion

        #region Accounts

        [HttpPost]
        public JsonResult GetAccounts(int userId)
        {
            return Json(_accountMapper.MapToModelList(_dataProvider.GetAccounts(userId)));
        }
        [HttpPost]
        public JsonResult SaveAccount(AccountModel account)
        {
            var dbAccount = _dataProvider.GetAccount(account.Id) ?? new Account();
            var entity = _accountMapper.MapToEntity(account, ref dbAccount);
            _dataProvider.SaveAccount(entity);
            return Json(entity.Id);
        }
        [HttpPost]
        public JsonResult DeleteAccount(int accountId)
        {
            _dataProvider.DeleteAccount(accountId);
            return Json(new object[1]);
        }

        #endregion

        #region Operations

        [HttpPost]
        public JsonResult MoveOperation(int operationId, int parentOperationId)
        {
            var category = _dataProvider.GetServiceCategory(operationId);
            var categoryTo = _dataProvider.GetServiceCategory(parentOperationId);
            category.ServiceCategoryId = categoryTo.Id;
            _dataProvider.SaveServiceCategory(category);
            return Json(new object[0]);
        }
        [HttpPost]
        public JsonResult MoveService(int serviceId, int parentOperationId)
        {
            var service = _dataProvider.GetService(serviceId);
            var categoryTo = _dataProvider.GetServiceCategory(parentOperationId);
            service.ServiceCategoryId = categoryTo.Id;
            _dataProvider.SaveService(service);
            return Json(new object[0]);
        }
        [HttpPost]
        public JsonResult SaveOperation(OperationMenuItemModel operation)
        {
            int id;
            if (!operation.IsService)
            {
                var dbCategory = _dataProvider.GetServiceCategory(operation.Id) ?? new ServiceCategory();
                dbCategory.Name = operation.Name;
                dbCategory.ServiceCategoryId = operation.ParentId;
                var locality = _dataProvider.GetLocality(dbCategory.LocalityId.GetValueOrDefault());
                if (locality != null && (!operation.LocalityTypeId.HasValue ||  !operation.LocalityId.HasValue))
                {
                    _dataProvider.DeleteLocality(locality.Id);
                    dbCategory.LocalityId = null;
                }
                else if (operation.LocalityTypeId.HasValue && operation.LocalityId.HasValue)
                {
                    if (locality == null)
                        locality = new Locality();
                    locality.LocalityId = operation.LocalityId.Value;
                    locality.LocalityType = operation.LocalityTypeId.Value;
                    _dataProvider.SaveLocality(locality);
                    dbCategory.LocalityId = locality.Id;
                }
                _dataProvider.SaveServiceCategory(dbCategory);
                id = dbCategory.Id;
            }
            else
            {
                var dbService = _dataProvider.GetService(operation.Id) ?? new Service();
                dbService.Name = operation.Name;
                dbService.ServiceCategoryId = operation.ParentId;
                dbService.ScriptName = operation.ScriptName;
                dbService.TemplateName = operation.TemplateName;
                var locality = _dataProvider.GetLocality(dbService.LocalityId.GetValueOrDefault());
                if (locality != null && (!operation.LocalityTypeId.HasValue || !operation.LocalityId.HasValue))
                {
                    _dataProvider.DeleteLocality(locality.Id);
                    dbService.LocalityId = null;
                }
                else if (operation.LocalityTypeId.HasValue && operation.LocalityId.HasValue)
                {
                    if (locality == null)
                        locality = new Locality();
                    locality.LocalityId = operation.LocalityId.Value;
                    locality.LocalityType = operation.LocalityTypeId.Value;
                    _dataProvider.SaveLocality(locality);
                    dbService.LocalityId = locality.Id;
                }
                _dataProvider.SaveService(dbService);
                id = dbService.Id;
            }
            return Json(id);
        }
        [HttpPost]
        public JsonResult DeleteOperation(int operationId, bool isService)
        {
            if (!isService)
            {
                _dataProvider.DeleteServiceCategory(operationId);
            }
            else
            {
                _dataProvider.DeleteService(operationId);
            }
            return Json(new object[0]);
        }
        public JsonResult GetOperationsMenuData()
        {
            var categories = _dataProvider.GetRootServiceCategories();
            var services = _dataProvider.GetRootServices();

            var result = categories.Select(categ => { return new OperationMenuItemModel(categ); }).ToList();
            result.AddRange(services.Select(service => new OperationMenuItemModel(service)));

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Service Parameters

        [HttpPost]
        public JsonResult GetServiceParameters(int serviceId)
        {
            return Json(_serviceParamMapper.MapToModelList(_dataProvider.GetServiceParameters(null, serviceId)));
        }
        [HttpPost]
        public JsonResult SaveServiceParameter(ServiceParameterModel parameter)
        {
            var dbServiceParam = _dataProvider.GetServiceParameter(parameter.Id) ?? new ServiceParameter();
            var entity = _serviceParamMapper.MapToEntity(parameter, ref dbServiceParam);
            _dataProvider.SaveServiceParameter(entity);
            return Json(entity.Id);
        }
        [HttpPost]
        public JsonResult DeleteServiceParameter(int serviceParamId)
        {
            _dataProvider.DeleteServiceParameter(serviceParamId);
            return Json(new object[0]);
        }

        #endregion

        #region Locality

        [HttpPost]
        public JsonResult GetRegions()
        {
            return Json(_regionMapper.MapToModelList(_dataProvider.GetRegions()));
        }
        [HttpPost]
        public JsonResult GetCities(int regionId)
        {
            return Json(_cityMapper.MapToModelList(_dataProvider.GetCities(regionId)));
        }
        [HttpPost]
        public JsonResult SaveRegion(RegionModel region)
        {
            var dbRegion = _dataProvider.GetRegion(region.Id) ?? new Region();
            var entity = _regionMapper.MapToEntity(region, ref dbRegion);
            _dataProvider.SaveRegion(entity);
            return Json(entity.Id);
        }
        [HttpPost]
        public JsonResult SaveCity(CityModel city)
        {
            var dbCity = _dataProvider.GetCity(city.Id) ?? new City();
            var entity = _cityMapper.MapToEntity(city, ref dbCity);
            _dataProvider.SaveCity(entity);
            return Json(entity.Id);
        }
        [HttpPost]
        public JsonResult DeleteCity(int cityId)
        {
            _dataProvider.DeleteCity(cityId);
            return Json(new object[1]);
        }
        [HttpPost]
        public JsonResult DeleteRegion(int regionId)
        {
            _dataProvider.DeleteRegion(regionId);
            return Json(new object[1]);
        }

        #endregion

        #region Session Passwords

        [HttpPost]
        public JsonResult GetSessionPasswords(int userId)
        {
            return Json(_sessionPasswordMapper.MapToModelList(_dataProvider.GetSessionPasswords(userId)));
        }
        [HttpPost]
        public JsonResult SaveSessionPassword(SessionPasswordModel sessionPassword)
        {
            var dbSessionPassword = _dataProvider.GetSessionPassword(sessionPassword.Id) ?? new SessionPassword();
            var entity = _sessionPasswordMapper.MapToEntity(sessionPassword, ref dbSessionPassword);
            _dataProvider.SaveSessionPassword(entity);
            return Json(entity.Id);
        }
        [HttpPost]
        public JsonResult DeleteSessionPassword(int sessionPasswordId)
        {
            _dataProvider.DeleteSessionPassword(sessionPasswordId);
            return Json(new object[1]);
        }

        #endregion
    }
}
