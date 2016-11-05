using EfDao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBank.Dal
{
    class ServiceCategoryDa : DaoBase<ServiceCategory, int>
    {
        private ServiceDa _serviceDa = new ServiceDa();
        private LocalityDa _localityDa = new LocalityDa();

        public override void Save(ServiceCategory serviceCategory)
        {
            base.Save(serviceCategory);
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

        public void Delete(int serviceCategoryId)
        {
            Delete(GetById(serviceCategoryId));
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

        private void ExtendServiceCategory(ServiceCategory category)
        {
            category.Services = _serviceDa.GetCategoryServices(category.Id);
            if (category.LocalityId.HasValue)
                category.Locality = _localityDa.GetById(category.LocalityId.Value);
            foreach (var categ in category.ServiceCategories)
                ExtendServiceCategory(categ);
        }

        public ICollection<ServiceCategory> GetRootServiceCategories()
        {
            var categs = GetAll().Where(c => c.Parent == null).ToList();
            foreach (var categ in categs)
                ExtendServiceCategory(categ);
            return categs;
        }

        public ICollection<ServiceCategory> GetServiceCategories()
        {
            var categs = GetAll().ToList();
            foreach (var categ in categs)
                ExtendServiceCategory(categ);
            return categs;
        }
    }
}
