using EfDao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBank.Dal
{
    public class CardTypeDa : DaoBase<CardType, int>
    {
        public override void Save(CardType cardType)
        {
            base.Save(cardType);
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

        public void Delete(int cardTypeId)
        {
            Delete(GetById(cardTypeId));
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }
    }
}
