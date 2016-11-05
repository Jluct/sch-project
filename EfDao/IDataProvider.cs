using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EfDao
{
    public interface IDataProvider<TEntity, in TId>
        where TEntity : IPersistentObject<TId>
        where TId : struct
    {
        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        TEntity GetById(TId id);
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        IList<TEntity> GetAll();
        /// <summary>
        /// Gets the count.
        /// </summary>
        long Count { get; }

        /// <summary>
        /// Gets the by field.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns></returns>
        IList<TEntity> GetByField(string fieldName, object fieldValue);
        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Save(TEntity entity);
        /// <summary>
        /// Saves the specified entity list.
        /// </summary>
        /// <param name="entityList">The entity list.</param>
        void Save(ICollection<TEntity> entityList);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Delete(TEntity entity);
        /// <summary>
        /// Deletes the specified entity list.
        /// </summary>
        /// <param name="entityList">The entity list.</param>
        void Delete(ICollection<TEntity> entityList);
    }
}
