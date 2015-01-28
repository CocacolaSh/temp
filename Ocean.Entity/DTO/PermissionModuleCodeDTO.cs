using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Entity.DTO
{
    public class PermissionModuleCodeDTO : BaseDTO
    {
        public PermissionModuleCodeDTO(PermissionModuleCode permissionModuleCode)
        {
            this.Id = permissionModuleCode.Id;
            this.CreateDate = permissionModuleCode.CreateDate;
            this.Name = permissionModuleCode.Name;
            this.Code = permissionModuleCode.Code;
            this.ModuleName = permissionModuleCode.PermissionModule.Name;
        }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 权限编码
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        /// 所属模块
        /// </summary>
        public string ModuleName { set; get; }
    }
}
