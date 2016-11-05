using EfDao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBank.Dal
{
    public class LocalityDa : DaoBase<Locality, int>
    {
        public override void Save(Locality locality)
        {
            base.Save(locality);
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

        public void Delete(int localityId)
        {
            Delete(GetById(localityId));
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }
    }
}
