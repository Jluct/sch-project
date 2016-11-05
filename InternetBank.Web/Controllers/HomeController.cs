using Common;
using InternetBank.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InternetBank.Models
{
    [Authorize]
    public class HomeController : Controller
    {
        private IUserManager _userManager = UserManagerFactory.Get();
        private IDataProvider _dataProvider = DataProviderFactory.Get();

        private IMapper<City, CityModel> _cityMapper = new Mapper<City, CityModel>();
        private IMapper<Region, RegionModel> _regionMapper = new Mapper<Region, RegionModel>();

        private User currentUser
        {
            get 
            {
                return _userManager.GetByLogin(User.Identity.Name);
            }
        }

        public ActionResult Index()
        {
            //int userId = 0;
            //int.TryParse((string)Session["UserId"], out userId);

            if (currentUser != null && (currentUser.IsBlocked || !currentUser.IsActivated))
                return RedirectToAction("Blocked", "User");

            if (currentUser == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LogOff", "User");
            }

            if (currentUser.IsAdmin)
                return RedirectToAction("Index", "Admin");

            ViewBag.UserFIO = currentUser.FIO.ToUpper();
            ViewBag.Cards = _dataProvider.GetAccounts(currentUser.Id).Where(a => a.IsOpened).Select(u =>
            {
                var card = _dataProvider.GetCards(u.Id).Where(c => !c.IsExpired && c.IsAttached && !c.IsBlocked).FirstOrDefault();
                if (card != null)
                    return new
                    {
                        Id = card.Id,
                        Number = card.Number,
                        IsBlocked = card.IsBlocked,
                        IsAttached = card.IsAttached,
                        CardType = _dataProvider.GetCardType(card.CardTypeId),
                        ExpireDate = card.ExpireDate.ToString("dd.MM.yyyy"),
                        Currency = ((Currencies)u.Currency).ToString(),
                        MinBalance = u.MinBalance
                    };
                else return null;
            }).Where(c => c != null).ToList();

            return View();
            //Response.Cookies.Add(new HttpCookie("Language", code));
        }

        //public JsonResult GetOperationInfo(int serviceId)
        //{

        //}

        public ActionResult Settings()
        {
            ViewBag.Regions = _regionMapper.MapToModelList(_dataProvider.GetRegions());
            var city = _dataProvider.GetCity(currentUser.CityId);
            ViewBag.RegionId = city.RegionId;
            ViewBag.CityId = city.Id;
            ViewBag.Cards = _dataProvider.GetAccounts(currentUser.Id).Where(a => a.IsOpened).Select(u =>
            {
                var card = _dataProvider.GetCards(u.Id).Where(c => !c.IsExpired && c.IsAttached && !c.IsBlocked).FirstOrDefault();
                if (card != null)
                    return new
                    {
                        Id = card.Id,
                        Number = card.Number,
                        Balance = u.Balance,
                        MinBalance = u.MinBalance
                    };
                else return null;
            }).Where(c => c != null).ToList();
            return View();
        }

        public JsonResult GetRegionCities(int regionId)
        {
            return Json(_cityMapper.MapToModelList(_dataProvider.GetCities(regionId)), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Payment()
        {
            return View();
        }

        private void FilterOperationsMenu(IList<OperationMenuItemModel> treeNodes, int regionId, int cityId)
        {
            for (var i = 0; i < treeNodes.Count; i++)
            {
                bool accessDenied = false;
                if (treeNodes[i].LocalityTypeId.HasValue && treeNodes[i].LocalityId.HasValue)
                {
                    switch ((LocalityTypes)treeNodes[i].LocalityTypeId)
                    {
                        case LocalityTypes.Region:
                            accessDenied = treeNodes[i].LocalityId != regionId;
                            break;
                        case LocalityTypes.City:
                            accessDenied = treeNodes[i].LocalityId != cityId;
                            break;
                    }
                    if (accessDenied)
                    {
                        treeNodes.RemoveAt(i);
                        i--;
                    }
                }
                if (!accessDenied && treeNodes[i].Children.Count > 0)
                    FilterOperationsMenu(treeNodes[i].Children, regionId, cityId);
            }
        }

        public JsonResult GetOperationsMenuData()
        {
            var categories = _dataProvider.GetRootServiceCategories();
            var services = _dataProvider.GetRootServices();

            var result = categories.Select(categ => { return new OperationMenuItemModel(categ); }).ToList();
            result.AddRange(services.Select(service => new OperationMenuItemModel(service)));

            FilterOperationsMenu(result, _dataProvider.GetCity(currentUser.CityId).RegionId, currentUser.CityId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAccounts()
        {
            return Json(_dataProvider.GetAccounts(currentUser.Id).Where(a => a.IsOpened).Select(u =>
            {
                var card = _dataProvider.GetCards(u.Id).Where(c => !c.IsExpired && c.IsAttached && !c.IsBlocked).FirstOrDefault();
                if (card != null)
                    return new
                    {
                        Id = card.Id,
                        Number = card.Number,
                        IsBlocked = card.IsBlocked,
                        IsAttached = card.IsAttached,
                        CardType = _dataProvider.GetCardType(card.CardTypeId)
                    };
                else return null;
            }).Where(c => c != null).ToList(), JsonRequestBehavior.AllowGet);
            //return Json(_dataProvider.GetAccounts(user.Id).Where(a => a.IsOpened).Select(u => new { u.Id, u.Number }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Cards()
        {
            ViewBag.Cards = _dataProvider.GetAccounts(currentUser.Id).Where(a => a.IsOpened).Select(u =>
            {
                var card = _dataProvider.GetCards(u.Id).Where(c => !c.IsExpired).FirstOrDefault();
                if (card != null) 
                    return new {
                        Id = card.Id,
                        Number = card.Number,
                        IsBlocked = card.IsBlocked,
                        IsAttached = card.IsAttached,
                        CardType = _dataProvider.GetCardType(card.CardTypeId)
                    }; else return null;
            }).Where(c => c != null).ToList();
            return View();
        }

        public ActionResult Operations()
        {
            var operations = _dataProvider.GetOperations(currentUser.Id);
            ViewBag.Operations = operations.Select(o => new { Date = o.Date.ToString("dd.MM.yyyy HH:mm:ss"), CardNumber = o.Card.Account.Number, CardType = o.Card.CardType.Name, OperationName = o.Service.Name, OperationData = o.Data, Currency = ((Currencies)o.Card.Account.Currency).ToString(), Sum = o.Sum }).ToList();
            return View();
        }

        public ActionResult AttachCard(int cardId)
        {
            var card = _dataProvider.GetCard(cardId);
            card.IsAttached = true;
            _dataProvider.SaveCard(card);
            return RedirectToAction("Cards");
        }

        public ActionResult DetachCard(int cardId)
        {
            var card = _dataProvider.GetCard(cardId);
            card.IsAttached = false;
            _dataProvider.SaveCard(card);
            return RedirectToAction("Cards");
        }

        public ActionResult BlockCard(int cardId)
        {
            var card = _dataProvider.GetCard(cardId);
            card.IsBlocked = true;
            _dataProvider.SaveCard(card);
            return RedirectToAction("Cards");
        }

        public ActionResult UnblockCard(int cardId)
        {
            var card = _dataProvider.GetCard(cardId);
            card.IsBlocked = false;
            _dataProvider.SaveCard(card);
            return RedirectToAction("Cards");
        }

        public ActionResult MoneyTransfer()
        {
            ViewBag.Cards = _dataProvider.GetAccounts(currentUser.Id).Where(a => a.IsOpened).Select(u =>
            {
                var card = _dataProvider.GetCards(u.Id).Where(c => !c.IsExpired && c.IsAttached && !c.IsBlocked).FirstOrDefault();
                if (card != null)
                    return new
                    {
                        Id = card.Id,
                        Number = card.Number,
                        IsBlocked = card.IsBlocked,
                        IsAttached = card.IsAttached,
                        CardType = _dataProvider.GetCardType(card.CardTypeId)
                    };
                else return null;
            }).Where(c => c != null).ToList();
            return View();
        }

        [HttpPost]
        public JsonResult Transfer(int cardIdFrom, int cardIdTo, decimal sum)
        {
            var cardFrom = _dataProvider.GetCard(cardIdFrom);
            var accountFrom = _dataProvider.GetAccount(cardFrom.AccountId);
            var cardTo = _dataProvider.GetCard(cardIdTo);
            var accountTo = _dataProvider.GetAccount(cardTo.AccountId);
            if (accountFrom.Balance - accountFrom.MinBalance >= sum)
            {
                accountFrom.Balance -= sum;
                accountTo.Balance += sum;
                _dataProvider.SaveAccount(accountFrom);
                _dataProvider.SaveAccount(accountTo);
                return Json(new string[0]);
            }
            return Json(new[] { "Не хватает средств для выполнения операции" });
        }

        [HttpPost]
        public JsonResult ChangeLocality(int cityId)
        {
            var locality = _dataProvider.GetCity(cityId);
            if (locality == null)
                return Json(new[] { "Город не найден" });
            currentUser.CityId = cityId;
            _userManager.SaveUser(currentUser);
            return Json(new string[0]);
        }

        [HttpPost]
        public JsonResult ChangePassword(string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
                return Json(new[] { "Новый пароль не задан" });
            if (currentUser.Password == oldPassword)
            {
                currentUser.Password = newPassword;
                _userManager.SaveUser(currentUser);
                return Json(new string[0]);
            }
            else
                return Json(new[] { "Пароль не совпадает" });
        }

        public JsonResult GetSessionPasswordNumber()
        {
            var passwords = _dataProvider.GetSessionPasswords(currentUser.Id);
            return Json(Session["SessionPasswordNumber"] = new Random(unchecked((int)DateTime.Now.Ticks)).Next(passwords.Count) + 1, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetBalance(int cardId)
        {
            var card = _dataProvider.GetCard(cardId);
            var account = _dataProvider.GetAccount(card.AccountId);
            return Json(account.Balance);
        }

        [HttpPost]
        public JsonResult SetMinBalance(int cardId, decimal minBalance)
        {
            var card = _dataProvider.GetCard(cardId);
            var account = _dataProvider.GetAccount(card.AccountId);
            if (account.Balance < minBalance)
                return Json(new string[] { "Неснижаемый остаток не может быть ниже текущего баланса" });
            account.MinBalance = minBalance;
            _dataProvider.SaveAccount(account);
            return Json(new string[0]);
        }

        [HttpPost]
        public JsonResult Pay(string sessionPassword, int cardId, int serviceId, Dictionary<string, ServiceParameterModel> parameters, decimal sum, string description)
        {
            if (Session["SessionPasswordNumber"] == null || string.IsNullOrEmpty(Session["SessionPasswordNumber"].ToString()))
                return Json(new[] { "Произошла ошибка" });

            var card = _dataProvider.GetCard(cardId);
            var account = _dataProvider.GetAccount(card.AccountId);

            if (account.Balance - account.MinBalance < sum)
                return Json(new[] { "У вас недостаточно средств для выполнения операции" });

            int passNumber = Convert.ToInt32(Session["SessionPasswordNumber"]);

            var userSessionPasswords = _dataProvider.GetSessionPasswords(currentUser.Id).ToList();
            if (passNumber <= 0 || passNumber > userSessionPasswords.Count)
                return Json(new[] { "Произошла ошибка" });
                
            if (userSessionPasswords[passNumber - 1].Password != sessionPassword)
                return Json(new[] { "Неверный сеансовый ключ" });

            foreach (var paramName in parameters.Keys)
            {
                var serviceParam = _dataProvider.GetServiceParameterByName(serviceId, paramName);
                if (serviceParam == null)
                    serviceParam = new ServiceParameter { AccountId = card.AccountId, ServiceId = serviceId, Name = paramName };
                serviceParam.ValueType = parameters[paramName].ValueType;
                serviceParam.DefaultValue = parameters[paramName].DefaultValue;
                _dataProvider.SaveServiceParameter(serviceParam);
            }
            _dataProvider.SaveOperation(new Operation { CardId = cardId, ServiceId = serviceId, Date = DateTime.UtcNow, Sum = sum, Data = description });
            account.Balance -= sum;
            _dataProvider.SaveAccount(account);
            return Json(new string[0]);
        }
    }
}
