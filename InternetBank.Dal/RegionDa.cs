using EfDao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBank.Dal
{
    public class RegionDa : DaoBase<Region, int>
    {
        public override void Save(Region region)
        {
            base.Save(region);
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

        public void Delete(int regionId)
        {
            Delete(GetById(regionId));
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }
    }
}
