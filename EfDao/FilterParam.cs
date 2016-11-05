using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;

namespace EfDao
{
    [DataContract]
    [KnownType(typeof(int[]))]
    public class FilterParam
    {
        /// <summary>
        /// Expression.
        /// </summary>
        [DataMember]
        public DaoExpression Expression { get; set; }

        /// <summary>
        /// Property name.
        /// </summary>
        [DataMember]
        public string Property { get; set; }

        /// <summary>
        /// Value.
        /// </summary>
        [DataMember]
        public object Value { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FilterParam() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="prop">Property name.</param>
        /// <param name="expression">Expression.</param>
        public FilterParam(string prop, DaoExpression expression)
        {
            Property = prop;
            Expression = expression;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="prop">Property name.</param>
        /// <param name="expression">Expression.</param>
        /// <param name="value">Value.</param>
        public FilterParam(string prop, DaoExpression expression, object value)
            : this(prop, expression)
        {
            Value = value;
        }
    }
}
