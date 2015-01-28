using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    public class PermissionModuleCode : BaseEntity
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { set; get; }
        
        /// <summary>
        /// 权限编码
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        /// 模块外键Id
        /// </summary>
        public Guid PermissionModuleId { set; get; }

        /// <summary>
        /// 模块实体
        /// </summary>
        public virtual PermissionModule PermissionModule { set; get; }
    }
}