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
    public class ComplainController : AdminBaseController
    {
        private readonly IComplainService _complainService;

        public ComplainController(IComplainService complainService)
        {
            this._complainService = complainService;
        }

        /// <summary>
        /// 初始化列表页面
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            if (!base.HasPermission("Complain", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return View();
        }

        /// <summary>
        /// 初始化详情页面
        /// </summary>
        [HttpGet]
        public ActionResult ComplainView(Guid id)
        {
            if (!base.HasPermission("Complain", PermissionOperate.view))
            {
                return base.ShowNotPermissionTip("");
            }

            return View();
        }
        /// <summary>
        /// 获取列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_ComplainList")]
        public ActionResult ComplainListProvide(ComplainDTO complainDTO)
        {
            if (!base.HasPermission("Complain", PermissionOperate.manager))
            {
                return null;
            }

            OceanDynamicList<object> list = _complainService.GetPageDynamicList(PageIndex, PageSize, complainDTO);

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
        /// 获取明细
        /// </summary>
        [HttpPost]
        [ActionName("_GetComplainDetail")]
        public ActionResult GetComplainDetail(Guid id)
        {
            if (!base.HasPermission("Complain", PermissionOperate.track))
            {
                return null;
            }

            Complain complain = _complainService.GetById(id);
            return Content(JsonConvert.SerializeObject(complain));
        }


    }
}