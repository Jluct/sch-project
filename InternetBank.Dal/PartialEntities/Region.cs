using EfDao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBank.Dal
{
    public partial class Region : IPersistentObject<int>
    {
        public bool Equals(IPersistentObject<int> other)
        {
            return Id == other.Id;
        }
    }
}
