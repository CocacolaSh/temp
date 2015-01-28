using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using Ocean.Core;
using Ocean.Entity.Mapping;
using System.Text.RegularExpressions;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity;
using System.Transactions;
using Ocean.Core.Data;
using Ocean.Core.Infrastructure;

namespace Ocean.Data
{
    /// <summary>
    /// Object context
    /// </summary>
    public class OceanObjectContext : DbContext, IDbContext
    {
        protected readonly IDataProvider _dataProvider;
        public OceanObjectContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            //this.Configuration.AutoDetectChangesEnabled = false;//自动发现［更新］
            //((IObjectContextAdapter) this).ObjectContext.ContextOptions.LazyLoadingEnabled = true;
            _dataProvider = EngineContext.Current.Resolve<IDataProvider>();
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //dynamically load all configuration
            System.Type configType = typeof(ProductMap);   //any of your configuration classes here
            var typesToRegister = Assembly.GetAssembly(configType).GetTypes()
            .Where(type => !String.IsNullOrEmpty(type.Namespace))
            .Where(type => type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
            //...or do it manually below. For example,
            //modelBuilder.Configurations.Add(new LanguageMap());

            base.OnModelCreating(modelBuilder);
        }

        public void UpdateAttach<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            #region 方式1
            //var _dc = (IObjectContextAdapter)this; // MyContext context=new MyContext(); MyContext 继承 DbContext
            //var _oc = _dc.ObjectContext;
            //var key = _oc.CreateEntityKey("OceanObjectContext", entity); //entity 为修改的对象
            ////MyDbSets 为 MyContext 中 public DbSet<MyDbSet> MyDbSets { get; set; }
            //ObjectStateEntry ose;
            //if (_oc.ObjectStateManager.TryGetObjectStateEntry(key, out ose))
            //{
            //    var _entity = (TEntity)ose.Entity;
            //    this.Entry(_entity).State = EntityState.Detached;
            //    // Detached状态，就是entity还没有attach到context（实际上是Attach到某个DbSet上）的状态
            //}
            //this.Set<TEntity>().Attach(entity);
            //this.Entry(entity).State = EntityState.Modified;
            #endregion

            #region 方式2
            //var context = ((IObjectContextAdapter)this).ObjectContext;
            //ObjectStateEntry objectStateEntry;
            //if (context.ObjectStateManager.TryGetObjectStateEntry(entity, out objectStateEntry))
            //{
            //    objectStateEntry.ApplyCurrentValues(entity);
            //    objectStateEntry.SetModified();
            //    //this.Commit();
            //}
            #endregion

            #region 方式3
            //var entry = this.Entry(entity);
            //if (entry.State == EntityState.Detached)
            //{
            //    var entityOrigin = Set<TEntity>().Find(entity.Id);
            //    EmitMapper.ObjectMapperManager
            //         .DefaultInstance.GetMapper<TEntity, TEntity>()
            //         .Map(entityToUpdate, entityOrigin);
            //}
            #endregion

            #region new
            //if (entity == null)
            //{
            //    throw new ArgumentException("Cannot add a null entity.");
            //}

            //var entry = _context.Entry<T>(entity);

            //if (entry.State == EntityState.Detached)
            //{
            //    var set = _context.Set<T>();
            //    T attachedEntity = set.Local.SingleOrDefault(e => e.Id == entity.Id);  // You need to have access to key

            //    if (attachedEntity != null)
            //    {
            //        var attachedEntry = _context.Entry(attachedEntity);
            //        attachedEntry.CurrentValues.SetValues(entity);
            //    }
            //    else
            //    {
            //        entry.State = EntityState.Modified; // This should attach entity
            //    }
            //}
            #endregion
            //db.Entry(oldperson).CurrentValues.SetValues(person);

            DbEntityEntry<TEntity> entry = this.Entry(entity);

            //if (entry.State == EntityState.Detached)
            //{
                var set = this.Set<TEntity>();
                TEntity attachedEntity = set.Local.SingleOrDefault(e => e.Id == entity.Id);
                if (attachedEntity == null)
                {
                    set.Attach(entity);
                    entry.State = EntityState.Modified;
                }
                else
                {
                    var attachedEntry = this.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(entity);
                }
            //}
        }

