using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;
using Ocean.Entity.DTO;
using Ocean.Entity;

namespace Ocean.Services
{
    public interface IAdminLoggerService : IService<AdminLogger>
    {
        /// <summary>
        /// 根据搜索条件获取分页数据
        /// </summary>
        PagedList<AdminLogger> GetPageList(int pageIndex, int pageSize, AdminLoggerDTO adminLoggerDTO);
    }
}