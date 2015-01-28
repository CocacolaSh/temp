using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Page;
using Ocean.Services;
using Ocean.Entity;
using Ocean.Core;
using Newtonsoft.Json;
using Ocean.Core.Logging;

namespace Ocean.Admin.Controllers
{
    public class MpReplyController : AdminBaseController
    {
        private readonly IMpMaterialService MpMaterialService;
        private readonly IMpReplyService MpReplyService;
        private readonly IMpMaterialItemService MpMaterialItemService;

        public MpReplyController(IMpMaterialService MpMaterialService, IMpMaterialItemService MpMaterialItemService, 
            IMpReplyService MpReplyService)
        {
            this.MpMaterialService = MpMaterialService;
            this.MpMaterialItemService = MpMaterialItemService;
            this.MpReplyService = MpReplyService;
        }

        /// <summary>
        /// 回复设置
        /// </summary>
        [HttpGet]
        public ActionResult Index(string type)
        {
            if (type == "beadded")
            {
                if (!base.HasPermission("mpattentionreply", PermissionOperate.manager))
                {
                    return base.ShowNotPermissionTip("");
                }
            }

            if (type == "autoreply")
            {
                if (!base.HasPermission("mpmessagereply", PermissionOperate.manager))
                {
                    return base.ShowNotPermissionTip("");
                }
            }

            MpReply reply = MpReplyService.GetByAction(type);
            if (reply==null)
            {
                reply = new MpReply();
                reply.Id = Guid.Empty;
                reply.Action = type;
                reply.CategoryID = Guid.Empty;
                reply.CreateDate = DateTime.Now;
                reply.CreateUser = Guid.Empty;
                reply.KeywordName = "";
                reply.MatchMode = 0;
                reply.MaterialID = Guid.Empty;
                reply.MpMaterial = new MpMaterial{Id=Guid.Empty,TypeName="text",TypeID=5,CreateDate=DateTime.Now};
                IList<MpMaterialItem> mpitems = new List<MpMaterialItem> { new MpMaterialItem{
                    Id=Guid.Empty,
                    CreateDate=DateTime.Now,
                    ReplyContent=""
                }};
                reply.MpMaterial.MpMaterialItems = mpitems;
                reply.RuleName = "";
                reply.SortIndex = 0;
                reply.UpdateDate = DateTime.Now;
                reply.UpdateUser = Guid.Empty;
            }
            ViewBag.strList = JsonConvert.SerializeObject(reply);
            return View(reply);
        }

        /// <summary>
        /// 关键词回复
        /// </summary>
        [HttpGet]
        public ActionResult KeyWord()
        {
            if (!base.HasPermission("mpkeywordreply", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            PagedList<Ocean.Entity.MpReply> list = MpReplyService.GetPageList(x => x.Action == "smartreply", PageIndex, PageSize);
            ViewBag.strList = JsonConvert.SerializeObject(list);
            ViewBag.totalCount = list.TotalItemCount;
            return View(list);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveReply(string data)
        {
            MpReplyService.SaveMpRepy(data);
            return JsonMessage(true, "");
        }

        public JsonResult DelReply(Guid id)
        {
            try
            {
                MpReplyService.BeginTransaction();
                MpReply reply = MpReplyService.GetALL(x=>x.Id==id).FirstOrDefault();
                if (reply!=null)
                {
                    if (reply.MpMaterial.TypeName == "text")
                    {
                        MpMaterialItemService.Delete(reply.MpMaterial.MpMaterialItems.FirstOrDefault());
                        MpMaterialService.Delete(reply.MpMaterial);
                    }
                    MpReplyService.Delete(reply);
                }
                MpReplyService.Commit();
                return JsonMessage(true, "");
            }
            catch (Exception e)
            {
                Log4NetImpl.Write("DelReply失败:" + e.Message);
                MpReplyService.Rollback();
                return JsonMessage(false, "");
                
            }
        }

    }
}