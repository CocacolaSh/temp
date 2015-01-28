using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Data;
using Ocean.Data;
using Ocean.Entity;

namespace Ocean.Services
{
    public class PermissionModuleService : ServiceBase<PermissionModule>, IPermissionModuleService
    {
        public PermissionModuleService(IRepository<PermissionModule> permissionModuleRepository, IDbContext context)
            : base(permissionModuleRepository, context)
        {
        }

        /// <summary>
        /// 更新根路径
        /// </summary>
        public void UpdateRootPath(string oldRootPath, string newRootPath)
        {
            string sql = string.Format("update {0} set RootPath=replace(RootPath,{1},{2}) where RootPath like {3}", "PermissionModule", "@OldRootPath", "@NewRootPath", "@OldRootPathLike");
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("OldRootPath", oldRootPath);
            dictionary.Add("NewRootPath", newRootPath);
            dictionary.Add("OldRootPathLike", oldRootPath + "%");
            ExcuteSql(sql, dictionary);
        }

        /// <summary>
        /// 统计子部门数量
        /// </summary>
        public int CountChildModuleNumber(Guid id)
        {
            return GetCount(string.Format(" where ParentId='{0}'", id));
        }
    }
}