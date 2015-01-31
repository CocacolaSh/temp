using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Page;
using Ocean.Entity;
using Ocean.Core;
using Ocean.Core.Common;
using System.Text;
using Ocean.Core.Utility;
using Ocean.Entity.DTO;
using Ocean.Entity.Enums.AdminLoggerModule;
using Ocean.Core.Enums;
using Ocean.Services;
using Ocean.Framework.Caching.Cache;

namespace Ocean.Admin.Controllers
{
    public class PermissionController : AdminBaseController
    {
        private readonly IAdminService _adminService;
        private readonly IAdminLoggerService _adminLoggerService;
        private readonly IPermissionModuleService _permissionModuleService;
        private readonly IPermissionModuleCodeService _permissionModuleCodeService;
        private readonly IPermissionRoleService _permissionRoleService;
        private readonly IPermissionOrganizationService _permissionOrganizationService;

        public PermissionController(IAdminService adminService, IAdminLoggerService _adminLoggerService
            , IPermissionModuleService permissionModuleService
            , IPermissionModuleCodeService permissionModuleCodeService
            , IPermissionRoleService permissionRoleService
            , IPermissionOrganizationService permissionOrganizationService)
        {
            this._adminService = adminService;
            this._adminLoggerService = _adminLoggerService;
            this._permissionModuleService = permissionModuleService;
            this._permissionModuleCodeService = permissionModuleCodeService;
            this._permissionRoleService = permissionRoleService;
            this._permissionOrganizationService = permissionOrganizationService;
        }

        /*************************************************管理员区域******************************************************/

