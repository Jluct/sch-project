using EfDao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBank.Dal
{
    public class AccountDa : DaoBase<Account, int>
    {
        public override void Save(Account account)
        {
            base.Save(account);
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

        public void Delete(int accountId)
        {
            Delete(GetById(accountId));
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

    }
}
