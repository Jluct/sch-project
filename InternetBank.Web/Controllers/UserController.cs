using InternetBank.Dal;
using InternetBank.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace InternetBank.Models
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        private IUserManager _userManager = UserManagerFactory.Get();

        public ActionResult LogOn()
        {
            return View();
        }

        public ActionResult Blocked()
        {
            EnsureUserSignedOut();
            return View();
        }

        [HttpPost]
        public virtual ActionResult LogOn(LogOnModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                // User should be logged out if he is already logged on.
                EnsureUserSignedOut(false);
            }

            if (ModelState.IsValid)
            {
                var user = _userManager.GetByLogin(model.Login);
                
                if (user != null && user.IsBlocked)
                    return RedirectToAction("Blocked", "User");

                if (user == null || user.Password != model.Password)
                {
                    var maxAttempts = Convert.ToInt32(ConfigurationManager.AppSettings["MaxLoginAttemptsCount"]);
                    if (user != null)
                    {
                        user.LoginAttempts++;
                        if (user.LoginAttempts >= maxAttempts)
                        {
                            user.IsBlocked = true;
                        }
                        _userManager.SaveUser(user);
                        if (user.IsBlocked)
                            return RedirectToAction("Blocked", "User");
                        var attempts = (maxAttempts - user.LoginAttempts);
                        var lastDigit = attempts % 10;
                        string ending = "ки";
                        if (lastDigit == 0 || lastDigit >= 5 || (attempts >= 10 && attempts <= 20))
                            ending = "ок";
                        else if (lastDigit == 1)
                            ending = "ка";
                        ModelState.AddModelError("Common", string.Format(UserResources.LoginOrPasswordIsIncorrect, attempts, ending));
                    }
                    else
                        ModelState.AddModelError("Common", "Неправильный логин или пароль");
                }
                else
                {
                    Session["UserId"] = user.Id;
                    user.LoginAttempts = 0;
                    _userManager.SaveUser(user);
                    FormsAuthentication.SetAuthCookie(user.Login, false);
                    return RedirectToAction("Index", "Home");
                }

                //IList<KeyValuePair<string, string>> errors;
                //int userId = UserManager.TryToLogIn(model.Login, model.Password, out errors);

                //if (userId != -1)
                //{
                //    Session["UserId"] = userId;
                //    Session["AuthStep1"] = true;

                //    IRedirectActionValues redirect;
                //    if (UserManager.RedirectActions.ContainsKey(Web.User.UserManager.LoginRedirectKey) &&
                //        AuthAndRegConfigSection.GetRegistrationType() == RegistrationType.Full)
                //    {
                //        redirect = UserManager.RedirectActions[Web.User.UserManager.LoginRedirectKey];
                //    }
                //    else
                //    {
                //        redirect = new RedirectActionElement { ActionName = "Index", ControllerName = "UserSecurity" };
                //    }

                //    return RedirectToAction(redirect.ActionName, redirect.ControllerName);
                //}
                //TryHandleErrors(errors);
            }

            return View(model);
        }

        public ActionResult LogOff()
        {
            EnsureUserSignedOut();
            return RedirectToAction("LogOn");
        }

        public ActionResult Register()
        {
            EnsureUserSignedOut();
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegistrationModel model)
        {
            EnsureUserSignedOut();
            if (ModelState.IsValid)
            {
                var user = _userManager.GetUserByPassportNumber(model.PassportNumber);
                if (user == null)
                {
                    ModelState.AddModelError("PassportNumber", UserResources.PasswordNumberIncorrect);
                }
                else if (user.IsBlocked)
                {
                    ModelState.AddModelError("Common", UserResources.AccountBlocked);
                }
                else if (!user.IsActivated)
                {
                    user.IsActivated = true;
                    user.Login = model.Login;
                    user.Password = model.Password;
                    _userManager.SaveUser(user);
                    return RedirectToAction("RegistrationCompleted", "User");
                }
                else
                {
                    ModelState.AddModelError("Common", UserResources.YouHaveAlreadyRegistered);
                }
            }
            return View(model);
        }

        public ActionResult RegistrationCompleted()
        {
            EnsureUserSignedOut();
            return View();
        }

        private void EnsureUserSignedOut(bool abandonSession = true)
        {
            try
            {
                var sessionToken = (string)Session["auth"];
                if (!String.IsNullOrEmpty(sessionToken))
                {
                    IList<KeyValuePair<string, string>> errors;
                    //UserManager.TryLogOut(sessionToken, out errors);
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex);
                throw;
            }
            finally
            {
                // Sign out
                FormsAuthentication.SignOut();

                ClearSession(abandonSession);

                if (abandonSession)
                {
                    Response.SetCookie(new HttpCookie("SessionExpiration") { Expires = DateTime.MinValue });
                }
            }
        }

        protected virtual void ClearSession(bool abandonSession)
        {
            if (abandonSession)
            {
                Session.Abandon();
            }
            Session.RemoveAll();
        }

