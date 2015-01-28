using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Ocean.Page;
using Ocean.Entity.Enums.AdminLoggerModule;
using Ocean.Entity;
using Ocean.Services;
using Ocean.Core.Common;

namespace Ocean.Admin.Controllers
{
    #region 树菜单

    #region 树菜单节点
    /// <summary>
    /// 树菜单节点
    /// </summary>
    class MenuNode
    {
        public MenuNode(string groupId, int id, int parentId, string text, string url)
        {
            GroupId = groupId;
            Id = id;
            ParentId = parentId;
            Text = text;
            Url = url;
        }

        /// <summary>
        /// 分组ID
        /// </summary>
        public string GroupId { set; get; }

        /// <summary>
        /// 当前节点ID
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// 上级ID
        /// </summary>
        public int ParentId { set; get; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Text { set; get; }

        /// <summary>
        /// 网址
        /// </summary>
        public string Url { set; get; }
    }
    #endregion

    #region 菜单列表

    /// <summary>
    /// 菜单列表
    /// </summary>
    class MenuList
    {
        private List<MenuNode> list = new List<MenuNode>();

        public int Add(string groupId, int parentId, string text, string url, bool hadPermission = true)
        {
            if (hadPermission)
            {
                //自动生成ID
                int Id = list.Count + 1;
                //添加节点到队列
                list.Add(new MenuNode(groupId, Id, parentId, text, url));
                return Id;
            }
            else
            {
                //return 0;
                //自动生成ID
                int Id = list.Count + 1;
                //添加节点到队列
                //list.Add(new MenuNode(groupId, Id, parentId, text + "[无权限]", ""));
                return Id;
            }
        }

