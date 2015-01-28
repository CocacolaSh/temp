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
    public class DrivingLicenseController : WebBaseController
    {
        IMpUserService _userService;
        IDrivingLicenseService _drivingLicenseService;
        public DrivingLicenseController(IMpUserService mpUserService,
            IDrivingLicenseService drivingLicenseService)
        {
            _userService = mpUserService;
            _drivingLicenseService = drivingLicenseService;
        }
        public ActionResult Index()
        {
            //当前登录用户

            ViewBag.Title = "行驶证";
            MpUserID = new Guid("E8325C13-9C76-4B70-911E-57395A2F4D69");
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            if (mpUser == null || mpUser.IsAuth != 1)
            {
                return Json(new { message = "您没有权限，请先申请成为业务员！" });
            }
            IList<DrivingLicense> drivingList = _drivingLicenseService.GetList("from DrivingLicense where MpUserId = '" + mpUser.Id + "'");

            //ViewBag.VehicleList = new JavaScriptSerializer().Serialize(Json(vehicleList).Data); //Json(vehicleList, JsonRequestBehavior.AllowGet);
            return View(drivingList);
        }


        public ActionResult ViewDetail()
        {
            //当前登录用户

            ViewBag.Title = "驾驶证详情";;
            MpUserID = new Guid("E8325C13-9C76-4B70-911E-57395A2F4D69");
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            if (mpUser == null || mpUser.IsAuth != 1)
            {
                return Json(new { message = "您没有权限，请先申请成为业务员！" });
            }
            DrivingLicense drivingLicense = new DrivingLicense();

            if (!string.IsNullOrWhiteSpace(RQuery["Id"]))
            {
                drivingLicense = _drivingLicenseService.GetById(new Guid(RQuery["Id"]));
            }
            //VehicleLicense vehicleLicense = _vehicleLicenseService.GetUnique("from VehicleLicense where Id = '" +id +  "'");

            if (drivingLicense != null && drivingLicense.MpUserId != Guid.Empty && drivingLicense.MpUserId != MpUserID)
            {
                return Json(new { message = "您没有权限，请先申请成为业务员！" });
            }
            return View(drivingLicense);
        }
        [HttpPost]
        [ActionName("_DrivingLicenseList")]
        public ActionResult DrivingLicenseListProvide(DrivingLicenseDTO drivingLicenseDTO)
        {
            MpUserID = new Guid("E8325C13-9C76-4B70-911E-57395A2F4D69");
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            if (mpUser == null || mpUser.IsAuth != 1)
            {
                return Json(new { message = "您没有权限，请先申请成为业务员！" });
            }
            drivingLicenseDTO.MpUserId = MpUserID;
            //IList<VehicleLicense> vehicleList = _vehicleLicenseService.GetList("from VehicleLicense where MpUserId = '" + mpUser.Id + "'");
            OceanDynamicList<object> drivingList = _drivingLicenseService.GetPageDynamicList(PageIndex, PageSize, drivingLicenseDTO, 0);
            //ViewBag.VehicleList = new JavaScriptSerializer().Serialize(Json(vehicleList).Data); //Json(vehicleList, JsonRequestBehavior.AllowGet);
            //return View(vehicleList);
            if (drivingList != null)
            {
                return Content(drivingList.ToJson(), "text/javascript");
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
        [ActionName("_DrivingLicenseApply")]
        public ActionResult DrivingLicenseApplyProvide(string isEdit)
        {
            DrivingLicense drivingLicense = new DrivingLicense();

            if (!string.IsNullOrWhiteSpace(RQuery["Id"]))
            {
                drivingLicense = _drivingLicenseService.GetById(new Guid(RQuery["Id"]));
            }

            UpdateModel<DrivingLicense>(drivingLicense);
            if (string.IsNullOrWhiteSpace(RQuery["Id"]))
            {

                drivingLicense.CreateDate = DateTime.Now;
                _drivingLicenseService.Insert(drivingLicense);
                return JsonMessage(true, "添加驾驶证成功");
            }
            else
            {
                _drivingLicenseService.Update(drivingLicense);
                return JsonMessage(true, "修改驾驶证成功");
            }
        }

        /// <summary>
        /// 车辆类型
        /// </summary>
        [HttpPost]
        [ActionName("_SexType")]
        public ActionResult SexTypeProvide()
        {
            string strType = "[";
            strType += "{\"id\":\"\",\"text\":\"--请选择--\"},";
            try
            {
                foreach (var item in EnumDataCache.Instance.GetList(Ocean.Entity.Enums.TypeIdentifying.TypeIdentifyingEnum.SexType))
                {
                    strType += "{\"id\":\"" + item.Value.ToString() + "\",\"text\":\"" + item.Name + "\"},";
                }
            }
            catch (System.Exception ex)
            {
                //
            }
            strType = strType.Substring(0, strType.Length - 1);
            strType += "]";
            return Content(strType);
        }
        /// <summary>
        /// 用途类型
        /// </summary>
        [HttpPost]
        [ActionName("_ClassType")]
        public ActionResult ClassTypeProvide()
        {
            string strType = "[";
            strType += "{\"id\":\"\",\"text\":\"--请选择--\"},";
            try
            {
                foreach (var item in EnumDataCache.Instance.GetList(Ocean.Entity.Enums.TypeIdentifying.TypeIdentifyingEnum.ClassType))
                {
                    strType += "{\"id\":\"" + item.Value.ToString() + "\",\"text\":\"" + item.Name + "\"},";
                }
            }
            catch (System.Exception ex)
            {
                //
            }
            strType = strType.Substring(0, strType.Length - 1);
            strType += "]";
            return Content(strType);
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