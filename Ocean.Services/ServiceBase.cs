using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;
using Ocean.Core.Data;
using Ocean.Data;
using System.Data.Common;
using System.Collections;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using Ocean.Core.Infrastructure;
using Ocean.Core.Common;
using System.Data;
using Ocean.Core.Utility;
using Ocean.Core.Data.DynamicQuery;
using Ocean.Core.Data.OrderBy;

namespace Ocean.Services
{
    public class ServiceBase<T> : IService<T> where T : BaseEntity
    {
        protected readonly IRepository<T> _repository;
        protected readonly IDataProvider _dataProvider;
        protected readonly IDbContext _context;

        public ServiceBase(IRepository<T> repository, IDbContext context)
        {
            _repository = repository;
            _context = context;
            _dataProvider = EngineContext.Current.Resolve<IDataProvider>();
        }

        #region linq

        public T GetById(object id)
        {
            return _repository.GetById(id);
        }

        public T Read(Expression<Func<T, bool>> predicate)
        {
            return _repository.Read(predicate);
        }

        public void Insert(T entity)
        {
            if (entity.Id == null || entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }

            entity.CreateDate = DateTime.Now;
            _repository.Insert(entity);
        }

        public void Update(T entity)
        {
            _repository.Update(entity);
        }

        public int Update(Expression<Func<T, bool>> where, Expression<Func<T>> updater)
        {
            var query = _repository.ObjectTable.Where(where);
            IList<object> parms = new List<object>();
            return _context.ExecuteSqlCommand(GetUpdateSql(query, parms, updater), null, parms.ToArray());
        }

        public void Delete(T entity)
        {
            _repository.Delete(entity);
        }

        public int Delete(Expression<Func<T, bool>> where)
        {
            var query = _repository.ObjectTable.Where(where);
            IList<object> parms = new List<object>();
            return _context.ExecuteSqlCommand(GetDeleteSql(query, parms),null, parms.ToArray());
        }

        public IQueryable<T> Table { get { return _repository.Table; } }

        public IList<T> GetALL()
        {
            return _repository.Table.ToList();
        }

        public IList<T> GetALL(Expression<Func<T, bool>> predicate, string keySelector, bool isAscendingOrder = true)
        {
            if(keySelector==null)
            {
                return _repository.Table.Where(predicate).ToList();
            }
            else if(predicate==null)
            {
                if (isAscendingOrder)
                {
                    return _repository.Table.OrderBy(keySelector).ToList();
                }
                else
                {
                    return _repository.Table.OrderByDescending(keySelector).ToList();
                }
            }
            else
            {
                return _repository.Table.Where(predicate).OrderBy(keySelector).ToList();
            }
        }
        public IList<T> GetALL(Expression<Func<T,bool>> predicate)
        {
            return GetALL(predicate,null);
        }
        public IList<T> GetALL(string keySelector, bool isAscendingOrder = true)
        {
            if (isAscendingOrder)
            {
                return _repository.Table.OrderBy(keySelector).ToList();
            }
            else
            {
                return _repository.Table.OrderByDescending(keySelector).ToList();
            }
        }

        public IList<T> GetALL(QueryCondition[] conditions, string key, bool isAscendingOrder = true)
        {
            QueryDescriptor descriptor = new QueryDescriptor
            {
                OrderBy = new OrderByClause { Key = key, Order = isAscendingOrder ? OrderSequence.ASC : OrderSequence.DESC },
                Conditions = conditions
            };
            return Table.Query(descriptor).ToList();
        }
        public IList<T> GetALL(QueryCondition[] conditions)
        {
            QueryDescriptor descriptor = new QueryDescriptor
            {
                //OrderBy = new OrderByClause { Key = "Price", Order = OrderSequence.ASC },
                //PageSize = 3,
                //PageIndex = 1,
                //Conditions = new QueryCondition[] {
                //    new QueryCondition { Key = "Name",Value = "Rice", Operator = QueryOperator.CONTAINS }
                //}
                Conditions = conditions
            };
            return Table.Query(descriptor).ToList();
        }

        public T GetUnique(Expression<Func<T, bool>> predicate)
        {
            return GetUnique(predicate, null);
        }

