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
    public class VehicleLicenseController : AdminBaseController
    {
        private readonly IVehicleLicenseService _vehicleLicenseService;
        private readonly IMpUserService _mpUserService;
        public VehicleLicenseController(IVehicleLicenseService vehicleLicenseService, IMpUserService mpUserService)
        {
            this._vehicleLicenseService = vehicleLicenseService;
            this._mpUserService = mpUserService;
        }

        /// <summary>
        /// 初始化行驶证页面
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            if (!base.HasPermission("VehicleLicense", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            IList<SelectListItem> importQuarterList = new List<SelectListItem>();
            int startQuarter = 1;
            int endQuarter = 4;
            DateRule dateR=DateRuleList.DateRules.Where(d=>d.Months.Where(m=>m==DateTime.Now.Month).Count()>0).First();
            int x = 0;
            for (int i =  (DateTime.Now.Year - DateRule.StartYear); i >=0; i--)
            {
                
                if ((i+DateRule.StartYear) ==DateRule.StartYear)
                {
                    startQuarter = DateRule.StartQuarter;
                }
                else
                {
                    startQuarter = 1;
                }
                if (i == (DateTime.Now.Year - DateRule.StartYear))
                {
                    endQuarter = dateR.Quarterly;
                }
                else
                {
                    endQuarter = 4;
                }
                for (int j = endQuarter; j >= startQuarter; j--)
                {
                    if (x == 0)
                    {
                        importQuarterList.Add(new SelectListItem() { Text = (i + DateRule.StartYear).ToString() + "年第" + StringHelper.GetQuarter(j) + "季度", Value = StringHelper.GetQuarter(DateRule.StartYear + i, j), Selected = true });
                        x++;
                    }
                    else
                    {
                        importQuarterList.Add(new SelectListItem() { Text = (i + DateRule.StartYear).ToString() + "年第" + StringHelper.GetQuarter(j) + "季度", Value = StringHelper.GetQuarter(DateRule.StartYear + i, j) });
                    }
                }
            }
            ViewBag.ImportQuarterList = importQuarterList;
            return View();
        }
        /// <summary>
        /// 初始化福农宝列表页面
        /// </summary>
        [HttpGet]
        public ActionResult VehicleLicense()
        {
            if (!base.HasPermission("vehiclelicense", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return View();
        }
        /// <summary>
        /// 获取福农宝列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_VehicleLicenseAllList")]
        public ActionResult VehicleLicenseListProvide(int isAll)
        {
            return View();
        }
        /// <summary>
        /// 获取列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_VehicleLicenseList")]
        public ActionResult VehicleLicenseListProvide(VehicleLicenseDTO vehicleLicenseDTO)
        {
            if (!base.HasPermission("VehicleLicense", PermissionOperate.manager))
            {
                return null;
            }

            int isAll = 1;
            
            if (!base.HasPermission("VehicleLicenseAll", PermissionOperate.manager))
            {
                vehicleLicenseDTO.MpUserId = base.LoginAdmin.MpUserId;
                isAll = 0;
            }

            OceanDynamicList<object> list = _vehicleLicenseService.GetPageDynamicList(PageIndex, PageSize, vehicleLicenseDTO, isAll);

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
        public ActionResult VehicleLicenseEdit()
        {
            if (!base.HasPermission("VehicleLicense", PermissionOperate.add) && !base.HasPermission("VehicleLicense", PermissionOperate.edit))
            {
                return base.ShowNotPermissionTip("");
            }

            string id = RQuery["Id"];
            VehicleLicense vehicleLicense = string.IsNullOrWhiteSpace(id) ? null : _vehicleLicenseService.GetById(new Guid(id));
            return AdminView(vehicleLicense);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        [HttpPost]
        [ActionName("_VehicleLicenseEdit")]
        public ActionResult VehicleLicenseProvide()
        {
            if (string.IsNullOrWhiteSpace(RQuery["VehicleLicenseId"]))
            {
                if (!base.HasPermission("VehicleLicense", PermissionOperate.add))
                {
                    return JsonMessage(false, "你没有添加驾驶证的权限");
                }
            }
            else
            {
                if (!base.HasPermission("VehicleLicense", PermissionOperate.edit))
                {
                    return JsonMessage(false, "你没有编辑驾驶证的权限");
                }
            }


            VehicleLicense vehicleLicense = new VehicleLicense();

            if (!string.IsNullOrWhiteSpace(RQuery["VehicleLicenseId"]))
            {
                vehicleLicense = _vehicleLicenseService.GetById(new Guid(RQuery["VehicleLicenseId"]));
            }

            UpdateModel<VehicleLicense>(vehicleLicense);
            if (string.IsNullOrWhiteSpace(RQuery["VehicleLicenseId"]))
            {
                //branch.Status = 1;
                try
                {
                    vehicleLicense.CreateDate = DateTime.Now;
                    _vehicleLicenseService.Insert(vehicleLicense);
                }
                catch (System.Exception ex)
                {
                	//
                }
                base.AddLog(string.Format("添加驾驶证[{0}]成功", vehicleLicense.Owner), AdminLoggerModuleEnum.VehicleLicense);
                return JsonMessage(true, "添加驾驶证成功");
            }
            else
            {
                _vehicleLicenseService.Update(vehicleLicense);
                base.AddLog(string.Format("修改驾驶证[{0}]成功", vehicleLicense.Owner), AdminLoggerModuleEnum.VehicleLicense);
                return JsonMessage(true, "修改驾驶证成功");
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        [HttpPost]
        [ActionName("_VehicleLicenseDelete")]
        public ActionResult VehicleLicenseProvide(Guid id)
        {
            if (!base.HasPermission("VehicleLicense", PermissionOperate.delete))
            {
                return JsonMessage(false, "你没有删除驾驶证的权限");
            }
            try
            {

                VehicleLicense vehicleLicense = _vehicleLicenseService.GetById(id);
                _vehicleLicenseService.Delete(id.ToString());
                base.AddLog(string.Format("删除驾驶证[{0}]成功", vehicleLicense.Owner), AdminLoggerModuleEnum.Branch);
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

        /// <summary>
        /// 获取业务员明细
        /// </summary>
        [HttpPost]
        [ActionName("_GetBussManDetail")]
        public ActionResult GetBussManDetail(Guid id)
        {
            if (!base.HasPermission("VehicleLicense", PermissionOperate.track))
            {
                return null;
            }

            VehicleLicense vehicleLicense = _vehicleLicenseService.GetById(id);
            if (vehicleLicense == null || vehicleLicense.MpUserId == Guid.Empty)
            {
                return Json(new { AssignBussinessMan = "", BussinessManPhone = "" });
            }
            MpUser bussMan = _mpUserService.GetById(vehicleLicense.MpUserId);
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
            if (!base.HasPermission("VehicleLicense", PermissionOperate.track))
            {
                return JsonMessage(false, "你没有分配业务员的权限");
            }

            VehicleLicense vehicleLicense = _vehicleLicenseService.GetById(id);

            if (vehicleLicense == null)
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
            vehicleLicense.MpUserId = bussManUser.Id;
            _vehicleLicenseService.Update(vehicleLicense);

            base.AddLog(string.Format("分配业务员[{0}]成功", vehicleLicense.Owner + "To" + bussMan.Trim()), AdminLoggerModuleEnum.VehicleLicense);
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