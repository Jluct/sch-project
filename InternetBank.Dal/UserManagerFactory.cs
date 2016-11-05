using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBank.Dal
{
    public static class UserManagerFactory
    {
        private static IUserManager _userManager;

        public static IUserManager Get()
        {
            return Get(UserManagerConfigurationSection.GetSection());
        }

        public static IUserManager Get(UserManagerConfigurationSection config)
        {
            return _userManager ?? (_userManager = Create(config));
        }

        private static IUserManager Create(UserManagerConfigurationSection config)
        {
            return TypeInitializer.GetType<IUserManager>(config.UserManager, "UserManager", config);
        }

    }
}
