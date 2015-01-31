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
    public class BaoXianController : AdminBaseController
    {
        private readonly IMpUserService _userService;
        private readonly IBaoXianService _baodanService;
        public BaoXianController(IMpUserService userService, IBaoXianService baodanService)
        {
            this._userService = userService;
            this._baodanService = baodanService;
        }

        /// <summary>
        /// 初始化列表页面
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            if (!base.HasPermission("BaoXian", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return View();
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_BaoXianList")]
        public ActionResult BaoXianListProvide(BaoXianDTO baoDanDTO)
        {
            if (!base.HasPermission("BaoXian", PermissionOperate.manager))
            {
                return null;
            }

            int isAll = 1;

            if (!base.HasPermission("BaoXianAll", PermissionOperate.manager))
            {
                baoDanDTO.MpUserId = base.LoginAdmin.MpUserId.HasValue ? base.LoginAdmin.MpUserId : Guid.Empty;
                isAll = 0;
            }
            OceanDynamicList<object> list = _baodanService.GetPageDynamicList(PageIndex, PageSize, baoDanDTO, isAll);

            if (list != null)
            {
                return Content(list.ToJson(), "text/javascript");
            }
            else
            {
                return Content("{\"rows\":[],\"total\":0}", "text/javascript");
            }
        }

        /// <summary>
        /// 初始化Edit页面
        /// </summary>
        [HttpGet]
        public ActionResult BaoXianEdit()
        {
            if (!base.HasPermission("BaoXian", PermissionOperate.add) && !base.HasPermission("BaoXian", PermissionOperate.edit))
            {
                return base.ShowNotPermissionTip("");
            }

            string id = RQuery["Id"];
            BaoXian baoDan = string.IsNullOrWhiteSpace(id) ? null : _baodanService.GetById(new Guid(id));
            return AdminView(baoDan);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        [HttpPost]
        [ActionName("_BaoXianEdit")]
        public ActionResult BaoXianProvide()
        {
            if (string.IsNullOrWhiteSpace(RQuery["BaoXianId"]))
            {
                if (!base.HasPermission("BaoXian", PermissionOperate.add))
                {
                    return JsonMessage(false, "你没有添加保单的权限");
                }
            }
            else
            {
                if (!base.HasPermission("BaoXian", PermissionOperate.edit))
                {
                    return JsonMessage(false, "你没有编辑保单的权限");
                }
            }


            BaoXian baoDan = new BaoXian();

            if (!string.IsNullOrWhiteSpace(RQuery["BaoXianId"]))
            {
                baoDan = _baodanService.GetById(new Guid(RQuery["BaoXianId"]));
            }

            UpdateModel<BaoXian>(baoDan);
            if (string.IsNullOrWhiteSpace(RQuery["BaoXianId"]))
            {
                //branch.Status = 1;
                try
                {
                    baoDan.CreateDate = DateTime.Now;
                    _baodanService.Insert(baoDan);
                }
                catch (System.Exception ex)
                {
                    //
                }
                base.AddLog(string.Format("添加保单[{0}]成功", baoDan.TouBaoRen), AdminLoggerModuleEnum.BaoXian);
                return JsonMessage(true, "添加保单成功");
            }
            else
            {
                _baodanService.Update(baoDan);
                base.AddLog(string.Format("修改保单[{0}]成功", baoDan.TouBaoRen), AdminLoggerModuleEnum.BaoXian);
                return JsonMessage(true, "修改保单成功");
            }
        }
        /// <summary>
        /// 车辆类型
        /// </summary>
        [HttpPost]
        [ActionName("_XianZhongType")]
        public ActionResult XianZhongTypeProvide()
        {
            string strType = "[";
            strType += "{\"id\":\"\",\"text\":\"--请选择--\"},";
            foreach (var item in EnumDataCache.Instance.GetList(Ocean.Entity.Enums.TypeIdentifying.TypeIdentifyingEnum.XianZhongType))
            {
                strType += "{\"id\":\"" + item.Value.ToString() + "\",\"text\":\"" + item.Name + "\"},";
            }
            strType = strType.Substring(0, strType.Length - 1);
            strType += "]";
            return Content(strType);
        }
    }
}