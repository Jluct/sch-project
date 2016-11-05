using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace InternetBank.Dal
{
    public class UserManagerConfigurationSection : ConfigurationSection
    {
        private static UserManagerConfigurationSection _instance;
        
        private const string SectionName = "userManagerConfig";
        private const string UserManagerElement = "userManager";

        [ConfigurationProperty(UserManagerElement)]
        public TypeElement UserManager
        {
            get { return (TypeElement)this[UserManagerElement]; }
            set { base[UserManagerElement] = value; }
        }

        #region util

        public static UserManagerConfigurationSection GetSection()
        {
            return _instance ?? (_instance = (UserManagerConfigurationSection)ConfigurationHelper.GetSection(SectionName));
        }

        #endregion util
    }
}
