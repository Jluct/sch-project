using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EfDao
{
    public class DaoBase<TEntity, TId> : ISortableDataProvider<TEntity, TId>
            where TEntity : class, IPersistentObject<TId>, new()
            where TId : struct, IComparable<TId>, IEquatable<TId>
    {
        #region ctor

        protected DaoBase()
            : this(null)
        { }

        protected DaoBase(string containerName)
        {
            _containerName = containerName;
            _threadLocalRepository = new ThreadLocal<ObjectSet<TEntity>>(CreateRepository);
        }

        #endregion

        public void LoadProperty(TEntity entity, string property)
        {
            Context.LoadProperty(entity, property);
        }

        private string RepositoryName
        {
            get
            {
                return Context.GetEntitySetFullName(typeof(TEntity).Name);
            }
        }

        protected ObjectContext Context
        {
            get { return EfObjectContextFactory.Instance.GetCurrentContext(_containerName); }
        }

        private readonly string _containerName;
        private readonly ThreadLocal<ObjectSet<TEntity>> _threadLocalRepository;
        protected ObjectSet<TEntity> Repository
        {
            get
            {
                if (_threadLocalRepository.Value.Context != Context)
                {
                    _threadLocalRepository.Value = CreateRepository();
                }
                return _threadLocalRepository.Value;
            }
        }

        #region Implementation of IDataProvider<TEntity,TId>

        public virtual IList<TEntity> GetByField(string fieldName, object fieldValue)
        {
            if (fieldValue == null)
                return Repository.Where(string.Format("it.{0} IS NULL", fieldName)).ToList();
            return Repository
                .Where(string.Format("it.{0} == @{0}", fieldName), new ObjectParameter(fieldName, fieldValue))
                .ToList();
        }

        public virtual TEntity GetById(TId id)
        {
            ObjectStateEntry entityState;
            TEntity resultEntity;

            var entitykey = new EntityKey(Context.DefaultContainerName + "." + RepositoryName, "Id", id);//Context.CreateEntityKey(_repositoryName, entity);
            bool isEntityAttached = Context.ObjectStateManager.TryGetObjectStateEntry(entitykey, out entityState);

            if (isEntityAttached && entityState.Entity != null)
            {
                resultEntity = (TEntity)entityState.Entity;
            }
            else
            {
                var query = from entity in Repository where entity.Id.Equals(id) select entity;
                resultEntity = query.FirstOrDefault();
            }
            return resultEntity;
        }

        public virtual TEntity GetByIdWith(TId id, params string[] include)
        {
            ObjectQuery<TEntity> src = include.Aggregate<string, ObjectQuery<TEntity>>(Repository, (current, i) => current.Include(i));
            return src.FirstOrDefault(g => g.Id.Equals(id));
        }

        public virtual IList<TEntity> GetAll()
        {
            var query = from entity in Repository select entity;
            var allEntities = query.ToList();
            return allEntities;
        }

        public virtual long Count
        {
            get { return Repository.LongCount(); }
        }

        public virtual void Save(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            AttachEntityIfNeeded(entity);


            Context.SaveChanges();
        }

        public virtual void Save(ICollection<TEntity> entityList)
        {
            if (entityList == null)
                throw new ArgumentNullException("entityList");

            foreach (var entity in entityList)
            {
                AttachEntityIfNeeded(entity);
            }


            Context.SaveChanges();
        }

        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
                return;

            Repository.DeleteObject(entity);

            Context.SaveChanges();
        }

        public virtual void Delete(ICollection<TEntity> entityList)
        {
            if (entityList == null || entityList.Count == 0)
                return;

            foreach (var entity in entityList)
            {
                Repository.DeleteObject(entity);
            }


            Context.SaveChanges();
        }

        #endregion

        #region Implementation of ISortableDataProvider<TEntity,TId>

        #region Not supported interface members (stateful behavior)
        public virtual int StartRecord
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public virtual int MaxRecords
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public virtual void AddSortParam(SortParam param)
        {
            throw new NotSupportedException();
        }

        public virtual void AddSortParam(ICollection<SortParam> param)
        {
            throw new NotSupportedException();
        }

        public virtual void ClearSortParam()
        {
            throw new NotSupportedException();
        }

        public virtual void AddFilterParam(FilterParam param)
        {
            throw new NotSupportedException();
        }

        public virtual void AddFilterParam(ICollection<FilterParam> param)
        {
            throw new NotSupportedException();
        }

        public virtual void ClearFilterParam()
        {
            throw new NotSupportedException();
        }

        public virtual IList<TEntity> Get()
        {
            throw new NotSupportedException();
        }
        #endregion

        public IList<TEntity> Get(Func<TEntity, bool> predicate)
        {
            return Repository.Where(predicate).ToList();
        }

        public virtual IList<TEntity> Get(FilterSortParam filterSortParam)
        {
            var query = CreateFilterSortQuery(filterSortParam);
            return query.ToList();
        }

        public virtual IList<TEntity> Get(FilterSortParam filterSortParam, out long totalRowCount)
        {
            var filterQuery = CreateFilterQuery(filterSortParam.Filters, null);
            var query = CreateSortedAndPagedQuery(filterSortParam, filterQuery);

            var entities = query != null ? query.ToList() : Repository.ToList(); //for empty filter return all entities
            totalRowCount = filterQuery != null ? filterQuery.LongCount() : Repository.LongCount();

            return entities;
        }

        protected virtual ObjectQuery<TEntity> CreateFilterSortQuery(FilterSortParam filterSortParam)
        {
            var query = CreateFilterQuery(filterSortParam.Filters, null);
            query = CreateSortedAndPagedQuery(filterSortParam, query);

            return query ?? Repository;
        }

        protected virtual ObjectQuery<TEntity> CreateFilterQuery(IList<FilterParam> filterParams, ObjectQuery<TEntity> existingQuery)
        {
            ObjectQuery<TEntity> query = existingQuery;

            if (filterParams != null && filterParams.Count > 0)
            {
                string filterString = "";
                var parameters = new List<ObjectParameter>(filterParams.Count);

                for (int index = 0; index < filterParams.Count; index++)
                {
                    var filterParam = filterParams[index];
                    var objectParams = GetObjectParameters(filterParam, index, ref filterString);
                    parameters.AddRange(objectParams);
                }

                query = existingQuery != null
                    ? existingQuery.Where(filterString, parameters.ToArray())
                    : Repository.Where(filterString, parameters.ToArray());
            }

            return query;
        }

        protected virtual ObjectQuery<TEntity> CreateSortQuery(IList<SortParam> sortParams, ObjectQuery<TEntity> existingQuery)
        {
            var query = existingQuery;

            if (sortParams != null && sortParams.Count > 0)
            {
                string sortString = sortParams.Aggregate(string.Empty,
                                                            (current, sortParam) =>
                                                            current +
                                                            ("it." + sortParam.Property + (sortParam.Ascending ? " asc" : " desc")));

                query = existingQuery != null
                    ? existingQuery.OrderBy(sortString)
                    : Repository.OrderBy(sortString);
            }

            return query;
        }

        protected virtual ObjectQuery<TEntity> CreatePagedQuery(int startRecord, int maxRecords, ObjectQuery<TEntity> existingQuery)
        {
            ObjectQuery<TEntity> query = existingQuery;

            if (startRecord > 0)
            {
                query = (ObjectQuery<TEntity>)(existingQuery != null
                    ? existingQuery.Skip(startRecord)
                    : Repository.Skip(startRecord));
            }

            query = CreateLimitQuery(maxRecords, query);

            return query;
        }

        protected virtual ObjectQuery<TEntity> CreateSortedAndPagedQuery(FilterSortParam filterSortParam, ObjectQuery<TEntity> existingQuery)
        {
            ObjectQuery<TEntity> query = existingQuery;

            if ((filterSortParam.SortParams == null ||
                    filterSortParam.SortParams.Count == 0) &&
                filterSortParam.StartRecord > 0)
            {
                //Skip requires OrderBy statement before
                //sort by Id ASC by default
                if (filterSortParam.SortParams == null)
                    filterSortParam.SortParams = new List<SortParam>();

                filterSortParam.SortParams.Add(new SortParam("Id", true));
            }

            if (filterSortParam.SortParams != null &&
                filterSortParam.SortParams.Count > 0 &&
                filterSortParam.StartRecord >= 0)
            {

                //both sorting & paging defined

                var sortString = filterSortParam.SortParams.Aggregate(string.Empty,
                                                                            (current, sortParam) =>
                                                                            current +
                                                                            ("it." + sortParam.Property +
                                                                            (sortParam.Ascending ? " asc" : " desc")));

                if (filterSortParam.StartRecord > 0)
                {
                    query = existingQuery != null
                        ? existingQuery.Skip(sortString, "@skip_records", new ObjectParameter("skip_records", filterSortParam.StartRecord))
                        : Repository.Skip(sortString, "@skip_records", new ObjectParameter("skip_records", filterSortParam.StartRecord));
                }
                else
                {
                    query = existingQuery != null
                        ? existingQuery.OrderBy(sortString)
                        : Repository.OrderBy(sortString);
                }

                query = CreateLimitQuery(filterSortParam.MaxRecords, query);
            }
            else if (filterSortParam.StartRecord >= 0)
            {
                //paging is defined only
                query = CreatePagedQuery(filterSortParam.StartRecord, filterSortParam.MaxRecords, query);
            }
            else
            {
                //limit is defined only
                query = CreateLimitQuery(filterSortParam.MaxRecords, query);
            }
            return query;
        }


        /// <summary>
        /// Applies MaxRecords limit
        /// </summary>
        /// <param name="maxRecords"></param>
        /// <param name="existingQuery"></param>
        /// <returns></returns>
        protected virtual ObjectQuery<TEntity> CreateLimitQuery(int maxRecords, ObjectQuery<TEntity> existingQuery)
        {
            ObjectQuery<TEntity> query = existingQuery;

            if (maxRecords > 0)
            {
                query = (ObjectQuery<TEntity>)(existingQuery != null
                    ? existingQuery.Take(maxRecords)
                    : Repository.Take(maxRecords));
            }

            return query;
        }

        #endregion

        public TEntity Create()
        {
            var entity = Context.CreateObject<TEntity>();
            return entity;
        }

        public void AddObject(TEntity entity)
        {
            Repository.AddObject(entity);
        }

        public void Attach(TEntity entity)
        {
            Repository.Attach(entity);
        }

        public void DeleteAll()
        {
            Context.ExecuteStoreCommand("DELETE FROM " + typeof(TEntity).Name + "Set");
        }

        protected TEntity AttachEntityIfNeeded(TEntity entity)
        {
            if (entity == null)
                return null;

            ObjectStateEntry entityState;
            TEntity attachedEntity = entity;

            var entitykey = Context.CreateEntityKey(RepositoryName, entity);
            bool isEntityAttached = Context.ObjectStateManager.TryGetObjectStateEntry(entitykey, out entityState);
            if (!isEntityAttached)
            {
                if (entity.Id.CompareTo(default(TId)) != 0)
                {
                    //id is set. Assume that this entity already exists in the db
                    Repository.Attach(entity);
                    Context.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
                }
                else
                {
                    //id is not set. Assume that this entity is new and add it.
                    Repository.AddObject(entity);
                }
            }
            else
            {
                if (entityState.State == EntityState.Unchanged || entityState.State == EntityState.Modified)
                {
                    Repository.ApplyCurrentValues(entity);
                    attachedEntity = (TEntity)entityState.Entity;
                }
            }
            return attachedEntity;
        }

        private ObjectSet<TEntity> CreateRepository()
        {
            var repository = Context.CreateObjectSet<TEntity>(RepositoryName);
            return repository;
        }

        private static IEnumerable<ObjectParameter> GetObjectParameters(FilterParam filterParam, int paramIndex, ref string filterQueryString)
        {
            List<ObjectParameter> objectParameters;

            var propertyName = filterParam.Property;
            var propertyValueParamName = propertyName.Replace('.', '_') + paramIndex;
            var propertyTargetValue = filterParam.Value;

            string comparisonOperator;
            switch (filterParam.Expression)
            {
                case DaoExpression.Eq:
                    comparisonOperator = /*propertyTargetValue is IEnumerable ? "IN" :*/ "=";
                    break;
                case DaoExpression.Gt:
                    comparisonOperator = ">";
                    break;
                case DaoExpression.Lt:
                    comparisonOperator = "<";
                    break;
                case DaoExpression.GtEq:
                    comparisonOperator = ">=";
                    break;
                case DaoExpression.LtEq:
                    comparisonOperator = "<=";
                    break;
                case DaoExpression.NotEq:
                    comparisonOperator = "<>";
                    break;
                case DaoExpression.Contains:
                    comparisonOperator = " LIKE ";
                    propertyTargetValue = "%" + propertyTargetValue.ToString().Replace("%", "%%") + "%";
                    break;
                case DaoExpression.StartsWith:
                    comparisonOperator = " LIKE ";
                    propertyTargetValue = propertyTargetValue.ToString().Replace("%", "%%") + "%";
                    break;
                case DaoExpression.EndsWith:
                    comparisonOperator = " LIKE ";
                    propertyTargetValue = "%" + propertyTargetValue.ToString().Replace("%", "%%");
                    break;
                default:
                    throw new NotSupportedException(filterParam.Expression + " is not supported");
            }

            if (paramIndex > 0) filterQueryString += " and ";
            if (propertyTargetValue is ICollection)
            {
                objectParameters = new List<ObjectParameter>();

                var valueCollection = (ICollection)propertyTargetValue;
                var valueIndex = 0;

                filterQueryString += "(";

                foreach (var value in valueCollection)
                {
                    var collectionItemValueParamName = propertyValueParamName + "_" + valueIndex;

                    if (valueIndex > 0)
                    {
                        filterQueryString += " or ";
                    }

                    filterQueryString += "it." + propertyName + comparisonOperator + "@" + collectionItemValueParamName;
                    objectParameters.Add(new ObjectParameter(collectionItemValueParamName, value));
                    valueIndex++;
                }
                filterQueryString += ")";
            }
            else
            {
                filterQueryString += "it." + propertyName + comparisonOperator + "@" + propertyValueParamName;
                objectParameters = new List<ObjectParameter>
            {
                new ObjectParameter(propertyValueParamName, propertyTargetValue)
            };
            }
            return objectParameters;
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

    }
}
