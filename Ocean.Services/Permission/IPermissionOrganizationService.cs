using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity;

namespace Ocean.Services
{
    public interface IPermissionOrganizationService : IService<PermissionOrganization>
    {
        /// <summary>
        /// 更新根路径
        /// </summary>
        /// <param name="oldRootPath">旧的根路径</param>
        /// <param name="newRootPath">新的根路径</param>
        void UpdateRootPath(string oldRootPath, string newRootPath);

        /// <summary>
        /// 根据上级Id删除部门
        /// </summary>
        /// <param name="parentId">上级Id</param>
        void DeleteOrganizationByParentId(Guid parentId);

        /// <summary>
        /// 统计子部门数量
        /// </summary>
        /// <param name="id">部门Id</param>
        /// <returns></returns>
        int CountChildOrganizationNumber(Guid id);
    }
}