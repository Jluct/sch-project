using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace EfDao
{
    [DataContract]
    public class SortParam
    {
        /// <summary>
        /// Ascending or descending.
        /// </summary>
        [DataMember]
        public bool Ascending { get; set; }

        /// <summary>
        /// Property name.
        /// </summary>
        [DataMember]
        public string Property { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SortParam() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="prop">Property name.</param>
        /// <param name="ascending">Is ascending.</param>
        public SortParam(string prop, bool ascending)
        {
            Property = prop;
            Ascending = ascending;
        }
    }
}