        public T GetUnique(Expression<Func<T, bool>> predicate, string keySelector, bool isAscendingOrder = true)
        {
            if (keySelector == null && predicate == null)
            {
                    return _repository.Table.Take(1).FirstOrDefault();
            }
            else if (keySelector == null)
            {
                return _repository.Table.OrderBy("CreateDate").Where(predicate).Take(1).FirstOrDefault();
            }
            else if (predicate == null)
            {
                if (isAscendingOrder)
                {
                    return _repository.Table.OrderBy(keySelector).Take(1).FirstOrDefault();
                }
                else
                {
                    return _repository.Table.OrderByDescending(keySelector).Take(1).FirstOrDefault();
                }
            }
            else
            {
                if (isAscendingOrder)
                {
                    return _repository.Table.Where(predicate).OrderBy(keySelector).Take(1).FirstOrDefault();
                }
                else
                {
                    return _repository.Table.Where(predicate).OrderByDescending(keySelector).Take(1).FirstOrDefault();
                }
            }
        }


        public IList<T> GetLimitList(int num, Expression<Func<T, bool>> predicate, string keySelector, bool isAscendingOrder = true)
        {
             if(keySelector==null&&predicate==null)
             {
                 if(num==0)
                 {
                     return GetALL();
                 }
                 else{
                 return _repository.Table.Take(num).ToList();
                 }
             }
            else if(keySelector==null)
            {
                return _repository.Table.Where(predicate).Take(num).ToList();
            }
            else if(predicate==null)
            {
                if (isAscendingOrder)
                {
                    return _repository.Table.OrderBy(keySelector).Take(num).ToList();
                }
                else
                {
                    return _repository.Table.OrderByDescending(keySelector).Take(num).ToList();
                }
            }
            else
            {
                return _repository.Table.Where(predicate).OrderBy(keySelector).Take(num).ToList();
            }
        }
        public IList<T> GetLimitList(int num)
        {
            return GetLimitList(num,null,true);
        }
        public IList<T> GetLimitList(int num,Expression<Func<T,bool>> predicate)
        {
            return GetLimitList(num,predicate,null);
        }
        public IList<T> GetLimitList(int num, string keySelector, bool isAscendingOrder = true)
        {
            if (isAscendingOrder)
            {
                return _repository.Table.OrderBy(keySelector).Take(num).ToList();
            }
            else
            {
                return _repository.Table.OrderByDescending(keySelector).Take(num).ToList();
            }
        }

        public IList<T> GetLimitList(int num, QueryCondition[] conditions, string key, bool isAscendingOrder = true)
        {
            QueryDescriptor descriptor = new QueryDescriptor
            {
                OrderBy = new OrderByClause { Key = key, Order = isAscendingOrder ? OrderSequence.ASC : OrderSequence.DESC },
                PageSize = num,
                PageIndex = 1,
                Conditions = conditions
            };
            return Table.Query(descriptor).ToList();
        }
        public IList<T> GetLimitList(int num, QueryCondition[] conditions)
        {
            QueryDescriptor descriptor = new QueryDescriptor
            {
                //OrderBy = new OrderByClause { Key = "Price", Order = OrderSequence.ASC },
                PageSize = num,
                PageIndex = 1,
                //Conditions = new QueryCondition[] {
                //    new QueryCondition { Key = "Name",Value = "Rice", Operator = QueryOperator.CONTAINS }
                //}
                Conditions = conditions
            };
            return Table.Query(descriptor).ToList();
        }

