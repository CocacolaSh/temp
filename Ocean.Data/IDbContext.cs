using System.Collections.Generic;
using System.Data.Entity;
using Ocean.Core;
using System.Data.Entity.Core.Objects;

namespace Ocean.Data
{
    public interface IDbContext 
    {
        IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

        int SaveChanges();

        /// <summary>
        /// 开始事务[嵌套事务时，最外层为开始,其余计数]
        /// </summary>
        void BeginTransaction();
        /// <summary>
        /// 提交事务[嵌套事务时，最外层才提交]
        /// </summary>
        void Commit();
        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="SureRollback">设置为false的时候为不回滚，一般用于嵌套事务使用</param>
        void Rollback(bool SureRollback = true);
        bool IsTrans { get; set; }

        IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : BaseEntity, new();
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="procName">存储过程名称</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        T ExcuteStoreProc<T>(string procName, params object[] parameters);
        /// <summary>
        /// Creates a raw SQL query that will return elements of the given generic type.  The type can be any type that has properties that match the names of the columns returned from the query, or can be a simple primitive type. The type does not have to be an entity type. The results of this query are never tracked by the context even if the type of object returned is an entity type.
        /// 创建一个原始SQL查询将返回给定泛型类型的元素。类型可以是任何类型的属性匹配查询返回的列的名称,或者可以是一个简单的原语类型。不需要一个实体类型的类型。这个查询的结果从来没有追踪的上下文,即使是一个实体类型返回的类型的对象。
        /// </summary>
        /// <typeparam name="TElement">The type of object returned by the query.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>Result</returns>
        IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters);
        OceanDynamicList<object> SqlQuery(string sql, params object[] parameters);
        OceanDynamic SqlDynamicQuery(string sql, params object[] parameters);
        IEnumerable<TElement> ObjectQuery<TElement>(string sql, params ObjectParameter[] parameters);
        /// <summary>
        /// Executes the given DDL/DML command against the database.
        /// 对数据库执行给定的DDL和DML命令。
        /// </summary>
        /// <param name="sql">The command string</param>
        /// <param name="timeout">Timeout value, in seconds. A null value indicates that the default value of the underlying provider will be used</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>The result returned by the database after executing the command.</returns>
        int ExecuteSqlCommand(string sql, int? timeout = null, params object[] parameters);

        /// <summary>
        /// 获取当前entity
        /// </summary>
        ObjectContext GetObjectContext { get; }

        ObjectParameter[] GetObjectParamsters(Dictionary<string, object> parms);

        DbSet<TEntity> EDbSet<TEntity>() where TEntity : BaseEntity;
        ObjectSet<TEntity> ObjSet<TEntity>() where TEntity : BaseEntity;

        void UpdateAttach<TEntity>(TEntity entity) where TEntity : BaseEntity;
    }
}