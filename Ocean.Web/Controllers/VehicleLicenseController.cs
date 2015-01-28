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
    public class VehicleLicenseController : WebBaseController
    {
        IMpUserService _userService;
        IVehicleLicenseService _vehicleLicenseService;
        public VehicleLicenseController(IMpUserService mpUserService,
            IVehicleLicenseService vehicleLicenseService)
        {
            _userService = mpUserService;
            _vehicleLicenseService = vehicleLicenseService;
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
            IList<VehicleLicense> vehicleList = _vehicleLicenseService.GetList("from VehicleLicense where MpUserId = '" + mpUser.Id + "'");

            //ViewBag.VehicleList = new JavaScriptSerializer().Serialize(Json(vehicleList).Data); //Json(vehicleList, JsonRequestBehavior.AllowGet);
            return View(vehicleList);
        }


        public ActionResult ViewDetail()
        {
            //当前登录用户

            ViewBag.Title = "行驶证详情";;
            MpUserID = new Guid("E8325C13-9C76-4B70-911E-57395A2F4D69");
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            if (mpUser == null || mpUser.IsAuth != 1)
            {
                return Json(new { message = "您没有权限，请先申请成为业务员！" });
            }
            VehicleLicense vehicleLicense = new VehicleLicense();

            if (!string.IsNullOrWhiteSpace(RQuery["Id"]))
            {
                vehicleLicense = _vehicleLicenseService.GetById(new Guid(RQuery["Id"]));
            }
            //VehicleLicense vehicleLicense = _vehicleLicenseService.GetUnique("from VehicleLicense where Id = '" +id +  "'");

            if (vehicleLicense != null && vehicleLicense.MpUserId != Guid.Empty && vehicleLicense.MpUserId != MpUserID)
            {
                return Json(new { message = "您没有权限，请先申请成为业务员！" });
            }
            return View(vehicleLicense);
        }
        [HttpPost]
        [ActionName("_VehicleLicenseList")]
        public ActionResult VehicleLicenseListProvide(VehicleLicenseDTO vehicleLicenseDTO)
        {
            MpUserID = new Guid("E8325C13-9C76-4B70-911E-57395A2F4D69");
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            if (mpUser == null || mpUser.IsAuth != 1)
            {
                return Json(new { message = "您没有权限，请先申请成为业务员！" });
            }
            vehicleLicenseDTO.MpUserId = MpUserID;
            //IList<VehicleLicense> vehicleList = _vehicleLicenseService.GetList("from VehicleLicense where MpUserId = '" + mpUser.Id + "'");
            OceanDynamicList<object> vehicleList = _vehicleLicenseService.GetPageDynamicList(PageIndex, PageSize, vehicleLicenseDTO, 0);
            //ViewBag.VehicleList = new JavaScriptSerializer().Serialize(Json(vehicleList).Data); //Json(vehicleList, JsonRequestBehavior.AllowGet);
            //return View(vehicleList);
            if (vehicleList != null)
            {
                return Content(vehicleList.ToJson(), "text/javascript");
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
        [ActionName("_VehicleLicenseApply")]
        public ActionResult VehicleLicenseApplyProvide(string isEdit)
        {
            VehicleLicense vehicleLicense = new VehicleLicense();

            if (!string.IsNullOrWhiteSpace(RQuery["Id"]))
            {
                vehicleLicense = _vehicleLicenseService.GetById(new Guid(RQuery["Id"]));
            }

            UpdateModel<VehicleLicense>(vehicleLicense);
            if (string.IsNullOrWhiteSpace(RQuery["Id"]))
            {

                vehicleLicense.CreateDate = DateTime.Now;
                _vehicleLicenseService.Insert(vehicleLicense);
                return JsonMessage(true, "添加驾驶证成功");
            }
            else
            {
                _vehicleLicenseService.Update(vehicleLicense);
                return JsonMessage(true, "修改驾驶证成功");
            }
        }

        /// <summary>
        /// 车辆类型
        /// </summary>
        [HttpPost]
        [ActionName("_VehicleType")]
        public ActionResult VehicleTypeProvide()
        {
            string strType = "[";
            strType += "{\"id\":\"\",\"text\":\"--请选择--\"},";
            foreach (var item in EnumDataCache.Instance.GetList(Ocean.Entity.Enums.TypeIdentifying.TypeIdentifyingEnum.VehicleType))
            {
                strType += "{\"id\":\"" + item.Value.ToString() + "\",\"text\":\"" + item.Name + "\"},";
            }
            strType = strType.Substring(0, strType.Length - 1);
            strType += "]";
            return Content(strType);
        }
        /// <summary>
        /// 用途类型
        /// </summary>
        [HttpPost]
        [ActionName("_UseCharacterType")]
        public ActionResult UseCharacterTypeProvide()
        {
            string strType = "[";
            strType += "{\"id\":\"\",\"text\":\"--请选择--\"},";
            foreach (var item in EnumDataCache.Instance.GetList(Ocean.Entity.Enums.TypeIdentifying.TypeIdentifyingEnum.UseCharacterType))
            {
                strType += "{\"id\":\"" + item.Value.ToString() + "\",\"text\":\"" + item.Name + "\"},";
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