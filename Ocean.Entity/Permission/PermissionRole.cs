using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    public class PermissionRole : BaseEntity
    {
        private ICollection<Admin> _admins;

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 权限集
        /// </summary>
        public string Permissions { set; get; }

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