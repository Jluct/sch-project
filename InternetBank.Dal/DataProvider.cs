using EfDao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBank.Dal
{
    public class DataProvider : IDataProvider
    {
        private ServiceCategoryDa _serviceCategoryDa = new ServiceCategoryDa();
        private IUserManager _userManager = UserManagerFactory.Get();
        private ServiceDa _serviceDa = new ServiceDa();
        private CardDa _cardDa = new CardDa();
        private AccountDa _accountDa = new AccountDa();
        private ServiceParameterDa _serviceParametersDa = new ServiceParameterDa();
        private RegionDa _regionDa = new RegionDa();
        private CityDa _cityDa = new CityDa();
        private LocalityDa _localityDa = new LocalityDa();
        private CardTypeDa _cardTypeDa = new CardTypeDa();
        private SessionPasswordDa _sessionPasswordDa = new SessionPasswordDa();
        private OperationDa _operationDa = new OperationDa();

        public DataProvider()
        { }

        public DataProvider(DataProviderConfigurationSection config)
        { }

        public ICollection<ServiceCategory> GetRootServiceCategories()
        {
            return _serviceCategoryDa.GetRootServiceCategories().ToList();
        }
        public ICollection<ServiceCategory> GetServiceCategories()
        {
            return _serviceCategoryDa.GetServiceCategories().ToList();
        }
        public ICollection<Service> GetRootServices()
        {
            return _serviceDa.GetRootServices().ToList();
        }
        public ServiceCategory GetServiceCategory(int categoryId)
        {
            return _serviceCategoryDa.GetById(categoryId);
        }
        public Service GetService(int serviceId)
        {
            return _serviceDa.GetById(serviceId);
        }
        public void SaveServiceCategory(ServiceCategory category)
        {
            _serviceCategoryDa.Save(category);
        }
        public void SaveService(Service service)
        {
            _serviceDa.Save(service);
        }
        public void DeleteServiceCategory(int categoryId)
        {
            _serviceCategoryDa.Delete(categoryId);
        }
        public void DeleteService(int serviceId)
        {
            _serviceDa.Delete(serviceId);
        }



        public ICollection<Card> GetCards(int accountId)
        {
            try
            {
                return _cardDa.GetByField("AccountId", accountId).ToList();
            }
            finally
            {
                EfObjectContextFactory.Instance.DisposeCurrentContext();
            }
        }
        public void SaveCard(Card card)
        {
            _cardDa.Save(card);
        }
        public Card GetCard(int cardId)
        {
            return _cardDa.GetById(cardId);
        }
        public ICollection<CardType> GetCardTypes()
        {
            return _cardTypeDa.GetAll().ToList();
        }
        public CardType GetCardType(int cardTypeId)
        {
            return _cardTypeDa.GetById(cardTypeId);
        }
        public void SaveCardType(CardType cardType)
        {
            _cardTypeDa.Save(cardType);
        }
        public void DeleteCardType(int cardTypeId)
        {
            _cardTypeDa.Delete(cardTypeId);
        }
        public void DeleteCard(int cardId)
        {
            _cardDa.Delete(cardId);
        }




        public ICollection<Account> GetAccounts(int userId)
        {
            return _accountDa.GetByField("UserId", userId).ToList();
        }
        public Account GetAccount(int accountId)
        {
            return _accountDa.GetById(accountId);
        }
        public void SaveAccount(Account account)
        {
            _accountDa.Save(account);
        }
        public void DeleteAccount(int accountId)
        {
            _accountDa.Delete(accountId);
        }







        public ICollection<ServiceParameter> GetServiceParameters(int serviceId)
        {
            return _serviceParametersDa.GetByField("ServiceId", serviceId).ToList();
        }
        public ICollection<ServiceParameter> GetServiceParameters(int? accountId, int serviceId)
        {
            return GetServiceParameters(serviceId).Where(p => p.AccountId == accountId).ToList();
        }
        public void SaveServiceParameter(ServiceParameter serviceParam)
        {
            _serviceParametersDa.Save(serviceParam);
        }
        public ServiceParameter GetServiceParameter(int serviceParamId)
        {
            return _serviceParametersDa.GetById(serviceParamId);
        }
        public ServiceParameter GetServiceParameterByName(string serviceParamName)
        {
            return _serviceParametersDa.GetByField("Name", serviceParamName).FirstOrDefault();
        }
        public void DeleteServiceParameter(int serviceParamId)
        {
            _serviceParametersDa.Delete(serviceParamId);
        }
        public ServiceParameter GetServiceParameterByName(int serviceId, string serviceParamName)
        {
            return GetServiceParameters(serviceId).SingleOrDefault(s => s.Name == serviceParamName);
        }




        public ICollection<Region> GetRegions()
        {
            return _regionDa.GetAll().ToList();
        }
        public ICollection<City> GetCities(int regionId)
        {
            if (GetRegion(regionId) != null)
                return _cityDa.GetByField("RegionId", regionId).ToList();
            else
                return _cityDa.GetAll().ToList();
        }
        public void SaveRegion(Region region)
        {
            _regionDa.Save(region);
        }
        public void SaveCity(City city)
        {
            _cityDa.Save(city);
        }
        public Region GetRegion(int regionId)
        {
            try
            {
                return _regionDa.GetById(regionId);
            }
            finally
            {
                EfObjectContextFactory.Instance.DisposeCurrentContext();
            }
        }
        public City GetCity(int cityId)
        {
            try
            {
                return _cityDa.GetById(cityId);
            }
            finally
            {
                EfObjectContextFactory.Instance.DisposeCurrentContext();
            }
        }
        public Locality GetLocality(int localityId)
        {
            return _localityDa.GetById(localityId);
        }
        public void SaveLocality(Locality locality)
        {
            _localityDa.Save(locality);
        }
        public void DeleteLocality(int localityId)
        {
            _localityDa.Delete(localityId);
        }
        public void DeleteRegion(int regionId)
        {
            _regionDa.Delete(regionId);
        }
        public void DeleteCity(int cityId)
        {
            _cityDa.Delete(cityId);
        }




        public SessionPassword GetSessionPassword(int sessionPasswordId)
        {
            return _sessionPasswordDa.GetById(sessionPasswordId);
        }
        public void SaveSessionPassword(SessionPassword sessionPassword)
        {
            _sessionPasswordDa.Save(sessionPassword);
        }
        public void DeleteSessionPassword(int sessionPasswordId)
        {
            _sessionPasswordDa.Delete(sessionPasswordId);
        }
        public ICollection<SessionPassword> GetSessionPasswords(int userId)
        {
            return _sessionPasswordDa.GetByField("UserId", userId).ToList();
        }



        public void SaveOperation(Operation operation)
        {
            _operationDa.Save(operation);
        }
        public ICollection<Operation> GetOperations(int userId)
        {
            return _operationDa.GetOperations(userId);
        }


    }
}
