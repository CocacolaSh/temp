using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using Ocean.Core;
using Ocean.Core.Data;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq.Expressions;

namespace Ocean.Data
{
    /// <summary>
    /// Entity Framework repository
    /// </summary>
    public partial class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IDbContext _context;
        private IDbSet<T> _entities;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public EfRepository(IDbContext context)
        {
            this._context = context;

        }

        public T GetById(object id)
        {
            return this.Entities.Find(id);
        }

        public T Read(Expression<Func<T, bool>> predicate)
        {
            return this.Entities.AsNoTracking().Single(predicate);
        }

        public void Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this.Entities.Add(entity);

                this._context.SaveChanges();

            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        public void Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this._context.UpdateAttach<T>(entity);
                this._context.SaveChanges();

            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        public void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                if (!this.Entities.Local.Contains(entity))
                {
                    this.Entities.Attach(entity);
                }
                    this.Entities.Remove(entity);
                this._context.SaveChanges();

            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new OceanException(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        public virtual IQueryable<T> Table
        {
            get
            {
                //ObjectQuery<T> parents = this.Entities as ObjectQuery<T>;
                //if (parents != null)
                //{
                //    string sql = parents.ToTraceString();
                //}
                return this.Entities.AsNoTracking().AsQueryable();
            }
        }

        public virtual ObjectSet<T> ObjectTable { get { return (_context.ObjSet<T>() as ObjectSet<T>); } }

        private IDbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();
                return _entities;
            }
        }

        public string GetTableName()
        {
            return _context.GetTableName<T>();
        }

        public string GetTableSql()
        {
            return _context.GetTableSql<T>();
        }

        public string GetTableColumnName()
        {
            return _context.GetTableColumnName<T>();
        }

        public string GetEntityTableName()
        {
            return _context.GetEntityTableName<T>();
        }
    }
}