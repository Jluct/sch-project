using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace InternetBank.Dal
{
    public class DataProviderConfigurationSection : ConfigurationSection
    {
        private static DataProviderConfigurationSection _instance;

        private const string SectionName = "dataProviderConfig";
        private const string DataProviderElement = "dataProvider";

        [ConfigurationProperty(DataProviderElement)]
        public TypeElement DataProvider
        {
            get { return (TypeElement)this[DataProviderElement]; }
            set { base[DataProviderElement] = value; }
        }

        #region util

        public static DataProviderConfigurationSection GetSection()
        {
            return _instance ?? (_instance = (DataProviderConfigurationSection)ConfigurationHelper.GetSection(SectionName));
        }

        #endregion util
    }
}
