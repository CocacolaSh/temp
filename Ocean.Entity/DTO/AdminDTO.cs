using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;

namespace Ocean.Entity.DTO
{
    public class AdminDTO : BaseDTO
    {
        public AdminDTO(Admin admin)
        {
            this.Id = admin.Id;
            this.CreateDate = admin.CreateDate;
            this.Name = admin.Name;
            this.State = admin.State;
            this.LastLoginDate = admin.LastLoginDate;
            this.LastLoginIP = admin.LastLoginIP;
            this.LoginCount = admin.LoginCount;
            this.OrganizationName = admin.PermissionOrganization.Name;
            this.RoleName = admin.PermissionRole.Name;
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 状态
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
        /// 所属部门名称
        /// </summary>
        public string OrganizationName { set; get; }

        /// <summary>
        /// 当前角色
        /// </summary>
        public string RoleName { set; get; }
    }
}