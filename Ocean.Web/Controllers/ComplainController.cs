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
    public class ComplainController : WebBaseController
    {
        IMpUserService _userService;
        IComplainService _complainService;
        public ComplainController(IMpUserService mpUserService,
            IComplainService complainService)
        {
            _userService = mpUserService;
            _complainService = complainService;
        }
        public ActionResult Index()
        {
            //当前登录用户
            try
            {
                ViewBag.Title = "投诉";
                MpUserID = new Guid("E8325C13-9C76-4B70-911E-57395A2F4D69");
                MpUser mpUser = _mpUserService.GetById(MpUserID);
                if (mpUser == null || mpUser.IsAuth != 1)
                {
                    return Json(new { message = "您没有权限，请先申请成为业务员！" });
                }
                IList<Complain> complainList = _complainService.GetList("from Complain where MpUserId = '" + mpUser.Id + "'");

                //ViewBag.VehicleList = new JavaScriptSerializer().Serialize(Json(vehicleList).Data); //Json(vehicleList, JsonRequestBehavior.AllowGet);
                return View(complainList);
            }
            catch (System.Exception ex)
            {
            	//
            }
            return View();
        }


        public ActionResult ViewDetail()
        {
            //当前登录用户

            ViewBag.Title = "投诉"; ;
            MpUserID = new Guid("E8325C13-9C76-4B70-911E-57395A2F4D69");
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            if (mpUser == null || mpUser.IsAuth != 1)
            {
                return Json(new { message = "您没有权限，请先申请成为业务员！" });
            }
            Complain complain = new Complain();

            if (!string.IsNullOrWhiteSpace(RQuery["Id"]))
            {
                complain = _complainService.GetById(new Guid(RQuery["Id"]));
            }
            //VehicleLicense vehicleLicense = _vehicleLicenseService.GetUnique("from VehicleLicense where Id = '" +id +  "'");

            if (complain != null && complain.MpUserId != Guid.Empty && complain.MpUserId != MpUserID)
            {
                return Json(new { message = "您没有权限，请先申请成为业务员！" });
            }
            return View(complain);
        }
        [HttpPost]
        [ActionName("_ComplainList")]
        public ActionResult ComplainListProvide(ComplainDTO complainDTO)
        {
            MpUserID = new Guid("E8325C13-9C76-4B70-911E-57395A2F4D69");
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            if (mpUser == null || mpUser.IsAuth != 1)
            {
                return Json(new { message = "您没有权限，请先申请成为业务员！" });
            }
            complainDTO.MpUserId = MpUserID;
            //IList<VehicleLicense> vehicleList = _vehicleLicenseService.GetList("from VehicleLicense where MpUserId = '" + mpUser.Id + "'");
            OceanDynamicList<object> complainList = _complainService.GetPageDynamicList(PageIndex, PageSize, complainDTO, 0);
            //ViewBag.VehicleList = new JavaScriptSerializer().Serialize(Json(vehicleList).Data); //Json(vehicleList, JsonRequestBehavior.AllowGet);
            //return View(vehicleList);
            if (complainList != null)
            {
                return Content(complainList.ToJson(), "text/javascript");
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
        [ActionName("_ComplainApply")]
        public ActionResult ComplainApplyProvide(string isEdit)
        {
            Complain complain = new Complain();

            if (!string.IsNullOrWhiteSpace(RQuery["Id"]))
            {
                complain = _complainService.GetById(new Guid(RQuery["Id"]));
            }
            MpUserID = new Guid("E8325C13-9C76-4B70-911E-57395A2F4D69");
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            if (mpUser == null || mpUser.IsAuth != 1)
            {
                return Json(new { message = "您没有权限，请先申请成为业务员！" });
            }
            UpdateModel<Complain>(complain);
            complain.MpUserId = MpUserID;
            if (string.IsNullOrWhiteSpace(RQuery["Id"]))
            {

                complain.CreateDate = DateTime.Now;
                _complainService.Insert(complain);
                return JsonMessage(true, "添加投诉成功");
            }
            else
            {
                _complainService.Update(complain);
                return JsonMessage(true, "修改投诉成功");
            }
        }


        [HttpPost]
        public JsonResult UploadPhoto()
        {
            HttpPostedFileBase uploadFiles0 = Request.Files["picUpload"];

            if (uploadFiles0 != null && uploadFiles0.ContentLength > 0 && !string.IsNullOrEmpty(uploadFiles0.FileName))
            {
                UpFileEntity fileEntity0 = new UpFileEntity() { Size = 2048 };
                fileEntity0.Dir = "/VendorImages/";
                fileEntity0.AllowType = ".jpg,.jpeg,.gif,.png,.bmp";

                AttachmentInfo attch0 = UploadProvider.GetInstance("Common").UploadFile(uploadFiles0, fileEntity0);
                if (string.IsNullOrEmpty(attch0.Error))
                {
                    return JsonMessage(true, attch0.FileName);
                }
                else
                {
                    return JsonMessage(false, attch0.Error);
                }
            }
            return JsonMessage(false, "上传不成功...");
        }

    }
}