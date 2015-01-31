using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    public class Admin : BaseEntity
    {
        private ICollection<KfNumber> _kfNumbers;

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { set; get; }

        /// <summary>
        /// 密匙
        /// </summary>
        public string PasswordKey { set; get; }

        /// <summary>
        /// 状态[0:未审核，1:已经审核，2:被冻结]
        /// </summary>
        public int State { set; get; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginDate { set; get; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        public string LastLoginIP { set; get; }

        /// <summary>
        /// 登录次数
        /// </summary>
        public int LoginCount { set; get; }

        /// <summary>
        /// 部门外键Id
        /// </summary>
        public Guid? PermissionOrganizationId { set; get; }

        /// <summary>
        /// 部门实体
        /// </summary>
        public virtual PermissionOrganization PermissionOrganization { set; get; }

        /// <summary>
        /// 角色外键Id
        /// </summary>
        public Guid? PermissionRoleId { set; get; }

        /// <summary>
        /// 角色实体
        /// </summary>
        public virtual PermissionRole PermissionRole { set; get; }

        /// <summary>
        /// MpUser外键Id
        /// </summary>
        public Guid ?MpUserId { set; get; }
        /// <summary>
        /// MpUser实体
        /// </summary>
        public virtual MpUser MpUser { set; get; }

        /// <summary>
        /// 客服工号集
        /// </summary>
        public virtual ICollection<KfNumber> KfNumbers
        {
            get { return _kfNumbers ?? (_kfNumbers = new List<KfNumber>()); }
            protected set { _kfNumbers = value; }
        }
    }
}