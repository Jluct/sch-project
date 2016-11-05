using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfDao;

namespace InternetBank.Dal
{
    class UserDa : DaoBase<User, int>
    {
        public override void Save(User user)
        {
            base.Save(user);
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

        public void Delete(int userId)
        {
            Delete(GetById(userId));
            EfObjectContextFactory.Instance.DisposeCurrentContext();
        }

        public User GetUserByLogin(string login)
        {
            return GetByField("Login", login).SingleOrDefault();
        }

        public User GetUserByPassportNumber(string number)
        {
            return GetByField("PassportNumber", number).SingleOrDefault();
        }

        //public bool TryToLogIn(string login, string password, out IDictionary<string, string> errors)
        //{
        //    errors = new Dictionary<string, string>();
        //    return true;
        //}
        
        //public IQueryable<User> GetOnlineUsers(int sessionTimeOut)
        //{
        //    DateTime endsessionDate = DateTime.UtcNow.AddMilliseconds(-sessionTimeOut);
        //    return Repository.Where(u => u.UserSession.Any(s => s.Date >= endsessionDate));
        //}

        //public bool IsOnline(int userId, int sessionTimeOut)
        //{
        //    DateTime endsessionDate = DateTime.UtcNow.AddMilliseconds(-sessionTimeOut);
        //    return Repository.Any(u => u.Id == userId && u.UserSession.Any(s => s.Date >= endsessionDate));
        //}
    }
}
