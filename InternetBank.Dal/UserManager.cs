using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBank.Dal
{
    public class UserManager : IUserManager
    {
        private UserDa _userDa = new UserDa();

        public UserManager()
        { }

        public UserManager(UserManagerConfigurationSection config)
        { }

        public IList<User> GetUsers()
        {
            return _userDa.GetAll().ToList();
        }

        public User GetById(int userId)
        {
            return _userDa.GetById(userId);
        }

        public User GetByLogin(string login)
        {
            return _userDa.GetUserByLogin(login);
        }

        public void SaveUser(User user)
        {
            _userDa.Save(user);
        }

        public void DeleteUser(int userId)
        {
            _userDa.Delete(userId);
        }

        public User GetUserByPassportNumber(string number)
        {
            return _userDa.GetUserByPassportNumber(number);
        }
    }
}
