using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EfDao
{
    public interface IPersistentObject<TId> : IEquatable<IPersistentObject<TId>> where TId : struct
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        TId Id { get; set; }
    }
}
