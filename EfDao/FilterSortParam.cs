using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace EfDao
{
    [DataContract]
    public class FilterSortParam
    {
        /// <summary>
        /// Gets or sets the sort params.
        /// </summary>
        /// <value>
        /// The sort params.
        /// </value>
        [DataMember]
        public IList<SortParam> SortParams { get; set; }

        /// <summary>
        /// Gets or sets the filters.
        /// </summary>
        /// <value>
        /// The filters.
        /// </value>
        [DataMember]
        public IList<FilterParam> Filters { get; set; }

        /// <summary>
        /// zero-based record number to return
        /// </summary>
        /// <value>
        /// The start record.
        /// </value>
        [DataMember]
        public int StartRecord { get; set; }

        /// <summary>
        /// Gets or sets the max records.
        /// </summary>
        /// <value>
        /// The max records.
        /// </value>
        [DataMember]
        public int MaxRecords { get; set; }
    }
}
