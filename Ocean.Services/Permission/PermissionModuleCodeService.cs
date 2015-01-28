using System;
using Ocean.Core.Data;
using Ocean.Data;
using Ocean.Core;
using Ocean.Entity;

namespace Ocean.Services
{
    public class PermissionModuleCodeService : ServiceBase<PermissionModuleCode>, IPermissionModuleCodeService
    {
        public PermissionModuleCodeService(IRepository<PermissionModuleCode> permissionModuleCodeRepository, IDbContext context)
            : base(permissionModuleCodeRepository, context)
        {
        }

        /// <summary>
        /// 根据模块Id获取分页权限列表
        /// </summary>
        public PagedList<PermissionModuleCode> GetPageListByModuleId(Guid moduleId, int pageIndex, int pageSize)
        { 
            //string sql = string.Format(" where PermissionModuleId = '{0}'", "@PermissionModuleId");
            //Dictionary<string, object> dictionary = new Dictionary<string, object>();
            //dictionary.Add("PermissionModuleId", moduleId);
            return GetPageList(c => c.PermissionModuleId == moduleId, "CreateDate", pageIndex, pageSize, true);
        }
    }
}
