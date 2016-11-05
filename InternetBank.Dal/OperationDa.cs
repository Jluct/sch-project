using EfDao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBank.Dal
{
    class OperationDa : DaoBase<Operation, int>
    {
        public override void Save(Operation operation)
        {
            base.Save(operation);
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

        public void Delete(int operationId)
        {
            Delete(GetById(operationId));
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

        public IList<Operation> GetOperations(int userId)
        {
            return Repository.Include("Card.Account").Include("Card.CardType").Include("Service").Where(o => o.Card.Account.UserId == userId).ToList();
        }
    }
}
