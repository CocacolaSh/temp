using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Page;
using Ocean.Services;
using Ocean.Core;
using Ocean.Entity;
using Ocean.Entity.DTO;
using Ocean.Communication.Model;
using Ocean.Communication.Comet;

namespace Ocean.Admin.Controllers
{
    public class KFController : AdminBaseController
    {
        private readonly IKfNumberService _kfNumberService;
        private readonly IKfMeetingService _kfMeetingService;
        private readonly IKfMeetingMessageService _kfMeetingMessageService;
        private readonly IAdminService _adminService;
        private readonly IMpUserService _mpUserService;

        public KFController(IKfNumberService kfNumberService, 
            IKfMeetingService kfMeetingService, 
            IKfMeetingMessageService kfMeetingMessageService,
            IAdminService adminService,
            IMpUserService mpUserService)
        {
            this._kfNumberService = kfNumberService;
            this._kfMeetingService = kfMeetingService;
            this._kfMeetingMessageService = kfMeetingMessageService;
            this._adminService = adminService;
            this._mpUserService = mpUserService;
        }

        //客服工号管理

        /// <summary>
        /// 初始化KfNumberList页面
        /// </summary>
        [HttpGet]
        public ActionResult KfNumberList()
        {
            return AdminView();
        }

        /// <summary>
        /// 获取客服工号列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_KfNumberList")]
        public ActionResult KfNumberListProvide()
        {
            PagedList<KfNumber> list = _kfNumberService.GetPageList("CreateDate", PageIndex, PageSize, true);
            return SwitchJsonList<KfNumber, KfNumberDTO>(list, list.TotalItemCount);
        }

        /// <summary>
        /// 初始化KfNumberEdit页面
        /// </summary>
        [HttpGet]
        public ActionResult KfNumberEdit()
        {
            string id = RQuery["Id"];
            KfNumber kfNumber = string.IsNullOrWhiteSpace(id) ? null : _kfNumberService.GetById(new Guid(id));
            return AdminView(kfNumber);
        }

        /// <summary>
        /// 编辑客服工号
        /// </summary>
        [HttpPost]
        [ActionName("_KfNumberEdit")]
        public ActionResult KfNumberEditProvide()
        {
            KfNumber kfNumber = new KfNumber();
            Guid oldAdminId = Guid.Empty;

            if (!string.IsNullOrWhiteSpace(RQuery["KfNumberId"]))
            {
                kfNumber = _kfNumberService.GetById(new Guid(RQuery["KfNumberId"]));
                oldAdminId = kfNumber.AdminId;
            }

            UpdateModel<KfNumber>(kfNumber);

            if (kfNumber.AdminId == Guid.Empty || kfNumber.AdminId == null)
            {
                return JsonMessage(false, "请选择分配管理员");
            }

            if (string.IsNullOrWhiteSpace(RQuery["KfNumberId"]))
            {
                Ocean.Entity.Admin admin = _adminService.GetById(kfNumber.AdminId);

                if (admin.KfNumbers.Count != 0)
                {
                    return JsonMessage(false, "当前所选管理员已被分配过工号");
                }

                _kfNumberService.Insert(kfNumber);
                return JsonMessage(true, "添加客服工号成功");
            }
            else
            {
                if (oldAdminId != kfNumber.AdminId)
                {
                    Ocean.Entity.Admin admin = _adminService.GetById(kfNumber.AdminId);

                    if (admin.KfNumbers.Count != 0)
                    {
                        return JsonMessage(false, "当前所选管理员已被分配过工号");
                    }
                }

                _kfNumberService.Update(kfNumber);
                return JsonMessage(true, "修改客服工号成功");
            }
        }

        /// <summary>
        /// 删除客服工号
        /// </summary>
        [HttpPost]
        [ActionName("_KfNumberDelete")]
        public ActionResult KfNumberDeleteProvide(Guid id)
        {
            //删除聊天记录和会话
            IList<KfMeeting> listKfMeeting = _kfMeetingService.GetList(" where KfNumberId='" + id + "'");
            foreach (KfMeeting kfMeeting in listKfMeeting)
            {
                //_kfMeetingMessageService.Delete(k => k.KfMeetingId == kfMeeting.Id);
                _kfMeetingMessageService.Delete("where KfMeetingId='" + kfMeeting.Id.ToString() + "'");
                _kfMeetingService.Delete(kfMeeting);
            } 
            //删除工号
            _kfNumberService.Delete(id.ToString());
            return JsonMessage(true, "删除成功");
        }

        //客服会话管理

        /// <summary>
        /// 初始化KfMeetingList页面
        /// </summary>
        [HttpGet]
        public ActionResult KfMeetingList()
        {
            if (!base.HasPermission("kfmeeting", PermissionOperate.view))
            {
                if (base.LoginAdmin.KfNumbers.Count == 0)
                {
                    return Content("当前用户不被允许查看所有记录，您必须先被分配一个工号");
                }
            }

            return AdminView();
        }

