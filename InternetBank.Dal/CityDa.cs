using EfDao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBank.Dal
{
    class CityDa : DaoBase<City, int>
    {
        public override void Save(City city)
        {
            base.Save(city);
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

        public void Delete(int cityId)
        {
            Delete(GetById(cityId));
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

    }
}
