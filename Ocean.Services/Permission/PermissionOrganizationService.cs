using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Data;
using Ocean.Data;
using Ocean.Entity;

namespace Ocean.Services
{
    public class PermissionOrganizationService : ServiceBase<PermissionOrganization>, IPermissionOrganizationService
    {
        public PermissionOrganizationService(IRepository<PermissionOrganization> permissionOrganizationRepository, IDbContext context)
            : base(permissionOrganizationRepository, context)
        {
           
        }

        /// <summary>
        /// 更新根路径
        /// </summary>
        public void UpdateRootPath(string oldRootPath, string newRootPath)
        {
            string sql = string.Format("update {0} set RootPath=replace(RootPath,{1},{2}) where RootPath like {3}", "PermissionOrganization", "@OldRootPath", "@NewRootPath", "@OldRootPathLike");
            Dictionary<string ,object> dictionary = new Dictionary<string,object>();
            dictionary.Add("OldRootPath",oldRootPath);
            dictionary.Add("NewRootPath", newRootPath);
            dictionary.Add("OldRootPathLike", oldRootPath + "%");
            ExcuteSql(sql, dictionary);
        }

        /// <summary>
        /// 根据上级Id删除部门
        /// </summary>
        public void DeleteOrganizationByParentId(Guid parentId)
        {
            string sql = string.Format("delete from {0} where RootPath like {1}", "PermissionOrganization", "@RootPath");
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("RootPath", parentId.ToString());
            ExcuteSql(sql, dictionary);
        }

        /// <summary>
        /// 统计子部门数量
        /// </summary>
        public int CountChildOrganizationNumber(Guid id)
        {
            return GetCount(string.Format(" where ParentId='{0}'", id));
        }
    }
}