using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class TypeElement : ConfigurationElement
    {
        #region const

        private const string TypeAttribute = "type";
        private const string MethodAttribute = "method";

        #endregion

        #region prop

        /// <summary>
        /// Gets or sets the type of the class.
        /// </summary>
        /// <value>
        /// The type of the class.
        /// </value>
        [ConfigurationProperty(TypeAttribute, IsRequired = true)]
        public string ClassType
        {
            get { return (string)this[TypeAttribute]; }
            set { this[TypeAttribute] = value; }
        }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        [ConfigurationProperty(MethodAttribute, IsRequired = false)]
        public string Method
        {
            get { return (string)this[MethodAttribute]; }
            set { this[MethodAttribute] = value; }
        }

        #endregion
    }
}