        /// <summary>
        /// 初始化AdminList页面
        /// </summary>
        [HttpGet]
        public ActionResult AdminList()
        {
            if (!base.HasPermission("admin", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return AdminView();
        }

        /// <summary>
        /// 获取管理员列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_AdminList")]
        public ActionResult AdminListProvide()
        {
            if (!base.HasPermission("admin", PermissionOperate.manager))
            {
                return null;
            }
            try
            {
                PagedList<Ocean.Entity.Admin> list = _adminService.GetPageList(PageIndex, PageSize);
                return SwitchJsonList<Ocean.Entity.Admin, AdminDTO>(list, list.TotalItemCount);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取管理员列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_AdminListJson")]
        public ActionResult AdminListJsonProvide()
        {
            IList<Ocean.Entity.Admin> listAdmin = _adminService.GetALL();
            IList<JsonObject> listJsonObject = new List<JsonObject>();

            foreach (Ocean.Entity.Admin item in listAdmin)
            {
                JsonObject jsonObject = new JsonObject();
                jsonObject["id"] = new JsonProperty(item.Id.ToString());
                jsonObject["text"] = new JsonProperty(item.Name);
                listJsonObject.Add(jsonObject);
            }

            return Content(JsonHelper.AnalysisJsons(listJsonObject, true));
        }

        /// <summary>
        /// 初始化AdminEdit页面
        /// </summary>
        [HttpGet]
        public ActionResult AdminEdit()
        {
            if (!base.HasPermission("admin", PermissionOperate.add) && !base.HasPermission("admin", PermissionOperate.edit))
            {
                return base.ShowNotPermissionTip("");
            }

            string id = RQuery["Id"];
            Ocean.Entity.Admin admin = string.IsNullOrWhiteSpace(id) ? null : _adminService.GetById(new Guid(id));
            return AdminView(admin);
        }

        /// <summary>
        /// 编辑管理员
        /// </summary>
        [HttpPost]
        [ActionName("_AdminEdit")]
        public ActionResult AdminEditProvide()
        {
            if (string.IsNullOrWhiteSpace(RQuery["AdminId"]))
            {
                if (!base.HasPermission("admin", PermissionOperate.add))
                {
                    return JsonMessage(false, "你没有添加管理员的权限");
                }
            }
            else
            {
                if (!base.HasPermission("admin", PermissionOperate.edit))
                {
                    return JsonMessage(false, "你没有编辑管理员的权限");
                }
            }

            Ocean.Entity.Admin admin = new Entity.Admin();
            string oldPassword = string.Empty;

            if (!string.IsNullOrWhiteSpace(RQuery["AdminId"]))
            {
                admin = _adminService.GetById(new Guid(RQuery["AdminId"]));
                oldPassword = admin.Password;
            }

            UpdateModel<Ocean.Entity.Admin>(admin);

            if (!string.IsNullOrWhiteSpace(RQuery["Password"]))
            {
                //判断密码有效性
                if (!StringValidate.IsNumCHA20(admin.Password))
                {
                    return JsonMessage(false, "添加管理员失败，密码必须由6-20字母、数字组成");
                }
            }

            if (RQuery["Password"] != RQuery["PasswordConfirm"])
            {
                return JsonMessage(false, "两次输入密码不匹配");
            }

            if (admin.PermissionOrganizationId == Guid.Empty || admin.PermissionOrganizationId == null)
            {
                return JsonMessage(false, "请选择所属部门");
            }

            if (admin.PermissionRoleId == Guid.Empty || admin.PermissionRoleId == null)
            {
                return JsonMessage(false, "请选择所属角色");
            }

            if (string.IsNullOrWhiteSpace(RQuery["AdminId"]))
            {
                admin.Password = Hash.MD5Encrypt(Hash.MD5Encrypt(admin.Password));
                admin.PasswordKey = Guid.NewGuid().ToString().Substring(0, 8);
                admin.State = 1;
                _adminService.Insert(admin);
                base.AddLog(string.Format("添加管理员[{0}]成功", admin.Name), AdminLoggerModuleEnum.Admin);
                return JsonMessage(true, "添加管理员成功");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(RQuery["Password"]))
                {
                    admin.Password = Hash.MD5Encrypt(Hash.MD5Encrypt(admin.Password));
                    admin.PasswordKey = Guid.NewGuid().ToString().Substring(0, 8);
                }
                else
                {
                    admin.Password = oldPassword;
                }

                _adminService.Update(admin);
                base.AddLog(string.Format("修改管理员[{0}]成功", admin.Name), AdminLoggerModuleEnum.Admin);
                return JsonMessage(true, "修改管理员成功");
            }
        }

        /// <summary>
        /// 删除管理员
        /// </summary>
        [HttpPost]
        [ActionName("_AdminDelete")]
        public ActionResult AdminDeleteProvide(Guid id)
        {
            if (!base.HasPermission("admin", PermissionOperate.delete))
            {
                return JsonMessage(false, "你没有删除管理员的权限");
            }

            Ocean.Entity.Admin admin = _adminService.GetById(id);
            _adminService.Delete(id.ToString());
            base.AddLog(string.Format("删除管理员[{0}]成功", admin.Name), AdminLoggerModuleEnum.Admin);
            return JsonMessage(true, "删除成功");
        }

        /// <summary>
        /// 冻结管理员
        /// </summary>
        [HttpPost]
        [ActionName("_AdminFreeze")]
        public ActionResult AdminFreezeProvide(Guid id)
        {
            if (!base.HasPermission("admin", PermissionOperate.freeze))
            {
                return JsonMessage(false, "你没有冻结管理员的权限");
            }

            Ocean.Entity.Admin admin = _adminService.GetById(id);

            if (_adminService.FreezeAdmin(id))
            {
                base.AddLog(string.Format("冻结管理员[{0}]成功", admin.Name), AdminLoggerModuleEnum.Admin);
            }

            return JsonMessage(true, "冻结成功");
        }

        /// <summary>
        /// 解冻管理员
        /// </summary>
        [HttpPost]
        [ActionName("_AdminUnFreeze")]
        public ActionResult AdminUnFreezeProvide(Guid id)
        {
            if (!base.HasPermission("admin", PermissionOperate.unfreeze))
            {
                return JsonMessage(false, "你没有解冻管理员的权限");
            }

            Ocean.Entity.Admin admin = _adminService.GetById(id);

            if (_adminService.UnFreezeAdmin(id))
            {
                base.AddLog(string.Format("解冻管理员[{0}]成功", admin.Name), AdminLoggerModuleEnum.Admin);
            }

            return JsonMessage(true, "解冻成功");
        }

        /// <summary>
        /// 初始化AdminLoggerList页面
        /// </summary>
        [HttpGet]
        public ActionResult AdminLoggerList()
        {
            if (!base.HasPermission("adminlogger", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            List<EnumItem> list = EnumExtensions.ToListEnumItem<AdminLoggerModuleEnum>();
            EnumItem enumItem = new EnumItem
            {
                EnumDescript = "--请选择--"
            };
            list.Insert(0, enumItem);
            SelectList dropdownList = new SelectList(list, "EnumValue", "EnumDescript");
            ViewBag.ListModule = dropdownList;
            return AdminView();
        }

        /// <summary>
        /// 获取管理员日记列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_AdminLoggerList")]
        public ActionResult AdminLoggerListProvide(AdminLoggerDTO adminLoggerDTO)
        {
            if (!base.HasPermission("adminlogger", PermissionOperate.manager))
            {
                return null;
            }

            PagedList<AdminLogger> list = _adminLoggerService.GetPageList(PageIndex, PageSize, adminLoggerDTO);
            return JsonList<AdminLogger>(list, list.TotalItemCount);
        }

        /// <summary>
        /// 删除管理员操作日记
        /// </summary>
        [HttpPost]
        [ActionName("_AdminLoggerDelete")]
        public ActionResult AdminLoggerDeleteProvide(string idList)
        {
            if (!base.HasPermission("adminlogger", PermissionOperate.delete))
            {
                return JsonMessage(false, "你没有删除系统日记的权限");
            }

            idList = idList.Trim(',');
            _adminLoggerService.Delete(idList);
            base.AddLog(string.Format("删除系统日记[{0}]成功", idList), AdminLoggerModuleEnum.AdminLogger);
            return JsonMessage(true, "删除成功");
        }

        /// <summary>
        /// 初始化PermissionOrganizationList页面
        /// </summary>
        [HttpGet]
        public ActionResult PermissionOrganizationList()
        {
            if (!base.HasPermission("organization", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return AdminView();
        }

        /// <summary>
        /// 获取部门列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_PermissionOrganizationList")]
        public ActionResult PermissionOrganizationListProvide()
        {
            if (!base.HasPermission("organization", PermissionOperate.manager))
            {
                return null;
            }

            bool isBuildTree = !string.IsNullOrWhiteSpace(Request["buildTree"]);
            IQueryable<PermissionOrganization> query = _permissionOrganizationService.Table
                .Where(m => (m.ParentId == Guid.Empty || m.ParentId == null));
            IList<PermissionOrganization> listPermissionOrganization = query.ToList();
            IList<JsonObject> listJsonObject = new List<JsonObject>();

            foreach (PermissionOrganization item in listPermissionOrganization)
            {
                JsonObject jsonObject = new JsonObject();

                if (isBuildTree)
                    BuildTree<PermissionOrganization>(item, _permissionOrganizationService, jsonObject, "ParentId", "Name", _permissionOrganizationService.Table.OrderBy(p => p.Sort).ToList());
                else
                    BuildGridTree<PermissionOrganization>(item, _permissionOrganizationService, jsonObject, "ParentId", new string[] { "name", "sort" }.ToList(), _permissionOrganizationService.Table.OrderBy(p => p.Sort).ToList());

                listJsonObject.Add(jsonObject);
            }

            if (isBuildTree)
                return Content(JsonHelper.AnalysisJsons(listJsonObject, true));
            else
                return Content(JsonHelper.AnalysisJsons(listJsonObject));
        }

        /// <summary>
        /// 初始化PermissionOrganizationEdit页面
        /// </summary>
        [HttpGet]
        public ActionResult PermissionOrganizationEdit()
        {
            if (!base.HasPermission("organization", PermissionOperate.add) && !base.HasPermission("organization", PermissionOperate.edit))
            {
                return base.ShowNotPermissionTip("");
            }

            string id = RQuery["Id"];
            PermissionOrganization permissionOrganization = string.IsNullOrWhiteSpace(id) ? null : _permissionOrganizationService.GetById(new Guid(id));
            return AdminView(permissionOrganization);
        }

        /// <summary>
        /// 编辑部门
        /// </summary>
        [HttpPost]
        [ActionName("_PermissionOrganizationEdit")]
        public ActionResult PermissionOrganizationEditProvide()
        {
            if (string.IsNullOrWhiteSpace(RQuery["OrganizationId"]))
            {
                if (!base.HasPermission("organization", PermissionOperate.add))
                {
                    return JsonMessage(false, "你没有添加部门的权限");
                }
            }
            else
            {
                if (!base.HasPermission("organization", PermissionOperate.edit))
                {
                    return JsonMessage(false, "你没有编辑部门的权限");
                }
            }

            PermissionOrganization permissionOrganization = new PermissionOrganization();
            Guid oldParentId = Guid.Empty;
            string oldRootPath = string.Empty;

            if (!string.IsNullOrWhiteSpace(RQuery["OrganizationId"]))
            {
                permissionOrganization = _permissionOrganizationService.GetById(new Guid(RQuery["OrganizationId"]));
                oldParentId = permissionOrganization.ParentId;
                oldRootPath = permissionOrganization.RootPath;
            }

            UpdateModel<PermissionOrganization>(permissionOrganization);

            if (string.IsNullOrWhiteSpace(RQuery["OrganizationId"]))//新增
            {
                _permissionOrganizationService.Insert(permissionOrganization);
                //更新RootPath
                if (permissionOrganization.ParentId == Guid.Empty)
                    permissionOrganization.RootPath = string.Format("{0},{1}", Guid.Empty, permissionOrganization.Id.ToString().ToUpper());
                else
                    permissionOrganization.RootPath = string.Format("{0},{1}",
                        _permissionOrganizationService.GetById(permissionOrganization.ParentId).RootPath, permissionOrganization.Id.ToString().ToUpper());

                _permissionOrganizationService.Update(permissionOrganization);
                base.AddLog(string.Format("添加部门[{0}]成功", permissionOrganization.Name), AdminLoggerModuleEnum.Organization);
                return JsonMessage(true, "添加部门成功");
            }
            else//修改
            {
                if (oldParentId != permissionOrganization.ParentId)
                {
                    PermissionOrganization permissionOrganizationParent = _permissionOrganizationService.GetById(permissionOrganization.ParentId);

                    if (permissionOrganizationParent != null && permissionOrganizationParent.RootPath.ToLower().Contains(permissionOrganization.Id.ToString().ToLower()))
                    {
                        return JsonMessage(false, "修改部门失败，上级部门不能是本身或本身的子节点");
                    }

                    //更新RootPath

                    permissionOrganization.RootPath = string.Format("{0},{1}",
                        permissionOrganizationParent == null ? Guid.Empty.ToString() : permissionOrganizationParent.RootPath, permissionOrganization.Id.ToString().ToUpper());
                    _permissionOrganizationService.Update(permissionOrganization);
                    //更新下级RootPath
                    _permissionOrganizationService.UpdateRootPath(oldRootPath, permissionOrganization.RootPath);
                }
                else
                {
                    _permissionOrganizationService.Update(permissionOrganization);
                }

                base.AddLog(string.Format("修改部门[{0}]成功", permissionOrganization.Name), AdminLoggerModuleEnum.Organization);
                return JsonMessage(true, "修改部门成功");
            }
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        [HttpPost]
        [ActionName("_PermissionOrganizationDelete")]
        public ActionResult PermissionOrganizationDeleteProvide(Guid id)
        {
            if (!base.HasPermission("organization", PermissionOperate.delete))
            {
                return JsonMessage(false, "你没有删除部门的权限");
            }

            PermissionOrganization permissionOrganization = _permissionOrganizationService.GetById(id);
            
            if (_permissionOrganizationService.CountChildOrganizationNumber(id) > 0)
            {
                return JsonMessage(false, "当前部门存在子部门，请先删除子部门");
            }

            if (permissionOrganization.Admins.Count > 0)
            {
                return JsonMessage(false, "当前部门存在管理员，请先删除管理员");
            }

            _permissionOrganizationService.Delete(permissionOrganization);
            base.AddLog(string.Format("删除部门[{0}]成功", permissionOrganization.Name), AdminLoggerModuleEnum.Organization);
            return JsonMessage(true, "删除成功");
        }

        /***************************************************************************************************************/

        /*************************************************权限区域******************************************************/

        /// <summary>
        /// 初始化PermissionModuleList页面
        /// </summary>
        [HttpGet]
        public ActionResult PermissionModuleList()
        {
            if (!base.HasPermission("module", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return AdminView();
        }

        /// <summary>
        /// 获取模块列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_PermissionModuleList")]
        public ActionResult PermissionModuleListProvide()
        {
            if (!base.HasPermission("module", PermissionOperate.manager))
            {
                return null;
            }

            bool isBuildTree = !string.IsNullOrWhiteSpace(Request["buildTree"]);
            IQueryable<PermissionModule> query = _permissionModuleService.Table
                .Where(m => (m.ParentId == Guid.Empty || m.ParentId == null)).OrderBy(m => m.Sort);
            IList<PermissionModule> listPermissionModule = query.ToList();
            IList<JsonObject> listJsonObject = new List<JsonObject>();

            foreach (PermissionModule item in listPermissionModule)
            {
                JsonObject jsonObject = new JsonObject();

                if (isBuildTree)
                    BuildTree<PermissionModule>(item, _permissionModuleService, jsonObject, "ParentId", "Name", _permissionModuleService.Table.OrderBy(m => m.Sort).ToList());
                else
                    BuildGridTree<PermissionModule>(item, _permissionModuleService, jsonObject, "ParentId", new string[] { "Name", "Identifying", "Url", "Sort" }.ToList(), _permissionModuleService.Table.OrderBy(m => m.Sort).ToList());          

                listJsonObject.Add(jsonObject);
            }

            if (isBuildTree)
                return Content(JsonHelper.AnalysisJsons(listJsonObject, true));
            else
                return Content(JsonHelper.AnalysisJsons(listJsonObject));
        }

        /// <summary>
        /// 初始化PermissionModuleEdit页面
        /// </summary>
        [HttpGet]
        public ActionResult PermissionModuleEdit()
        {
            if (!base.HasPermission("module", PermissionOperate.add) && !base.HasPermission("module", PermissionOperate.edit))
            {
                return base.ShowNotPermissionTip("");
            }

            string id = RQuery["Id"];
            PermissionModule permissionModule = string.IsNullOrWhiteSpace(id) ? null : _permissionModuleService.GetById(new Guid(id));
            return AdminView(permissionModule);
        }

        /// <summary>
        /// 编辑模块
        /// </summary>
        [HttpPost]
        [ActionName("_PermissionModuleEdit")]
        public ActionResult PermissionModuleEditProvide()
        {
            if (string.IsNullOrWhiteSpace(RQuery["ModuleId"]))
            {
                if (!base.HasPermission("module", PermissionOperate.add))
                {
                    return JsonMessage(false, "你没有添加模块的权限");
                }
            }
            else
            {
                if (!base.HasPermission("module", PermissionOperate.edit))
                {
                    return JsonMessage(false, "你没有编辑模块的权限");
                }
            }

            PermissionModule permissionModule = new PermissionModule();
            Guid oldParentId = Guid.Empty;
            string oldRootPath = string.Empty;

            if (!string.IsNullOrWhiteSpace(RQuery["ModuleId"]))
            {
                permissionModule = _permissionModuleService.GetById(new Guid(RQuery["ModuleId"]));
                oldParentId = permissionModule.ParentId;
                oldRootPath = permissionModule.RootPath;
            }

            UpdateModel<PermissionModule>(permissionModule);

            if (string.IsNullOrWhiteSpace(RQuery["ModuleId"]))//新增
            {
                _permissionModuleService.Insert(permissionModule);
                //更新RootPath
                if (permissionModule.ParentId == Guid.Empty)
                    permissionModule.RootPath = string.Format("{0},{1}", Guid.Empty, permissionModule.Id.ToString().ToUpper());
                else
                    permissionModule.RootPath = string.Format("{0},{1}",
                        _permissionModuleService.GetById(permissionModule.ParentId).RootPath, permissionModule.Id.ToString().ToUpper());

                _permissionModuleService.Update(permissionModule);
                base.AddLog(string.Format("添加模块[{0}]成功", permissionModule.Name), AdminLoggerModuleEnum.Module);
                return JsonMessage(true, "添加模块成功");
            }
            else//修改
            {
                if (oldParentId != permissionModule.ParentId)
                {
                    PermissionModule permissionModuleParent = _permissionModuleService.GetById(permissionModule.ParentId);

                    if (permissionModuleParent != null && permissionModuleParent.RootPath.ToLower().Contains(permissionModule.Id.ToString().ToLower()))
                    {
                        return JsonMessage(false, "修改模块失败，上级模块不能是本身或本身的子节点");
                    }

                    //更新RootPath

                    permissionModule.RootPath = string.Format("{0},{1}",
                        permissionModuleParent == null ? Guid.Empty.ToString() : permissionModuleParent.RootPath, permissionModule.Id.ToString().ToUpper());
                    _permissionModuleService.Update(permissionModule);
                    //更新下级RootPath
                    _permissionModuleService.UpdateRootPath(oldRootPath, permissionModule.RootPath);
                }
                else
                {
                    _permissionModuleService.Update(permissionModule);
                }

                base.AddLog(string.Format("修改模块[{0}]成功", permissionModule.Name), AdminLoggerModuleEnum.Module);
                return JsonMessage(true, "修改模块成功");
            }
        }

        /// <summary>
        /// 删除模块
        /// </summary>
        [HttpPost]
        [ActionName("_PermissionModuleDelete")]
        public ActionResult PermissionModuleDeleteProvide(Guid id)
        {
            if (!base.HasPermission("module", PermissionOperate.delete))
            {
                return JsonMessage(false, "你没有删除模块的权限");
            }

            PermissionModule permissionModule = _permissionModuleService.GetById(id);

            if (permissionModule.PermissionModuleCodes.Count > 0)
            {
                return JsonMessage(false, "当前模块存在权限集，请先删除权限集");
            }

            if (_permissionModuleService.CountChildModuleNumber(id) > 0)
            {
                return JsonMessage(false, "当前模块存在子模块，请先删除子模块");
            }

            _permissionModuleService.Delete(permissionModule);
            base.AddLog(string.Format("删除模块[{0}]成功", permissionModule.Name), AdminLoggerModuleEnum.Module);
            return JsonMessage(true, "删除成功");
        }

        /// <summary>
        /// 初始化PermissionModuleCodeList页面
        /// </summary>
        [HttpGet]
        public ActionResult PermissionModuleCodeList()
        {
            if (!base.HasPermission("permission", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return AdminView();
        }

        /// <summary>
        /// 获取权限数据
        /// </summary>
        [HttpPost]
        [ActionName("_PermissionModuleCodeList")]
        public ActionResult PermissionModuleCodeListProvide()
        {
            if (!base.HasPermission("permission", PermissionOperate.manager))
            {
                return null;
            }

            string moduleId = RQuery["ModuleId"];
            PagedList<PermissionModuleCode> list = null;

            if (string.IsNullOrWhiteSpace(moduleId))
                list = _permissionModuleCodeService.GetPageList("CreateDate", PageIndex, PageSize, true);
            else
                list = _permissionModuleCodeService.GetPageListByModuleId(new Guid(moduleId), PageIndex, PageSize);

            return SwitchJsonList<PermissionModuleCode, PermissionModuleCodeDTO>(list, list.TotalItemCount);
        }

        /// <summary>
        /// 初始化PermissionModuleCodeEdit页面
        /// </summary>
        [HttpGet]
        public ActionResult PermissionModuleCodeEdit()
        {
            if (!base.HasPermission("permission", PermissionOperate.add) && !base.HasPermission("permission", PermissionOperate.edit))
            {
                return base.ShowNotPermissionTip("");
            }

            if (!string.IsNullOrWhiteSpace(RQuery["PermissionModuleId"]))
            {
                PermissionModule permissionModule = _permissionModuleService.GetById(new Guid(RQuery["PermissionModuleId"]));
                ViewBag.PermissionModule = permissionModule;
            }

            string id = RQuery["Id"];
            PermissionModuleCode permissionModuleCode = string.IsNullOrWhiteSpace(id) ? null : _permissionModuleCodeService.GetById(new Guid(id));

            if (permissionModuleCode != null)
            {
                ViewBag.PermissionModule = permissionModuleCode.PermissionModule;
            }

            return AdminView(permissionModuleCode);
        }

        /// <summary>
        /// 编辑权限
        /// </summary>
        [HttpPost]
        [ActionName("_PermissionModuleCodeEdit")]
        public ActionResult PermissionModuleCodeEditProvide()
        {
            string permissionModuleId = RQuery["PermissionModuleId"];
            string permissionModuleCodeId = RQuery["PermissionModuleCodeId"];

            if (string.IsNullOrWhiteSpace(permissionModuleCodeId))
            {
                if (!base.HasPermission("permission", PermissionOperate.add))
                {
                    return JsonMessage(false, "你没有添加权限的权限");
                }
            }
            else
            {
                if (!base.HasPermission("permission", PermissionOperate.edit))
                {
                    return JsonMessage(false, "你没有编辑权限的权限");
                }
            }

            if (string.IsNullOrWhiteSpace(permissionModuleCodeId))
            {
                PermissionModuleCode permissionModuleCode = new PermissionModuleCode();
                UpdateModel<PermissionModuleCode>(permissionModuleCode);
                _permissionModuleCodeService.Insert(permissionModuleCode);
                AdminPermissionsCache.Instance.RemoveCache();
                base.AddLog(string.Format("添加权限[{0}]成功", permissionModuleCode.Name), AdminLoggerModuleEnum.Permission);
                return JsonMessage(true, "添加权限成功");
            }
            else
            {
                PermissionModuleCode permissionModuleCode = _permissionModuleCodeService.GetById(new Guid(permissionModuleCodeId));
                UpdateModel<PermissionModuleCode>(permissionModuleCode);
                _permissionModuleCodeService.Update(permissionModuleCode);
                AdminPermissionsCache.Instance.RemoveCache();
                base.AddLog(string.Format("修改权限[{0}]成功", permissionModuleCode.Name), AdminLoggerModuleEnum.Permission);
                return JsonMessage(true, "修改权限成功");
            }
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        [HttpPost]
        [ActionName("_PermissionModuleCodeDelete")]
        public ActionResult PermissionModuleCodeDeleteProvide(Guid id)
        {
            if (!base.HasPermission("permission", PermissionOperate.delete))
            {
                return JsonMessage(false, "你没有删除权限的权限");
            }

            PermissionModuleCode permissionModuleCode = _permissionModuleCodeService.GetById(id);
            _permissionModuleCodeService.Delete(id.ToString());
            AdminPermissionsCache.Instance.RemoveCache();
            base.AddLog(string.Format("删除权限[{0}]成功", permissionModuleCode.Name), AdminLoggerModuleEnum.Permission);
            return JsonMessage(true, "删除成功");
        }

        /// <summary>
        /// 初始化PermissionRoleList页面
        /// </summary>
        [HttpGet]
        public ActionResult PermissionRoleList()
        {
            if (!base.HasPermission("role", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            ViewBag.Permissions = string.Empty;
            ViewBag.Name = string.Empty;
            ViewBag.CurrentCheckedId = string.Empty;
            IList<PermissionModule> list1 = _permissionModuleService.GetALL("Sort", true);
            IList<PermissionModule> list2 = new List<PermissionModule>();
            BuildPermissionModule(list1, Guid.Empty, list2);
            //所有权限
            IList<PermissionModuleCode> listPermission = _permissionModuleCodeService.GetALL("CreateDate", true);
            ViewBag.ListPermission = listPermission;
            //当前角色权限
            string permissionRoleId = RQuery["permissionRoleId"];

            if (!string.IsNullOrWhiteSpace(permissionRoleId))
            {
                PermissionRole permissionRole = _permissionRoleService.GetById(new Guid(permissionRoleId));
                ViewBag.Permissions = permissionRole.Permissions;
                ViewBag.Name = permissionRole.Name;
                ViewBag.CurrentCheckedId = permissionRole.Id;
            }

            return View(list2);
        }

        /// <summary>
        /// 组装层级模块
        /// </summary>
        private void BuildPermissionModule(IList<PermissionModule> list1, Guid parentId, IList<PermissionModule> list2)
        {
            foreach (PermissionModule model in list1)
            {
                if (model.ParentId == parentId)
                {
                    string nbsp = "";

                    for (int i = 0; i < StringHelper.GetDotCount(model.RootPath) - 1; i++)
                    {
                        nbsp += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    }

                    model.Name = nbsp + model.Name;
                    list2.Add(model);
                    BuildPermissionModule(list1, model.Id, list2);
                }
            }
        }

        /// <summary>
        /// 获取角色列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_PermissionRoleList")]
        public ActionResult PermissionRoleListProvide()
        {
            if (!base.HasPermission("role", PermissionOperate.manager))
            {
                return null;
            }

            IList<PermissionRole> listPermissionRole = _permissionRoleService.GetALL();
            IList<JsonObject> listJsonObject = new List<JsonObject>();

            foreach (PermissionRole item in listPermissionRole)
            {
                JsonObject jsonObject = new JsonObject();
                jsonObject["id"] = new JsonProperty(item.Id.ToString());
                jsonObject["text"] = new JsonProperty(item.Name);
                listJsonObject.Add(jsonObject);
            }

            return Content(JsonHelper.AnalysisJsons(listJsonObject, true));
        }

        /// <summary>
        /// 添加/编辑角色
        /// </summary>
        [HttpPost]
        [ActionName("_PermissionRoleEdit")]
        public ActionResult PermissionRoleEditProvide()
        {
            string currentCheckedId = RQuery["currentCheckedId"];
            PermissionRole permissionRole = null;

            if (string.IsNullOrWhiteSpace(currentCheckedId))
                permissionRole = new PermissionRole();
            else
                permissionRole = _permissionRoleService.GetById(new Guid(currentCheckedId));

            UpdateModel<PermissionRole>(permissionRole);

            if (string.IsNullOrWhiteSpace(currentCheckedId))
            {
                _permissionRoleService.Insert(permissionRole);
                base.AddLog(string.Format("添加角色[{0}]成功", permissionRole.Name), AdminLoggerModuleEnum.Role);
                return JsonMessage(true, "添加角色成功");
            }
            else
            {
                _permissionRoleService.Update(permissionRole);
                base.AddLog(string.Format("编辑角色[{0}]成功", permissionRole.Name), AdminLoggerModuleEnum.Role);
                return JsonMessage(true, "编辑角色成功");
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        [HttpPost]
        [ActionName("_PermissionRoleDelete")]
        public ActionResult PermissionRoleDeleteProvide(Guid id)
        {
            if (!base.HasPermission("role", PermissionOperate.delete))
            {
                return JsonMessage(false, "你没有删除角色的权限");
            }

            if (_permissionRoleService.GetById(id).Admins.Count > 0)
            {
                return JsonMessage(false, "删除失败，有管理员为该角色");
            }

            PermissionRole permissionRole = _permissionRoleService.GetById(id);
            _permissionRoleService.Delete(id.ToString());
            base.AddLog(string.Format("删除角色[{0}]成功", permissionRole.Name), AdminLoggerModuleEnum.Role);
            return JsonMessage(true, "删除成功");
        }

        /***************************************************************************************************************/
    }
}