        /// <summary>
        /// 获取分组Json数据
        /// </summary>
        /// <param name="groupId">分组ID</param>
        /// <param name="parentId">上级菜单ID</param>
        /// <returns></returns>
        private string GetGroupJson(string groupId, int parentId)
        {
            StringBuilder strJson = new StringBuilder();

            foreach (MenuNode node in list)
            {
                if (node.GroupId != groupId || node.ParentId != parentId)
                {
                    continue;
                }

                string strChildren = GetGroupJson(groupId, node.Id);
                string strIcon = "icon-nav";

                if (!String.IsNullOrEmpty(strChildren))
                {
                    strChildren = ",\"children\": " + strChildren;
                    strIcon = "icon-sys";
                }

                strJson.AppendFormat("{{\"id\": {0}, \"text\": \"{1}\", \"url\": \"{2}\", \"iconCls\":\"{3}\" {4}}},", node.Id.ToString(), node.Text, node.Url, strIcon, strChildren);
            }

            if (strJson.ToString() != "")
            {
                return "[ " + strJson.ToString().Substring(0, strJson.ToString().Length - 1) + "]";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 获取完整JSON数据
        /// </summary>
        /// <returns></returns>
        public string GetJsonData()
        {
            //获取菜单分组
            List<string> GroupList = new List<string>();

            foreach (MenuNode node in list)
            {
                if (!GroupList.Contains(node.GroupId)) GroupList.Add(node.GroupId);
            }

            StringBuilder strJsonData = new StringBuilder();

            foreach (string group in GroupList)
            {
                strJsonData.AppendLine(group + ":" + GetGroupJson(group, 0) + ",");
            }

            string JsonData = strJsonData.ToString().Trim();
            JsonData = JsonData.Remove(JsonData.Length - 1, 1);
            return JsonData;
        }
    }
    #endregion

    #endregion

    public class HomeController : AdminBaseController
    {
        // GET: /Home/

        private readonly IAdminService _adminService;

        public HomeController(IAdminService adminService)
        {
            this._adminService = adminService;
        }

        public ActionResult Message()
        {
            return Content("多加的功能［3月3号给出］");
        }

        public ActionResult KF()
        {
            return Content("<a href='/Kf/Chat' target='_blank'>点击打开新窗口进入客服服务界面</a>");
            //return Content("<a href='/Kf/Chat' target='_blank'>点击打开新窗口进入客服服务界面</a>");<br/><br/><a href='/Kf/Index' target='_blank'>点击打开新窗口进入访客服务界面</a>
        }

        public ActionResult Main()
        {
            return View(); 
        }

        public ActionResult Index()
        {
            MenuList menuList = new MenuList();
            string GroupId = "";
            int ParentId = 0;

            GroupId = "m1";
            ParentId = menuList.Add(GroupId, 0, "常用操作", "");
            menuList.Add(GroupId, ParentId, "桌面", "/Home/Main");
            ParentId = menuList.Add(GroupId, 0, "角色权限管理", "");
            menuList.Add(GroupId, ParentId, "管理员管理", "/Permission/AdminList", base.HasPermission("admin", PermissionOperate.manager));
            menuList.Add(GroupId, ParentId, "部门管理", "/Permission/PermissionOrganizationList", base.HasPermission("organization", PermissionOperate.manager));
            menuList.Add(GroupId, ParentId, "模块管理", "/Permission/PermissionModuleList", base.HasPermission("module", PermissionOperate.manager));
            menuList.Add(GroupId, ParentId, "权限管理", "/Permission/PermissionModuleCodeList", base.HasPermission("permission", PermissionOperate.manager));
            menuList.Add(GroupId, ParentId, "角色管理", "/Permission/PermissionRoleList", base.HasPermission("role", PermissionOperate.manager));
            ParentId = menuList.Add(GroupId, 0, "日记管理", "");
            menuList.Add(GroupId, ParentId, "系统日记", "/Permission/AdminLoggerList", base.HasPermission("adminlogger", PermissionOperate.manager));

            GroupId = "m2";
            ParentId = menuList.Add(GroupId, 0, "证件管理", "");
            menuList.Add(GroupId, ParentId, "驾驶证清单", "/DrivingLicense/index", base.HasPermission("DrivingLicense", PermissionOperate.manager));
            menuList.Add(GroupId, ParentId, "行驶证清单", "/VehicleLicense/index", base.HasPermission("VehicleLicense", PermissionOperate.manager));
            ParentId = menuList.Add(GroupId, 0, "保险单管理", "");
            menuList.Add(GroupId, ParentId, "保险单清单", "/Baoxian/index", base.HasPermission("Baoxian", PermissionOperate.manager)); 
            ParentId = menuList.Add(GroupId, 0, "下线管理", "");
            menuList.Add(GroupId, ParentId, "我的下线", "/MyUnder/index", base.HasPermission("MyUnder", PermissionOperate.manager));
            ParentId = menuList.Add(GroupId, 0, "投诉管理", "");
            menuList.Add(GroupId, ParentId, "投诉列表", "/Complain/index", base.HasPermission("Complain", PermissionOperate.manager));
            
            GroupId = "m3";
            ParentId = menuList.Add(GroupId, 0, "基本设置", "");
            menuList.Add(GroupId, ParentId, "公众账号绑定", "/MpCenter/Index", base.HasPermission("mpcenterindex", PermissionOperate.manager));
            menuList.Add(GroupId, ParentId, "二维码生成", "/MpCenter/QrCode", base.HasPermission("mpcenterqrcode", PermissionOperate.manager));
            ParentId = menuList.Add(GroupId, 0, "用户管理", "");
            menuList.Add(GroupId, ParentId, "分组管理", "/MpUser/Index", base.HasPermission("mpgroup", PermissionOperate.manager));
            menuList.Add(GroupId, ParentId, "用户管理", "/MpUser/UserIndex", base.HasPermission("mpuser", PermissionOperate.manager));
            ParentId = menuList.Add(GroupId, 0, "素材管理", "");
            menuList.Add(GroupId, ParentId, "图文消息", "/MpMaterial/Index", base.HasPermission("mpmaterial", PermissionOperate.manager));
            ParentId = menuList.Add(GroupId, 0, "回复设置", "");
            menuList.Add(GroupId, ParentId, "关注时自动回复", "/MpReply/Index?type=beadded", base.HasPermission("mpattentionreply", PermissionOperate.manager));
            menuList.Add(GroupId, ParentId, "消息自动回复", "/MpReply/Index?type=autoreply", base.HasPermission("mpmessagereply", PermissionOperate.manager));
            menuList.Add(GroupId, ParentId, "关键词自动回复", "/MpReply/Keyword", base.HasPermission("mpkeywordreply", PermissionOperate.manager));
            //menuList.Add(GroupId, ParentId, "自定义菜单设置", "/Home/Message", base.HasPermission("mpmenusetting", PermissionOperate.manager));
            
            GroupId = "m4";
            ParentId = menuList.Add(GroupId, 0, "枚举管理", "");
            menuList.Add(GroupId, ParentId, "枚举类型管理", "/Enum/EnumTypeList", base.HasPermission("enumtype", PermissionOperate.manager));
            menuList.Add(GroupId, ParentId, "枚举值管理", "/Enum/EnumDataList", base.HasPermission("enumdata", PermissionOperate.manager));
            ParentId = menuList.Add(GroupId, 0, "配置管理", "");
            menuList.Add(GroupId, ParentId, "安全IP", "/Configuration/SafeIPEdit", base.HasPermission("safeip", PermissionOperate.manager));
            ParentId = menuList.Add(GroupId, 0, "任务调度管理", "");
            menuList.Add(GroupId, ParentId, "任务调度", "/Task", base.HasPermission("scheduletask", PermissionOperate.manager));

            
            ViewBag.JsonData = menuList.GetJsonData();
            return View();
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        [ActionName("_LogOut")]
        [HttpPost]
        public ActionResult LogOut()
        {
            AdminLogin.Instance.AdminLogout();
            base.AddLog(string.Format("管理员[{0}]安全退出成功", base.LoginAdmin.Name), AdminLoggerModuleEnum.Admin);
            return JsonMessage(true, "退出成功");
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        [ActionName("_UpdatePwd")]
        [HttpPost]
        public ActionResult UpdatePwd()
        {
            Guid id = new Guid(RQuery["Id"]);
            Ocean.Entity.Admin admin = _adminService.GetById(id);

            if (RQuery["NewPassword"] != RQuery["ConfirmPwd"])
            {
                return JsonMessage(false, "两次输入密码不正确");
            }

            if (Hash.MD5Encrypt(Hash.MD5Encrypt(RQuery["Password"])) != admin.Password)
            {
                return JsonMessage(false, "原密码错误");
            }

            admin.Password = Hash.MD5Encrypt(Hash.MD5Encrypt(RQuery["NewPassword"]));
            _adminService.Update(admin);
            base.AddLog(string.Format("管理员[{0}]修改密码成功", admin.Name), AdminLoggerModuleEnum.Admin);
            return JsonMessage(true, "修改成功");
        }

        public ActionResult ExceteSql(string sql)
        {
            _adminService.ExcuteSql("alter table MpQrScene alter column ImgUrl varchar(1000);");
            return Content("success");
        }
    }
}