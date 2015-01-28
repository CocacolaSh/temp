using System;
using System.Collections.Generic;
using Ocean.Core.Data;
using Ocean.Data;
using Ocean.Entity;

namespace Ocean.Services
{
    public class AdminService : ServiceBase<Admin>, IAdminService
    {
        public AdminService(IRepository<Admin> adminRepository, IDbContext context)
            : base(adminRepository, context)
        {

        }

        /// <summary>
        /// 根据用户名查询管理员
        /// </summary>
        public Admin GetAdminByName(string name)
        {
            return base.GetUnique(a => a.Name == name);
        }

        /// <summary>
        /// 冻结管理员
        /// </summary>
        public bool FreezeAdmin(Guid id)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("Id", id);
            return ExcuteSql(string.Format("Update Admin set State=2 where Id = {0} and State=1", "@Id"), dictionary) > 0;
        }

        /// <summary>
        /// 解冻管理员
        /// </summary>
        public bool UnFreezeAdmin(Guid id)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("Id", id);
            return ExcuteSql(string.Format("Update Admin set State=1 where Id = {0} and State=2", "@Id"), dictionary) > 0;
        }
    }
}