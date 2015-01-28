using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity;

namespace Ocean.Services
{
    public interface IAdminService : IService<Admin>
    {
        /// <summary>
        /// 根据用户名查询管理员
        /// </summary>
        Admin GetAdminByName(string name);

        /// <summary>
        /// 冻结管理员
        /// </summary>
        bool FreezeAdmin(Guid id);

        /// <summary>
        /// 解冻管理员
        /// </summary>
        bool UnFreezeAdmin(Guid id);
    }
}