        /// <summary>
        /// 获取客服会话列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_KfMeetingList")]
        public ActionResult KfMeetingListProvide(KfMeetingDTO kfMeetingDTO)
        {
            if (!base.HasPermission("kfmeeting", PermissionOperate.view))
            {
                if (base.LoginAdmin.KfNumbers.Count == 0)
                {
                    return null;
                }

                KfNumber kfNumber = (KfNumber)base.LoginAdmin.KfNumbers.ToList()[0];
                kfMeetingDTO.KfNumberId = kfNumber.Id;
            }

            PagedList<KfMeeting> list = _kfMeetingService.GetPageList(PageIndex, PageSize, kfMeetingDTO);
            return SwitchJsonList<KfMeeting, KfMeetingDTO>(list, list.TotalItemCount);
        }

        //客服聊天记录管理

        /// <summary>
        /// 初始化KfMeetingMessageList页面
        /// </summary>
        [HttpGet]
        public ActionResult KfMeetingMessageList()
        {
            if (!base.HasPermission("kfmeetingmessage", PermissionOperate.view))
            {
                if (base.LoginAdmin.KfNumbers.Count == 0)
                {
                    return Content("当前用户不被允许查看所有记录，您必须先被分配一个工号");
                }
            }

            string kfMeetingId = RQuery["KfMeetingId"];

            if (!string.IsNullOrWhiteSpace(kfMeetingId))
                ViewBag.KfMeetingId = kfMeetingId;
            else
                ViewBag.KfMeetingId = string.Empty;

            string mpUserId = RQuery["MpUserId"];

            if (!string.IsNullOrWhiteSpace(mpUserId))
                ViewBag.MpUserId = mpUserId;
            else
                ViewBag.MpUserId = string.Empty;
            
            return AdminView();
        }

        /// <summary>
        /// 获取客服会话聊天记录列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_KfMeetingMessageList")]
        public ActionResult KfMeetingMessageListProvide(KfMeetingMessageDTO kfMeetingMessageDTO)
        {
            if (!base.HasPermission("kfmeetingmessage", PermissionOperate.view))
            {
                if (base.LoginAdmin.KfNumbers.Count == 0)
                {
                    return null;
                }

                KfNumber kfNumber = (KfNumber)base.LoginAdmin.KfNumbers.ToList()[0];
                kfMeetingMessageDTO.KfNumberId = kfNumber.Id;
            }

            if (kfMeetingMessageDTO.MpUserId != null && kfMeetingMessageDTO.MpUserId != Guid.Empty)
            {
                KfNumber kfNumber = (KfNumber)base.LoginAdmin.KfNumbers.ToList()[0];
                kfMeetingMessageDTO.KfNumberId = kfNumber.Id;
            }

            PagedList<KfMeetingMessage> list = _kfMeetingMessageService.GetPageList(PageIndex, PageSize, kfMeetingMessageDTO);
            IList<KfMeetingMessage> list2 = list.OrderBy(m => m.CreateDate).ToList();
            return SwitchJsonList<KfMeetingMessage, KfMeetingMessageDTO>(list2, list.TotalItemCount);        
        }

        /// <summary>
        /// 删除聊天记录
        /// </summary>
        [HttpPost]
        [ActionName("_KfMeetingMessageDelete")]
        public ActionResult KfMeetingMessageDeleteProvide(string idList)
        {
            idList = idList.Trim(',');
            _kfMeetingMessageService.Delete(idList);
            return JsonMessage(true, "删除成功");
        }

        /// <summary>
        /// 客服系统
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.ListMpUser = _mpUserService.GetALL();
            return View();
        }

        /// <summary>
        /// 客服聊天服务页面
        /// </summary>
        [HttpGet]
        public ActionResult Chat()
        {
            if (base.LoginAdmin.KfNumbers.Count == 0)
            {
                return Content("请选分配工号给当前管理员，方可进入客服服务界面");
            }

            KfNumber kfNumber = (KfNumber)base.LoginAdmin.KfNumbers.ToList()[0];
            //设置客服为上线状态
            kfNumber.IsOnline = true;
            _kfNumberService.Update(kfNumber);
            //填充页面信息
            ViewBag.Department = base.LoginAdmin.PermissionOrganization.Name;
            ViewBag.KfNumber = kfNumber.Number + "[" + kfNumber.NickName + "]";
            ViewBag.RoleName = base.LoginAdmin.PermissionRole.Name;
            ViewBag.OnlineStatus = kfNumber.OnlineStatus;
            string hash = Guid.NewGuid().ToString();
            ViewBag.Hash = hash;
            //获取与当前客服还未结束的会话
            IList<KfMeeting> listKfMeeting = _kfMeetingService.Table.Where(k => k.KfNumberId == kfNumber.Id && !k.IsEnd).ToList();
            ViewBag.ListKfMeeting = listKfMeeting;
            //更新在线客服缓存
            OnlineKFModel onlineKFModel = new OnlineKFModel();
            onlineKFModel.Hash = hash;
            onlineKFModel.KfNumberId = kfNumber.Id;
            onlineKFModel.LastActiveDate = DateTime.Now;
            MessageManager.Instance.CacheLocalKF(onlineKFModel);
            return View();
        }

        /// <summary>
        /// 访客聊天服务页面
        /// </summary>
        [HttpGet]
        public ActionResult Customer()
        {
            ViewBag.SendUserId = new Guid(Request["SendUserId"]);
            return View();
        }
    }
}