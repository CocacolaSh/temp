using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Page;
using Ocean.Core;
using Ocean.Entity.Enums;
using Ocean.Entity.DTO;
using Ocean.Core.Common;
using Ocean.Core.Utility;
using Ocean.Framework.Caching.Cache;
using Ocean.Entity.Enums.AdminLoggerModule;
using Ocean.Entity;
using Ocean.Services;

namespace Ocean.Admin.Controllers
{
    public class EnumController : AdminBaseController
    {
        private readonly IEnumTypeService _enumTypeService;
        private readonly IEnumDataService _enumDataService;

        public EnumController(IEnumTypeService enumTypeService, IEnumDataService enumDataService)
        {
            this._enumTypeService = enumTypeService;
            this._enumDataService = enumDataService;
        }

        /// <summary>
        /// 更新枚举缓存
        /// </summary>
        [HttpPost]
        [ActionName("_UpdateCache")]
        public ActionResult UpdateCache()
        {
            if (!base.HasPermission("enumtype", PermissionOperate.cache) && !base.HasPermission("enumdata", PermissionOperate.cache))
            {
                return JsonMessage(false, "你没有更新缓存的权限");
            }

            EnumDataCache.Instance.RemoveCache();
            base.AddLog(string.Format("更新枚举缓存成功"), AdminLoggerModuleEnum.Enum);
            return JsonMessage(true, "更新枚举缓存成功");
        }

