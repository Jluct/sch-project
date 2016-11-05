using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBank.Dal
{
    public interface IUserManager
    {
        IList<User> GetUsers();
        User GetById(int userId);
        User GetByLogin(string login);
        User GetUserByPassportNumber(string number);
        void SaveUser(User user);
        void DeleteUser(int userId);
    }
}
