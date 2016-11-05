using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EfDao
{
    public interface ISortableDataProvider<TEntity, TId> : IDataProvider<TEntity, TId>
        where TEntity : class, IPersistentObject<TId>, new()
        where TId : struct
    {
        /// <summary>
        /// Adds the sort param.
        /// </summary>
        /// <param name="param">The param.</param>
        void AddSortParam(SortParam param);
        /// <summary>
        /// Adds the sort param.
        /// </summary>
        /// <param name="param">The param.</param>
        void AddSortParam(ICollection<SortParam> param);
        /// <summary>
        /// Clears the sort param.
        /// </summary>
        void ClearSortParam();

        /// <summary>
        /// Adds the filter param.
        /// </summary>
        /// <param name="param">The param.</param>
        void AddFilterParam(FilterParam param);
        /// <summary>
        /// Adds the filter param.
        /// </summary>
        /// <param name="param">The param.</param>
        void AddFilterParam(ICollection<FilterParam> param);
        /// <summary>
        /// Clears the filter param.
        /// </summary>
        void ClearFilterParam();

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        IList<TEntity> Get();
        /// <summary>
        /// Gets the entities according to the specified filter and sort parameters
        /// </summary>
        /// <param name="filterSortParam">The filter and sort parameters.</param>
        /// <returns>
        /// Entity list. Can be empty. Null value is never returned
        /// </returns>
        /// <remarks>
        /// Stateless variant of the <see cref="Get()"/> method.
        /// </remarks>
        IList<TEntity> Get(FilterSortParam filterSortParam);
        /// <summary>
        /// Gets the entities according to the specified filter and sort parameters.
        /// </summary>
        /// <param name="filterSortParam">The filter and sort parameters.</param>
        /// <param name="totalRowCount">The total row count in the repository according to the passed.</param>
        /// <returns>
        /// Entity list. Can be empty. Null value is never returned
        /// </returns>
        /// <remarks>
        /// Stateless variant of the <see cref="Get()"/> method with paging support.
        /// </remarks>
        IList<TEntity> Get(FilterSortParam filterSortParam, out long totalRowCount);

        /// <summary>
        /// Gets or sets the start record.
        /// </summary>
        /// <value>
        /// The start record.
        /// </value>
        int StartRecord { set; get; }
        /// <summary>
        /// Gets or sets the max records.
        /// </summary>
        /// <value>
        /// The max records.
        /// </value>
        int MaxRecords { set; get; }
    }
}
