using System;
using Ocean.Core;
using Ocean.Entity;

namespace Ocean.Services
{
    public interface IPermissionModuleCodeService : IService<PermissionModuleCode>
    {
        /// <summary>
        /// 根据模块Id获取分页权限列表
        /// </summary>
        PagedList<PermissionModuleCode> GetPageListByModuleId(Guid moduleId, int pageIndex, int pageSize);
    }
}