using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Base object mapper interface
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public interface IMapper<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        /// <summary>
        /// Translates entity object to model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Mapped model</returns>
        TModel MapToModel(TEntity entity);

        /// <summary>
        /// Translates entity collection to model list.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns>The models list</returns>
        IList<TModel> MapToModelList(ICollection<TEntity> entities);

        /// <summary>
        /// Maps a model to entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Mapped entity</returns>
        /// <remarks>
        /// This method creates entity object by itself
        /// </remarks>
        TEntity MapToEntity(TModel model);

        /// <summary>
        /// Maps a model collection to entity collection.
        /// </summary>
        /// <param name="models">The models.</param>
        /// <returns>Mapped entity collection</returns>
        /// <remarks>
        /// This method creates entity object (and collection) by itself
        /// </remarks>
        ICollection<TEntity> MapToEntityList(ICollection<TModel> models);

        /// <summary>
        /// Maps a model to entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>Mapped entity</returns>
        /// <remarks>
        /// Doesn't create entity by itself and uses passed entity as the target.
        /// This entity reference is also used as return value (for convenience)
        /// </remarks>
        TEntity MapToEntity(TModel model, ref TEntity entity);

        /// <summary>
        /// Maps a model collection to entity collection.
        /// </summary>
        /// <param name="models">The models.</param>
        /// <param name="entities">The entities.</param>
        /// <returns>Mapped entity collection</returns>
        /// <remarks>
        /// This method doesn't create entities (and collection) by itself and uses passed entity collection as the target.
        /// This entity collection is also used as return value (for convenience)
        /// </remarks>
        ICollection<TEntity> MapToEntityList(ICollection<TModel> models, ref ICollection<TEntity> entities);
    }
}
