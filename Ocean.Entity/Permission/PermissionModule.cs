using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    public class PermissionModule : BaseEntity
    {
        private ICollection<PermissionModuleCode> _permissionModuleCodes;

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name 
        {
            get { return base.GetField<string>("Name"); }
            set { SetField("Name", value); }
        }

        /// <summary>
        /// 上级Id
        /// </summary>
        public Guid ParentId
        {
            get { return base.GetFieldGuid("ParentId"); }
            set { SetField("ParentId", value); }
        }

        /// <summary>
        /// 模块标识
        /// </summary>
        public string Identifying
        {
            get { return base.GetField<string>("Identifying"); }
            set { SetField("Identifying", value); }
        }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string Url
        {
            get { return base.GetField<string>("Url"); }
            set { SetField("Url", value); }
        }

        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort
        {
            get { return base.GetField<int>("Sort"); }
            set { SetField("Sort", value); }
        }

        /// <summary>
        /// 路径
        /// </summary>
        public string RootPath { set; get; }

        /// <summary>
        /// 模块权限集
        /// </summary>
        public virtual ICollection<PermissionModuleCode> PermissionModuleCodes
        {
            get { return _permissionModuleCodes ?? (_permissionModuleCodes = new List<PermissionModuleCode>()); }
            protected set { _permissionModuleCodes = value; }
        }
    }
}