using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Core.Utility;
using Ocean.Core.Common;
using Ocean.Page;
using Ocean.Framework.Configuration.global.config;
using Ocean.Entity.Enums.AdminLoggerModule;
using Ocean.Entity;
using Ocean.Services;

namespace Ocean.Admin.Controllers
{
    public class LoginController : PageBaseController
    {
        private readonly IAdminService _adminService;
        private readonly IAdminLoggerService _adminLoggerService;

        public LoginController(IAdminService adminService, IAdminLoggerService _adminLoggerService)
        {
            this._adminService = adminService;
            this._adminLoggerService = _adminLoggerService;
        }

        /// <summary>
        /// 系统登录页面
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 登录
        /// </summary>
        [ActionName("_Login")]
        [HttpPost]
        public ActionResult Login()
        {
            Ocean.Entity.Admin admin = new Entity.Admin();
            string adminName = Request["username"];
            string adminPassword = Request["password"];
            AdminLogger adminLogger = new AdminLogger();
            adminLogger.AdminName = adminName;
            adminLogger.CreateDate = DateTime.Now;
            adminLogger.FromIP = IpHelper.UserHostAddress;
            adminLogger.Module = (int)AdminLoggerModuleEnum.Admin;

            if (adminName.Length == 0)
            {
                return JsonMessage(false, "账号不能为空");
            }

            if (adminPassword.Length == 0)
            {
                return JsonMessage(false, "密码不能为空");
            }

            admin = _adminService.GetAdminByName(adminName);

            if (admin == null)
            {
                adminLogger.Description = string.Format("账号不存在,登录失败");
                _adminLoggerService.Insert(adminLogger);
                return JsonMessage(false, "账号不存在");
            }

            if (admin.Password != Hash.MD5Encrypt(Hash.MD5Encrypt(adminPassword)))
            {
                adminLogger.Description = string.Format("密码错误,登录失败");
                _adminLoggerService.Insert(adminLogger);
                return JsonMessage(false, "密码错误");
            }
            else if (admin.State == 2)
            {
                adminLogger.Description = string.Format("账号已被冻结,登录失败");
                _adminLoggerService.Insert(adminLogger);
                return JsonMessage(false, "账号已被冻结，请与管理员取得联系");
            }
            else
            {
                adminLogger.Description = string.Format("成功登录后台管理系统");
                _adminLoggerService.Insert(adminLogger);
                AdminLogin.Instance.CreateAdminCookie(admin.Id, admin.Password, admin.PasswordKey, GlobalConfig.GetConfig()["SafeCode"]);
                admin.LastLoginDate = DateTime.Now;
                admin.LoginCount = admin.LoginCount + 1;
                admin.LastLoginIP = IpHelper.UserHostAddress;
                _adminService.Update(admin);
                return JsonMessage(true, "登录成功");
            }
        }
    }
}