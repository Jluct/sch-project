using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InternetBank.Dal
{
    public static class DataProviderFactory
    {
        private static IDataProvider _dataProvider;

        public static IDataProvider Get()
        {
            return Get(DataProviderConfigurationSection.GetSection());
        }

        public static IDataProvider Get(DataProviderConfigurationSection config)
        {
            return _dataProvider ?? (_dataProvider = Create(config));
        }

        private static IDataProvider Create(DataProviderConfigurationSection config)
        {
            return TypeInitializer.GetType<IDataProvider>(config.DataProvider, "DataProvider", config);
        }
    }
}