        public PagedList<T> GetPageList(Expression<Func<T, bool>> predicate, string keySelector, int pageIndex, int pageSize, bool isAscendingOrder=true)
        {
            IList<T> items;
            int totalCount = 0;
            pageIndex = pageIndex == 0 ? 1 : pageIndex;
            if (predicate == null && keySelector == null)
            {
                if (pageIndex == 1)
                {
                    items = _repository.Table.Take<T>(pageSize).ToList();
                }
                else
                {
                    items = _repository.Table.Skip<T>((pageIndex-1) * pageSize).Take<T>(pageSize).ToList();
                }
                totalCount = _repository.Table.Count();
            }
            else
            {
                if (predicate == null)
                {
                    if (isAscendingOrder)
                    {
                        if (pageIndex == 1)
                        {
                            items = _repository.Table.OrderBy<T>(keySelector).Take<T>(pageSize).ToList();
                        }
                        else
                        {
                            items = _repository.Table.OrderBy(keySelector).Skip<T>((pageIndex-1) * pageSize).Take<T>(pageSize).ToList();
                        }
                    }
                    else
                    {
                        if (pageIndex == 1)
                        {
                            items = _repository.Table.OrderByDescending(keySelector).Take<T>(pageSize).ToList();
                        }
                        else
                        {
                            items = _repository.Table.OrderByDescending(keySelector).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize).ToList();
                        }
                    }
                    totalCount = _repository.Table.Count();
                }
                else if (keySelector == null)
                {
                    if (pageIndex == 1)
                    {
                        items = _repository.Table.OrderByDescending(t => t.CreateDate).Where(predicate).Take<T>(pageSize).ToList();
                    }
                    else
                    {
                        items = _repository.Table.OrderByDescending(t => t.CreateDate).Where(predicate).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize).ToList();
                    }
                    totalCount = _repository.Table.Where(predicate).Count();
                }
                else
                {
                    //var param = Expression.Parameter(typeof(T), "N");
                    //var sortExpression = Expression.Lambda<Func<T, object>>(Expression.Convert(Expression.Property(param, "Name"), typeof(object)), param);
                    if (isAscendingOrder)
                    {
                        if (pageIndex == 1)
                        {
                            items = _repository.Table.Where(predicate).OrderBy(keySelector).Take<T>(pageSize).ToList();
                        }
                        else
                        {
                            items = _repository.Table.Where(predicate).OrderBy(keySelector).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize).ToList();
                        }
                    }
                    else
                    {
                        if (pageIndex == 1)
                        {
                            items = _repository.Table.Where(predicate).OrderByDescending(keySelector).Take<T>(pageSize).ToList();
                        }
                        else
                        {
                            items = _repository.Table.Where(predicate).OrderByDescending(keySelector).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize).ToList();
                        }
                    }
                    totalCount = _repository.Table.Where(predicate).Count();
                }
            }
            PagedList<T> pageItems = new PagedList<T>(items, pageIndex, pageSize, totalCount);
            pageItems.TotalItemCount = totalCount;
            return pageItems;
        }
        public PagedList<T> GetPageList(int pageIndex, int pageSize)
        {
            IList<T> items;
            pageIndex = pageIndex == 0 ? 1 : pageIndex;

            if (pageIndex == 1)
            {
                items = _repository.Table.Take<T>(pageSize).ToList();
            }
            else
            {
                items = _repository.Table.Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize).ToList();
            }
            int totalCount=_repository.Table.Count();;
            PagedList<T> pageItems = new PagedList<T>(items, pageIndex, pageSize,totalCount);
            pageItems.TotalItemCount = totalCount;
            return pageItems;
        }
        public PagedList<T> GetPageList(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize)
        {
            return GetPageList(predicate, null, pageIndex, pageSize);
        }
        public PagedList<T> GetPageList(string keySelector, int pageIndex, int pageSize, bool isAscendingOrder = true)
        {
            IList<T> items;
            int totalCount = 0;
            pageIndex = pageIndex == 0 ? 1 : pageIndex;
            if (isAscendingOrder)
            {
                if (pageIndex == 1)
                {
                    items = _repository.Table.OrderBy<T>(keySelector).Take<T>(pageSize).ToList();
                }
                else
                {
                    items = _repository.Table.OrderBy(keySelector).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize).ToList();
                }
            }
            else
            {
                if (pageIndex == 1)
                {
                    items = _repository.Table.OrderByDescending(keySelector).Take<T>(pageSize).ToList();
                }
                else
                {
                    items = _repository.Table.OrderByDescending(keySelector).Skip<T>((pageIndex-1) * pageSize).Take<T>(pageSize).ToList();
                }
            }
            totalCount = _repository.Table.Count();
            PagedList<T> pageItems = new PagedList<T>(items, pageIndex, pageSize, totalCount);
            pageItems.TotalItemCount = totalCount;
            return pageItems;
        }

        public IList<T> GetPageList(QueryCondition[] conditions, string key, int pageIndex, int pageSize,bool isAscendingOrder = true)
        {
            QueryDescriptor descriptor = new QueryDescriptor
            {
                OrderBy = new OrderByClause { Key = key, Order = isAscendingOrder ? OrderSequence.ASC : OrderSequence.DESC },
                PageSize = pageSize,
                PageIndex = pageIndex,
                Conditions = conditions
            };
            int totalCount;
            PagedList<T> pageItems = new PagedList<T>(Table.Query(descriptor, out totalCount).ToList(), pageIndex, pageSize, totalCount);
             pageItems.TotalItemCount = totalCount;
             return pageItems;
        }
        public IList<T> GetPageList(QueryCondition[] conditions,int pageIndex, int pageSize)
        {
            QueryDescriptor descriptor = new QueryDescriptor
            {
                //OrderBy = new OrderByClause { Key = "Price", Order = OrderSequence.ASC },
                PageSize = pageSize,
                PageIndex = pageIndex,
                //Conditions = new QueryCondition[] {
                //    new QueryCondition { Key = "Name",Value = "Rice", Operator = QueryOperator.CONTAINS }
                //}
                Conditions = conditions
            };
            int totalCount;
            PagedList<T> pageItems = new PagedList<T>(Table.Query(descriptor, out totalCount).ToList(), pageIndex, pageSize, totalCount);
            pageItems.TotalItemCount = totalCount;
            return pageItems;
        }

        #endregion

        #region SQL
        public int Delete(string sql)
        {
            if (!string.IsNullOrEmpty(sql) && !sql.ToLower().StartsWith("delete") && !sql.ToLower().StartsWith("where"))
            {
                string parmstr = sql;
                Dictionary<string, object> parms = new Dictionary<string, object>();
                parms.Add("Id", sql.Split(','));
                return Delete("", parms);
            }
            else
            {
                return Delete(sql, null);
            }
        }
        public int Delete(Dictionary<string, object> parms)
        {
            return Delete(null, parms);
        }
        public int Delete(string sql, Dictionary<string, object> parms)
        {
            if (parms == null)
            {
                return _context.ExecuteSqlCommand(GetDeleteSql(sql, null));
            }
            else
            {
                return _context.ExecuteSqlCommand(GetDeleteSql(sql, parms), null, GetDeleteParameters(parms));
            }
        }
        public int ExcuteSql(string sql)
        {
            return ExcuteSql(sql, null);
        }
        public int ExcuteSql(string sql, Dictionary<string, object> parms)
        {
            if (parms == null)
            {
                return _context.ExecuteSqlCommand(sql);
            }
            else
            {
                return _context.ExecuteSqlCommand(sql, null, GetParamsters(parms));
            }
        }
        public TKey ExcuteProc<TKey>(string procName, Dictionary<string, object> parms)
        {
            return this._context.ExcuteStoreProc<TKey>(procName, GetParamsters(parms));
        }
        public int GetCount(string sql, Dictionary<string, object> parms)
        {
            if (parms == null)
            {
                return _context.SqlQuery<int>(GetCountSql(sql)).First();
            }
            else
            {
                return _context.SqlQuery<int>(GetCountSql(sql), GetParamsters(parms)).First();
            }
        }

        public int GetCount(string sql)
        {
            return GetCount(sql, null);
        }

        public IList<T> GetList(string sql)
        {
            return GetList(sql, null);
        }

        public IList<T> GetList(string sql, Dictionary<string, object> parms)
        {
            if (parms == null)
            {
                return _context.SqlQuery<T>(GetSql(sql)).ToList();
            }
            else
            {
                return _context.SqlQuery<T>(GetSql(sql), GetParamsters(parms)).ToList();
            }
        }
        public OceanDynamicList<object> GetDynamicList(string sql)
        {
            return GetDynamicList(sql, null);
        }
        public OceanDynamicList<object> GetDynamicList(string sql, Dictionary<string, object> parms)
        {
            if (parms == null)
            {
                return _context.SqlQuery(GetSql(sql));
            }
            else
            {
                return _context.SqlQuery(GetSql(sql), GetParamsters(parms));
            }
        }

        public OceanDynamicList<object> GetDynamicList(string sql,int pageIndex,int pageSize, Dictionary<string, object> parms)
        {
            OceanDynamicList<object> dynamicPageList;
            int totalCount = GetCount(sql,parms);
            if (parms == null)
            {
                dynamicPageList = _context.SqlQuery(GetRowNumberPageSql(sql, pageIndex, pageSize));
            }
            else
            {
                dynamicPageList = _context.SqlQuery(GetRowNumberPageSql(sql,pageIndex,pageSize), GetParamsters(parms));
            }
            if (dynamicPageList != null)
            {
                dynamicPageList.PageSize = pageSize;
                dynamicPageList.CurrentPageIndex = pageIndex;
                dynamicPageList.TotalItemCount = totalCount;
            }
            return dynamicPageList;
        }
        public OceanDynamicList<object> GetDynamicList(string sql, int pageIndex, int pageSize)
        {
            return GetDynamicList(sql, pageIndex, pageSize, null);
        }


        public IList<TKey> GetList<TKey>(string sql)
        {
            return GetList <TKey>(sql, null);
        }

        public IList<TKey> GetList<TKey>(string sql, Dictionary<string, object> parms)
        {
            if (parms == null)
            {
                return _context.SqlQuery<TKey>(GetSql(sql)).ToList();
            }
            else
            {
                return _context.SqlQuery<TKey>(GetSql(sql), GetParamsters(parms)).ToList();
            }
        }


        public T GetUnique(string sql)
        {
            return GetUnique(sql, null);
        }

        public T GetUnique(string sql, Dictionary<string, object> parms)
        {
            if (parms == null)
            {
                return _context.SqlQuery<T>(GetSql(sql)).FirstOrDefault();
            }
            else
            {
                return _context.SqlQuery<T>(GetSql(sql), GetParamsters(parms)).FirstOrDefault();
            }
        }

        public TKey GetUnique<TKey>(string sql)
        {
            return GetUnique<TKey>(sql, null);
        }

        public TKey GetUnique<TKey>(string sql, Dictionary<string, object> parms)
        {
            if (parms == null)
            {
                return _context.SqlQuery<TKey>(GetSql(sql)).FirstOrDefault();
            }
            else
            {
                return _context.SqlQuery<TKey>(GetSql(sql), GetParamsters(parms)).FirstOrDefault();
            }
        }

        public OceanDynamic GetDynamicUnique(string sql)
        {
            return GetDynamicUnique(sql, null);
        }

        public OceanDynamic GetDynamicUnique(string sql, Dictionary<string, object> parms)
        {
            if (parms == null)
            {
                return _context.SqlDynamicQuery(GetSql(sql));
            }
            else
            {
                return _context.SqlDynamicQuery(GetSql(sql), GetParamsters(parms));
            }
        }


        public PagedList<T> GetPageList(string sql, int pageIndex, int pageSize)
        {
            return GetPageList(sql, null, pageIndex, pageSize);
        }

        public PagedList<T> GetPageList(string sql, Dictionary<string, object> parms, int pageIndex, int pageSize)
        {
            if (parms == null)
            {
                int count = GetCount(GetCountSql(sql));
                IList<T> tlist = _context.SqlQuery<T>(GetRowNumberPageSql(sql, pageIndex, pageSize)).ToList();
                PagedList<T> tpage = new PagedList<T>(tlist, pageIndex, pageSize, count);
                tpage.TotalItemCount = count;
                return tpage;
            }
            else
            {
                int count = GetCount(GetCountSql(sql), parms);
                IList<T> tlist = _context.SqlQuery<T>(GetRowNumberPageSql(sql, pageIndex, pageSize),GetParamsters(parms)).ToList();
                PagedList<T> tpage = new PagedList<T>(tlist, pageIndex, pageSize, count);
                tpage.TotalItemCount = count;
                return tpage;
            }
        }

        public PagedList<TKey> GetPageList<TKey>(string sql, int pageIndex, int pageSize)
        {
            return GetPageList<TKey>(sql, null, pageIndex, pageSize);
        }

        public PagedList<TKey> GetPageList<TKey>(string sql, Dictionary<string, object> parms, int pageIndex, int pageSize)
        {
            if (parms == null)
            {
                int count = GetCount(GetCountSql(sql));
                IList<TKey> tlist = _context.SqlQuery<TKey>(GetRowNumberPageSql(sql, pageIndex, pageSize)).ToList();
                PagedList<TKey> tpage = new PagedList<TKey>(tlist, pageIndex, pageSize, count);
                tpage.TotalItemCount = count;
                return tpage;
            }
            else
            {
                int count = GetCount(GetCountSql(sql), parms);
                IList<TKey> tlist = _context.SqlQuery<TKey>(GetRowNumberPageSql(sql, pageIndex, pageSize), GetParamsters(parms)).ToList();
                PagedList<TKey> tpage = new PagedList<TKey>(tlist, pageIndex, pageSize, count);
                tpage.TotalItemCount = count;
                return tpage;
            }
        }

        #endregion

        #region ESQL
        #endregion

        #region private
        private DbParameter[] GetParamsters(Dictionary<string, object> parms)
        {
            if (parms == null)
                return null;
            DbParameter[] dbParams = new DbParameter[parms.Keys.Count];
            int i = 0;
            foreach (KeyValuePair<string, object> parm in parms)
            {
                dbParams[i] = _dataProvider.GetParameter();
                dbParams[i].ParameterName = "@" + parm.Key;
                dbParams[i].Value = parm.Value;
                i++;
            }
            return dbParams;
        }

        private string GetCountSql(string sql)
        {
            sql = GetSql(sql);
            int fromIndex = sql.ToLower().IndexOf(" from");
            sql = Regex.Replace(sql, "'(@\\w*?Id)'","${1}");
            return "select count(1)" + RemoveOrders(sql.Substring(fromIndex));
        }

        private string RemoveOrders(string sql)
        {
            sql = Regex.Replace(sql, "order\\s*by[\\w|\\W|\\s|\\S]*", string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            sql.Replace("fetch", "");
            return sql;
        }

        #region 获取删除语句
        private string GetDeleteSql(string sql, Dictionary<string, object> parms)
        {
            #region sql 字符串切割函数-暂不使用
            //--------------------------这个函数用来切割字符串的-----------------   
            //--函数的参数  第一个是要切割的字符串 第二个是要以什么字符串切割   
            //CREATE FUNCTION [dbo].[Split](@Text NVARCHAR(MAX),@Sign NVARCHAR(4000))   
            //RETURNS  @tempTable TABLE([VALUE] NVARCHAR(4000))   
            //AS  
            //BEGIN  
            //  DECLARE @StartIndex INT                --开始查找的位置   
            //     DECLARE @FindIndex  INT                --找到的位置   
            //     DECLARE @Content    VARCHAR(4000)    --找到的值   
            //   --初始化一些变量   
            //     SET @StartIndex = 1 --T-SQL中字符串的查找位置是从1开始的   
            //    SET @FindIndex=0   

            //    --开始循环查找字符串逗号   
            //     WHILE(@StartIndex <= LEN(@Text))   
            //    BEGIN  
            //        --查找字符串函数 CHARINDEX  第一个参数是要找的字符串   
            //       --                            第二个参数是在哪里查找这个字符串   
            //        --                            第三个参数是开始查找的位置   
            //       --返回值是找到字符串的位置   
            //        SELECT @FindIndex = CHARINDEX(@Sign,@Text,@StartIndex)   
            //        --判断有没找到 没找到返回0   
            //         IF(@FindIndex =0 OR @FindIndex IS NULL)   
            //         BEGIN  
            //            --如果没有找到者表示找完了   
            //           SET @FindIndex = LEN(@Text)+1   
            //        END  
            //        --截取字符串函数 SUBSTRING  第一个参数是要截取的字符串   
            //        --                            第二个参数是开始的位置   
            //        --                            第三个参数是截取的长度   
            //        --@FindIndex-@StartIndex 表示找的的位置-开始找的位置=要截取的长度   
            //        --LTRIM 和 RTRIM 是去除字符串左边和右边的空格函数   
            //         SET @Content = LTRIM(RTRIM(SUBSTRING(@Text,@StartIndex,@FindIndex-@StartIndex)))   
            //         --初始化下次查找的位置   
            //         SET @StartIndex = @FindIndex+1   
            //         --把找的的值插入到要返回的Table类型中   
            //        INSERT INTO @tempTable ([VALUE]) VALUES (@Content)    
            //     END  
            //    RETURN  
            //END  
            #endregion

            StringBuilder parmsBuilder = null;
            if (parms != null)
            {
                parmsBuilder = new StringBuilder();
                int i=0;
                foreach (KeyValuePair<string, object> kvp in parms)
                {
                    parmsBuilder.Append(kvp.Key);
                    if (kvp.Value.GetType().BaseType == typeof(Array))
                    {
                        parmsBuilder.Append(" in ('+@");
                        parmsBuilder.Append(kvp.Key);
                        parmsBuilder.Append("+')");
                    }
                    else
                    {
                        parmsBuilder.Append("='+@");
                        parmsBuilder.Append(kvp.Key);
                        parmsBuilder.Append("+'");
                    }
                    if(i>0)
                    {
                        parmsBuilder.Append(",");
                    }
                    i++;
                }
            }

            if (string.IsNullOrEmpty(sql))
            {
                string tableName=_repository.GetTableName();
                sql = "exec('delete from " + tableName.Substring(0, tableName.IndexOf(" AS"));
                if (parmsBuilder != null)
                {
                    sql += " where " + parmsBuilder.ToString() + "')";
                }
                else
                {
                    sql += "')";
                }
            }
            else
            {
                if (sql.Trim().StartsWith("where"))
                {
                    string tableName = _repository.GetTableName();
                    sql = "exec('delete from " + tableName.Substring(0, tableName.IndexOf(" AS")) + " " + sql.Replace("'","''") + "')";
                }
            }
            return sql;
        }
        /// <summary>
        /// 将query转化为删除语句
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private string GetDeleteSql(IQueryable<T> query,IList<object> parms)
        {
            string sql = query.GetSqlString<T>(parms);
            sql = "delete " + sql.Substring(sql.IndexOf("from", StringComparison.OrdinalIgnoreCase));
            sql = sql.Replace("[Extent1].", "").Replace("AS [Extent1]", "").Replace("__linq__", "");
            return sql;
        }
        private DbParameter[] GetDeleteParameters(Dictionary<string, object> parms)
        {
            if (parms == null)
                return null;
            DbParameter[] dbParams = new DbParameter[parms.Keys.Count];
            int i = 0;
            foreach (KeyValuePair<string, object> parm in parms)
            {
                dbParams[i] = _dataProvider.GetParameter();
                dbParams[i].ParameterName = "@" + parm.Key;
                if (parm.Value.GetType().BaseType==typeof(Array)&& parm.Value.GetType() != typeof(int[]))
                {
                    Array arr = parm.Value as Array;
                    if (arr != null)
                    {
                        int x = 0;
                        IEnumerator a= arr.GetEnumerator();
                        while(a.MoveNext())
                        {
                            if (x == 0)
                            {
                                dbParams[i].Value = "'" + a.Current.ToString() + "'";
                            }
                            else {
                                dbParams[i].Value += ",'" + a.Current.ToString()+"'";
                            }
                            x++;
                        }
                    }
                }
                else
                {
                    dbParams[i].Value = parm.Value;
                }
                i++;
            }
            return dbParams;
        }
        #endregion

        #region 获取更新语句
        public string GetUpdateSql(IQueryable<T> query, IList<object> parms, Expression<Func<T>> updater)
        {
            string sql = query.GetSqlString<T>(parms);
            sql = sql.Substring(sql.IndexOf("from", StringComparison.OrdinalIgnoreCase)).Replace("__linq__", "");
            int paramindex = parms.Count;
             //获取Update的赋值语句
             var valueObj = updater.Compile().Invoke();
            MemberInitExpression updateMemberExpr = (MemberInitExpression)updater.Body;
             StringBuilder updateBuilder = new StringBuilder();          
             Type valueType = typeof(T);
             foreach (var bind in updateMemberExpr.Bindings.Cast<MemberAssignment>())
             {
                 string name = bind.Member.Name;
                 updateBuilder.AppendFormat("{0}=@p{1},", name, paramindex++);                
                 var value = valueType.GetProperty(name).GetValue(valueObj,null);//
                 parms.Add(value);
             }
             if (updateBuilder.Length == 0)
             {
                 throw new Exception("请填写要更新的值");
             }
             else
             {
                 sql = " update [Extent1] set " + updateBuilder.Remove(updateBuilder.Length - 1, 1).ToString() + " " + sql;
             }
             return sql;
        }
        #endregion

        private string GetSql(string sql)
        {
            #region 正则分组替换
            ////方法一
            //static string CustomReplace(System.Text.RegularExpressions.Match m) 
            //{ 
            //    return m.Groups[1].Value; //直接返回分组1
            //}
            //string sourceString = "....."; 
            //string pattern = @"(A\d{1,2})(,A\d{1,2})"; 
            //System.Text.RegularExpressions.MatchEvaluator myEvaluator = new System.Text.RegularExpressions.MatchEvaluator(CustomReplace); 
            //System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase| System.Text.RegularExpressions.RegexOptions.Multiline); 
            //string resultString = reg.Replace(sourceString, myEvaluator); 

            ////方法二
            //string sourceString = "......";
            //string toWidth = "300"; //自定义的宽度
            //string pattern = "(<embed .+? width\\s{0,}=\\s{0,}\"{0,1})(\\d+)(\"{0,1})";
            //System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //string resultString = reg.Replace(sourceString, "${1}" + toWidth + "${3}");
            #endregion

            if (string.IsNullOrEmpty(sql))
            {
                //sql = _repository.GetTableSql();
                //Regex reg_ = new Regex("(\\s+AS\\s+\\[[\\w_-]+)_(\\])",RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //return reg_.Replace(sql, "${1}${2}");
                return "select * from " + _repository.GetTableName();
            }
            else
            {
                string _sql = sql.ToLower().Trim();
                if (_sql.Trim().StartsWith("where") || _sql.Trim().StartsWith("order by"))
                {
                    return "select * from " + _repository.GetTableName() +" "+ sql;
                }
                else if (_sql.Trim().StartsWith("from"))
                {
                    return "select * " + sql;
                }
                else
                {
                    return sql;
                }
            }
        }

        private string GetRowNumberPageSql(string sql,int pageIndex,int pageSize)
        {
            //指定页号的SQL语句的模版
            //SQL 2005 数据库，使用 Row_Number()分页
            //set nocount on;
            //with t_pager as (
            //   select *,rn = ROW_NUMBER() OVER (ORDER BY id desc) FROM test_indexorder
            // )
            //SELECT id,name,content,co1,co2,co3,co4,co5 from t_rn WHERE rn between 19007 and 19057;

            //去重：在over( partition by column1    --,重复列n 分组 )

            //另一种方法
            //select *
            //from (
            //    select row_number()over(order by __tc__)__rn__,*
            //    from (select top 开始位置+10 0 __tc__,* from Student where Age>18 order by Age)t
            //)tt
            //where __rn__>开始位置
            pageIndex=pageIndex==0?1:pageIndex;
            pageSize=pageSize==0?10:pageSize;
            var _sql = new StringBuilder();
            string outSql;
            string columnStr = getColumnsFromSql(sql, out outSql);
            string orderbyStr = getOrderByFromSql(outSql, out outSql);
            //sql.Append("set nocount on; ");
            _sql.Append("with t_pager as (select rn = ROW_NUMBER() OVER ( ");
            _sql.Append(orderbyStr);
            _sql.Append(" ),");
            _sql.Append(columnStr);
            _sql.Append(outSql);
            _sql.Append(" ) select *");
            _sql.Append("  from t_pager where rn between " + ((pageIndex - 1) * pageSize+1).ToString() + " and " + (pageIndex * pageSize).ToString());
            return Regex.Replace(_sql.ToString(), "'(@\\w*?Id)'", "${1}");
        }

        /// <summary>
        /// 从sql提取查询的列
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private string getColumnsFromSql(string sql,out string subSql)
        {
            sql = GetSql(sql).TrimStart();
            int fromIndex = sql.ToLower().IndexOf(" from");
            string columnStr= sql.Substring(7, fromIndex - 6);
            subSql = sql.Substring(fromIndex);
            return columnStr;
        }

        private string getOrderByFromSql(string sql, out string subSql)
        {
            Regex reg = new Regex(@"\s+order\s+by\s+.*?\s(desc|asc)|\s+order\s+by\s+.*?\s", RegexOptions.IgnoreCase);
            Match match=reg.Match(sql);
            if (match.Success)
            {
                string orderbyStr = match.Value;
                subSql = reg.Replace(sql, "");
                return orderbyStr;
            }
            else
            {
                subSql = sql;
                return " order by id ";
            }
        }
        private string clearColumnsPre(string columnStr)
        {
            Regex reg = new Regex(@"([a-zA-Z_]*?)\.", RegexOptions.IgnoreCase);
            return reg.Replace(columnStr, "");
        }
        /// <summary>
        /// 约束：分页字段前缀固定 t_pager
        /// </summary>
        /// <param name="columnStr"></param>
        /// <returns></returns>
        private string getTPagerPreColumnSql(string columnStr)
        {
            Regex reg=new Regex("\\[(\\w+\\)]\\.");
            return reg.Replace(columnStr, "t_pager.");
        }

        #endregion

        #region 事务
        public void BeginTransaction()
        {
            _context.BeginTransaction();
        }

        public void Commit()
        {
            _context.Commit();
        }
        public void Rollback(bool SureRollback = true)
        {
            _context.Rollback();
        }
        #endregion
    }
}