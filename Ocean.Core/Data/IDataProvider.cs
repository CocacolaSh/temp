using System.Data.Common;

namespace Ocean.Core.Data
{
    /// <summary>
    /// Data provider interface
    /// 数据提供者接口
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Initialize database
        /// 初始化数据库
        /// </summary>
        void InitDatabase();

        /// <summary>
        /// A value indicating whether this data provider supports stored procedures
        /// 一个值,该值指示此数据提供者是否支持存储过程
        /// </summary>
        bool StoredProceduredSupported { get; }

        /// <summary>
        /// Gets a support database parameter object (used by stored procedures)
        /// 得到一个支持数据库参数对象(使用存储过程)
        /// </summary>
        /// <returns>Parameter</returns>
        DbParameter GetParameter();
        /// <summary>
        /// Gets a support database DbTransaction
        /// 得到一个支持数据库事务
        /// </summary>
        /// <returns>DbTransaction</returns>
        DbTransaction GetTransaction(DbConnection conn);
    }
}
