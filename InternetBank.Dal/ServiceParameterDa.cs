using EfDao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBank.Dal
{
    class ServiceParameterDa : DaoBase<ServiceParameter, int>
    {
        public override void Save(ServiceParameter param)
        {
            base.Save(param);
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

        public void Delete(int paramId)
        {
            Delete(GetById(paramId));
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

    }
}
