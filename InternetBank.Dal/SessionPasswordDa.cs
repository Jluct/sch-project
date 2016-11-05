using EfDao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBank.Dal
{
    class SessionPasswordDa : DaoBase<SessionPassword, int>
    {
        public override void Save(SessionPassword sessionPassword)
        {
            base.Save(sessionPassword);
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

        public void Delete(int sessionPasswordId)
        {
            Delete(GetById(sessionPasswordId));
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }
    }
}
