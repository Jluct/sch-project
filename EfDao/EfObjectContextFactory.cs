using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EfDao
{
    public class EfObjectContextFactory
    {
        private readonly string _connectionStringName;
        private readonly string _containerName;

        #region Singleton

        protected EfObjectContextFactory()
        {
            var config = EfDaoConfigurationSection.GetSection();
            _connectionStringName = config.ConnectionStringKey;
            _containerName = config.ContainerName;
        }

        private static EfObjectContextFactory _instance;
        public static EfObjectContextFactory Instance
        {
            get { return _instance ?? (_instance = new EfObjectContextFactory()); }
        }

        #endregion

        private readonly ThreadLocal<IDictionary<string, ObjectContext>> _threadLocalContext = new ThreadLocal<IDictionary<string, ObjectContext>>(() => new Dictionary<string, ObjectContext>());
        public ObjectContext GetCurrentContext()
        {
            return GetCurrentContext(null);
        }

        public ObjectContext GetCurrentContext(string conatinerName)
        {
            conatinerName = conatinerName ?? _containerName;
            if (_threadLocalContext.Value == null)
            {
                _threadLocalContext.Value = new Dictionary<string, ObjectContext>();
            }
            if (_threadLocalContext.Value.ContainsKey(conatinerName))
            {
                return _threadLocalContext.Value[conatinerName];
            }

            var context = CreateContext(conatinerName);
            _threadLocalContext.Value.Add(conatinerName, context);
            return context;
        }

        public ObjectContext CreateContext(string containerName)
        {
            var context = new ObjectContext(string.Format("name={0}", containerName));
            context.DefaultContainerName = containerName;

            context.ContextOptions.ProxyCreationEnabled = true;
            context.ContextOptions.LazyLoadingEnabled = false;

            return context;
        }

        public void DisposeCurrentContext()
        {
            if (!_threadLocalContext.IsValueCreated || _threadLocalContext.Value == null) return;
            foreach (var objectContext in _threadLocalContext.Value.Values)
            {
                var entries = objectContext.ObjectStateManager
                    .GetObjectStateEntries(~EntityState.Detached)
                    .Where(e => e.State != EntityState.Detached && !e.IsRelationship);
                foreach (ObjectStateEntry entry in entries)
                {
                    objectContext.Detach(entry.Entity);
                }

                objectContext.Dispose();
            }
            _threadLocalContext.Value = null;
        }
    }
}
