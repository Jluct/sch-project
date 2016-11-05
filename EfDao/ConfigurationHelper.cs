using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Xml;

namespace EfDao
{
    public abstract class ConfigurationHelper
    {
        /// <summary>
        /// Gets the section.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns></returns>
        public static ConfigurationSection GetSection(string sectionName)
        {
            return GetConfiguration().GetSection(sectionName);
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <returns></returns>
        public static Configuration GetConfiguration()
        {
            Configuration configuration;
            try
            {
                configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
            catch (Exception)
            {
                var mapping = new VirtualDirectoryMapping(AppDomain.CurrentDomain.BaseDirectory, true);
                var map = new WebConfigurationFileMap();
                map.VirtualDirectories.Add("/", mapping);
                configuration = WebConfigurationManager.OpenMappedWebConfiguration(map, "/", System.Web.Hosting.HostingEnvironment.ApplicationHost.GetSiteName());
            }

            return configuration;
        }

        /// <summary>
        /// loads configuration element by xml tag name and configSource 
        /// </summary>
        /// <param name="xmlTagName">xml tag name to load</param>
        /// <param name="configSource">relative path to config file</param>
        /// <returns></returns>
        public static string LoadConfigSource(string xmlTagName, string configSource)
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configSource);
            var xml = new XmlDocument();
            xml.Load(file);
            return xml.GetElementsByTagName(xmlTagName)[0].OuterXml;
        }
    }
}
