using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity;

namespace Ocean.Services
{
    public interface IPermissionModuleService : IService<PermissionModule>
    {
        /// <summary>
        /// 更新根路径
        /// </summary>
        /// <param name="oldRootPath">旧的根路径</param>
        /// <param name="newRootPath">新的根路径</param>
        void UpdateRootPath(string oldRootPath, string newRootPath);

        /// <summary>
        /// 统计子模块数量
        /// </summary>
        /// <param name="id">模块Id</param>
        /// <returns></returns>
        int CountChildModuleNumber(Guid id);
    }
}