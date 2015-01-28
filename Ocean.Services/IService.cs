using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;
using Ocean.Data;
using System.Linq.Expressions;
using Ocean.Core.Data.DynamicQuery;

namespace Ocean.Services
{
    public interface IService<T> : IServiceBase where T : BaseEntity
    {
        #region LINQ
        T GetById(object id);
        void Insert(T entity);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="where">lambda条件表达式</param>
        /// <param name="updater">lambda更新内容表达式[()=>new T(){propertyname=value,}]</param>
        /// <returns></returns>
        int Update(Expression<Func<T, bool>> where, Expression<Func<T>> updater);
        void Update(T entity);
        /// <summary>
        /// linq方式自动生成删除语句
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        int Delete(Expression<Func<T, bool>> where);
        void Delete(T entity);
        IQueryable<T> Table { get; }

        T Read(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 获取所有记录
        /// </summary>
        /// <returns></returns>
        IList<T> GetALL();
        /// <summary>
        /// 获取所有记录
        /// </summary>
        /// <param name="predicate">Lanbda表达式条件</param>
        /// <param name="keySelector">排序字段名称</param>
        /// <returns></returns>
        IList<T> GetALL(Expression<Func<T, bool>> predicate, string keySelector, bool isAscendingOrder = true);
        /// <summary>
        /// 获取所有记录
        /// </summary>
        /// <param name="predicate">Lanbda表达式条件</param>
        /// <returns></returns>
        IList<T> GetALL(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 获取所有记录
        /// </summary>
        /// <param name="keySelector">排序字段名称</param>
        /// <returns></returns>
        IList<T> GetALL(string keySelector, bool isAscendingOrder = true);
        /// <summary>
        /// 动态查询条件-获取所有记录
        /// </summary>
        /// <param name="conditions">条件数组，参数方式
        /// new QueryCondition[] {
        ///            new QueryCondition { Key = "Name",Value = "test1", Operator = QueryOperator.CONTAINS }
        ///        }
        /// </param>
        /// <param name="key">排序字段［暂时都只支持单字段排序］</param>
        /// <param name="isAscendingOrder">是否正序</param>
        /// <returns></returns>
        IList<T> GetALL(QueryCondition[] conditions, string key, bool isAscendingOrder = true);
        /// <summary>
        /// 动态查询条件-获取所有记录
        /// </summary>
        /// <param name="conditions">条件数组，参数方式
        /// new QueryCondition[] {
        ///            new QueryCondition { Key = "Name",Value = "test1", Operator = QueryOperator.CONTAINS }
        ///        }
        /// </param>
        /// <returns></returns>
        IList<T> GetALL(QueryCondition[] conditions);

        /// <summary>
        /// 返回单条记录
        /// </summary>
        /// <param name="predicate">Lanbda表达式条件</param>
        /// <returns></returns>
        T GetUnique(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 返回单条记录
        /// </summary>
        /// <param name="predicate">Lanbda表达式条件</param>
        /// <param name="keySelector">排序字段名称</param>
        /// <param name="isAscendingOrder">是否正序</param>
        /// <returns></returns>
        T GetUnique(Expression<Func<T, bool>> predicate, string keySelector, bool isAscendingOrder = true);

        /// <summary>
        /// 获取TOP n 记录
        /// </summary>
        /// <param name="num">n</param>
        /// <param name="predicate">Lanbda表达式条件</param>
        /// <param name="keySelector">排序字段名称</param>
        /// <returns></returns>
        IList<T> GetLimitList(int num, Expression<Func<T, bool>> predicate, string keySelector, bool isAscendingOrder = true);
        /// <summary>
        /// 获取TOP n 记录
        /// </summary>
        /// <param name="num">n</param>
        /// <returns></returns>
        IList<T> GetLimitList(int num);

        /// <summary>
        /// 获取TOP n 记录
        /// </summary>
        /// <param name="num">n</param>
        /// <param name="predicate">Lanbda表达式条件</param>
        /// <returns></returns>
        IList<T> GetLimitList(int num, Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 获取TOP n 记录
        /// </summary>
        /// <param name="num">n</param>
        /// <param name="keySelector">排序字段名称</param>
        /// <returns></returns>
        IList<T> GetLimitList(int num, string keySelector, bool isAscendingOrder = true);
        /// <summary>
        /// 动态查询条件-获取TOP n 记录
        /// </summary>
        /// <param name="num">TOP n</param>
        /// <param name="conditions">条件数组，参数方式
        /// new QueryCondition[] {
        ///            new QueryCondition { Key = "Name",Value = "test1", Operator = QueryOperator.CONTAINS }
        ///        }
        /// </param>
        /// <param name="key">排序字段［暂时都只支持单字段排序］</param>
        /// <param name="isAscendingOrder">是否正序</param>
        /// <returns></returns>
        IList<T> GetLimitList(int num, QueryCondition[] conditions, string key, bool isAscendingOrder = true);
        /// <summary>
        /// 动态查询条件-获取TOP n 记录
        /// </summary>
        /// <param name="num">TOP n</param>
        /// <param name="conditions">条件数组，参数方式
        /// new QueryCondition[] {
        ///            new QueryCondition { Key = "Name",Value = "test1", Operator = QueryOperator.CONTAINS }
        ///        }
        /// </param>
        /// <returns></returns>
        IList<T> GetLimitList(int num, QueryCondition[] conditions);

        /// <summary>
        /// 获取分页记录-
        /// </summary>
        /// <param name="predicate">Lanbda表达式条件</param>
        /// <param name="keySelector">排序字段名称</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns></returns>
        PagedList<T> GetPageList(Expression<Func<T, bool>> predicate, string keySelector, int pageIndex, int pageSize, bool isAscendingOrder = true);
        /// <summary>
        /// 获取分页记录-
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns></returns>
        PagedList<T> GetPageList(int pageIndex, int pageSize);
        /// <summary>
        /// 获取分页记录-
        /// </summary>
        /// <param name="predicate">Lanbda表达式条件</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns></returns>
        PagedList<T> GetPageList(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize);
        /// <summary>
        /// 获取分页记录-
        /// </summary>
        /// <param name="keySelector">排序字段名称</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns></returns>
        PagedList<T> GetPageList(string keySelector, int pageIndex, int pageSize, bool isAscendingOrder = true);
        /// <summary>
        /// 动态查询条件-获取分页记录
        /// </summary>
        /// <param name="conditions">条件数组，参数方式
        /// new QueryCondition[] {
        ///            new QueryCondition { Key = "Name",Value = "test1", Operator = QueryOperator.CONTAINS }
        ///        }
        /// </param>
        /// <param name="key">排序字段［暂时都只支持单字段排序］</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="isAscendingOrder">是否正序</param>
        /// <returns></returns>
        IList<T> GetPageList(QueryCondition[] conditions, string key, int pageIndex, int pageSize, bool isAscendingOrder = true);
        /// <summary>
        /// 动态查询条件-获取分页记录
        /// </summary>
        /// <param name="conditions">条件数组，参数方式
        /// new QueryCondition[] {
        ///            new QueryCondition { Key = "Name",Value = "test1", Operator = QueryOperator.CONTAINS }
        ///        }
        /// </param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns></returns>
        IList<T> GetPageList(QueryCondition[] conditions, int pageIndex, int pageSize);

        #endregion

        #region SQL语句
        /// <summary>
        /// 执行删除-可自动根据参数拼接完成删除语句
        /// </summary>
        /// <param name="sql">可为空｜完整｜只带where 语句</param>
        /// <returns></returns>
        int Delete(string sql);
        /// <summary>
        /// 执行删除-可自动根据参数拼接完成删除语句
        /// </summary>
        /// <param name="parms">参数[当sql为空时，参数会自动根据key生成参数],如果批量处理的话，请传入相应类型的数据T[]</param>
        /// <returns></returns>
        int Delete(Dictionary<string, object> parms);
        /// <summary>
        /// 执行删除-可自动根据参数拼接完成删除语句
        /// </summary>
        /// <param name="sql">可为空｜完整｜只带where 语句</param>
        /// <param name="parms">参数[当sql为空时，参数会自动根据key生成参数],如果批量处理的话，请传入相应类型的数据T[]</param>
        /// <returns></returns>
        int Delete(string sql, Dictionary<string, object> parms);
        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="sql">完整的语句</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        int ExcuteSql(string sql, Dictionary<string, object> parms);
        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="sql">完整的语句</param>
        /// <returns></returns>
        int ExcuteSql(string sql);

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        TKey ExcuteProc<TKey>(string procName, Dictionary<string, object> parms);
        /// <summary>
        /// 获取记录总数
        /// </summary>
        /// <param name="sql">完整的语句，或只带where或只带from的字符串</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        int GetCount(string sql, Dictionary<string, object> parms);

        /// <summary>
        /// 获取记录总数
        /// </summary>
        /// <param name="sql">完整的语句，或只带where或只带from的字符串</param>
        /// <returns></returns>
        int GetCount(string sql);

        /// <summary>
        /// 获取记录列表
        /// </summary>
        /// <param name="sql">完整的语句，或只带where或只带from的字符串</param>
        /// <returns></returns>
        IList<T> GetList(string sql);
        IList<TKey> GetList<TKey>(string sql);

        /// <summary>
        /// 获取记录列表
        /// </summary>
        /// <param name="sql">完整的语句，或只带where或只带from的字符串</param>
        /// <param name="parms"></param>
        /// <returns></returns>
        IList<T> GetList(string sql, Dictionary<string, object> parms);
        IList<TKey> GetList<TKey>(string sql, Dictionary<string, object> parms);

        /// <summary>
        /// 获取动态类型记录列表-主要用于
        /// </summary>
        /// <param name="sql">完整的语句，或只带where或只带from的字符串</param>
        /// <param name="parms"></param>
        /// <returns></returns>
        OceanDynamicList<object> GetDynamicList(string sql);
        /// <summary>
        /// 获取动态类型记录列表-主要用于
        /// </summary>
        /// <param name="sql">完整的语句，或只带where或只带from的字符串</param>
        /// <param name="parms">参数列表</param>
        /// <returns></returns>
        OceanDynamicList<object> GetDynamicList(string sql, Dictionary<string, object> parms);
        /// <summary>
        /// 获取动态类型记录列表-主要用于
        /// </summary>
        /// <param name="sql">完整的语句，或只带where或只带from的字符串</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="parms">参数列表</param>
        /// <returns></returns>
        OceanDynamicList<object> GetDynamicList(string sql, int pageIndex, int pageSize, Dictionary<string, object> parms);
        /// <summary>
        /// 获取动态类型记录列表-主要用于
        /// </summary>
        /// <param name="sql">完整的语句，或只带where或只带from的字符串</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns></returns>
        OceanDynamicList<object> GetDynamicList(string sql, int pageIndex, int pageSize);
        /// <summary>
        /// 获取单条记录
        /// </summary>
        /// <param name="sql">完整的语句，或只带where或只带from的字符串</param>
        /// <returns></returns>
        T GetUnique(string sql);
        TKey GetUnique<TKey>(string sql);
        /// <summary>
        /// 获取单条记录
        /// </summary>
        /// <param name="sql">完整的语句，或只带where或只带from的字符串</param>
        /// <param name="parms"></param>
        /// <returns></returns>
        T GetUnique(string sql, Dictionary<string, object> parms);
        TKey GetUnique<TKey>(string sql, Dictionary<string, object> parms);

        /// <summary>
        /// 获取动态类型单条记录
        /// </summary>
        /// <param name="sql">完整的语句，或只带where或只带from的字符串</param>
        /// <returns></returns>
        OceanDynamic GetDynamicUnique(string sql);
        /// <summary>
        /// 获取动态类型单条记录
        /// </summary>
        /// <param name="sql">完整的语句，或只带where或只带from的字符串</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        OceanDynamic GetDynamicUnique(string sql, Dictionary<string, object> parms);

        /// <summary>
        /// 获取分页记录
        /// </summary>
        /// <param name="sql">完整的语句，或只带where或只带from的字符串</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns></returns>
        PagedList<T> GetPageList(string sql, int pageIndex, int pageSize);
        /// <summary>
        /// 获取分页记录
        /// </summary>
        /// <param name="sql">完整的语句，或只带where或只带from的字符串</param>
        /// <param name="parms"></param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns></returns>
        PagedList<T> GetPageList(string sql, Dictionary<string, object> parms, int pageIndex, int pageSize);

        PagedList<TKey> GetPageList<TKey>(string sql, int pageIndex, int pageSize);

        PagedList<TKey> GetPageList<TKey>(string sql, Dictionary<string, object> parms, int pageIndex, int pageSize);

        #endregion

        #region 事务
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
        #endregion
    }
}