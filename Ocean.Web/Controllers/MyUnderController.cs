//#define userlogin
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Services;
using Ocean.Entity;
using Autofac;
using Ocean.Core.Infrastructure.DependencyManagement;
using Ocean.Core;
using Ocean.Entity.Enums;
using Ocean.Core.Plugins.Upload;
using System.IO;
using Ocean.Page;
using Ocean.Core.Utility;
using Ocean.Core.Logging;
using Ocean.Framework.Mvc.Extensions;
using System.Xml;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Ocean.Entity.DTO;
using Ocean.Framework.Caching.Cache;

namespace Ocean.Web.Controllers
{
    public class MyUnderController : WebBaseController
    {
        IMpUserService _userService;
        public MyUnderController(IMpUserService mpUserService,
            IVehicleLicenseService vehicleLicenseService)
        {
            _userService = mpUserService;
        }
        public ActionResult Index()
        {
            //当前登录用户

            ViewBag.Title = "我的下线";
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            if (mpUser == null || mpUser.IsAuth != 1)
            {
                return Json(new { message = "您没有权限，请先申请成为业务员！" });
            }
            IList<MpUser> myUnderList = _mpUserService.GetList("from MpUser where ParentPhone = (select MobilePhone from MpUser where Id = '" + mpUser.Id + "')");

            //ViewBag.VehicleList = new JavaScriptSerializer().Serialize(Json(vehicleList).Data); //Json(vehicleList, JsonRequestBehavior.AllowGet);
            return View(myUnderList);
        }


        public ActionResult ViewDetail()
        {
            //当前登录用户

            ViewBag.Title = "下线详情"; ;
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            if (mpUser == null || mpUser.IsAuth != 1)
            {
                return Json(new { message = "您没有权限，请先申请成为业务员！" });
            }
            MpUser mpUserUnder = new MpUser();

            if (!string.IsNullOrWhiteSpace(RQuery["Id"]))
            {
                mpUserUnder = _userService.GetById(new Guid(RQuery["Id"]));
            }
            //VehicleLicense vehicleLicense = _vehicleLicenseService.GetUnique("from VehicleLicense where Id = '" +id +  "'");

            if (mpUserUnder != null && mpUserUnder.ParentPhone != mpUser.MobilePhone)
            {
                return Json(new { message = "您没有权限，请先申请成为业务员！" });
            }
            return View(mpUserUnder);
        }
        [HttpPost]
        [ActionName("_MyUnderList")]
        public ActionResult MyUnderListProvide(MyUnderDTO myUnderDTO)
        {
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            if (mpUser == null || mpUser.IsAuth != 1)
            {
                return Json(new { message = "您没有权限，请先申请成为业务员！" });
            }
            myUnderDTO.MpUserId = MpUserID;
            //IList<VehicleLicense> vehicleList = _vehicleLicenseService.GetList("from VehicleLicense where MpUserId = '" + mpUser.Id + "'");
            PagedList<MpUser> myUnderList = _mpUserService.GetUnderUsers(PageIndex, PageSize, myUnderDTO,0);
            //ViewBag.VehicleList = new JavaScriptSerializer().Serialize(Json(vehicleList).Data); //Json(vehicleList, JsonRequestBehavior.AllowGet);
            //return View(vehicleList);
            if (myUnderList != null)
            {
                return JsonList<MpUser>(myUnderList, myUnderList.TotalItemCount);
            }
            else
            {
                return Content("{\"rows\":[],\"total\":0}", "text/javascript");
            }
        }

        /// <summary>
        /// 提交
        /// </summary>
        [HttpPost]
        [ActionName("_AuthApply")]
        public ActionResult AuthApplyProvide(Guid id, int IsAuth)
        {
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            if (mpUser == null || mpUser.IsAuth != 1)
            {
                return Json(new { success = false, message = "您没有权限，请先申请成为业务员！" });
            }
            MpUser myUnder = _userService.GetById(id);
            if (myUnder == null)
            {
                return Json(new { success = false, message = "该下线不存在！" });
            }
            if (myUnder.ParentPhone != mpUser.MobilePhone)
            {
                return Json(new {success = false, message = "该下线不是您的下线！" });
            }
            myUnder.IsAuth = IsAuth;
            _mpUserService.Update(myUnder);

            return JsonMessage(true, "修改成功");
        }

    }
}