using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBank.Dal
{
    public interface IDataProvider
    {
        ServiceCategory GetServiceCategory(int categoryId);
        Service GetService(int serviceId);
        void SaveServiceCategory(ServiceCategory category);
        void SaveService(Service service);
        void DeleteServiceCategory(int categoryId);
        void DeleteService(int serviceId);
        ICollection<ServiceCategory> GetRootServiceCategories();
        ICollection<ServiceCategory> GetServiceCategories();
        ICollection<Service> GetRootServices();

        ICollection<ServiceParameter> GetServiceParameters(int serviceId);
        ICollection<ServiceParameter> GetServiceParameters(int? accountId, int serviceId);
        void SaveServiceParameter(ServiceParameter serviceParam);
        void DeleteServiceParameter(int serviceParamId);
        ServiceParameter GetServiceParameter(int serviceParamId);
        ServiceParameter GetServiceParameterByName(int serviceId, string serviceParamName);

        ICollection<CardType> GetCardTypes();
        CardType GetCardType(int cardTypeId);
        void SaveCardType(CardType cardType);
        void DeleteCardType(int cardTypeId);
        ICollection<Card> GetCards(int accountId);
        Card GetCard(int cardId);
        void SaveCard(Card card);
        void DeleteCard(int cardId);

        ICollection<Account> GetAccounts(int userId);
        Account GetAccount(int accountId);
        void SaveAccount(Account account);
        void DeleteAccount(int accountId);

        ICollection<Region> GetRegions();
        ICollection<City> GetCities(int regionId);
        void SaveRegion(Region region);
        void SaveCity(City city);
        Region GetRegion(int regionId);
        City GetCity(int cityId);
        void DeleteRegion(int regionId);
        void DeleteCity(int cityId);

        Locality GetLocality(int localityId);
        void SaveLocality(Locality locality);
        void DeleteLocality(int localityId);

        ICollection<SessionPassword> GetSessionPasswords(int userId);
        SessionPassword GetSessionPassword(int sessionPasswordId);
        void SaveSessionPassword(SessionPassword sessionPassword);
        void DeleteSessionPassword(int sessionPasswordId);

        void SaveOperation(Operation operation);
        ICollection<Operation> GetOperations(int userId);
    }
}
