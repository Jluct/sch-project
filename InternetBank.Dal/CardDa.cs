using EfDao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBank.Dal
{
    public class CardDa : DaoBase<Card, int>
    {
        public override void Save(Card card)
        {
            base.Save(card);
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

        public void Delete(int cardId)
        {
            Delete(GetById(cardId));
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }
    }
}