#if DEBUG
        private User SaveUser(string login, string password, bool isActivated, bool isAdmin, int cityId, string FIO, string passportNumber)
        {
            var user = new User { Login = login, Password = password, FIO = FIO, PassportNumber = passportNumber, PassportData = "", IsActivated = isActivated, IsAdmin = isAdmin, CityId = cityId };
            _userManager.SaveUser(user);
            return user;
        }

        private void SaveBeltel(IDataProvider dataProvider, ServiceCategory beltel, int regionId, string beltelCategName, string code, string telmask)
        {
            var locality = new Locality { LocalityId = regionId, LocalityType = (int)LocalityTypes.Region }; dataProvider.SaveLocality(locality);
            var beltelObl = new ServiceCategory { Name = beltelCategName, ServiceCategoryId = beltel.Id, LocalityId = locality.Id }; dataProvider.SaveServiceCategory(beltelObl);
            var internet = new Service { Name = "Интернет (BYFLY, ZALA, Максифон)", ServiceCategoryId = beltelObl.Id, ScriptName = "BeltelecomInternetViewModel", TemplateName = "BeltelecomInternetTemplate" }; dataProvider.SaveService(internet);
            var telephone = new Service { Name = "Телефон", ServiceCategoryId = beltelObl.Id, ScriptName = "TelephoneViewModel", TemplateName = "TelephoneTemplate" }; dataProvider.SaveService(telephone);
            dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = internet.Id, Name = "Mask", ValueType = (int)ServiceParameterValueTypes.String, DefaultValue = "9999999999999" });
            dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = telephone.Id, Name = code, ValueType = (int)ServiceParameterValueTypes.String, DefaultValue = telmask });
        }

        private void SaveBeltel(IDataProvider dataProvider, ServiceCategory beltel, int regionId, string beltelCategName, CityTelephone[] telephones)
        {
            var oblLocality = new Locality { LocalityId = regionId, LocalityType = (int)LocalityTypes.Region }; dataProvider.SaveLocality(oblLocality);
            var beltelObl = new ServiceCategory { Name = beltelCategName, ServiceCategoryId = beltel.Id, LocalityId = oblLocality.Id }; dataProvider.SaveServiceCategory(beltelObl);
            foreach (var tel in telephones)
            {
                var locality = new Locality { LocalityId = tel.CityId, LocalityType = (int)LocalityTypes.City }; dataProvider.SaveLocality(locality);
                var internet = new Service { Name = "Интернет (BYFLY, ZALA, Максифон)", ServiceCategoryId = beltelObl.Id, ScriptName = "BeltelecomInternetViewModel", TemplateName = "BeltelecomInternetTemplate", LocalityId = locality.Id }; dataProvider.SaveService(internet);
                var telephone = new Service { Name = "Телефон", ServiceCategoryId = beltelObl.Id, ScriptName = "TelephoneViewModel", TemplateName = "TelephoneTemplate", LocalityId = locality.Id }; dataProvider.SaveService(telephone);
                dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = internet.Id, Name = "Mask", ValueType = (int)ServiceParameterValueTypes.String, DefaultValue = "9999999999999" });
                dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = telephone.Id, Name = tel.Code, ValueType = (int)ServiceParameterValueTypes.String, DefaultValue = tel.TelMask });
            }
        }

        struct CityTelephone
        {
            public int CityId;
            public string Code;
            public string TelMask;

            public CityTelephone(int cityId, string code, string telMask)
            {
                CityId = cityId;
                Code = code;
                TelMask = telMask;
            }
        }

        public ActionResult FillDatabase()
        {
            var userManager = UserManagerFactory.Get();
            var dataProvider = DataProviderFactory.Get();


            #region Localities

            var minskReg = new Region { Name = "Минская обл." }; dataProvider.SaveRegion(minskReg);
            var mogilevReg = new Region { Name = "Могилевская обл." }; dataProvider.SaveRegion(mogilevReg);
            var brestReg = new Region { Name = "Брестская обл." }; dataProvider.SaveRegion(brestReg);
            var vitebskReg = new Region { Name = "Витебская обл." }; dataProvider.SaveRegion(vitebskReg);
            var grodnoReg = new Region { Name = "Гродненская обл." }; dataProvider.SaveRegion(grodnoReg);
            var gomelReg = new Region { Name = "Гомельская обл." }; dataProvider.SaveRegion(gomelReg);
         

            var minsk = new City { Name = "Минск", RegionId = minskReg.Id }; dataProvider.SaveCity(minsk);
            var beresino = new City { Name = "Березино", RegionId = minskReg.Id }; dataProvider.SaveCity(beresino);
            var borisov = new City { Name = "Борисов", RegionId = minskReg.Id }; dataProvider.SaveCity(borisov);
            var vileika = new City { Name = "Вилейка", RegionId = minskReg.Id }; dataProvider.SaveCity(vileika);
            var vologin = new City { Name = "Воложин", RegionId = minskReg.Id }; dataProvider.SaveCity(vologin);
            var derginsk = new City { Name = "Держинск", RegionId = minskReg.Id }; dataProvider.SaveCity(derginsk);
            var godino = new City { Name = "Жодино", RegionId = minskReg.Id }; dataProvider.SaveCity(godino);
            var kleck = new City { Name = "Клецк", RegionId = minskReg.Id }; dataProvider.SaveCity(kleck);
            var kopil = new City { Name = "Копыль", RegionId = minskReg.Id }; dataProvider.SaveCity(kopil);
            var krupki = new City { Name = "Крупки", RegionId = minskReg.Id }; dataProvider.SaveCity(krupki);
            var logoisk = new City { Name = "Логойск", RegionId = minskReg.Id }; dataProvider.SaveCity(logoisk);
            var luban = new City { Name = "Любань", RegionId = minskReg.Id }; dataProvider.SaveCity(luban);
            var molodechno = new City { Name = "Молодечно", RegionId = minskReg.Id }; dataProvider.SaveCity(molodechno);
            var nesvig = new City { Name = "Несвиж", RegionId = minskReg.Id }; dataProvider.SaveCity(nesvig);

            var mogilev = new City { Name = "Могилев", RegionId = mogilevReg.Id }; dataProvider.SaveCity(mogilev);
            var belinichy = new City { Name = "Белыничи", RegionId = mogilevReg.Id }; dataProvider.SaveCity(belinichy);
            var bobruisk = new City { Name = "Бобоуйск", RegionId = mogilevReg.Id }; dataProvider.SaveCity(bobruisk);
            var bixov = new City { Name = "Быхов", RegionId = mogilevReg.Id }; dataProvider.SaveCity(bixov);
            var glusk = new City { Name = "Глуск", RegionId = mogilevReg.Id }; dataProvider.SaveCity(glusk);
            var gorki = new City { Name = "Горки", RegionId = mogilevReg.Id }; dataProvider.SaveCity(gorki);
            var dribin = new City { Name = "Дрибин", RegionId = mogilevReg.Id }; dataProvider.SaveCity(dribin);
            var Kirovsk = new City { Name = "Кировск", RegionId = mogilevReg.Id }; dataProvider.SaveCity(Kirovsk);
            var Klimovichy = new City { Name = "Климовичи", RegionId = mogilevReg.Id }; dataProvider.SaveCity(Klimovichy);
            var Klichev = new City { Name = "Кличев", RegionId = mogilevReg.Id }; dataProvider.SaveCity(Klichev);
            var Kostukovichy = new City { Name = "Костюковичи", RegionId = mogilevReg.Id }; dataProvider.SaveCity(Kostukovichy);
            var Krasnopolie = new City { Name = "Краснополье", RegionId = mogilevReg.Id }; dataProvider.SaveCity(Krasnopolie);
            var Krichev = new City { Name = "Кричев", RegionId = mogilevReg.Id }; dataProvider.SaveCity(Krichev);
            var Krugloe = new City { Name = "Круглое", RegionId = mogilevReg.Id }; dataProvider.SaveCity(Krugloe);
            var Mstislavl = new City { Name = "Мстиславль", RegionId = mogilevReg.Id }; dataProvider.SaveCity(Mstislavl);
            var Osipovichy = new City { Name = "Осиповичи", RegionId = mogilevReg.Id }; dataProvider.SaveCity(Osipovichy);
            var Slavgorod = new City { Name = "Славгород", RegionId = mogilevReg.Id }; dataProvider.SaveCity(Slavgorod);
            var Xotimsk = new City { Name = "Хотимск", RegionId = mogilevReg.Id }; dataProvider.SaveCity(Xotimsk);
            var Chays = new City { Name = "Чаусы", RegionId = mogilevReg.Id }; dataProvider.SaveCity(Chays);
            var Cherikov = new City { Name = "Чериков", RegionId = mogilevReg.Id }; dataProvider.SaveCity(Cherikov);
            var Shklov = new City { Name = "Шклов", RegionId = mogilevReg.Id }; dataProvider.SaveCity(Shklov);

            var grodno = new City { Name = "Гродно", RegionId = grodnoReg.Id }; dataProvider.SaveCity(grodno);
            var volcovisk = new City { Name = "Волковыск", RegionId = grodnoReg.Id }; dataProvider.SaveCity(volcovisk);
            var voronovo = new City { Name = "Вороново", RegionId = grodnoReg.Id }; dataProvider.SaveCity(voronovo);
            var diatlovo = new City { Name = "Дятлово", RegionId = grodnoReg.Id }; dataProvider.SaveCity(diatlovo);
            var zelva = new City { Name = "Зельва", RegionId = grodnoReg.Id }; dataProvider.SaveCity(zelva);
            var ivie = new City { Name = "Ивье", RegionId = grodnoReg.Id }; dataProvider.SaveCity(ivie);
            var korelichy = new City { Name = "Кореличи", RegionId = grodnoReg.Id }; dataProvider.SaveCity(korelichy);
            var lida = new City { Name = "Лида", RegionId = grodnoReg.Id }; dataProvider.SaveCity(lida);
            var mosti = new City { Name = "Мосты", RegionId = grodnoReg.Id }; dataProvider.SaveCity(mosti);
            var novogrudok = new City { Name = "Новогрудок", RegionId = grodnoReg.Id }; dataProvider.SaveCity(novogrudok);
            var ostrovec = new City { Name = "Островец", RegionId = grodnoReg.Id }; dataProvider.SaveCity(ostrovec);
            var oshmiany = new City { Name = "Ошмяны", RegionId = grodnoReg.Id }; dataProvider.SaveCity(oshmiany);
            var svisloch = new City { Name = "Свислочь", RegionId = grodnoReg.Id }; dataProvider.SaveCity(svisloch);
            var slonim = new City { Name = "Слоним", RegionId = grodnoReg.Id }; dataProvider.SaveCity(slonim);
            var smorgon = new City { Name = "Сморгонь", RegionId = grodnoReg.Id }; dataProvider.SaveCity(smorgon);
            var schychin = new City { Name = "Щучин", RegionId = grodnoReg.Id }; dataProvider.SaveCity(schychin);

            var gomel = new City { Name = "Гомель", RegionId = gomelReg.Id }; dataProvider.SaveCity(gomel);
            var Bragin = new City { Name = "Брагин", RegionId = gomelReg.Id }; dataProvider.SaveCity(Bragin);
            var Vetka = new City { Name = "Ветка", RegionId = gomelReg.Id }; dataProvider.SaveCity(Vetka);
            var Dabrush = new City { Name = "Добрушь", RegionId = gomelReg.Id }; dataProvider.SaveCity(Dabrush);
            var Elsk = new City { Name = "Ельск", RegionId = gomelReg.Id }; dataProvider.SaveCity(Elsk);
            var Gitkovachy = new City { Name = "Житковичи", RegionId = gomelReg.Id }; dataProvider.SaveCity(Gitkovachy);
            var Globin = new City { Name = "Жлобин", RegionId = gomelReg.Id }; dataProvider.SaveCity(Globin);
            var Kalinkovachy = new City { Name = "Калинковичи", RegionId = gomelReg.Id }; dataProvider.SaveCity(Kalinkovachy);
            var Korma = new City { Name = "Корма", RegionId = gomelReg.Id }; dataProvider.SaveCity(Korma);
            var Lelchici = new City { Name = "Лельчици", RegionId = gomelReg.Id }; dataProvider.SaveCity(Lelchici);
            var Loev = new City { Name = "Лоев", RegionId = gomelReg.Id }; dataProvider.SaveCity(Loev);
            var Mozir = new City { Name = "Мозырь", RegionId = gomelReg.Id }; dataProvider.SaveCity(Mozir);
            var NArovlia = new City { Name = "Наровля", RegionId = gomelReg.Id }; dataProvider.SaveCity(NArovlia);
            var Petrikov = new City { Name = "Петриков", RegionId = gomelReg.Id }; dataProvider.SaveCity(Petrikov);
            var Rogachev = new City { Name = "Рогачев", RegionId = gomelReg.Id }; dataProvider.SaveCity(Rogachev);
            var Svetlogorsk = new City { Name = "Светлогорск", RegionId = gomelReg.Id }; dataProvider.SaveCity(Svetlogorsk);
            var Chexhersk = new City { Name = "Чечерск", RegionId = gomelReg.Id }; dataProvider.SaveCity(Chexhersk);

            var vitebsk = new City { Name = "Витебск", RegionId = vitebskReg.Id }; dataProvider.SaveCity(vitebsk);
            var Braslav = new City { Name = "Браслав", RegionId = vitebskReg.Id }; dataProvider.SaveCity(Braslav);
            var Verxnedvinsk = new City { Name = "Верхнедвинск", RegionId = vitebskReg.Id }; dataProvider.SaveCity(Verxnedvinsk);
            var Glubokoe = new City { Name = "Глубокое", RegionId = vitebskReg.Id }; dataProvider.SaveCity(Glubokoe);
            var Gorodok = new City { Name = "Городок", RegionId = vitebskReg.Id }; dataProvider.SaveCity(Gorodok);
            var Dokshici = new City { Name = "Докшицы", RegionId = vitebskReg.Id }; dataProvider.SaveCity(Dokshici);
            var dybrovno = new City { Name = "Дубровно", RegionId = vitebskReg.Id }; dataProvider.SaveCity(dybrovno);
            var Lepel = new City { Name = "Лепель", RegionId = vitebskReg.Id }; dataProvider.SaveCity(Lepel);
            var Liozno = new City { Name = "Лиозно", RegionId = vitebskReg.Id }; dataProvider.SaveCity(Liozno);
            var Miori = new City { Name = "Миоры", RegionId = vitebskReg.Id }; dataProvider.SaveCity(Miori);
            var Novopolock = new City { Name = "Новополоцк", RegionId = vitebskReg.Id }; dataProvider.SaveCity(Novopolock);
            var Orsha = new City { Name = "Орша", RegionId = vitebskReg.Id }; dataProvider.SaveCity(Orsha);
            var Polock = new City { Name = "Полоцк", RegionId = vitebskReg.Id }; dataProvider.SaveCity(Polock);
            var postavi = new City { Name = "Поставы", RegionId = vitebskReg.Id }; dataProvider.SaveCity(postavi);
            var Senno = new City { Name = "Сенно", RegionId = vitebskReg.Id }; dataProvider.SaveCity(Senno);
            var tolochin = new City { Name = "Толочин", RegionId = vitebskReg.Id }; dataProvider.SaveCity(tolochin);
            var yshachy = new City { Name = "Ушачи", RegionId = vitebskReg.Id }; dataProvider.SaveCity(yshachy);
            var Chashniki = new City { Name = "Чашники", RegionId = vitebskReg.Id }; dataProvider.SaveCity(Chashniki);

            var brest = new City { Name = "Брест", RegionId = brestReg.Id }; dataProvider.SaveCity(brest);
            var Baranovichy = new City { Name = "Барановичи", RegionId = brestReg.Id }; dataProvider.SaveCity(Baranovichy);
            var Gorodische = new City { Name = "Городище", RegionId = brestReg.Id }; dataProvider.SaveCity(Gorodische);
            var Bereza = new City { Name = "Береза", RegionId = brestReg.Id }; dataProvider.SaveCity(Bereza);
            var Beloozerst = new City { Name = "Белоозеоск", RegionId = brestReg.Id }; dataProvider.SaveCity(Beloozerst);
            var Domachevo = new City { Name = "Домачево", RegionId = brestReg.Id }; dataProvider.SaveCity(Domachevo);
            var Gancevichy = new City { Name = "Ганцевичи", RegionId = brestReg.Id }; dataProvider.SaveCity(Gancevichy);
            var Drogichin = new City { Name = "Дрогичин", RegionId = brestReg.Id }; dataProvider.SaveCity(Drogichin);
            var Antipol = new City { Name = "Антополь", RegionId = brestReg.Id }; dataProvider.SaveCity(Antipol);
            var Gabinka = new City { Name = "Жабинка", RegionId = brestReg.Id }; dataProvider.SaveCity(Gabinka);
            var Ivanovo = new City { Name = "Ивано", RegionId = brestReg.Id }; dataProvider.SaveCity(Ivanovo);
            var Ivacevichy = new City { Name = "Ивацевичи", RegionId = brestReg.Id }; dataProvider.SaveCity(Ivacevichy);
            var kossovo = new City { Name = "Коссово", RegionId = brestReg.Id }; dataProvider.SaveCity(kossovo);
            var kamenec = new City { Name = "Каменец", RegionId = brestReg.Id }; dataProvider.SaveCity(kamenec);
            var Visokoe = new City { Name = "Высокое", RegionId = brestReg.Id }; dataProvider.SaveCity(Visokoe);
            var kobrin = new City { Name = "Кобрин", RegionId = brestReg.Id }; dataProvider.SaveCity(kobrin);
            var luninec = new City { Name = "Луниннец", RegionId = brestReg.Id }; dataProvider.SaveCity(luninec);
            var liaxovochy = new City { Name = "Ляховичи", RegionId = brestReg.Id }; dataProvider.SaveCity(liaxovochy);
            var pinsk = new City { Name = "Пинск", RegionId = brestReg.Id }; dataProvider.SaveCity(pinsk);
            var Prugan = new City { Name = "Пружаны", RegionId = brestReg.Id }; dataProvider.SaveCity(Prugan);
            var stopin = new City { Name = "Стопин", RegionId = brestReg.Id }; dataProvider.SaveCity(stopin);

            #endregion


            var visaElectron = new CardType { Name = "Visa Electron", IconName = "/Content/Images/Cards/Visa Electron.JPG" }; dataProvider.SaveCardType(visaElectron);
            var visaClassic = new CardType { Name = "Visa Classic", IconName = "/Content/Images/Cards/Visa Classic.png" }; dataProvider.SaveCardType(visaClassic);
            var visaGold = new CardType { Name = "Visa Gold", IconName = "/Content/Images/Cards/Visa Gold.png" }; dataProvider.SaveCardType(visaGold);
            var visaPlatinum = new CardType { Name = "Visa Platinum", IconName = "/Content/Images/Cards/Visa Platinum.png" }; dataProvider.SaveCardType(visaPlatinum);
            var maestro = new CardType { Name = "Maestro", IconName = "/Content/Images/Cards/Maestro.png" }; dataProvider.SaveCardType(maestro);
            var masterCardStandard = new CardType { Name = "MasterCard Standard", IconName = "/Content/Images/Cards/MasterCard Standard.png" }; dataProvider.SaveCardType(masterCardStandard);
            var masterCardGold = new CardType { Name = "MasterCard Gold", IconName = "/Content/Images/Cards/MasterCard Gold.png" }; dataProvider.SaveCardType(masterCardGold);
            var белКарт = new CardType { Name = "БелКарт", IconName = "/Content/Images/Cards/БелКарт.png" }; dataProvider.SaveCardType(белКарт);
            var forChildren = new CardType { Name = "Для детей и подростков", IconName = "/Content/Images/Cards/For Children.png" }; dataProvider.SaveCardType(forChildren);


            SaveUser("458_prudnikova_y_n", "Jn999999", true, true, mogilev.Id,"Прудникова Юлия Николаевна", "8975646M009BP5");
            var clientUser6 = SaveUser("Karnitsky_N_D", "Tt111111", true, false, Kirovsk.Id, "Карнитский Николай Дмитриевич", "7898675M009BP3");
            var clientUser = SaveUser("Ivan.Petrov", "Ip199111", true, false, kobrin.Id, "Петров Иван Сергеевич", "77578654M007BP1");
            var clientUser1 = SaveUser("V_A_Aldanova", "Kq121312", true, false, pinsk.Id, "Алданова Валена Александровна", "0098675M009BP3" );
            var clientUser2 = SaveUser("Eliseenko", "Hy576756", true, false, Kirovsk.Id, "Елисеенко Алла Григорьевна", "0098775M089BP1");
            var clientUser3 = SaveUser("K_S_Minchyk","Hj769000", true, false, nesvig.Id, "Минчук Константиг Станислававия", "0009675M009BP3");
            var clientUser4 = SaveUser("Kulman_a_a", "er000111", true, false, lida.Id, "Кульман Анна Аркадьевна", "0096665M089BP7");
            var clientUser5 = SaveUser("A_M_Kovalenko", "Am786754", true, false, minsk.Id, "Коваленко Александр Михайлович", "0099800M009BP3");


            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser6.Id, Password = "19911991" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser6.Id, Password = "09871234" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser6.Id, Password = "89786756" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser6.Id, Password = "46793847" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser6.Id, Password = "78653210" });

            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser.Id, Password = "19911991" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser.Id, Password = "09871234" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser.Id, Password = "89786756" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser.Id, Password = "46793847" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser.Id, Password = "78653210" });

            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser1.Id, Password = "47834643" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser1.Id, Password = "78493484" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser1.Id, Password = "89786756" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser1.Id, Password = "53653434" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser1.Id, Password = "09876535" });

            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser2.Id, Password = "47389464" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser2.Id, Password = "99893043" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser2.Id, Password = "73846466" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser2.Id, Password = "23435343" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser2.Id, Password = "09997372" });

            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser3.Id, Password = "46375237" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser3.Id, Password = "09354728" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser3.Id, Password = "76452416" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser3.Id, Password = "53432217" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser3.Id, Password = "73634436" });

            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser4.Id, Password = "24363143" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser4.Id, Password = "09874634" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser4.Id, Password = "73635342" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser4.Id, Password = "12223636" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser4.Id, Password = "64744535" });

            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser5.Id, Password = "19834874" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser5.Id, Password = "09838847" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser5.Id, Password = "89786756" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser5.Id, Password = "37463923" });
            dataProvider.SaveSessionPassword(new SessionPassword { UserId = clientUser5.Id, Password = "46345252" });


            var account = new Account { UserId = clientUser.Id, Number = "2234 6745 2342 8967", OpenDate = DateTime.Now.AddMonths(-2), CloseDate = DateTime.Now.AddYears(1), Balance = 4657000, MinBalance = 0, Currency = (int)Currencies.BYR, IsOpened = true }; dataProvider.SaveAccount(account);
            dataProvider.SaveCard(new Card { AccountId = account.Id, CardTypeId = visaElectron.Id, Number = "2234 6745 2342 8967", CreateDate = DateTime.Now.AddMonths(-2), ExpireDate = DateTime.Now.AddYears(1), IsAttached = true });

            var account1 = new Account { UserId = clientUser1.Id, Number = "2234 8745 7645 8967", OpenDate = DateTime.Now.AddMonths(-2), CloseDate = DateTime.Now.AddYears(1), Balance = 8700000, MinBalance = 2000000, Currency = (int)Currencies.BYR, IsOpened = true }; dataProvider.SaveAccount(account1);
            dataProvider.SaveCard(new Card { AccountId = account1.Id, CardTypeId = visaElectron.Id, Number = "2234 8745 7645 8967", CreateDate = DateTime.Now.AddMonths(-2), ExpireDate = DateTime.Now.AddYears(1), IsAttached = true });

            var account2 = new Account { UserId = clientUser2.Id, Number = "3542 6745 8754 9878", OpenDate = DateTime.Now.AddMonths(-2), CloseDate = DateTime.Now.AddYears(1), Balance = 6786000, MinBalance = 0, Currency = (int)Currencies.BYR, IsOpened = true }; dataProvider.SaveAccount(account2);
            dataProvider.SaveCard(new Card { AccountId = account2.Id, CardTypeId = visaClassic.Id, Number = "3542 6745 8754 9878", CreateDate = DateTime.Now.AddMonths(-2), ExpireDate = DateTime.Now.AddYears(1), IsAttached = true });
            var account2_1 = new Account { UserId = clientUser2.Id, Number = "3542 6745 8754 8967", OpenDate = DateTime.Now.AddMonths(-2), CloseDate = DateTime.Now.AddYears(1), Balance = 67543220, MinBalance = 0, Currency = (int)Currencies.BYR, IsOpened = true }; dataProvider.SaveAccount(account2_1);
            dataProvider.SaveCard(new Card { AccountId = account2_1.Id, CardTypeId = visaElectron.Id, Number = "3542 6745 8754 8967", CreateDate = DateTime.Now.AddMonths(-2), ExpireDate = DateTime.Now.AddYears(1), IsAttached = true });

            var account3 = new Account { UserId = clientUser3.Id, Number = "3892 3424 8754 1765", OpenDate = DateTime.Now.AddMonths(-2), CloseDate = DateTime.Now.AddYears(1), Balance = 675000, MinBalance = 0, Currency = (int)Currencies.BYR, IsOpened = true }; dataProvider.SaveAccount(account3);
            dataProvider.SaveCard(new Card { AccountId = account3.Id, CardTypeId = visaClassic.Id, Number = "3544 3424 8754 1765", CreateDate = DateTime.Now.AddMonths(-2), ExpireDate = DateTime.Now.AddYears(1), IsAttached = true });

            var account4 = new Account { UserId = clientUser4.Id, Number = "4545 7865 8754 1765", OpenDate = DateTime.Now.AddMonths(-2), CloseDate = DateTime.Now.AddYears(1), Balance = 6432100, MinBalance = 0, Currency = (int)Currencies.BYR, IsOpened = true }; dataProvider.SaveAccount(account4);
            dataProvider.SaveCard(new Card { AccountId = account4.Id, CardTypeId = visaElectron.Id, Number = "4545 7865 8754 1765", CreateDate = DateTime.Now.AddMonths(-2), ExpireDate = DateTime.Now.AddYears(1), IsAttached = true });

            var account5 = new Account { UserId = clientUser5.Id, Number = "8978 1232 8754 6745", OpenDate = DateTime.Now.AddMonths(-2), CloseDate = DateTime.Now.AddYears(1), Balance = 7896540, MinBalance = 0, Currency = (int)Currencies.BYR, IsOpened = true }; dataProvider.SaveAccount(account5);
            dataProvider.SaveCard(new Card { AccountId = account5.Id, CardTypeId = белКарт.Id, Number = "8978 1232 8754 6745", CreateDate = DateTime.Now.AddMonths(-2), ExpireDate = DateTime.Now.AddYears(1), IsAttached = true });

            var account6 = new Account { UserId = clientUser6.Id, Number = "9088 1232 9876 9087", OpenDate = DateTime.Now.AddMonths(-2), CloseDate = DateTime.Now.AddYears(1), Balance = 7896540, MinBalance = 0, Currency = (int)Currencies.BYR, IsOpened = true }; dataProvider.SaveAccount(account6);
            dataProvider.SaveCard(new Card { AccountId = account6.Id, CardTypeId = белКарт.Id, Number = "9088 1232 9876 9087", CreateDate = DateTime.Now.AddMonths(-2), ExpireDate = DateTime.Now.AddYears(1), IsAttached = true });

            
            var erip = new ServiceCategory { Name = "Система \"Расчет\" (ЕРИП)", ServiceCategoryId = null }; dataProvider.SaveServiceCategory(erip);
                var beltel = new ServiceCategory { Name = "БЕЛТЕЛЕКОМ", ServiceCategoryId = erip.Id }; dataProvider.SaveServiceCategory(beltel);
                SaveBeltel(dataProvider, beltel, mogilevReg.Id, "Могилев и Могилевская область", new [] {
                    new CityTelephone(mogilev.Id, "222", "999999"),
                    new CityTelephone(belinichy.Id, "2232", "99999"),
                    new CityTelephone(bobruisk.Id, "2251", "99999"),
                    new CityTelephone(bixov.Id, "2231", "99999"),
                    new CityTelephone(glusk.Id, "2230", "99999"),
                    new CityTelephone(gorki.Id, "2233", "99999"),
                    new CityTelephone(dribin.Id, "2248", "99999"),
                    new CityTelephone(Kirovsk.Id, "2237", "99999"),
                    new CityTelephone(Klimovichy.Id, "2244", "99999"),
                    new CityTelephone(Klichev.Id, "2241", "99999"),
                    new CityTelephone(Kostukovichy.Id, "2245", "99999"),
                    new CityTelephone(Krasnopolie.Id, "2238", "99999"),
                    new CityTelephone(Krichev.Id, "2241", "99999"),
                    new CityTelephone(Mstislavl.Id, "2240", "99999"),
                    new CityTelephone(Osipovichy.Id, "2235", "99999"),
                    new CityTelephone(Slavgorod.Id, "2246", "99999"),
                    new CityTelephone(Xotimsk.Id, "2247", "99999"),
                    new CityTelephone(Chays.Id, "2242", "99999"),
                    new CityTelephone(Cherikov.Id, "2243", "99999"),
                    new CityTelephone(Shklov.Id, "2239", "99999")
                                    });
                SaveBeltel(dataProvider, beltel, minskReg.Id, "Минск и Минская область", "17", "9999999");
                SaveBeltel(dataProvider, beltel, brestReg.Id, "Брест и Брестская область", "162", "999999");
                SaveBeltel(dataProvider, beltel, vitebskReg.Id, "Витебск и Витебск область", "212", "999999");
                SaveBeltel(dataProvider, beltel, gomelReg.Id, "Гомель и Гомельская область", "232", "999999");
                SaveBeltel(dataProvider, beltel, grodnoReg.Id, "Гродно и Голдненская область", "152", "999999");
                    //var minskObl = new ServiceCategory { Name = "Могилев и Могилевская область", ServiceCategoryId = beltel.Id }; dataProvider.SaveServiceCategory(mogilevObl);
                    //    var internet = new Service { Name = "Интернет (BYFLY, ZALA, Максифон)", ServiceCategoryId = mogilevObl.Id, ScriptName = "BeltelecomInternetViewModel", TemplateName = "BeltelecomInternetTemplate" }; dataProvider.SaveService(internet);
                    //    var telephone = new Service { Name = "Телефон", ServiceCategoryId = mogilevObl.Id, ScriptName = "TelephoneViewModel", TemplateName = "TelephoneTemplate" }; dataProvider.SaveService(telephone);
                    //var mogilevObl = new ServiceCategory { Name = "Могилев и Могилевская область", ServiceCategoryId = beltel.Id }; dataProvider.SaveServiceCategory(mogilevObl);

            var mobileTel = new ServiceCategory { Name = "Мобильный телефон", ServiceCategoryId = null }; dataProvider.SaveServiceCategory(mobileTel);
                var mts = new Service { Name = "МТС", ServiceCategoryId = mobileTel.Id, ScriptName = "TelephoneViewModel", TemplateName = "TelephoneTemplate" }; dataProvider.SaveService(mts);
                var life = new Service { Name = "life:)", ServiceCategoryId = mobileTel.Id, ScriptName = "TelephoneViewModel", TemplateName = "TelephoneTemplate" }; dataProvider.SaveService(life);
                var velcom = new Service { Name = "VELCOM", ServiceCategoryId = mobileTel.Id, ScriptName = "TelephoneViewModel", TemplateName = "TelephoneTemplate" }; dataProvider.SaveService(velcom);

            var communal = new ServiceCategory { Name = "Коммунальные", ServiceCategoryId = null }; dataProvider.SaveServiceCategory(communal);
                var common = new Service { Name = "Общие", ServiceCategoryId = communal.Id, ScriptName = "CommunalPaymentsViewModel", TemplateName = "CommunalPaymentsTemplate" }; dataProvider.SaveService(common);
                var water = new Service { Name = "Водоснабжение", ServiceCategoryId = communal.Id, ScriptName = "WaterServiceViewModel", TemplateName = "WaterElectricServiceTemplate" }; dataProvider.SaveService(water);
                var electric = new Service { Name = "Электричество", ServiceCategoryId = communal.Id, ScriptName = "ElectricServiceViewModel", TemplateName = "WaterElectricServiceTemplate" }; dataProvider.SaveService(electric);
                var capRemont = new Service { Name = "Капитальный ремонт", ServiceCategoryId = communal.Id, ScriptName = "CommunalPaymentsViewModel", TemplateName = "CommunalPaymentsTemplate" }; dataProvider.SaveService(capRemont);
                var gas = new Service { Name = "Газ", ServiceCategoryId = communal.Id, ScriptName = "CommunalPaymentsViewModel", TemplateName = "CommunalPaymentsTemplate" }; dataProvider.SaveService(gas);


            dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = mts.Id, Name = "29", ValueType = (int)ServiceParameterValueTypes.String, DefaultValue = "[2578]999999" });
            dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = mts.Id, Name = "33", ValueType = (int)ServiceParameterValueTypes.String, DefaultValue = "9999999" });

            dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = life.Id, Name = "25", ValueType = (int)ServiceParameterValueTypes.String, DefaultValue = "9999999" });

            dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = velcom.Id, Name = "29", ValueType = (int)ServiceParameterValueTypes.String, DefaultValue = "[1369]999999" });
            dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = velcom.Id, Name = "44", ValueType = (int)ServiceParameterValueTypes.String, DefaultValue = "9999999" });

            dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = common.Id, Name = "Mask", ValueType = (int)ServiceParameterValueTypes.String, DefaultValue = "999999999" });

            dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = water.Id, Name = "Mask", ValueType = (int)ServiceParameterValueTypes.String, DefaultValue = "999999999" });
            dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = water.Id, Name = "Tariff", ValueType = (int)ServiceParameterValueTypes.Int, DefaultValue = "15000" });

            dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = electric.Id, Name = "Mask", ValueType = (int)ServiceParameterValueTypes.String, DefaultValue = "999999999" });
            dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = electric.Id, Name = "150", ValueType = (int)ServiceParameterValueTypes.Int, DefaultValue = "955" });
            dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = electric.Id, Name = "300", ValueType = (int)ServiceParameterValueTypes.Int, DefaultValue = "1240" });
            dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = electric.Id, Name = "999999", ValueType = (int)ServiceParameterValueTypes.Int, DefaultValue = "1470" });

            dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = capRemont.Id, Name = "Mask", ValueType = (int)ServiceParameterValueTypes.String, DefaultValue = "999999999" });

            dataProvider.SaveServiceParameter(new ServiceParameter { ServiceId = gas.Id, Name = "Mask", ValueType = (int)ServiceParameterValueTypes.String, DefaultValue = "999999999" });

            return RedirectToAction("Index", "Home");
        }
#endif
    }
}