        /// <summary>
        /// 初始化EnumTypeList页面
        /// </summary>
        [HttpGet]
        public ActionResult EnumTypeList()
        {
            if (!base.HasPermission("enumtype", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return AdminView();
        }

        /// <summary>
        /// 获取枚举类型列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_EnumTypeList")]
        public ActionResult EnumTypeListProvide()
        {
            if (!base.HasPermission("enumtype", PermissionOperate.manager))
            {
                return null;
            }

            PagedList<EnumType> list = _enumTypeService.GetPageList("Sort", PageIndex, PageSize, true);
            return SwitchJsonList<EnumType, EnumTypeDTO>(list, list.TotalItemCount);
        }

        /// <summary>
        /// 获取枚举类型树
        /// </summary>
        [HttpPost]
        [ActionName("_EnumTypeTree")]
        public ActionResult EnumTypeTreeProvide()
        {
            IQueryable<EnumType> query = _enumTypeService.Table.OrderBy(e => e.Sort);
            IList<EnumType> listEnumType = query.ToList();
            IList<JsonObject> listJsonObject = new List<JsonObject>();

            foreach (EnumType item in listEnumType)
            {
                JsonObject jsonObject = new JsonObject();
                jsonObject["id"] = new JsonProperty(item.Id.ToString());
                jsonObject["text"] = new JsonProperty(item.Name);
                listJsonObject.Add(jsonObject);
            }

            return Content(JsonHelper.AnalysisJsons(listJsonObject, true));
        }

        /// <summary>
        /// 初始化EnumTypeEdit页面
        /// </summary>
        [HttpGet]
        public ActionResult EnumTypeEdit()
        {
            if (!base.HasPermission("enumtype", PermissionOperate.add) && !base.HasPermission("enumtype", PermissionOperate.edit))
            {
                return base.ShowNotPermissionTip("");
            }

            string id = RQuery["Id"];
            EnumType enumType = string.IsNullOrWhiteSpace(id) ? null : _enumTypeService.GetById(new Guid(id));
            return AdminView(enumType);
        }

        /// <summary>
        /// 编辑枚举类型
        /// </summary>
        [HttpPost]
        [ActionName("_EnumTypeEdit")]
        public ActionResult EnumTypeEditProvide()
        {
            if (string.IsNullOrWhiteSpace(RQuery["EnumTypeId"]))
            {
                if (!base.HasPermission("enumtype", PermissionOperate.add))
                {
                    return JsonMessage(false, "你没有添加枚举类型的权限");
                }
            }
            else
            {
                if (!base.HasPermission("enumtype", PermissionOperate.edit))
                {
                    return JsonMessage(false, "你没有编辑枚举类型的权限");
                }
            }

            EnumType enumType = new EnumType();

            if (!string.IsNullOrWhiteSpace(RQuery["EnumTypeId"]))
            {
                enumType = _enumTypeService.GetById(new Guid(RQuery["EnumTypeId"]));
            }

            UpdateModel<EnumType>(enumType);
            //去除不必要的验证
            //this.ModelState.Remove("Remark");

            if (this.ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(RQuery["EnumTypeId"]))
                {
                    _enumTypeService.Insert(enumType);
                    base.AddLog(string.Format("添加枚举类型[{0}]成功", enumType.Name), AdminLoggerModuleEnum.Enum);
                    return JsonMessage(true, "添加枚举类型成功");
                }
                else
                {
                    _enumTypeService.Update(enumType);
                    base.AddLog(string.Format("修改枚举类型[{0}]成功", enumType.Name), AdminLoggerModuleEnum.Enum);
                    return JsonMessage(true, "修改枚举类型成功");
                }
            }
            else
            {
                return JsonMessage(false, "操作失败[输入有误]");
            }
        }

        /// <summary>
        /// 删除枚举类型
        /// </summary>
        [HttpPost]
        [ActionName("_EnumTypeDelete")]
        public ActionResult AdminDeleteProvide(Guid id)
        {
            if (!base.HasPermission("enumtype", PermissionOperate.delete))
            {
                return JsonMessage(false, "你没有删除枚举类型的权限");
            }

            EnumType enumType = _enumTypeService.GetById(id);
            _enumTypeService.Delete(id.ToString());
            base.AddLog(string.Format("删除枚举类型[{0}]成功", enumType.Name), AdminLoggerModuleEnum.Enum);
            return JsonMessage(true, "删除成功");
        }

        /// <summary>
        /// 初始化EnumDataList页面
        /// </summary>
        [HttpGet]
        public ActionResult EnumDataList()
        {
            if (!base.HasPermission("enumdata", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return AdminView();
        }

        /// <summary>
        /// 获取枚举值数据
        /// </summary>
        [HttpPost]
        [ActionName("_EnumDataList")]
        public ActionResult EnumDataListProvide()
        {
            if (!base.HasPermission("enumdata", PermissionOperate.manager))
            {
                return null;
            }

            string enumTypeId = RQuery["EnumTypeId"];
            PagedList<EnumData> list = null;

            if (string.IsNullOrWhiteSpace(enumTypeId))
                list = _enumDataService.GetPageList("Sort", PageIndex, PageSize, true);
            else
                list = _enumDataService.GetPageListByEnumTypeId(new Guid(enumTypeId), PageIndex, PageSize);

            return SwitchJsonList<EnumData, EnumDataDTO>(list, list.TotalItemCount);
        }

        /// <summary>
        /// 初始化EnumDataEdit页面
        /// </summary>
        [HttpGet]
        public ActionResult EnumDataEdit()
        {
            if (!base.HasPermission("enumdata", PermissionOperate.add) && !base.HasPermission("enumdata", PermissionOperate.edit))
            {
                return base.ShowNotPermissionTip("");
            }

            if (!string.IsNullOrWhiteSpace(RQuery["EnumTypeId"]))
            {
                EnumType enumType = _enumTypeService.GetById(new Guid(RQuery["EnumTypeId"]));
                ViewBag.EnumType = enumType;
            }

            string id = RQuery["Id"];
            EnumData enumData = string.IsNullOrWhiteSpace(id) ? null : _enumDataService.GetById(new Guid(id));

            if (enumData != null)
            {
                ViewBag.EnumType = enumData.EnumType;
            }

            return AdminView(enumData);
        }

        /// <summary>
        /// 编辑枚举值
        /// </summary>
        [HttpPost]
        [ActionName("_EnumDataEdit")]
        public ActionResult EnumDataEditProvide()
        {
            if (string.IsNullOrWhiteSpace(RQuery["EnumDataId"]))
            {
                if (!base.HasPermission("enumdata", PermissionOperate.add))
                {
                    return JsonMessage(false, "你没有添加枚举值的权限");
                }
            }
            else
            {
                if (!base.HasPermission("enumdata", PermissionOperate.edit))
                {
                    return JsonMessage(false, "你没有编辑枚举值的权限");
                }
            }

            string enumDataId = RQuery["EnumDataId"];
            EnumData enumData = new EnumData();

            if (!string.IsNullOrWhiteSpace(enumDataId))
            {
                enumData = _enumDataService.GetById(new Guid(enumDataId));
            }

            UpdateModel<EnumData>(enumData);

            if (string.IsNullOrWhiteSpace(enumDataId))
            {
                _enumDataService.Insert(enumData);
                base.AddLog(string.Format("添加枚举值[{0}]成功", enumData.Name), AdminLoggerModuleEnum.Enum);
                return JsonMessage(true, "添加枚举值成功");
            }
            else
            {
                _enumDataService.Update(enumData);
                base.AddLog(string.Format("修改枚举值[{0}]成功", enumData.Name), AdminLoggerModuleEnum.Enum);
                return JsonMessage(true, "修改枚举值成功");
            }
        }

        /// <summary>
        /// 删除枚举值
        /// </summary>
        [HttpPost]
        [ActionName("_EnumDataDelete")]
        public ActionResult EnumDataDeleteProvide(Guid id)
        {
            if (!base.HasPermission("enumdata", PermissionOperate.delete))
            {
                return JsonMessage(false, "你没有删除枚举值的权限");
            }

            EnumData enumData = _enumDataService.GetById(id);
            _enumDataService.Delete(id.ToString());
            base.AddLog(string.Format("删除枚举值[{0}]成功", enumData.Name), AdminLoggerModuleEnum.Enum);
            return JsonMessage(true, "删除成功");
        }
    }
}