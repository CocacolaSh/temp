using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Page;
using Ocean.Core;
using Ocean.Entity;
using Ocean.Services;
using Ocean.Entity.DTO;
using Ocean.Core.Utility;
using Ocean.Entity.Enums.AdminLoggerModule;
using Ocean.Entity.Enums.Branch;

namespace Ocean.Admin.Controllers
{
    public class BranchController : AdminBaseController
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            this._branchService = branchService;
        }

        /// <summary>
        /// 初始化BranchIndex页面
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            if (!base.HasPermission("branch", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return View();
        }

        /// <summary>
        /// 获网点类型
        /// </summary>
        [HttpPost]
        [ActionName("_BranchType")]
        public ActionResult BranchTypeProvide()
        {
            string strType = "[";
            strType += "{\"id\":\"\",\"text\":\"--请选择--\"},";
            foreach (var item in EnumExtensions.ToListEnumItem<BranchEnum>())
            {
                strType += "{\"id\":\"" + item.EnumValue.ToString() + "\",\"text\":\"" + item.EnumDescript + "\"},";
            }
            strType = strType.Substring(0, strType.Length - 1);
            strType += "]";
            return Content(strType);
        }

        /// <summary>
        /// 获取周边网点列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_BranchList")]
        public ActionResult BranchListProvide()
        {
            if (!base.HasPermission("branch", PermissionOperate.manager))
            {
                return null;
            }

            BranchDTO branchDTO = new BranchDTO();
            UpdateModel<BranchDTO>(branchDTO);
            PagedList<Branch> list = _branchService.GetPageList(PageIndex, PageSize, branchDTO);
            return JsonList<Branch>(list, list.TotalItemCount);
        }

        /// <summary>
        /// 初始化BranchEdit页面
        /// </summary>
        [HttpGet]
        public ActionResult BranchEdit()
        {
            if (!base.HasPermission("branch", PermissionOperate.add) && !base.HasPermission("branch", PermissionOperate.edit))
            {
                return base.ShowNotPermissionTip("");
            }

            string id = RQuery["Id"];
            Branch branch = string.IsNullOrWhiteSpace(id) ? null : _branchService.GetById(new Guid(id));
            return AdminView(branch);
        }

        /// <summary>
        /// 编辑周边网点
        /// </summary>
        [HttpPost]
        [ActionName("_BranchEdit")]
        public ActionResult BranchEditProvide()
        {
            if (string.IsNullOrWhiteSpace(RQuery["BranchId"]))
            {
                if (!base.HasPermission("branch", PermissionOperate.add))
                {
                    return JsonMessage(false, "你没有添加周边网点的权限");
                }
            }
            else
            {
                if (!base.HasPermission("branch", PermissionOperate.edit))
                {
                    return JsonMessage(false, "你没有编辑周边网点的权限");
                }
            }

            if (string.IsNullOrWhiteSpace(RQuery["Type"]))
            {
                return JsonMessage(false, "请选择网点类型");
            }

            Branch branch = new Branch();

            if (!string.IsNullOrWhiteSpace(RQuery["BranchId"]))
            {
                branch = _branchService.GetById(new Guid(RQuery["BranchId"]));
            }

            UpdateModel<Branch>(branch);

            if (string.IsNullOrWhiteSpace(RQuery["BranchId"]))
            {
                branch.Status = 1;
                _branchService.Insert(branch);
                base.AddLog(string.Format("添加周边网点[{0}]成功", branch.Name), AdminLoggerModuleEnum.Branch);
                return JsonMessage(true, "添加周边网点成功");
            }
            else
            {
                _branchService.Update(branch);
                base.AddLog(string.Format("修改周边网点[{0}]成功", branch.Name), AdminLoggerModuleEnum.Branch);
                return JsonMessage(true, "修改周边网点成功");
            }
        }

        /// <summary>
        /// 删除周边网点
        /// </summary>
        [HttpPost]
        [ActionName("_BranchDelete")]
        public ActionResult BranchDeleteProvide(Guid id)
        {
            if (!base.HasPermission("branch", PermissionOperate.delete))
            {
                return JsonMessage(false, "你没有删除周边网点的权限");
            }

            Branch branch = _branchService.GetById(id);
            _branchService.Delete(id.ToString());
            base.AddLog(string.Format("删除周边网点[{0}]成功", branch.Name), AdminLoggerModuleEnum.Branch);
            return JsonMessage(true, "删除成功");
        }

        /// <summary>
        /// 冻结周边网点
        /// </summary>
        [HttpPost]
        [ActionName("_BranchFreeze")]
        public ActionResult BranchFreezeProvide(Guid id)
        {
            if (!base.HasPermission("branch", PermissionOperate.freeze))
            {
                return JsonMessage(false, "你没有冻结周边网点的权限");
            }

            if (_branchService.FreezeBranch(id))
            {
                Branch branch = _branchService.GetById(id);
                base.AddLog(string.Format("冻结周边网点[{0}]成功", branch.Name), AdminLoggerModuleEnum.Branch);
            }

            return JsonMessage(true, "冻结成功");
        }

        /// <summary>
        /// 解冻周边网点
        /// </summary>
        [HttpPost]
        [ActionName("_BranchUnFreeze")]
        public ActionResult BranchUnFreezeProvide(Guid id)
        {
            if (!base.HasPermission("branch", PermissionOperate.unfreeze))
            {
                return JsonMessage(false, "你没有解冻周边网点的权限");
            }

            if (_branchService.UnFreezeBranch(id))
            {
                Branch branch = _branchService.GetById(id);
                base.AddLog(string.Format("解冻周边网点[{0}]成功", branch.Name), AdminLoggerModuleEnum.Branch);
            }

            return JsonMessage(true, "解冻成功");
        }

        /// <summary>
        /// 初始化MapPage页面
        /// </summary>
        [HttpGet]
        public ActionResult MapPage()
        {
            ViewBag.Longitude = TypeConverter.StrToDouble(RQuery["Longitude"], 118.657543);
            ViewBag.Latitude = TypeConverter.StrToDouble(RQuery["Latitude"], 24.73636);
            return View();
        }
    }
}