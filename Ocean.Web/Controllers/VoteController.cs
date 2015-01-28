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
using Ocean.Entity.Enums.Loan;
using Ocean.Page;
using Ocean.Core.Utility;
using Ocean.Core.Logging;
using Ocean.Entity.DTO.Plugins;
using Ocean.Framework.Mvc.Extensions;
using System.Xml;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Ocean.Web.Controllers
{
    public class VoteController : WebBaseController
    {
        private readonly IMpUserService _mpUserService;
        private readonly IVoteBaseService _voteBaseService;
        private readonly IVoteInfoService _voteInfoService;
        private readonly IVoteItemService _voteItemService;

        public VoteController(IMpUserService mpUserService
            , IVoteInfoService voteInfoService
            , IVoteItemService voteItemService
            , IVoteBaseService voteBaseService)
        {
            _mpUserService = mpUserService;
            _voteInfoService = voteInfoService;
            _voteItemService = voteItemService;
            _voteBaseService = voteBaseService;
        }

        #region 活动
        [HttpGet]
        public ActionResult VoteSummary(Guid Id)
        {

            if (Id == Guid.Empty)
            {
                throw new OceanException("参数有误，请检查！");
            }

            try
            {
                //页面上的Plugins
                IList<VoteBase> VoteBases = _voteBaseService.GetALL();
                ViewBag.ContentPlugins = VoteBases;
                VoteBase VoteBase = VoteBases.Where(p => p.Id == Id).FirstOrDefault();
                ViewBag.CurVoteBase = VoteBase;
                if (VoteBase == null)
                {
                    throw new OceanException("对不起，未存在本投票，请检查！");
                }
                ViewBag.Title = VoteBase.VoteTitle;


                IList<VoteItem> voteItemList = _voteItemService.GetList("from VoteItem where VoteParentId='" + Id.ToString() + "'");
                ViewBag.voteItemList = voteItemList;
                if (MpUserID != Guid.Empty)
                {
                    IList<VoteInfo> voteInfoList = _voteInfoService.GetList("from VoteInfo where MpUserID='" + MpUserID.ToString() + "' and VoteItemID in (select ID from VoteItem where VoteParentId='" + Id.ToString() + "')");
                    ViewBag.voteSubmitItemList = voteInfoList;
                }

                return View("vote");
            }
            catch (System.Exception ex)
            {
                throw new OceanException(ex.Message, ex);
            }
            

        }
        #endregion

        #region 投票
        [HttpPost]
        public ActionResult Vote(Guid baseId, Guid itemId, string t)
        {
            try
            {
                if (MpUserID == Guid.Empty)
                {
                    string rawUrl = "http://wx.ssrcb.com/vote/VoteSummary?id=" + WebHelper.GetGuid("baseId", Guid.Empty);
                    if (string.IsNullOrEmpty(RQuery["openid"]))
                    {
                        Log4NetImpl.Write("open.weixin.qq.com");
                        return Json(new { isLogin = true, message = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri=http://wx.ssrcb.com/mpuser/autologin?refUrl={1}&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect", MpCenterCache.AppID, rawUrl) });
                    }
                }
                if (baseId == Guid.Empty || itemId == Guid.Empty)
                {
                    return Json(new { message = "参数有误，请检查！" });
                }

                VoteBase VoteBase = _voteBaseService.GetById(baseId);
                if (VoteBase == null)
                {
                    throw new OceanException("对不起，未存在本投票，请检查！");
                }
                if (VoteBase.VoteStartDate > DateTime.Now)
                {
                    return Json(new { message = "对不起，投票还未开始！" });
                }
                else if (VoteBase.VoteEndDate < DateTime.Now)
                {
                    return Json(new { message = "对不起，投票已经结束！" });
                }

                VoteItem voteItem = _voteItemService.GetUnique("from VoteItem where VoteParentId='" + baseId.ToString() + "' and Id='" + itemId.ToString() + "'");
                if (voteItem == null)
                {
                    throw new OceanException("对不起，未存在本投票，请检查！");
                }

                int voteCount = _voteInfoService.GetCount("from VoteInfo where MpUserID='" + MpUserID.ToString() + "' and VoteItemID in (select id from VoteItem where VoteParentId = '" + baseId.ToString() + "')");
                if (voteCount >= VoteBase.VoteCount)
                {
                    return Json(new { message = "对不起，您已经投了 " + voteCount + " 票！" });
                }
                voteCount = _voteInfoService.GetCount("from VoteInfo where MpUserID='" + MpUserID.ToString() + "' and VoteItemID='" + itemId.ToString() + "'");
                if (voteCount > 0)
                {
                    return Json(new { message = "对不起，您已经投了该选项！" });
                }
                VoteInfo newVoteInfo = new VoteInfo();
                newVoteInfo.MpUserId = MpUserID;
                newVoteInfo.VoteDate = DateTime.Now;
                newVoteInfo.VoteItemID = itemId;
                _voteInfoService.Insert(newVoteInfo);

                voteItem.VoteCount = voteItem.VoteCount + 1;
                _voteItemService.Update(voteItem);
                return Json(new { message = "感谢您的参与！", isSuccess = true, voteCount = voteItem.VoteCount });
            }
            catch (System.Exception ex)
            {
                throw new OceanException(ex.Message, ex);
            }

        }

        #endregion

        #region 活动
        [HttpGet]
        public ActionResult VoteSummary2(Guid Id)
        {

            if (Id == Guid.Empty)
            {
                throw new OceanException("参数有误，请检查！");
            }

            try
            {
                //页面上的Plugins
                IList<VoteBase> VoteBases = _voteBaseService.GetALL();
                ViewBag.ContentPlugins = VoteBases;
                VoteBase VoteBase = VoteBases.Where(p => p.Id == Id).FirstOrDefault();
                ViewBag.CurVoteBase = VoteBase;
                if (VoteBase == null)
                {
                    throw new OceanException("对不起，未存在本投票，请检查！");
                }
                ViewBag.Title = VoteBase.VoteTitle;


                IList<VoteItem> voteItemList = _voteItemService.GetList("from VoteItem where VoteParentId='" + Id.ToString() + "'");
                ViewBag.voteItemList = voteItemList;
                if (MpUserID != Guid.Empty)
                {
                    IList<VoteInfo> voteInfoList = _voteInfoService.GetList("from VoteInfo where MpUserID='" + MpUserID.ToString() + "' and VoteItemID in (select ID from VoteItem where VoteParentId='" + Id.ToString() + "')");
                    ViewBag.voteSubmitItemList = voteInfoList;
                }

                return View("vote2");
            }
            catch (System.Exception ex)
            {
                throw new OceanException(ex.Message, ex);
            }


        }
        #endregion

        #region 投票
        [HttpPost]
        public ActionResult Vote2(Guid baseId, Guid itemId, string Remark, string t)
        {
            try
            {
                if (MpUserID == Guid.Empty)
                {
                    string rawUrl = "http://wx.ssrcb.com/vote/VoteSummary?id=" + WebHelper.GetGuid("baseId", Guid.Empty);
                    if (string.IsNullOrEmpty(RQuery["openid"]))
                    {
                        Log4NetImpl.Write("open.weixin.qq.com");
                        return Json(new { isLogin = true, message = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri=http://wx.ssrcb.com/mpuser/autologin?refUrl={1}&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect", MpCenterCache.AppID, rawUrl) });
                    }
                }
                if (baseId == Guid.Empty || itemId == Guid.Empty)
                {
                    return Json(new { message = "参数有误，请检查！" });
                }

                VoteBase VoteBase = _voteBaseService.GetById(baseId);
                if (VoteBase == null)
                {
                    throw new OceanException("对不起，未存在本投票，请检查！");
                }
                if (VoteBase.VoteStartDate > DateTime.Now)
                {
                    return Json(new { message = "对不起，投票还未开始！" });
                }
                else if (VoteBase.VoteEndDate < DateTime.Now)
                {
                    return Json(new { message = "对不起，投票已经结束！" });
                }

                VoteItem voteItem = _voteItemService.GetUnique("from VoteItem where VoteParentId='" + baseId.ToString() + "' and Id='" + itemId.ToString() + "'");
                if (voteItem == null)
                {
                    throw new OceanException("对不起，未存在本投票，请检查！");
                }

                int voteCount = _voteInfoService.GetCount("from VoteInfo where MpUserID='" + MpUserID.ToString() + "' and VoteItemID='" + itemId.ToString() + "' ");
                if (voteCount >= VoteBase.VoteCount)
                {
                    return Json(new { message = "对不起，您已经投了 " + voteCount + " 票！" });
                }
                voteCount = _voteInfoService.GetCount("from VoteInfo where MpUserID='" + MpUserID.ToString() + "' and VoteItemID='" + itemId.ToString() + "'");
                if (voteCount > 0)
                {
                    return Json(new { message = "对不起，您已经投了改选项！" });
                }
                VoteInfo newVoteInfo = new VoteInfo();
                newVoteInfo.MpUserId = MpUserID;
                newVoteInfo.VoteDate = DateTime.Now;
                newVoteInfo.VoteItemID = itemId;
                newVoteInfo.Remark = Remark.Trim();
                _voteInfoService.Insert(newVoteInfo);

                voteItem.VoteCount = voteItem.VoteCount + 1;
                _voteItemService.Update(voteItem);
                return Json(new { message = "感谢您的参与！", isSuccess = true, voteCount = voteItem.VoteCount });
            }
            catch (System.Exception ex)
            {
                throw new OceanException(ex.Message, ex);
            }

        }

        #endregion
    }
}