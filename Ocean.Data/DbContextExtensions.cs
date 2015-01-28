using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Ocean.Core;
using System.Text.RegularExpressions;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Metadata.Edm;
using System.Collections.Generic;

namespace Ocean.Data
{
    public static class DbContextExtensions {
        /// <summary>
        /// Loads the database copy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="currentCopy">The current copy.</param>
        /// <returns></returns>
        public static T LoadDatabaseCopy<T>(this IDbContext context, T currentCopy) where T : BaseEntity {
            return InnerGetCopy(context, currentCopy, e => e.GetDatabaseValues());
        }

        private static T InnerGetCopy<T>(IDbContext context, T currentCopy, Func<DbEntityEntry<T>, DbPropertyValues> func) where T : BaseEntity {
            //Get the database context
            DbContext dbContext = CastOrThrow(context);

            //Get the entity tracking object
            DbEntityEntry<T> entry = GetEntityOrReturnNull(currentCopy, dbContext);

            //The output 
            T output = null;

            //Try and get the values
            if (entry != null) {
                DbPropertyValues dbPropertyValues = func(entry);
                if(dbPropertyValues != null) {
                    output = dbPropertyValues.ToObject() as T;
                }
            }

            return output;
        }

        /// <summary>
        /// Gets the entity or return null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="currentCopy">The current copy.</param>
        /// <param name="dbContext">The db context.</param>
        /// <returns></returns>
        private static DbEntityEntry<T> GetEntityOrReturnNull<T>(T currentCopy, DbContext dbContext) where T : BaseEntity {
            return dbContext.ChangeTracker.Entries<T>().Where(e => e.Entity == currentCopy).FirstOrDefault();
        }

        private static DbContext CastOrThrow(IDbContext context) {
            DbContext output = (context as DbContext);

            if(output == null) {
                throw new InvalidOperationException("Context does not support operation.");
            }

            return output;
        }

        /// <summary>
        /// Loads the original copy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="currentCopy">The current copy.</param>
        /// <returns></returns>
        public static T LoadOriginalCopy<T>(this IDbContext context, T currentCopy) where T : BaseEntity {
            return InnerGetCopy(context, currentCopy, e => e.OriginalValues);
        }
        /// <summary>
        /// get table name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetTableName<T>(this ObjectContext context) where T : BaseEntity
        {
            string sql = context.GetTableSql<T>();
            Regex regex = new Regex("FROM\\s+(?<table>.+\\s+AS.+\\s?)");
            Match match = regex.Match(sql);

            string table = match.Groups["table"].Value;
            return table;
        }
        /// <summary>
        /// 获取实体中查询列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetTableColumnName<T>(this ObjectContext context) where T : BaseEntity
        {
            string sql = context.GetTableSql<T>();
            Regex regex = new Regex("SELECT\\s+(?<column>.+\\s+)FROM");
            Match match = regex.Match(sql);

            string table = match.Groups["column"].Value;
            return table;
        }
        public static string GetTableSql<T>(this ObjectContext context) where T : BaseEntity
        {
            return context.CreateObjectSet<T>().ToTraceString();
            
        }



        /// <summary>
        /// get table name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetTableName<T>(this IDbContext context) where T : BaseEntity
        {
            return context.GetObjectContext.GetTableName<T>();
        }
        /// <summary>
        /// get table name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetTableSql<T>(this IDbContext context) where T : BaseEntity
        {
            return context.GetObjectContext.GetTableSql<T>();
        }
        /// <summary>
        /// get table name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetTableColumnName<T>(this IDbContext context) where T : BaseEntity
        {
            return context.GetObjectContext.GetTableColumnName<T>();
        }
        /// <summary>
        /// 获取EntityContainer.EntityName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static string GetEntityTableName<T>(this IDbContext dbContext) where T : BaseEntity
        {
            ObjectContext context = dbContext.GetObjectContext;
            string entitySetName = context.MetadataWorkspace
                        .GetEntityContainer(context.DefaultContainerName, DataSpace.CSpace)
                        .BaseEntitySets.Where(bes => bes.ElementType.Name == typeof(T).Name).First().Name;

            return String.Format("{0}.{1}", context.DefaultContainerName, entitySetName);
        }

        /// <summary>
        /// 从entity linq 中获取SQL语句
        /// </summary>
        /// <typeparam name="T">BaseEntity</typeparam>
        /// <param name="query">LINQ查询</param>
        /// <returns></returns>
        public static string GetSqlString<T>(this IQueryable<T> query,IList<object> parms) where T : BaseEntity
        {
            ObjectQuery<T> objQuery = query as ObjectQuery<T>;
            if (parms == null)
            {
                parms = new List<object>();
            }
            foreach (var para in objQuery.Parameters)
            {
                parms.Add(para.Value);
            }
            return objQuery.ToTraceString();
        }
    }
}