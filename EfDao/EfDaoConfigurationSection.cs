using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace EfDao
{
    public class EfDaoConfigurationSection : ConfigurationSection
    {
        #region const

        public const string SectionName = "entityFrameworkConfig";

        private const string ConnectionStringKeyAttribute = "connectionStringKey";
        private const string ContainerNameAttribute = "containerName";

        #endregion

        #region prop

        [ConfigurationProperty(ConnectionStringKeyAttribute, IsRequired = true)]
        public string ConnectionStringKey
        {
            get { return (string)this[ConnectionStringKeyAttribute]; }
            set { this[ConnectionStringKeyAttribute] = value; }
        }

        [ConfigurationProperty(ContainerNameAttribute, IsRequired = true)]
        public string ContainerName
        {
            get { return (string)this[ContainerNameAttribute]; }
            set { this[ContainerNameAttribute] = value; }
        }

        #endregion

        #region util

        public static EfDaoConfigurationSection GetSection()
        {
            return (EfDaoConfigurationSection)ConfigurationHelper.GetSection(SectionName);
        }

        #endregion
    }
}
