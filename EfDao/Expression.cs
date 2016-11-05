using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfDao
{
    public enum DaoExpression
    {
        /// <summary>
        /// Equal.
        /// </summary>
        Eq = 0,

        /// <summary>
        /// Greater than.
        /// </summary>
        Gt = 1,

        /// <summary>
        /// Less than.
        /// </summary>
        Lt = 2,

        /// <summary>
        /// Greater than or equal.
        /// </summary>
        GtEq = 3,

        /// <summary>
        /// Less than or equal.
        /// </summary>
        LtEq = 4,

        /// <summary>
        /// Not equal.
        /// </summary>
        NotEq = 5,

        /// <summary>
        /// Contains.
        /// </summary>
        Contains = 6,

        /// <summary>
        /// Starts with.
        /// </summary>
        StartsWith = 7,

        /// <summary>
        /// Ends with.
        /// </summary>
        EndsWith = 8
    }
}
