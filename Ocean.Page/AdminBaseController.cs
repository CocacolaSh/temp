using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ocean.Core.Infrastructure;
using Ocean.Core.Utility;
using Ocean.Framework.Caching.Cache;
using Ocean.Entity.Enums.AdminLoggerModule;
using Ocean.Entity;
using Ocean.Services;

namespace Ocean.Page
{
    public class AdminBaseController : PageBaseController
    {
        public AdminBaseController()
        {
            this.LoginAdmin = AdminLogin.Instance.Admin;
            this.IsLogin = this.LoginAdmin != null;
        }

        /// <summary>
        /// AdminService
        /// </summary>
        protected IAdminService AdminService
        {
            get
            {
                return EngineContext.Current.Resolve<IAdminService>();
            }
        }

        /// <summary>
        /// AdminLoggerService
        /// </summary>
        protected IAdminLoggerService AdminLoggerService
        {
            get
            {
                return EngineContext.Current.Resolve<IAdminLoggerService>();
            }
        }

        #region 登陆用户信息
        /// <summary>
        /// 登陆用户信息
        /// </summary>
        protected Admin LoginAdmin { set; get; }
        #endregion

        #region 登陆用户Id
        /// <summary>
        /// 登陆用户Id
        /// </summary>
        public Guid AdminId
        {
            get
            {
                return LoginAdmin.Id;
            }
        }
        #endregion

        #region 是否已登陆
        /// <summary>
        /// 是否已登陆
        /// </summary>
        protected bool IsLogin
        {
            get;
            set;
        }
        #endregion

        #region 返回管理视图
        /// <summary>
        /// 返回管理视图
        /// </summary>
        protected ActionResult AdminView(object model = null)
        {
            string view = string.Format("{0}", RouteData.Values["Action"].ToString());
            return View(view, model);
        }
        #endregion

        #region 权限判断
        /// <summary>
        /// 是否有权限
        /// </summary>
        /// <param name="moduleCode">模块编码</param>
        /// <param name="operateCode">权限编码</param>
        /// <returns></returns>
        protected bool HasPermission(string moduleCode, string operateCode)
        {
            if (this.LoginAdmin == null)
            {
                return false;
            }
            //组合权限编码
            string PermissionCode = moduleCode + "_" + operateCode;
            //获取权限列表
            List<PermissionModuleCode> permissionModuleCodeList = AdminPermissionsCache.Instance.CacheData;
            //判断用户权限列表是否有权限Id 
            foreach (PermissionModuleCode model in permissionModuleCodeList)
            {
                if (model.Code.Equals(PermissionCode, StringComparison.OrdinalIgnoreCase))
                {
                    return (AdminService.GetById(LoginAdmin.Id).PermissionRole.Permissions.ToLower()).Contains(model.Id.ToString().ToLower());
                }
            }
            return false;
        }

        /// <summary>
        /// 是否有权限
        /// </summary>
        /// <param name="moduleCode">模块编码</param>
        /// <param name="operateType">权限编码</param>
        /// <returns></returns>
        protected bool HasPermission(string moduleCode, PermissionOperate operateType)
        {
            return HasPermission(moduleCode, operateType.ToString());
        }

        /// <summary>
        /// 把用户权限赋值到视图数据中
        /// </summary>
        protected void SetPermissionToViewData()
        {
            //获取权限列表
            List<PermissionModuleCode> permissionModuleCodeList = AdminPermissionsCache.Instance.CacheData;
            //判断用户权限列表是否有权限Id 
            foreach (PermissionModuleCode model in permissionModuleCodeList)
            {
                ViewData[model.Code] = (AdminService.GetById(LoginAdmin.Id).PermissionRole.Permissions.ToLower()).Contains(model.Id.ToString().ToLower());
            }
        }
        #endregion

        #region 显示没有权限的提示信息
        /// <summary>
        /// 显示没有权限的提示信息
        /// </summary>
        protected ActionResult ShowNotPermissionTip(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "[对不起，您没有权限进行此操作！]";
            }

            return Content(message);
        }
        #endregion

        #region 添加系统日志
        /// <summary>
        /// 添加系统日志
        /// </summary>
        /// <param name="description"></param>
        protected void AddLog(string description, AdminLoggerModuleEnum adminLoggerModuleEnum)
        {
            AdminLogger adminLogger = new AdminLogger();
            adminLogger.AdminName = LoginAdmin.Name;
            adminLogger.Description = description;
            adminLogger.FromIP = IpHelper.UserHostAddress;
            adminLogger.Module = (int)adminLoggerModuleEnum;
            AdminLoggerService.Insert(adminLogger);
        }
        #endregion

        #region protected override void OnActionExecuting(ActionExecutingContext filterContext)
        /// <summary>
        /// protected override void OnActionExecuting(ActionExecutingContext filterContext)
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string safeIP = ConfigurationCache.Instance.GetValue(ConfigurationEnum.Config_SafeIP);
            bool canVisit = true;

            if (!string.IsNullOrWhiteSpace(safeIP))
            {
                canVisit = false;
                string currentIP = IpHelper.UserHostAddress;
                string[] arrIP = safeIP.Split('|');

                foreach (string ip in arrIP)
                {
                    if (string.Equals(ip, currentIP, StringComparison.OrdinalIgnoreCase))
                    {
                        canVisit = true;
                        break;
                    }
                }
            }

            if (this.LoginAdmin == null || (!canVisit))
                filterContext.Result = new RedirectResult("/Login");
            else
                SetPermissionToViewData();

            base.OnActionExecuting(filterContext);
        }
        #endregion
    }
}