using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using Ocean.Core.Utility;
using Ocean.Core.Common;
using System.Web;
using Ocean.Core.Infrastructure;
using Ocean.Framework.Configuration.global.config;
using Ocean.Entity;
using Ocean.Services;

namespace Ocean.Page
{
    public class AdminLogin
    {
        private static object objLock = new object();

        #region private static AdminLogin instance 管理员登录实例

        private static AdminLogin instance = null;

        /// <summary>
        /// 管理员登录实例
        /// </summary>
        public static AdminLogin Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (objLock)
                    {
                        if (instance == null)
                        {
                            instance = new AdminLogin();
                        }

                        return instance;
                    }
                }

                return instance;
            }
        }
        #endregion

        #region 储蓄管理员登录信息Cookie名称
        /// <summary>
        /// 储蓄管理员登录信息Cookie名称
        /// </summary>
        private const string CookieUserName = "ocean.admin";
        #endregion

        #region [创建Admin Cookie] public void CreateAdminCookie(Guid id, string password, string passwordkey, string safeCode)
        /// <summary>
        /// 创建Admin Cookie
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="password">密码</param>
        /// <param name="passwordKey">密钥</param>
        /// <param name="safeCode">安全码</param>
        public void CreateAdminCookie(Guid id, string password, string passwordKey, string safeCode)
        {
            string ip = IpHelper.UserHostAddress;
            FormsAuthentication.SetAuthCookie(StrCrypt.EncryptDes(string.Format("{0}|{1}|{2}|{3}", id, StrCrypt.EncryptDes(password, passwordKey), safeCode, IpHelper.UserHostAddress), ip), false);
        }
        #endregion

        #region [退出管理后台] public void AdminLogout()
        /// <summary>
        /// 退出后台管理后台
        /// </summary>
        public void AdminLogout()
        {
            FormsAuthentication.SignOut();
        }
        #endregion

        #region [获取当前登陆管理员信息]
        /// <summary>
        /// 获取当前登陆管理员信息
        /// </summary>
        public Admin Admin
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return null;
                }

                if (HttpContext.Current.User == null)
                {
                    return null;
                }

                if (!(HttpContext.Current.User.Identity is FormsIdentity))
                {
                    HttpContext.Current.Response.Write(HttpContext.Current.User.Identity.Name);
                    return null;
                }

                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    return null;
                }

                string ip = IpHelper.UserHostAddress;
                FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
                string value = identity.Name;

                if (!StrCrypt.DecryptDes(ref value, ip))
                {
                    return null;
                }

                if (value == null)
                {
                    return null;
                }

                string[] Obj = value.Split('|');

                if (Obj.Length != 4)
                {
                    return null;
                }

                if (Obj[3] != IpHelper.UserHostAddress)
                {
                    return null;
                }

                if (Obj[2] != GlobalConfig.GetConfig()["SafeCode"])
                {
                    return null;
                }

                var adminService = EngineContext.Current.Resolve<IAdminService>();
                Admin modelAdmin = adminService.GetById(new Guid(Obj[0]));

                if (modelAdmin != null && StrCrypt.DecryptDes(ref Obj[1], modelAdmin.PasswordKey))
                {
                    if (Obj[1] == modelAdmin.Password)
                    {
                        return modelAdmin;
                    }
                }

                return null;
            }
        }
        #endregion
    }
}