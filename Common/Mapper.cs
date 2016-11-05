using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum MappingDirections
    {
        /// <summary>
        /// 
        /// </summary>
        MapNone = 0,
        /// <summary>
        /// 
        /// </summary>
        MapChildren = 1,
        /// <summary>
        /// 
        /// </summary>
        MapParents = 2,
        /// <summary>
        /// 
        /// </summary>
        MapBoth = MapChildren | MapParents
    }

    /// <summary>
    /// Simple base implementation of <see cref="IMapper{TEntity,TModel}"/>.
    /// Copies only scalar properties. Collections and reference properties are skipped.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public class Mapper<TEntity, TModel> : IMapper<TEntity, TModel>
        where TEntity : class, new()
        where TModel : class, new()
    {
        /// <summary>
        /// Translates entity object to model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// Mapped model
        /// </returns>
        public virtual TModel MapToModel(TEntity entity)
        {
            return MapToModel(entity, MappingDirections.MapNone);
        }

        /// <summary>
        /// Maps to model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        public virtual TModel MapToModel(TEntity entity, MappingDirections direction)
        {
            if (entity == null)
                return null;

            var model = new TModel();
            ObjectMapper.Map(entity, model);
            return model;
        }

        /// <summary>
        /// Translates entity collection to model list.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns>
        /// The models list
        /// </returns>
        public virtual IList<TModel> MapToModelList(ICollection<TEntity> entities)
        {
            if (entities == null)
                return null;

            return entities.Select(MapToModel).Where(x => x != null).ToList();
        }

        /// <summary>
        /// Maps to model list.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        public virtual IList<TModel> MapToModelList(ICollection<TEntity> entities, MappingDirections direction)
        {
            if (entities == null)
                return null;

            return entities.Select(e => MapToModel(e, direction)).Where(x => x != null).ToList();
        }

        /// <summary>
        /// Maps a model to entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Mapped entity
        /// </returns>
        /// <remarks>
        /// This method creates entity object by itself
        /// </remarks>
        public virtual TEntity MapToEntity(TModel model)
        {
            if (model == null)
                return null;

            var entity = new TEntity();
            MapToEntity(model, ref entity);
            return entity;
        }

        /// <summary>
        /// Maps a model collection to entity collection.
        /// </summary>
        /// <param name="models">The models.</param>
        /// <returns>
        /// Mapped entity collection
        /// </returns>
        /// <remarks>
        /// This method creates entity object (and collection) by itself
        /// </remarks>
        public virtual ICollection<TEntity> MapToEntityList(ICollection<TModel> models)
        {
            if (models == null)
                return null;

            return models.Select(MapToEntity).Where(x => x != null).ToList();
        }


        /// <summary>
        /// Maps a model to entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// Mapped entity
        /// </returns>
        /// <remarks>
        /// Doesn't create entity by itself and uses passed entity as the target.
        /// This entity reference is also used as return value (for convenience)
        /// </remarks>
        public virtual TEntity MapToEntity(TModel model, ref TEntity entity)
        {
            if (model == null)
                entity = null;

            if (entity == null)
                return null;

            ObjectMapper.Map(model, entity);

            return entity;
        }

        /// <summary>
        /// Maps a model collection to entity collection.
        /// </summary>
        /// <param name="models">The models.</param>
        /// <param name="entities">The entities.</param>
        /// <returns>
        /// Mapped entity collection
        /// </returns>
        /// <remarks>
        /// This method doesn't create entities (and collection) by itself and uses passed entity collection as the target.
        /// This entity collection is also used as return value (for convenience)
        /// </remarks>
        public virtual ICollection<TEntity> MapToEntityList(ICollection<TModel> models, ref ICollection<TEntity> entities)
        {
            if (models == null)
                entities = null;

            if (entities == null)
                return null;

            if (models.Count != entities.Count)
                throw new ArgumentException("Models & entities count is not equal");

            var modelEnumerator = models.GetEnumerator();
            var entityEnumerator = entities.GetEnumerator();

            while (modelEnumerator.MoveNext() && entityEnumerator.MoveNext())
            {
                var model = modelEnumerator.Current;
                var entity = entityEnumerator.Current;

                if (model == null)
                    throw new ArgumentException("Model shouldn't be null");
                if (entity == null)
                    throw new ArgumentException("Entity shouldn't be null");

                MapToEntity(model, ref entity);
            }

            //for (int index = 0; index < models.Count; index++)
            //{
            //    var model = models[index];
            //    var entity = entities[index];
            //    MapToEntity(model, ref entity);
            //}

            return entities;
        }
    }
}
