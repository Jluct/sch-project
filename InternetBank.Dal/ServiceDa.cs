using EfDao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBank.Dal
{
    class ServiceDa : DaoBase<Service, int>
    {
        private LocalityDa _localityDa = new LocalityDa();

        public override void Save(Service service)
        {
            base.Save(service);
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }
        
        public void Delete(int serviceId)
        {
            Delete(GetById(serviceId));
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

        private void ExtendService(Service service)
        {
            if (service.LocalityId.HasValue)
                service.Locality = _localityDa.GetById(service.LocalityId.Value);
        }

        public ICollection<Service> GetCategoryServices(int categoryId)
        {
            var services = GetByField("ServiceCategoryId", categoryId);
            foreach (var service in services)
                ExtendService(service);
            return services;
        }

        public ICollection<Service> GetRootServices()
        {
            var services = GetByField("ServiceCategoryId", null);
            foreach(var service in services)
                ExtendService(service);
            return services;
        }
    }
}
