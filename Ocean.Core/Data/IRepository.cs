using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq.Expressions;
using System;

namespace Ocean.Core.Data
{
    /// <summary>
    /// Repository
    /// </summary>
    public partial interface IRepository<T> where T : BaseEntity
    {
        T GetById(object id);
        T Read(Expression<Func<T, bool>> predicate);
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        IQueryable<T> Table { get; }
        ObjectSet<T> ObjectTable { get; }

        #region tools
        string GetTableName();
        string GetTableSql();
        string GetTableColumnName();
        string GetEntityTableName();
        #endregion
    }
}