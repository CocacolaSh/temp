using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Page;
using Ocean.Services;
using Ocean.Entity;
using Ocean.Core;
using Ocean.Entity.DTO;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using Ocean.Core.Logging;
using System.Data;
using Ocean.Core.Npoi;
using Newtonsoft.Json;
using Ocean.Core.Utility;
using System.Threading;
using Ocean.Entity.Enums.AdminLoggerModule;
using Ocean.Framework.Caching.Cache;
using Ocean.Core.Plugins.Upload;

namespace Ocean.Admin.Controllers
{
    public class DrivingLicenseController : AdminBaseController
    {
        private readonly IDrivingLicenseService _drivingLicenseService;
        private readonly IMpUserService _mpUserService;
        public DrivingLicenseController(IDrivingLicenseService drivingLicenseService, IMpUserService mpUserService)
        {
            this._drivingLicenseService = drivingLicenseService;
            this._mpUserService = mpUserService;
        }

        /// <summary>
        /// 初始化架驶证页面
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            if (!base.HasPermission("DrivingLicense", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return View();
        }
        /// <summary>
        /// 初始化页面
        /// </summary>
        [HttpGet]
        public ActionResult DrivingLicense()
        {
            if (!base.HasPermission("DrivingLicense", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        [ActionName("_DrivingLicenseAllList")]
        public ActionResult DrivingLicenseListProvide(int isAll)
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        [ActionName("_DrivingLicenseList")]
        public ActionResult DrivingLicenseListProvide(DrivingLicenseDTO drivingLicenseDTO)
        {
            if (!base.HasPermission("DrivingLicense", PermissionOperate.manager))
            {
                return null;
            }

            int isAll = 1;

            if (!base.HasPermission("DrivingLicenseAll", PermissionOperate.manager))
            {
                drivingLicenseDTO.MpUserId = base.LoginAdmin.MpUserId;
                isAll = 0;
            }

            OceanDynamicList<object> list = _drivingLicenseService.GetPageDynamicList(PageIndex, PageSize, drivingLicenseDTO, isAll);

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
        public ActionResult DrivingLicenseEdit()
        {
            if (!base.HasPermission("DrivingLicense", PermissionOperate.add) && !base.HasPermission("DrivingLicense", PermissionOperate.edit))
            {
                return base.ShowNotPermissionTip("");
            }

            string id = RQuery["Id"];
            DrivingLicense drivingLicense = string.IsNullOrWhiteSpace(id) ? null : _drivingLicenseService.GetById(new Guid(id));
            return AdminView(drivingLicense);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        [HttpPost]
        [ActionName("_DrivingLicenseEdit")]
        public ActionResult DrivingLicenseProvide()
        {
            if (string.IsNullOrWhiteSpace(RQuery["DrivingLicenseId"]))
            {
                if (!base.HasPermission("DrivingLicense", PermissionOperate.add))
                {
                    return JsonMessage(false, "你没有添加驾驶证的权限");
                }
            }
            else
            {
                if (!base.HasPermission("DrivingLicense", PermissionOperate.edit))
                {
                    return JsonMessage(false, "你没有编辑驾驶证的权限");
                }
            }


            DrivingLicense drivingLicense = new DrivingLicense();

            if (!string.IsNullOrWhiteSpace(RQuery["DrivingLicenseId"]))
            {
                drivingLicense = _drivingLicenseService.GetById(new Guid(RQuery["DrivingLicenseId"]));
            }
            try
            {
                UpdateModel<DrivingLicense>(drivingLicense);
            }
            catch (System.Exception ex)
            {
            	//
            }
            if (string.IsNullOrWhiteSpace(RQuery["DrivingLicenseId"]))
            {
                //branch.Status = 1;
                try
                {
                    drivingLicense.CreateDate = DateTime.Now;
                    _drivingLicenseService.Insert(drivingLicense);
                    base.AddLog(string.Format("添加驾驶证[{0}]成功", drivingLicense.Name), AdminLoggerModuleEnum.DrivingLicense);
                }
                catch (System.Exception ex)
                {
                    //
                }
                return JsonMessage(true, "添加驾驶证成功");
            }
            else
            {
                _drivingLicenseService.Update(drivingLicense);
                base.AddLog(string.Format("修改驾驶证[{0}]成功", drivingLicense.Name), AdminLoggerModuleEnum.DrivingLicense);
                return JsonMessage(true, "修改驾驶证成功");
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        [HttpPost]
        [ActionName("_DrivingLicenseDelete")]
        public ActionResult DrivingLicenseProvide(Guid id)
        {
            if (!base.HasPermission("DrivingLicense", PermissionOperate.delete))
            {
                return JsonMessage(false, "你没有删除驾驶证的权限");
            }
            try
            {

                DrivingLicense drivingLicense = _drivingLicenseService.GetById(id);
                _drivingLicenseService.Delete(id.ToString());
                base.AddLog(string.Format("删除驾驶证[{0}]成功", drivingLicense.Name), AdminLoggerModuleEnum.Branch);
            }
            catch (System.Exception ex)
            {

            }
            return JsonMessage(true, "删除成功");
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
        /// 驾照类型
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

        /// <summary>
        /// 获取业务员明细
        /// </summary>
        [HttpPost]
        [ActionName("_GetBussManDetail")]
        public ActionResult GetBussManDetail(Guid id)
        {
            if (!base.HasPermission("DrivingLicense", PermissionOperate.track))
            {
                return null;
            }

            DrivingLicense drivingLicense = _drivingLicenseService.GetById(id);
            if (drivingLicense == null || drivingLicense.MpUserId == Guid.Empty)
            {
                return Json(new { AssignBussinessMan = "", BussinessManPhone = "" });
            }
            MpUser bussMan = _mpUserService.GetById(drivingLicense.MpUserId);
            if (bussMan == null)
            {
                return Json(new { AssignBussinessMan = "", BussinessManPhone = "" });
            }
            return Json(new { AssignBussinessMan = bussMan.Name, BussinessManPhone = bussMan.MobilePhone });
        }

        /// <summary>
        /// 分配业务员
        /// </summary>
        [HttpPost]
        [ActionName("_AllotEdit")]
        public ActionResult AllotEditProvide(Guid id)
        {
            if (!base.HasPermission("DrivingLicense", PermissionOperate.track))
            {
                return JsonMessage(false, "你没有分配业务员的权限");
            }

            DrivingLicense drivingLicense = _drivingLicenseService.GetById(id);

            if (drivingLicense == null)
            {
                return JsonMessage(false, "该笔行驶证已不存在，不能继续操作！");
            }
            string bussMan = RQuery["AssignBussinessMan"].ToString();
            string bussPhone = RQuery["BussinessManPhone"].ToString();
            MpUser bussManUser = _mpUserService.GetUnique("from MpUser where Name = '" + bussMan.Trim() + "' and MobilePhone = '" + bussPhone.Trim() + "'");
            if (bussManUser == null)
            {
                return JsonMessage(false, "业务员信息有错！");
            }
            drivingLicense.MpUserId = bussManUser.Id;
            _drivingLicenseService.Update(drivingLicense);

            base.AddLog(string.Format("分配业务员[{0}]成功", drivingLicense.Name + "To" + bussMan.Trim()), AdminLoggerModuleEnum.DrivingLicense);
            return JsonMessage(true, "处理成功");
        }
        public ActionResult Excute()
        {
            //string sql = @"delete FunongbaoApply where CreateDate>'2014-04-01'";
            //_funongbaoService.ExcuteSql(sql);
            return Content("excute");
        }
    }
}