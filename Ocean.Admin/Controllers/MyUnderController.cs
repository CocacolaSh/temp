using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Page;
using Ocean.Core;
using Ocean.Entity.Enums;
using Ocean.Entity.DTO;
using Ocean.Framework.Caching.Cache;
using Ocean.Core.Utility;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.HSSF.UserModel;
using Newtonsoft.Json;
using Ocean.Entity.Enums.Loan;
using Ocean.Entity.Enums.AdminLoggerModule;
using Ocean.Entity;
using Ocean.Entity.Enums.TypeIdentifying;
using Ocean.Services;

namespace Ocean.Admin.Controllers
{
    public class MyUnderController : AdminBaseController
    {
        private readonly IMpUserService _userService;
        public MyUnderController(IMpUserService userService)
        {
            this._userService = userService;
        }

        /// <summary>
        /// 初始化列表页面
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            if (!base.HasPermission("MyUnder", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return View();
        }

        ///// <summary>
        ///// 初始化详情页面
        ///// </summary>
        //[HttpGet]
        //public ActionResult MyUnderView(Guid id)
        //{
        //    if (!base.HasPermission("MyUnder", PermissionOperate.view))
        //    {
        //        return base.ShowNotPermissionTip("");
        //    }

        //    Complain complain = _complainService.GetById(id);
        //    return View(complain);
        //}
        /// <summary>
        /// 获取列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_MyUnderList")]
        public ActionResult MyUnderListProvide(MyUnderDTO myUnderDTO)
        {
            if (!base.HasPermission("MyUnder", PermissionOperate.manager))
            {
                return null;
            }

            int isAll = 1;

            if (!base.HasPermission("MyUnderAll", PermissionOperate.manager))
            {
                myUnderDTO.MpUserId = base.LoginAdmin.MpUserId;
                isAll = 0;
            }
            PagedList<MpUser> list = _userService.GetUnderUsers(PageIndex, PageSize, myUnderDTO, 0);

            return JsonList<MpUser>(list, list.TotalItemCount);
        }
        [HttpPost]
        [ActionName("_AuthPass")]
        public ActionResult AuthPassProvide(Guid id, int isAuth)
        {
            if (!base.HasPermission("MyUnder", PermissionOperate.visit))
            {
                return null;
            }
            MpUser user = _userService.GetById(id);
            user.IsAuth = isAuth;
            _userService.Update(user);

            base.AddLog(string.Format("修改认证[{0}]成功", user.Name + " to " + isAuth.ToString()), AdminLoggerModuleEnum.MyUnder);
            return JsonMessage(true, "修改成功");
        }

        ///// <summary>
        ///// 受理业务
        ///// </summary>
        //[HttpPost]
        //[ActionName("_AcceptEdit")]
        //public ActionResult AcceptEditProvide(Guid id)
        //{
        //    if (!base.HasPermission("Complain", PermissionOperate.track))
        //    {
        //        return JsonMessage(false, "你没有处理情况登记的权限");
        //    }

        //    Complain complain = _complainService.GetById(id);

        //    if (complain.ProcessStatus != 0)
        //    {
        //        return JsonMessage(false, "该投诉已被受理，不能继续操作！");
        //    }

        //    UpdateModel<Complain>(complain);
        //    _complainService.Update(complain);
        //    base.AddLog(string.Format("受理投诉业务[{0}]", complain.Name), AdminLoggerModuleEnum.Complain);
        //    return JsonMessage(true, "处理成功");
        //}
    }
}