        /// <summary>
        /// Attach an entity to the context or return an already attached entity (if it was already attached)
        /// </summary>
        /// <typeparam name="TEntity">TEntity</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Attached entity</returns>
        protected virtual TEntity AttachEntityToContext<TEntity>(TEntity entity) where TEntity : BaseEntity, new()
        {
            //little hack here until Entity Framework really supports stored procedures
            //otherwise, navigation properties of loaded entities are not loaded until an entity is attached to the context
            var alreadyAttached = Set<TEntity>().Local.Where(x => x.Id == entity.Id).FirstOrDefault();
            if (alreadyAttached == null)
            {
                //attach new entity
                Set<TEntity>().Attach(entity);
                return entity;
            }
            else
            {
                //entity is already loaded.
                return alreadyAttached;
            }
        }

        public string CreateDatabaseScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public ObjectSet<TEntity> ObjSet<TEntity>() where TEntity : BaseEntity
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateObjectSet<TEntity>();
        }

        public DbSet<TEntity> EDbSet<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public T ExcuteStoreProc<T>(string procName, params object[] parameters)
        {
            int hasOutputParameters = -1;
            if (parameters != null)
            {
                int i = 0;
                foreach (var p in parameters)
                {
                    var outputP = p as DbParameter;
                    if (outputP == null)
                        continue;

                    if (outputP.Direction == ParameterDirection.InputOutput ||
                        outputP.Direction == ParameterDirection.Output)
                        hasOutputParameters = i;
                    i++;
                }
            }

            //var context = ((IObjectContextAdapter)(this)).ObjectContext;
            //var connection = context.Connection;
            var connection = this.Database.Connection;
            //Don't close the connection after command execution

            //open the connection for use
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            //create a command object
            using (var cmd = connection.CreateCommand())
            {
                //command to execute
                cmd.CommandText = procName;
                cmd.CommandType = CommandType.StoredProcedure;

                // move parameters to command object
                if (parameters != null)
                    foreach (var p in parameters)
                        cmd.Parameters.Add(p);

                return (T)Convert.ChangeType(cmd.ExecuteNonQuery(), typeof(T));
                if (hasOutputParameters != -1)
                {
                    return (T)cmd.Parameters[hasOutputParameters].Value;
                }
            }
        }
        
        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            //HACK: Entity Framework Code First doesn't support doesn't support output parameters
            //That's why we have to manually create command and execute it.
            //just wait until EF Code First starts support them
            //
            //More info: http://weblogs.asp.net/dwahlin/archive/2011/09/23/using-entity-framework-code-first-with-stored-procedures-that-have-output-parameters.aspx

            bool hasOutputParameters = false;
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var outputP = p as DbParameter;
                    if (outputP == null)
                        continue;

                    if (outputP.Direction == ParameterDirection.InputOutput ||
                        outputP.Direction == ParameterDirection.Output)
                        hasOutputParameters = true;
                }
            }

            var context = ((IObjectContextAdapter)(this)).ObjectContext;
            if (!hasOutputParameters)
            {
                //no output parameters
                var result = this.Database.SqlQuery<TEntity>(commandText, parameters).ToList();
                for (int i = 0; i < result.Count; i++)
                    result[i] = AttachEntityToContext(result[i]);
                        
                return result;
                
                //var result = context.ExecuteStoreQuery<TEntity>(commandText, parameters).ToList();
                //foreach (var entity in result)
                //    Set<TEntity>().Attach(entity);
                //return result;
            }
            else
            {
                //var connection = context.Connection;
                var connection = this.Database.Connection;
                //Don't close the connection after command execution

                //open the connection for use
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                //create a command object
                using (var cmd = connection.CreateCommand())
                {
                    //command to execute
                    cmd.CommandText = commandText;
                    cmd.CommandType = CommandType.StoredProcedure;

                    // move parameters to command object
                    if (parameters != null)
                        foreach (var p in parameters)
                            cmd.Parameters.Add(p);

                    //database call
                    var reader = cmd.ExecuteReader();
                    //return reader.DataReaderToObjectList<TEntity>();
                    var result = context.Translate<TEntity>(reader).ToList();
                    for (int i = 0; i < result.Count; i++)
                        result[i] = AttachEntityToContext(result[i]);
                    //close up the reader, we're done saving results
                    reader.Close();

                    if (connection.State != ConnectionState.Closed)
                        connection.Close();
                    return result;
                }
            }
        }

        /// <summary>
        /// Creates a raw SQL query that will return elements of the given generic type.  The type can be any type that has properties that match the names of the columns returned from the query, or can be a simple primitive type. The type does not have to be an entity type. The results of this query are never tracked by the context even if the type of object returned is an entity type.
        /// </summary>
        /// <typeparam name="TElement">The type of object returned by the query.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>Result</returns>
        public IEnumerable<TElement> ObjectQuery<TElement>(string sql, params ObjectParameter[] parameters)
        {
            return GetObjectContext.CreateQuery<TElement>(sql, parameters).AsEnumerable();
            //return this.Database.SqlQuery<TElement>(sql, parameters);
        }

        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return this.Database.SqlQuery<TElement>(sql, parameters);
        }

        public OceanDynamicList<object> SqlQuery(string sql, params object[] parameters)
        {
            //var connection = context.Connection;
            var connection = this.Database.Connection;
            //Don't close the connection after command execution

            //open the connection for use
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            //create a command object
            using (var cmd = connection.CreateCommand())
            {
                //command to execute
                cmd.CommandText = sql;

                // move parameters to command object
                if (parameters != null)
                    foreach (var p in parameters)
                        cmd.Parameters.Add(p);

                //database call
                var reader = cmd.ExecuteReader();
                //return reader.DataReaderToObjectList<TEntity>();
                int filedCount = reader.FieldCount;
                string[] columns = new string[filedCount];
                object[] values = new object[filedCount];
                IList<object> valueList = new List<object>();
                int i = 0;
                while (reader.Read())
                {
                    for (int j = 0; j < filedCount; j++)
                    {
                        if (i == 0)
                        {
                            string filedName = reader.GetName(j);
                            columns[j] = filedName;
                        }
                        values[j] = reader.GetValue(j);
                    }
                    i++;
                    valueList.Add(values);
                    values = new object[filedCount];
                }
                //close up the reader, we're done saving results
                reader.Close();

                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                if (valueList.Count == 0)
                {
                    return null;
                }
                else
                {
                    return new OceanDynamicList<object>(valueList, columns);
                }
            }
        }

        public OceanDynamic SqlDynamicQuery(string sql, params object[] parameters)
        {
            //var connection = context.Connection;
            var connection = this.Database.Connection;
            //Don't close the connection after command execution

            //open the connection for use
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            //create a command object
            using (var cmd = connection.CreateCommand())
            {
                //command to execute
                cmd.CommandText = sql;

                // move parameters to command object
                if (parameters != null)
                    foreach (var p in parameters)
                        cmd.Parameters.Add(p);

                //database call
                var reader = cmd.ExecuteReader();
                //return reader.DataReaderToObjectList<TEntity>();
                int filedCount = reader.FieldCount;
                string[] columns = new string[filedCount];
                object[] values = new object[filedCount];
                int i = 0;
                while (reader.Read())
                {
                    for (int j = 0; j < filedCount; j++)
                    {
                        if (i == 0)
                        {
                            string filedName = reader.GetName(j);
                            columns[j] = filedName;
                        }
                        values[j] = reader.GetValue(j);
                    }
                    i++;
                    break;
                }
                //close up the reader, we're done saving results
                reader.Close();

                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                if (i == 0)
                {
                    return null;
                }
                else
                {
                    return new OceanDynamic(values, columns);
                }
            }
        }

        /// <summary>
        /// Executes the given DDL/DML command against the database.
        /// </summary>
        /// <param name="sql">The command string</param>
        /// <param name="timeout">Timeout value, in seconds. A null value indicates that the default value of the underlying provider will be used</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>The result returned by the database after executing the command.</returns>
        public int ExecuteSqlCommand(string sql, int? timeout = null, params object[] parameters)
        {
            int? previousTimeout = null;
            if (timeout.HasValue)
            {
                //store previous timeout
                previousTimeout = ((IObjectContextAdapter)this).ObjectContext.CommandTimeout;
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = timeout;
            }

            var result = this.Database.ExecuteSqlCommand(sql, parameters);

            if (timeout.HasValue)
            {
                //Set previous timeout back
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = previousTimeout;
            }

            //return result
            return result;
        }

        #region vebin- get table name
        public ObjectContext GetObjectContext
        {
            get { return ((IObjectContextAdapter)this).ObjectContext; }
        }

        public ObjectParameter[] GetObjectParamsters(Dictionary<string, object> parms)
        {
            if (parms == null)
                return null;
            ObjectParameter[] dbParams = new ObjectParameter[parms.Keys.Count];
            int i = 0;
            foreach (KeyValuePair<string, object> parm in parms)
            {
                dbParams[i] = new ObjectParameter(parm.Key, parm.Value);
                i++;
            }
            return dbParams;
        }


        //private readonly static Dictionary<Type, EntitySetBase> _mappingCache = new Dictionary<Type, EntitySetBase>();

        //private ObjectContext _ObjectContext
        //{
        //    get { return (this as IObjectContextAdapter).ObjectContext; }
        //}

        //private EntitySetBase GetEntitySet(Type type)
        //{
        //    if (_mappingCache.ContainsKey(type))
        //        return _mappingCache[type];

        //    type = GetObjectType(type);
        //    string baseTypeName = type.BaseType.Name;
        //    string typeName = type.Name;

        //    ObjectContext octx = _ObjectContext;
        //    var es = octx.MetadataWorkspace
        //                    .GetItemCollection(DataSpace.SSpace)
        //                    .GetItems<EntityContainer>()
        //                    .SelectMany(c => c.BaseEntitySets
        //                                    .Where(e => e.Name == typeName
        //                                    || e.Name == baseTypeName))
        //                    .FirstOrDefault();

        //    if (es == null)
        //        throw new ArgumentException("Entity type not found in GetEntitySet", typeName);

        //    return es;
        //}

        //internal String GetTableName(Type type)
        //{
        //    EntitySetBase es = GetEntitySet(type);

        //    //if you are using EF6
        //    return String.Format("[{0}].[{1}]", es.Schema, es.Table);

        //    //if you have a version prior to EF6
        //    //return string.Format( "[{0}].[{1}]", 
        //    //        es.MetadataProperties["Schema"].Value, 
        //    //        es.MetadataProperties["Table"].Value );
        //}

        //internal Type GetObjectType(Type type)
        //{
        //    return System.Data.Entity.Core.Objects.ObjectContext.GetObjectType(type);
        //}
        #endregion

        #region transaction
        public bool IsTrans { get; set; }
        public int TransCount { get; set; }
        private DbContextTransaction _trans;
        public void BeginTransaction()
        {
            TransCount++;
            if (!IsTrans)
            {
                _trans = Database.BeginTransaction();
                IsTrans = true;
            }
        }

        public void Commit()
        {
            if (TransCount > 1)
            {
                TransCount--;
            }
            else
            {
                _trans.Commit();
                _trans.Dispose();
                TransCount = 0;
                IsTrans = false;
            }
        }
        public void Rollback(bool SureRollback=true)
        {
            if (SureRollback)
            {
                _trans.Rollback();
                _trans.Dispose();
                TransCount = 0;
                IsTrans = false;
            }
            else
            {
                TransCount--;
            }
        }
        #endregion
    }
}

//public static List<T> MapToList<T>(this DbDataReader dr) where T : new()
//    {
//        if (dr != null && dr.HasRows)
//        {
//            var entity = typeof(T);
//            var entities = new List<T>();
//            var propDict = new Dictionary<string, PropertyInfo>();
//            var props = entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
//            propDict = props.ToDictionary(p => p.Name.ToUpper(), p => p);

//            while (dr.Read())
//            {
//                T newObject = new T();
//                for (int index = 0; index < dr.FieldCount; index++)
//                {
//                    if (propDict.ContainsKey(dr.GetName(index).ToUpper()))
//                    {
//                        var info = propDict[dr.GetName(index).ToUpper()];
//                        if ((info != null) && info.CanWrite)
//                        {
//                            var val = dr.GetValue(index);
//                            info.SetValue(newObject, (val == DBNull.Value) ? null : val, null);
//                        }
//                    }
//                }
//                entities.Add(newObject);
//            }
//            return entities;
//        }
//        return null;
//    }