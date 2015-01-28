using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    public class PermissionOrganization : BaseEntity
    {
        private ICollection<Admin> _admins;

        /// <summary>
        /// 部门名称
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
        /// 管理员集
        /// </summary>
        public virtual ICollection<Admin> Admins
        {
            get { return _admins ?? (_admins = new List<Admin>()); }
            protected set { _admins = value; }
        }
    }
}