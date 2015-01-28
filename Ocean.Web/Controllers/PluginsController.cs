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
    public class PluginsController : WebBaseController
    {
        private readonly IPluginService _pluginService;
        private readonly IPluginResultService _pluginResultService;
        private readonly IPluginUsedService _pluginUsedService;
        private readonly IPluginAllowUserService _pluginAllowUserService;
        private readonly IMobileCodeService _mobileCodeService;
        private readonly IMpUserService _mpUserService;
        private static Dictionary<Guid, IList<PrizeSiteUser>> prizeSiteUsers = new Dictionary<Guid, IList<PrizeSiteUser>>();
        public PluginsController(IMpUserService mpUserService
            , IPluginService pluginService
            , IPluginResultService pluginResultService
            , IPluginUsedService pluginUsedService
            , IPluginAllowUserService pluginAllowUserService
            , IMobileCodeService mobileCodeService)
        //, IFunongbaoApplyService funongbaoApplyService)
        //: base(funongbaoService)
        {
            _pluginService = pluginService;
            _pluginResultService = pluginResultService;
            _pluginUsedService = pluginUsedService;
            _mpUserService = mpUserService;
            _pluginAllowUserService = pluginAllowUserService;
            _mobileCodeService = mobileCodeService;
        }

        #region 共用方法
        /// <summary>
        /// 将数据塞到ViewData
        /// </summary>
        /// <param name="viewdata"></param>
        /// <param name="widgetNode"></param>
        public void GetDataToViewData(XmlNode widgetNode)
        {
            if (widgetNode == null)
            {
                return;
            }
            foreach (XmlNode propNode in widgetNode.ChildNodes)
            {
                if (propNode.Name.Trim().ToLower() == "cst_plugin_items")
                {
                    foreach (XmlNode itemNode in propNode.ChildNodes)
                    {
                        ViewData[itemNode.Name] = itemNode.InnerText;
                    }
                }
                else
                {
                    ViewData[propNode.Name] = propNode.InnerText;
                }
            }
        }


        public IList<PrizeSiteUser> GetPrizeUsers(Guid pluginId)
        {
            IList<PrizeSiteUser> prizeSiteUserList;
            if (!prizeSiteUsers.TryGetValue(pluginId, out  prizeSiteUserList))
            {
                IList<PluginResult> submitList = _pluginResultService.GetList("from PluginResult where PluginId='" + pluginId.ToString() + "'");
                if (submitList != null && submitList.Count > 0)
                {
                    prizeSiteUserList = new List<PrizeSiteUser>();
                    Regex reg = new Regex("<Cst_Plugin_ItemIndex>(\\d+)</Cst_Plugin_ItemIndex>");
                    foreach (PluginResult submit in submitList)
                    {
                        Match match = reg.Match(submit.Value);
                        if (match.Success)
                        {
                            int itemIndex = TypeConverter.StrToInt(match.Groups[1].Value);
                            PrizeSiteUser prizeSiteUser = prizeSiteUserList.Where(u => u.Prize_Index == itemIndex).FirstOrDefault();
                            if (prizeSiteUser == null)
                            {
                                prizeSiteUser = new PrizeSiteUser();
                                prizeSiteUser.Prize_Index = itemIndex;
                                prizeSiteUser.Users.Add(submit.MpUserId);
                                prizeSiteUserList.Add(prizeSiteUser);
                            }
                            else
                            {
                                //if (prizeSiteUser.Users.Where(u => u == submit.Site_User_Id).Count() == 0)//过滤掉，一个人中两次的情况[目前的设置只支持单次固定中奖]
                                //{
                                prizeSiteUser.Users.Add(submit.MpUserId);
                                //}
                            }
                        }
                    }
                }
                else
                {
                    prizeSiteUserList = new List<PrizeSiteUser>();
                    prizeSiteUsers[pluginId] = prizeSiteUserList;
                }
            }
            else
            {
                prizeSiteUserList = prizeSiteUsers[pluginId];
            }
            return prizeSiteUserList;
        }


        #endregion

        #region 插件系统页

        #region 抽奖活动
        [HttpGet]
        public ActionResult Lottery(Guid Id)
        {
#if !userlogin
            //if (SiteUserID == 0)
            //{
            //    return Redirect("/siteuser/ulogin.htm");
            //}
            //else
            //{
#endif
            if (Id == Guid.Empty)
            {
                throw new OceanException("参数有误，请检查！");
            }

            //页面上的Plugins
            IList<Plugin> plugins = _pluginService.GetALL();
            ViewBag.ContentPlugins = plugins;
            Plugin curPlugin = plugins.Where(p => p.Id == Id).FirstOrDefault();
            ViewBag.CurPlugin = curPlugin;
            if (curPlugin == null)
            {
                throw new OceanException("对不起，不存在该插件或未开通，请检查！");
            }
            ViewBag.Title = curPlugin.RPluginBase.Name + "_石狮农商银行";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(curPlugin.Value);
            XmlNode pluginNode = xmlDoc.SelectSingleNode("plugin");
            GetDataToViewData(pluginNode);

            //int prizeItemCount = TypeConverter.StrToInt(ViewBag.Cst_Plugin_Items_Count, 1);
            //IList<PrizeGift> prizeGift = new List<PrizeGift>(prizeItemCount);
            //IList<string> prizeItemsName = new List<string>(12);
            //prizeItemsName.Add("一");
            //prizeItemsName.Add("二");
            //prizeItemsName.Add("三");
            //prizeItemsName.Add("四");
            //prizeItemsName.Add("五");
            //prizeItemsName.Add("六");
            //prizeItemsName.Add("七");
            //prizeItemsName.Add("八");
            //prizeItemsName.Add("九");
            //prizeItemsName.Add("十");
            //prizeItemsName.Add("十一");
            //prizeItemsName.Add("十二");
            //for (int i = 0; i < prizeItemCount; i++)
            //{
            //    PrizeGift gift = new PrizeGift();
            //    gift.ID = i;
            //    gift.Name = ViewData.GetString("Cst_Plugin_Items_PrizeName" + i.ToString());
            //    gift.Odds = ViewData.GetDouble("Cst_Plugin_Items_PrizePercent" + i.ToString(), 0.00) / 100.00;
            //    gift.Pic = ViewData.GetString("Cst_Plugin_Items_PrizePic" + i.ToString());
            //    gift.Quantity = ViewData.GetInt("Cst_Plugin_Items_PrizeNum" + i.ToString());
            //    gift.Angle = ViewData.GetInt("Cst_Plugin_Items_PrizeAngle" + i.ToString());
            //    gift.Alias_Name = ViewData.GetString("Cst_Plugin_Items_PrizeAliasName" + i.ToString());
            //    if (ViewData["Cst_Plugin_Items_PrizeUsedNum" + i.ToString()] != null)
            //    {
            //        gift.Leavings_Quantity = ViewData.GetInt("Cst_Plugin_Items_PrizeUsedNum" + i.ToString());
            //    }
            //    else
            //    {
            //        gift.Leavings_Quantity = gift.Quantity;
            //    }
            //    gift.Prize_Name = prizeItemsName[i] + "等奖";
            //    if (!string.IsNullOrEmpty(gift.Alias_Name))
            //    {
            //        gift.Prize_Name = gift.Alias_Name;
            //    }
            //    gift.Has_Gift = 1;
            //    prizeGift.Add(gift);
            //}
            //ViewBag.PrizeGift = prizeGift;

            PluginUsed pluginUsed = _pluginUsedService.GetUnique("from PluginUsed where MpUserId='" + this.MpUserID.ToString() + "' and PluginId='" + Id.ToString() + "'");
            if (pluginUsed == null)
            {
                pluginUsed = new PluginUsed();
                pluginUsed.PluginId = Id;
                pluginUsed.HasUseCount = ViewData.GetInt("Cst_Plugin_UseCount", 1);
                pluginUsed.MpUserId = this.MpUserID;
                pluginUsed.CreateDate = DateTime.Now;
                pluginUsed.UseDate = DateTime.Now;
                _pluginUsedService.Insert(pluginUsed);
            }
            else
            {
                if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_UseEveryDay) && ViewBag.Cst_Plugin_UseEveryDay == "1")
                {
                    if (pluginUsed.UseDate.ToShortDateString() != DateTime.Now.ToShortDateString())
                    {
                        pluginUsed.HasUseCount = ViewData.GetInt("Cst_Plugin_UseCount", 1);
                        pluginUsed.UseDate = DateTime.Now;
                        _pluginUsedService.Update(pluginUsed);

                    }
                }
            }

            IList<PluginResult> pluginResultList = _pluginResultService.GetList("from PluginResult where PluginId='" + Id.ToString() + "'");
            ViewBag.PluginResultList = pluginResultList;
            if (pluginResultList != null)
            {
                IEnumerable<PluginResult> myplugin_SubmitList = _pluginResultService.GetList("from PluginResult where PluginId='" + Id.ToString() + "' and MpUserId='" + this.MpUserID + "'");
                ViewBag.MyPlugin_SubmitList = myplugin_SubmitList;
            }
            if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_StartDate))
            {
                DateTime endDate = TypeConverter.StrToDateTime(ViewBag.Cst_Plugin_StartDate.ToString());
                if (endDate <= DateTime.Now)
                {
                    if (curPlugin.IsOpen == 1)
                    {
                        curPlugin.IsOpen = 0;
                        _pluginService.Update(curPlugin);
                    }
                }
            }
            ViewBag.Has_Use_Count = pluginUsed.HasUseCount;
            ViewBag.MpUserArr = this.MpUserArr;
            ViewBag.MpUserID = this.MpUserID;

            return View("lottery");
#if !userlogin
            //}
#endif
        }
        [HttpGet]
        public ActionResult LotteryBak(Guid Id)
        {
#if !userlogin
            //if (SiteUserID == 0)
            //{
            //    return Redirect("/siteuser/ulogin.htm");
            //}
            //else
            //{
#endif
            if (Id == Guid.Empty)
            {
                throw new OceanException("参数有误，请检查！");
            }

            //页面上的Plugins
            IList<Plugin> plugins = _pluginService.GetALL();
            ViewBag.ContentPlugins = plugins;
            Plugin curPlugin = plugins.Where(p => p.Id == Id).FirstOrDefault();
            ViewBag.CurPlugin = curPlugin;
            if (curPlugin == null)
            {
                throw new OceanException("对不起，不存在该插件或未开通，请检查！");
            }
            ViewBag.Title = curPlugin.RPluginBase.Name + "_石狮农商银行";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(curPlugin.Value);
            XmlNode pluginNode = xmlDoc.SelectSingleNode("plugin");
            GetDataToViewData(pluginNode);

            int prizeItemCount = TypeConverter.StrToInt(ViewBag.Cst_Plugin_Items_Count, 1);
            IList<PrizeGift> prizeGift = new List<PrizeGift>(prizeItemCount);
            IList<string> prizeItemsName = new List<string>(12);
            prizeItemsName.Add("一");
            prizeItemsName.Add("二");
            prizeItemsName.Add("三");
            prizeItemsName.Add("四");
            prizeItemsName.Add("五");
            prizeItemsName.Add("六");
            prizeItemsName.Add("七");
            prizeItemsName.Add("八");
            prizeItemsName.Add("九");
            prizeItemsName.Add("十");
            prizeItemsName.Add("十一");
            prizeItemsName.Add("十二");
            for (int i = 0; i < prizeItemCount; i++)
            {
                PrizeGift gift = new PrizeGift();
                gift.ID = i;
                gift.Name = ViewData.GetString("Cst_Plugin_Items_PrizeName" + i.ToString());
                gift.Odds = ViewData.GetDouble("Cst_Plugin_Items_PrizePercent" + i.ToString(), 0.00) / 100.00;
                gift.Pic = ViewData.GetString("Cst_Plugin_Items_PrizePic" + i.ToString());
                gift.Quantity = ViewData.GetInt("Cst_Plugin_Items_PrizeNum" + i.ToString());
                gift.Angle = ViewData.GetInt("Cst_Plugin_Items_PrizeAngle" + i.ToString());
                gift.Alias_Name = ViewData.GetString("Cst_Plugin_Items_PrizeAliasName" + i.ToString());
                if (ViewData["Cst_Plugin_Items_PrizeUsedNum" + i.ToString()] != null)
                {
                    gift.Leavings_Quantity = ViewData.GetInt("Cst_Plugin_Items_PrizeUsedNum" + i.ToString());
                }
                else
                {
                    gift.Leavings_Quantity = gift.Quantity;
                }
                gift.Prize_Name = prizeItemsName[i] + "等奖";
                if (!string.IsNullOrEmpty(gift.Alias_Name))
                {
                    gift.Prize_Name = gift.Alias_Name;
                }
                gift.Has_Gift = 1;
                prizeGift.Add(gift);
            }
            ViewBag.PrizeGift = prizeGift;

            PluginUsed pluginUsed = _pluginUsedService.GetUnique("from PluginUsed where MpUserId='" + this.MpUserID.ToString() + "' and PluginId='" + Id.ToString() + "'");
            if (pluginUsed == null)
            {
                pluginUsed = new PluginUsed();
                pluginUsed.PluginId = Id;
                pluginUsed.HasUseCount = ViewData.GetInt("Cst_Plugin_UseCount", 1);
                pluginUsed.MpUserId = this.MpUserID;
                pluginUsed.CreateDate = DateTime.Now;
                pluginUsed.UseDate = DateTime.Now;
                _pluginUsedService.Insert(pluginUsed);
            }
            else
            {
                if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_UseEveryDay) && ViewBag.Cst_Plugin_UseEveryDay == "1")
                {
                    if (pluginUsed.UseDate.ToShortDateString() != DateTime.Now.ToShortDateString())
                    {
                        pluginUsed.HasUseCount = ViewData.GetInt("Cst_Plugin_UseCount", 1);
                        pluginUsed.UseDate = DateTime.Now;
                        _pluginUsedService.Update(pluginUsed);

                    }
                }
            }

            IList<PluginResult> pluginResultList = _pluginResultService.GetList("from PluginResult where PluginId='" + Id.ToString() + "'");
            ViewBag.PluginResultList = pluginResultList;
            if (pluginResultList != null)
            {
                IEnumerable<PluginResult> myplugin_SubmitList = _pluginResultService.GetList("from PluginResult where PluginId='" + Id.ToString() + "' and MpUserId='" + this.MpUserID + "'");
                ViewBag.MyPlugin_SubmitList = myplugin_SubmitList;
            }
            if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_StartDate))
            {
                DateTime endDate = TypeConverter.StrToDateTime(ViewBag.Cst_Plugin_StartDate.ToString());
                if (endDate <= DateTime.Now)
                {
                    if (curPlugin.IsOpen == 1)
                    {
                        curPlugin.IsOpen = 0;
                        _pluginService.Update(curPlugin);
                    }
                }
            }
            ViewBag.Has_Use_Count = pluginUsed.HasUseCount;
            ViewBag.MpUserArr = this.MpUserArr;
            ViewBag.MpUserID = this.MpUserID;

            return View("lottery");
#if !userlogin
            //}
#endif
        }
        [HttpPost]
        public ActionResult Lottery(Guid Id, string t)
        {
            try
            {
                Log4NetImpl.Write("抽奖开始：1");
                if (Id == Guid.Empty)
                {
                    return Json(new { message = "参数有误，请检查！" });
                }

                Plugin plugin = _pluginService.GetById(Id);
                if (plugin == null)
                {
                    return Json(new { message = "对不起，不存在该插件或未开通，请检查！" });
                }
#if !userlogin
                if (MpUserID == Guid.Empty)
                {
                    return Json(new { message = "该抽奖页已失效,请在微信里发\"" + plugin.Name + "\"命令！" });
                }
                else
                {
#endif
                    //设置布局页
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(plugin.Value);
                    XmlNode pluginNode = xmlDoc.SelectSingleNode("plugin");
                    GetDataToViewData(pluginNode);
                    if (ViewBag.Cst_Plugin_Title == null)
                    {
                        return Json(new { message = "对不起，您还未配置或配置错误，请到插件中心进行配置下！" });
                    }
                    if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_StartDate))
                    {
                        DateTime startDate = TypeConverter.StrToDateTime(ViewBag.Cst_Plugin_StartDate.ToString());
                        if (startDate > DateTime.Now)
                        {
                            return Json(new { message = "对不起，抽奖还未开始！" });
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_EndDate))
                            {
                                DateTime endDate = TypeConverter.StrToDateTime(ViewBag.Cst_Plugin_EndDate.ToString());
                                if (endDate < DateTime.Now)
                                {
                                    return Json(new { message = "对不起，抽奖已经结束！" });
                                }
                            }
                        }
                    }
                    PluginUsed pluginUsed = _pluginUsedService.GetUnique("from PluginUsed where MpUserId='" + this.MpUserID.ToString() + "' and PluginId='" + Id.ToString() + "'");
                    bool usedCount = false;
                    if (pluginUsed == null)
                    {
                        pluginUsed = new PluginUsed();
                        pluginUsed.PluginId = Id;
                        pluginUsed.HasUseCount = ViewData.GetInt("Cst_Plugin_UseCount", 1) - 1;
                        pluginUsed.MpUserId = this.MpUserID;
                        pluginUsed.UseDate = DateTime.Now;
                        pluginUsed.CreateDate = DateTime.Now;
                        _pluginUsedService.Insert(pluginUsed);
                    }
                    else
                    {

                        if (pluginUsed.HasUseCount <= 0)
                        {
                            return Json(new { message = "对不起，您已经不能再抽奖！" });
                        }
                        usedCount = true;
                    }
                    Log4NetImpl.Write("抽奖开始：加载次数限制");
                    int prizeCount = TypeConverter.StrToInt(ViewBag.Cst_Plugin_PrizeCount, 12);
                    int prizeItemCount = TypeConverter.StrToInt(ViewBag.Cst_Plugin_Items_Count, 1);
                    IList<PrizeGift> prizeGift = new List<PrizeGift>(prizeItemCount);
                    IList<Double> prizeItems = new List<Double>(prizeItemCount);
                    IList<string> prizeItemsName = new List<string>(prizeCount);
                    IList<int> prizeAngle = new List<int>(12);
                    prizeItemsName.Add("一");
                    prizeItemsName.Add("二");
                    prizeItemsName.Add("三");
                    prizeItemsName.Add("四");
                    prizeItemsName.Add("五");
                    prizeItemsName.Add("六");
                    prizeItemsName.Add("七");
                    prizeItemsName.Add("八");
                    prizeItemsName.Add("九");
                    prizeItemsName.Add("十");
                    prizeItemsName.Add("十一");
                    prizeItemsName.Add("十二");
                    for (int i = 0; i < prizeItemCount; i++)
                    {
                        PrizeGift gift = new PrizeGift();
                        gift.ID = i;
                        gift.Name = ViewData.GetString("Cst_Plugin_Items_PrizeName" + i.ToString());
                        gift.Alias_Name = ViewData.GetString("Cst_Plugin_Items_PrizeAliasName" + i.ToString());
                        gift.Odds = ViewData.GetDouble("Cst_Plugin_Items_PrizePercent" + i.ToString(), 0.00) / 100.00;
                        gift.Pic = ViewData.GetString("Cst_Plugin_Items_PrizePic" + i.ToString());
                        gift.Quantity = ViewData.GetInt("Cst_Plugin_Items_PrizeNum" + i.ToString());
                        if (ViewData["Cst_Plugin_Items_PrizeUsedNum" + i.ToString()] != null)
                        {
                            gift.Leavings_Quantity = ViewData.GetInt("Cst_Plugin_Items_PrizeUsedNum" + i.ToString());
                        }
                        else
                        {
                            gift.Leavings_Quantity = gift.Quantity;
                        }
                        gift.Angle = ViewData.GetInt("Cst_Plugin_Items_PrizeAngle" + i.ToString());
                        prizeAngle.Add(gift.Angle);
                        gift.Prize_Name = prizeItemsName[i] + "等奖";
                        if (!string.IsNullOrEmpty(gift.Alias_Name))
                        {
                            gift.Prize_Name = gift.Alias_Name;
                        }
                        gift.Has_Gift = 1;
                        gift.Users = ViewData.GetString("Cst_Plugin_Items_SiteUsers" + i.ToString());
                        gift.UsersName = ViewData.GetString("Cst_Plugin_Items_SiteUsersName" + i.ToString());
                        prizeItems.Add(gift.Odds);
                        prizeGift.Add(gift);
                    }
                    int averageAngle = 360 / prizeCount;
                    Log4NetImpl.Write("抽奖开始：概率与礼物设置");
                    if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_Prize_Multi) && ViewBag.Cst_Plugin_Prize_Multi == "1")
                    {
                        //多结果奖项
                        prizeGift.Add(new PrizeGift() { ID = prizeItemCount, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 1000000, Angle = 0, Prize_Name = "继续加油" });
                    }
                    else
                    {
                        if (prizeCount - prizeItemCount > 0)
                        {
                            for (int j = prizeItemCount; j < prizeCount; j++)
                            {
                                int angleNoUse = 0;
                                for (int a = 0; a < 12; a++)
                                {
                                    IEnumerable<int> angleTemp = prizeAngle.Where(an => an == a * averageAngle);
                                    if (angleTemp.Count() > 0)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        angleNoUse = a * averageAngle;
                                        prizeAngle.Add(angleNoUse);
                                        break;
                                    }
                                }
                                prizeGift.Add(new PrizeGift() { ID = j, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = angleNoUse, Prize_Name = "继续加油" });
                            }
                        }
                    }
                    prizeItems.OrderBy(d => d);

                    Log4NetImpl.Write("抽奖开始：设置未中奖项");

                    int result = -1;
                    string resultStr = "";

                    IList<PrizeSiteUser> prizeSiteUserList = GetPrizeUsers(Id);

                    if (prizeGift.Where(g => g.Has_Gift == 1 && g.Leavings_Quantity > 0).Count() == 0)
                    {
                        return JsonMessage(false, "对不起，已经没有奖品！");
                    }

                    if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_Prize_Multi) && ViewBag.Cst_Plugin_Prize_Multi == "1")
                    {
                        if (prizeItems.Count < 3)
                        {
                            return JsonMessage(false, "对不起，系统配置错误，中奖奖项必须3项或以上！");
                        }
                        result = LotteryUtils.LotteryMulti(prizeItems, prizeGift, prizeItems.Count, this.MpUserID, Id, ViewData.GetInt("Cst_Plugin_ZeroPrize"), out resultStr, prizeSiteUserList);
                    }
                    else
                    {
                        result = LotteryUtils.Lottery(prizeItems, prizeGift, prizeCount, this.MpUserID, Id, ViewData.GetInt("Cst_Plugin_ZeroPrize"), prizeSiteUserList);
                    }
                    Log4NetImpl.Write("抽奖开始：抽奖");
                    if (result == -1)
                    {
                        return Json(new PrizeGift() { ID = -1, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = -300, Prize_Name = "继续加油" });
                        //return JsonMessage(false, "对不起，根据系统的配置，已无法再次抽奖！");
                    }
                    else if (result == -2)
                    {
                        return Json(new PrizeGift() { ID = -2, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = -300, Prize_Name = "继续加油" });
                        //return JsonMessage(false, "对不起，根据系统的配置，已无法再次抽奖！");
                    }
                    else
                    {
                        PrizeGift hasPrizeGift = prizeGift[result];
                        if (string.IsNullOrEmpty(ViewBag.Cst_Plugin_Prize_Multi) || ViewBag.Cst_Plugin_Prize_Multi != "1")
                        {
                            hasPrizeGift.Rate_Array = "";
                        }
                        hasPrizeGift.Alias_Name = hasPrizeGift.Alias_Name ?? "";
                        if (hasPrizeGift.Has_Gift == 1 && hasPrizeGift.Leavings_Quantity > 0)
                        {
                            //中奖业务处理
                            PluginResult pluginResult = new PluginResult();
                            pluginResult.Address = "";
                            pluginResult.CreateDate = DateTime.Now;
                            pluginResult.Email = "";
                            pluginResult.IsUse = 0;
                            pluginResult.MobilePhone = "";
                            pluginResult.Name = "";
                            pluginResult.Phone = "";
                            pluginResult.PluginId = Id;
                            pluginResult.MpUserId = this.MpUserID;
                            pluginResult.SN = "";
                            pluginResult.UserName = (string.IsNullOrEmpty(this.Name) ? this.NickName : this.Name);

                            //增加中奖用户列表
                            PrizeSiteUser prizeSiteUser = prizeSiteUserList.Where(u => u.Prize_Index == result).FirstOrDefault();
                            if (prizeSiteUser == null)
                            {
                                prizeSiteUser = new PrizeSiteUser();
                                prizeSiteUser.Prize_Index = result;
                                prizeSiteUser.Users.Add(this.MpUserID);
                                prizeSiteUserList.Add(prizeSiteUser);
                            }
                            else
                            {
                                prizeSiteUser.Users.Add(this.MpUserID);
                            }

                            if (this.MpUserArr != null)
                            {
                                pluginResult.Summary = "恭喜用户[" + (string.IsNullOrEmpty(this.Name) ? this.NickName : this.Name) + "]获得" + hasPrizeGift.Prize_Name + "[" + hasPrizeGift.Name + "]";
                            }
                            else
                            {
                                pluginResult.Summary = "恭喜匿名用户获得" + hasPrizeGift.Prize_Name + "[" + hasPrizeGift.Name + "]";
                            }
                            pluginResult.Value = "<?xml version=\"1.0\" encoding=\"utf-8\"?><PluginSubmit><Cst_Plugin_ItemIndex>" + result.ToString() + "</Cst_Plugin_ItemIndex><Cst_Plugin_PrizeLevel>" + hasPrizeGift.Prize_Name + "</Cst_Plugin_PrizeLevel><Cst_Plugin_PrizeName>" + hasPrizeGift.Name + "</Cst_Plugin_PrizeName></PluginSubmit>";

                            #region 更新中奖数量
                            XmlNode itemsNode = pluginNode.SelectSingleNode("Cst_Plugin_Items");
                            XmlNode node = itemsNode.SelectSingleNode("Cst_Plugin_Items_PrizeUsedNum" + result.ToString());
                            if (node == null)
                            {
                                node = xmlDoc.CreateElement("Cst_Plugin_Items_PrizeUsedNum" + result.ToString());
                                node.InnerText = (hasPrizeGift.Quantity - 1).ToString();
                                itemsNode.AppendChild(node);
                            }
                            else
                            {
                                node.InnerText = (hasPrizeGift.Leavings_Quantity - 1).ToString();
                            }
                            StringWriter sw = new StringWriter();
                            XmlTextWriter tx = new XmlTextWriter(sw);
                            xmlDoc.WriteTo(tx);
                            string strXmlText = sw.ToString();
                            plugin.Value = strXmlText;
                            _pluginService.Update(plugin);
                            #endregion

                            _pluginResultService.Insert(pluginResult);
                            hasPrizeGift.ResultID = pluginResult.Id;
                            Log4NetImpl.Write("抽奖开始：中奖");
                        }
                        if (usedCount)
                        {
                            pluginUsed.HasUseCount = pluginUsed.HasUseCount - 1;
                            _pluginUsedService.Update(pluginUsed);
                        }
                        return Content(JsonConvert.SerializeObject(hasPrizeGift), "text/javascript");
                    }
#if !userlogin
                }
#endif
            }
            catch (Exception ex)
            {
                throw new OceanException(ex.Message, ex);
            }
        }

        [HttpPost]
        public ActionResult LotteryResult(Guid pluginId, Guid resultId)
        {
#if !userlogin
            if (MpUserID == Guid.Empty)
            {
                return JsonMessage(false, "你还未登陆！");
            }
            else
            {
#endif
                if (pluginId == Guid.Empty || resultId == Guid.Empty)
                {
                    return JsonMessage(false, "参数有误，请检查！");
                }
                else
                {
                    PluginResult pluginResult = _pluginResultService.GetUnique("from PluginResult where MpUserId='" + this.MpUserID.ToString() + "' and Id='" + resultId.ToString() + "'");
                    if (pluginResult == null)
                    {
                        return JsonMessage(false, "对不起，您无法领取该奖，未中过奖！");
                    }
                    else
                    {
                        if (pluginResult.IsUse == 1)
                        {
                            return JsonMessage(false, "对不起，您已经领取完奖！");
                        }
                        else
                        {
                            TryUpdateModel<PluginResult>(pluginResult);
                            _pluginResultService.Update(pluginResult);
                        }
                    }

                }
                return JsonMessage(true, "申请兑奖成功,如需更改信息，请在我的中奖记录里，申请兑奖!");
#if !userlogin
            }
#endif
        }

        public ActionResult GetResult(Guid pluginId, Guid resultId)
        {
#if !userlogin
            if (MpUserID == Guid.Empty)
            {
                return JsonMessage(false, "你还未登陆！");
            }
            else
            {
#endif
                if (pluginId == Guid.Empty || resultId == Guid.Empty)
                {
                    return JsonMessage(false, "参数有误，请检查！");
                }
                else
                {
                    PluginResult pluginResult = _pluginResultService.GetUnique("from PluginResult where MpUserId='" + this.MpUserID.ToString() + "' and Id='" + resultId.ToString() + "'");
                    if (pluginResult == null)
                    {
                        return JsonMessage(false, "对不起，您未中过该奖！");
                    }
                    else
                    {
                        return Json(pluginResult);
                    }
                }
#if !userlogin
            }
#endif
        }

        public ActionResult CertSubmit(Guid pluginId, string Name, string Phone, string CertNo2)
        {
#if !userlogin
            if (MpUserID == Guid.Empty)
            {
                return JsonMessage(false, "你还未关注本行微信或者未登陆！");
            }
            else
            {
#endif
                if (pluginId == Guid.Empty)
                {
                    return JsonMessage(false, "参数有误，请检查！");
                }
                else
                {
                    try
                    {
                        PluginAllowUser allowUser = _pluginAllowUserService.GetUnique("from PluginAllowUser where Name='" + Name.ToString().Trim() + "' and Phone='" + Phone.ToString().Trim() + "' and CertNo2='" + CertNo2.ToString().Trim().ToLower() + "'");
                        if (allowUser == null)
                        {
                            return JsonMessage(false, "对不起，您无法参与摇奖，本月交易未满3笔或不是我行签约用户！");
                        }
                        else
                        {
                            if (allowUser.MPUserID != null && allowUser.MPUserID != string.Empty)
                            {
                                return JsonMessage(false, "对不起，此信息已经被绑定，请不要多次摇奖！");
                            }

                            allowUser.MPUserID = this.MpUserID.ToString();
                            _pluginAllowUserService.Update(allowUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        return JsonMessage(false, "对不起，出现异常，请联系我行工作人员！");
                    }
                }
                return JsonMessage(true, "恭喜您，可以摇奖了!");
#if !userlogin
            }
#endif
        }
        public ActionResult CertSubmit2(Guid pluginId, string Phone, string Code)
        {
#if !userlogin
            if (MpUserID == Guid.Empty)
            {
                return JsonMessage(false, "你还未关注本行微信或者未登陆！");
            }
            else
            {
#endif
                if (pluginId == Guid.Empty || Code == string.Empty)
                {
                    return JsonMessage(false, "参数有误，请检查！");
                }
                else
                {
                    try
                    {
                        PluginAllowUser allowUser = _pluginAllowUserService.GetUnique("from PluginAllowUser where Phone='" + Phone.ToString().Trim() + "'");
                        if (allowUser == null)
                        {
                            //return JsonMessage(false, "对不起，您无法参与摇奖，不是我行签约用户或未使用手机银行！");
                            return JsonMessage(false, "亲！您本月没有摇奖资格，谢谢关注我行手机银行！");
                        }
                        else
                        {
                            if (allowUser.MPUserID != null && allowUser.MPUserID != string.Empty)
                            {
                                return JsonMessage(false, "对不起，此信息已经被绑定，请不要多次摇奖！");
                            }

                            //allowUser.MPUserID = this.MpUserID.ToString();
                            //_pluginAllowUserService.Update(allowUser);
                            int useCount = _mobileCodeService.GetMobileCodeCount(this.MpUserID, 4);//当天使用次数
                            if (useCount <= 5)
                            {
                                MobileCode lastCode = _mobileCodeService.GetMobileCode(this.MpUserID, 4);//是否存在未使用的证码
                                if (lastCode != null)
                                {
                                    //验证
                                    MobileCode mobileCode = _mobileCodeService.GetMobileCode(Phone, Code, MpUserID, 4);

                                    if (mobileCode != null)
                                    {
                                        TimeSpan ts = DateTime.Now.Subtract(mobileCode.CreateDate).Duration();

                                        if (ts.Minutes > 10)
                                        {
                                            return JsonMessage(false, "验证码已经失效，请重新获取");
                                            //isSuccess = false;
                                        }

                                        allowUser.MPUserID = this.MpUserID.ToString();
                                        _pluginAllowUserService.Update(allowUser);
                                    }
                                    else
                                    {
                                        lastCode.Time += 1;
                                        _mobileCodeService.Update(lastCode);
                                        return JsonMessage(false, "对不起，验证码有误，请检查！");
                                        //isSuccess = false;
                                    }
                                }
                                else
                                {
                                    return JsonMessage(false, "对不起，您还未点击发送验证码！");
                                    //isSuccess = false;
                                }
                            }
                            else
                            {
                                return JsonMessage(false, "对不起，您今天最多只能发起5次验证码！");
                                //isSuccess = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log4NetImpl.Write(ex.Message);
                        return JsonMessage(false, "对不起，出现异常，请联系我行工作人员！");
                    }
                }
                return JsonMessage(true, "恭喜您，可以摇奖了!");
#if !userlogin
            }
#endif
        }
        public ActionResult CertAndGetMobileCode(Guid pluginId, string mobile, int OperateType)
        {
#if !userlogin
            if (MpUserID == Guid.Empty)
            {
                return JsonMessage(false, "你还未关注本行微信或者未登陆！");
            }
            else
            {
#endif
                if (pluginId == Guid.Empty)
                {
                    return JsonMessage(false, "参数有误，请检查！");
                }
                else
                {
                    try
                    {
                        PluginAllowUser allowUser = _pluginAllowUserService.GetUnique("from PluginAllowUser where Phone='" + mobile.ToString().Trim() + "'");
                        if (allowUser == null)
                        {
                            return JsonMessage(false, "对不起，您无法参与摇奖，不是我行签约用户或未使用手机银行！");
                        }
                        else
                        {
                            if (allowUser.MPUserID != null && allowUser.MPUserID != string.Empty)
                            {
                                return JsonMessage(false, "对不起，此信息已经被绑定，请不要多次摇奖！");
                            }
                            return GetMobileCode(mobile, OperateType);

                        }
                    }
                    catch (Exception ex)
                    {
                        return JsonMessage(false, "对不起，出现异常，请联系我行工作人员！");
                    }
                }
                return JsonMessage(true, "恭喜您，可以摇奖了!");
#if !userlogin
            }
#endif
        }
        private static readonly object SequenceLock = new object();
        [HttpPost]
        public ActionResult Lottery2(Guid Id, string t)
        {
            try
            {
                Log4NetImpl.Write("抽奖开始：1");
                if (Id == Guid.Empty)
                {
                    return Json(new { message = "参数有误，请检查！" });
                }

                if (MpUserID == Guid.Empty)
                {
                    string rawUrl = "http://wx.ssrcb.com/plugins/lottery?id=" + WebHelper.GetGuid("Id", Guid.Empty);
                    if (string.IsNullOrEmpty(RQuery["openid"]))
                    {
                        Log4NetImpl.Write("open.weixin.qq.com");
                        return Json(new { isLogin = true, message = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri=http://wx.ssrcb.com/mpuser/autologin?refUrl={1}&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect", MpCenterCache.AppID, rawUrl) });
                        //return JavaScript("window.location = '" + string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri=http://localhost:2014/mpuser/autologin?refUrl={1}&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect" + "'", MpCenterCache.AppID, rawUrl));
                        //FunongbaoApply apply = _funongbaoApplyService.GetByFunongbaoId(CurrentFunongbao.Id);
                        //return JavaScript("window.location = 'http://localhost:2014/FunongBao/AutoLogin'");
                        //Response.Redirect(string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri=http://localhost:2014/mpuser/autologin?refUrl={1}&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect", MpCenterCache.AppID, rawUrl));
                        //return RedirectToRoute(new { controller = "FunongBao", action = "AutoLogin" });
                        //return Json(new { message = "请先登录我行微信，可先返回点击 [我的福农宝]！" }); 
                        //return Json(new { isLogin = true, message = string.Format("http://localhost:2014/mpuser/autologin2?refUrl={1}&openid=oy4Pvjp9_Yb7dttwfx9G2BJuGwyw", MpCenterCache.AppID, rawUrl) });

                    }
                    //else
                    //{
                    //    Response.Redirect("http://localhost:2014/mpuser/autologin?openid=" + RQuery["openid"]);
                    //}
                    //return Json(new { message = "请先登录我行微信，可先返回点击 [我的福农宝]！" });
                }
                //else
                //{

                //lock (SequenceLock)
                //{
                Log4NetImpl.Write("SequenceLock");
                Plugin plugin = _pluginService.GetById(Id);
                if (plugin == null)
                {
                    return Json(new { message = "对不起，不存在该插件或未开通，请检查！" });
                }

                //设置布局页
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(plugin.Value);
                XmlNode pluginNode = xmlDoc.SelectSingleNode("plugin");
                GetDataToViewData(pluginNode);
                if (ViewBag.Cst_Plugin_Title == null)
                {
                    return Json(new { message = "对不起，您还未配置或配置错误，请到插件中心进行配置下！" });
                }
                if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_StartDate))
                {
                    DateTime startDate = TypeConverter.StrToDateTime(ViewBag.Cst_Plugin_StartDate.ToString());
                    if (startDate > DateTime.Now)
                    {
                        return Json(new { message = "对不起，抽奖还未开始！" });
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_EndDate))
                        {
                            DateTime endDate = TypeConverter.StrToDateTime(ViewBag.Cst_Plugin_EndDate.ToString());
                            if (endDate < DateTime.Now)
                            {
                                return Json(new { message = "对不起，抽奖已经结束！" });
                            }
                        }
                    }
                }

                PluginAllowUser allowUser = _pluginAllowUserService.GetUnique("from PluginAllowUser where MPUserID='" + this.MpUserID.ToString() + "'");
                if (allowUser == null)
                {
                    return Json(new { message = "请先验证信息再抽奖！", isVertfy = true });
                }
                else if (allowUser.TradeNo < 3)
                {
                    return Json(new { message = "上月手机银行交易不足三笔，无法参加活动，请再接再励！" });
                }

                PluginUsed pluginUsed = _pluginUsedService.GetUnique("from PluginUsed where MpUserId='" + this.MpUserID.ToString() + "' and PluginId='" + Id.ToString() + "'");
                bool usedCount = false;
                if (pluginUsed == null)
                {
                    pluginUsed = new PluginUsed();
                    pluginUsed.PluginId = Id;
                    pluginUsed.HasUseCount = ViewData.GetInt("Cst_Plugin_UseCount", 1) - 1;
                    pluginUsed.MpUserId = this.MpUserID;
                    pluginUsed.UseDate = DateTime.Now;
                    pluginUsed.CreateDate = DateTime.Now;
                    _pluginUsedService.Insert(pluginUsed);
                }
                else
                {
                    if (pluginUsed.HasUseCount <= 0)
                    {
                        return Json(new { message = "对不起，您已经不能再抽奖！" });
                    }
                    int count = _pluginResultService.GetCount("from PluginResult where pluginId='" + Id.ToString() + "' and MpUserId='" + this.MpUserID.ToString() + "'");
                    if (ViewData.GetInt("Cst_Plugin_ZeroPrize") == 1 && count > 0)
                    {
                        return Json(new { message = "恭喜您，您已经中奖，不能再抽奖！" });
                    }
                    usedCount = true;
                }
                //Log4NetImpl.Write("抽奖开始：加载次数限制");
                int prizeCount = TypeConverter.StrToInt(ViewBag.Cst_Plugin_PrizeCount, 12);
                int prizeItemCount = TypeConverter.StrToInt(ViewBag.Cst_Plugin_Items_Count, 1);
                IList<PrizeGift> prizeGift = new List<PrizeGift>(prizeItemCount);
                IList<Double> prizeItems = new List<Double>(prizeItemCount);
                IList<string> prizeItemsName = new List<string>(prizeCount);
                IList<int> prizeAngle = new List<int>(12);
                prizeItemsName.Add("一");
                prizeItemsName.Add("二");
                prizeItemsName.Add("三");
                prizeItemsName.Add("四");
                prizeItemsName.Add("五");
                prizeItemsName.Add("六");
                prizeItemsName.Add("七");
                prizeItemsName.Add("八");
                prizeItemsName.Add("九");
                prizeItemsName.Add("十");
                prizeItemsName.Add("十一");
                prizeItemsName.Add("十二");
                for (int i = 0; i < prizeItemCount; i++)
                {
                    PrizeGift gift = new PrizeGift();
                    gift.ID = i;
                    gift.Name = ViewData.GetString("Cst_Plugin_Items_PrizeName" + i.ToString());
                    gift.Alias_Name = ViewData.GetString("Cst_Plugin_Items_PrizeAliasName" + i.ToString());
                    gift.Odds = ViewData.GetDouble("Cst_Plugin_Items_PrizePercent" + i.ToString(), 0.00) / 100.00;
                    gift.Pic = ViewData.GetString("Cst_Plugin_Items_PrizePic" + i.ToString());
                    gift.Quantity = ViewData.GetInt("Cst_Plugin_Items_PrizeNum" + i.ToString());
                    if (ViewData["Cst_Plugin_Items_PrizeUsedNum" + i.ToString()] != null)
                    {
                        gift.Leavings_Quantity = ViewData.GetInt("Cst_Plugin_Items_PrizeUsedNum" + i.ToString());
                    }
                    else
                    {
                        gift.Leavings_Quantity = gift.Quantity;
                    }
                    gift.Angle = ViewData.GetInt("Cst_Plugin_Items_PrizeAngle" + i.ToString());
                    prizeAngle.Add(gift.Angle);
                    gift.Prize_Name = prizeItemsName[i] + "等奖";
                    if (!string.IsNullOrEmpty(gift.Alias_Name))
                    {
                        gift.Prize_Name = gift.Alias_Name;
                    }
                    gift.Has_Gift = 1;
                    gift.Users = ViewData.GetString("Cst_Plugin_Items_SiteUsers" + i.ToString());
                    gift.UsersName = ViewData.GetString("Cst_Plugin_Items_SiteUsersName" + i.ToString());
                    prizeItems.Add(gift.Odds);
                    prizeGift.Add(gift);
                }
                int averageAngle = 360 / prizeCount;
                Log4NetImpl.Write("抽奖开始：概率与礼物设置");
                if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_Prize_Multi) && ViewBag.Cst_Plugin_Prize_Multi == "1")
                {
                    //多结果奖项
                    prizeGift.Add(new PrizeGift() { ID = prizeItemCount, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 1000000, Angle = 0, Prize_Name = "继续加油" });
                }
                else
                {
                    if (prizeCount - prizeItemCount > 0)
                    {
                        for (int j = prizeItemCount; j < prizeCount; j++)
                        {
                            int angleNoUse = 0;
                            for (int a = 0; a < 12; a++)
                            {
                                IEnumerable<int> angleTemp = prizeAngle.Where(an => an == a * averageAngle);
                                if (angleTemp.Count() > 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    angleNoUse = a * averageAngle;
                                    prizeAngle.Add(angleNoUse);
                                    break;
                                }
                            }
                            prizeGift.Add(new PrizeGift() { ID = j, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = angleNoUse, Prize_Name = "继续加油" });
                            //prizeGift.Add(new PrizeGift() { ID = j, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = 30, Prize_Name = "继续加油" });
                        }
                    }
                }
                prizeItems.OrderBy(d => d);

                //Log4NetImpl.Write("抽奖开始：设置未中奖项");

                int result = -1;
                string resultStr = "";

                pluginUsed.HasUseCount = pluginUsed.HasUseCount - 1;
                _pluginUsedService.Update(pluginUsed);

                IList<PrizeSiteUser> prizeSiteUserList = GetPrizeUsers(Id);

                if (prizeGift.Where(g => g.Has_Gift == 1 && g.Leavings_Quantity > 0).Count() == 0)
                {
                    //return JsonMessage(false, "对不起，已经没有奖品！");
                    return Json(new PrizeGift() { ID = -1, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = 300, Prize_Name = "继续加油" });
                }

                if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_Prize_Multi) && ViewBag.Cst_Plugin_Prize_Multi == "1")
                {
                    if (prizeItems.Count < 3)
                    {
                        return JsonMessage(false, "对不起，系统配置错误，中奖奖项必须3项或以上！");
                    }
                    result = LotteryUtils.LotteryMulti(prizeItems, prizeGift, prizeItems.Count, this.MpUserID, Id, ViewData.GetInt("Cst_Plugin_ZeroPrize"), out resultStr, prizeSiteUserList);
                }
                else
                {
                    result = LotteryUtils.Lottery2(prizeItems, prizeGift, prizeCount, this.MpUserID, Id, ViewData.GetInt("Cst_Plugin_ZeroPrize"), prizeSiteUserList, allowUser.TradeNo);
                }
                //Log4NetImpl.Write("抽奖开始：抽奖");
                if (result == -1)
                {
                    return Json(new PrizeGift() { ID = -1, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = 240, Prize_Name = "继续加油" });
                    //return JsonMessage(false, "对不起，根据系统的配置，已无法再次抽奖！");
                }
                else if (result == -2)
                {
                    return Json(new PrizeGift() { ID = -2, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = 180, Prize_Name = "继续加油" });
                    //return JsonMessage(false, "对不起，根据系统的配置，已无法再次抽奖！");
                }
                else
                {
                    PrizeGift hasPrizeGift = prizeGift[result];
                    if (string.IsNullOrEmpty(ViewBag.Cst_Plugin_Prize_Multi) || ViewBag.Cst_Plugin_Prize_Multi != "1")
                    {
                        hasPrizeGift.Rate_Array = "";
                    }
                    hasPrizeGift.Alias_Name = hasPrizeGift.Alias_Name ?? "";
                    if (hasPrizeGift.Has_Gift == 1 && hasPrizeGift.Leavings_Quantity > 0)
                    {
                        //中奖业务处理
                        PluginResult pluginResult = new PluginResult();
                        pluginResult.Address = "";
                        pluginResult.CreateDate = DateTime.Now;
                        pluginResult.Email = "";
                        pluginResult.IsUse = 0;
                        pluginResult.MobilePhone = "";
                        pluginResult.Name = "";
                        pluginResult.Phone = "";
                        pluginResult.PluginId = Id;
                        pluginResult.MpUserId = this.MpUserID;
                        pluginResult.SN = "";
                        pluginResult.UserName = (string.IsNullOrEmpty(this.Name) ? this.NickName : this.Name);

                        //增加中奖用户列表
                        PrizeSiteUser prizeSiteUser = prizeSiteUserList.Where(u => u.Prize_Index == result).FirstOrDefault();
                        if (prizeSiteUser == null)
                        {
                            prizeSiteUser = new PrizeSiteUser();
                            prizeSiteUser.Prize_Index = result;
                            prizeSiteUser.Users.Add(this.MpUserID);
                            prizeSiteUserList.Add(prizeSiteUser);
                        }
                        else
                        {
                            prizeSiteUser.Users.Add(this.MpUserID);
                        }

                        if (this.MpUserArr != null)
                        {
                            pluginResult.Summary = "恭喜用户[" + (string.IsNullOrEmpty(this.Name) ? this.NickName : this.Name) + "]获得" + hasPrizeGift.Prize_Name + "[" + hasPrizeGift.Name + "]";
                        }
                        else
                        {
                            pluginResult.Summary = "恭喜匿名用户获得" + hasPrizeGift.Prize_Name + "[" + hasPrizeGift.Name + "]";
                        }
                        pluginResult.Value = "<?xml version=\"1.0\" encoding=\"utf-8\"?><PluginSubmit><Cst_Plugin_ItemIndex>" + result.ToString() + "</Cst_Plugin_ItemIndex><Cst_Plugin_PrizeLevel>" + hasPrizeGift.Prize_Name + "</Cst_Plugin_PrizeLevel><Cst_Plugin_PrizeName>" + hasPrizeGift.Name + "</Cst_Plugin_PrizeName></PluginSubmit>";

                        #region 更新中奖数量
                        XmlNode itemsNode = pluginNode.SelectSingleNode("Cst_Plugin_Items");
                        XmlNode node = itemsNode.SelectSingleNode("Cst_Plugin_Items_PrizeUsedNum" + result.ToString());
                        if (node == null)
                        {
                            node = xmlDoc.CreateElement("Cst_Plugin_Items_PrizeUsedNum" + result.ToString());
                            node.InnerText = (hasPrizeGift.Quantity - 1).ToString();
                            itemsNode.AppendChild(node);
                        }
                        else
                        {
                            node.InnerText = (hasPrizeGift.Leavings_Quantity - 1).ToString();
                        }
                        StringWriter sw = new StringWriter();
                        XmlTextWriter tx = new XmlTextWriter(sw);
                        xmlDoc.WriteTo(tx);
                        string strXmlText = sw.ToString();
                        plugin.Value = strXmlText;
                        _pluginService.Update(plugin);
                        #endregion

                        _pluginResultService.Insert(pluginResult);
                        hasPrizeGift.ResultID = pluginResult.Id;
                        Log4NetImpl.Write("抽奖开始：中奖");
                    }
                    //if (usedCount)
                    //{
                    //    pluginUsed.HasUseCount = pluginUsed.HasUseCount - 1;
                    //    _pluginUsedService.Update(pluginUsed);
                    //}
                    return Content(JsonConvert.SerializeObject(hasPrizeGift), "text/javascript");
                }

                //}

                //}

            }
            catch (Exception ex)
            {
                throw new OceanException(ex.Message, ex);
            }
        }

        [HttpPost]
        public ActionResult Lottery3(Guid Id, string t)
        {//测试压力用
            try
            {
                Log4NetImpl.Write("抽奖开始：1");
                if (Id == Guid.Empty)
                {
                    return Json(new { message = "参数有误，请检查！" });
                }

                lock (SequenceLock)
                {
                    Log4NetImpl.Write("SequenceLock");
                    Plugin plugin = _pluginService.GetById(Id);
                    if (plugin == null)
                    {
                        return Json(new { message = "对不起，不存在该插件或未开通，请检查！" });
                    }

                    //设置布局页
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(plugin.Value);
                    XmlNode pluginNode = xmlDoc.SelectSingleNode("plugin");
                    GetDataToViewData(pluginNode);
                    if (ViewBag.Cst_Plugin_Title == null)
                    {
                        return Json(new { message = "对不起，您还未配置或配置错误，请到插件中心进行配置下！" });
                    }
                    if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_StartDate))
                    {
                        DateTime startDate = TypeConverter.StrToDateTime(ViewBag.Cst_Plugin_StartDate.ToString());
                        if (startDate > DateTime.Now)
                        {
                            return Json(new { message = "对不起，抽奖还未开始！" });
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_EndDate))
                            {
                                DateTime endDate = TypeConverter.StrToDateTime(ViewBag.Cst_Plugin_EndDate.ToString());
                                if (endDate < DateTime.Now)
                                {
                                    return Json(new { message = "对不起，抽奖已经结束！" });
                                }
                            }
                        }
                    }

                    PluginUsed pluginUsed = _pluginUsedService.GetUnique("from PluginUsed where MpUserId='" + this.MpUserID.ToString() + "' and PluginId='" + Id.ToString() + "'");
                    bool usedCount = false;
                    if (pluginUsed == null)
                    {
                        pluginUsed = new PluginUsed();
                        pluginUsed.PluginId = Id;
                        pluginUsed.HasUseCount = ViewData.GetInt("Cst_Plugin_UseCount", 1) - 1;
                        pluginUsed.MpUserId = this.MpUserID;
                        pluginUsed.UseDate = DateTime.Now;
                        pluginUsed.CreateDate = DateTime.Now;
                        _pluginUsedService.Insert(pluginUsed);
                    }
                    else
                    {
                        if (pluginUsed.HasUseCount <= 0)
                        {
                            return Json(new { message = "对不起，您已经不能再抽奖！" });
                        }
                        int count = _pluginResultService.GetCount("from PluginResult where pluginId='" + Id.ToString() + "' and MpUserId='" + this.MpUserID.ToString() + "'");
                        if (ViewData.GetInt("Cst_Plugin_ZeroPrize") == 1 && count > 0)
                        {
                            return Json(new { message = "恭喜您，您已经中奖，不能再抽奖！" });
                        }
                        usedCount = true;
                    }
                    //Log4NetImpl.Write("抽奖开始：加载次数限制");
                    int prizeCount = TypeConverter.StrToInt(ViewBag.Cst_Plugin_PrizeCount, 12);
                    int prizeItemCount = TypeConverter.StrToInt(ViewBag.Cst_Plugin_Items_Count, 1);
                    IList<PrizeGift> prizeGift = new List<PrizeGift>(prizeItemCount);
                    IList<Double> prizeItems = new List<Double>(prizeItemCount);
                    IList<string> prizeItemsName = new List<string>(prizeCount);
                    IList<int> prizeAngle = new List<int>(12);
                    prizeItemsName.Add("一");
                    prizeItemsName.Add("二");
                    prizeItemsName.Add("三");
                    prizeItemsName.Add("四");
                    prizeItemsName.Add("五");
                    prizeItemsName.Add("六");
                    prizeItemsName.Add("七");
                    prizeItemsName.Add("八");
                    prizeItemsName.Add("九");
                    prizeItemsName.Add("十");
                    prizeItemsName.Add("十一");
                    prizeItemsName.Add("十二");
                    for (int i = 0; i < prizeItemCount; i++)
                    {
                        PrizeGift gift = new PrizeGift();
                        gift.ID = i;
                        gift.Name = ViewData.GetString("Cst_Plugin_Items_PrizeName" + i.ToString());
                        gift.Alias_Name = ViewData.GetString("Cst_Plugin_Items_PrizeAliasName" + i.ToString());
                        gift.Odds = ViewData.GetDouble("Cst_Plugin_Items_PrizePercent" + i.ToString(), 0.00) / 100.00;
                        gift.Pic = ViewData.GetString("Cst_Plugin_Items_PrizePic" + i.ToString());
                        gift.Quantity = ViewData.GetInt("Cst_Plugin_Items_PrizeNum" + i.ToString());
                        if (ViewData["Cst_Plugin_Items_PrizeUsedNum" + i.ToString()] != null)
                        {
                            gift.Leavings_Quantity = ViewData.GetInt("Cst_Plugin_Items_PrizeUsedNum" + i.ToString());
                        }
                        else
                        {
                            gift.Leavings_Quantity = gift.Quantity;
                        }
                        gift.Angle = ViewData.GetInt("Cst_Plugin_Items_PrizeAngle" + i.ToString());
                        prizeAngle.Add(gift.Angle);
                        gift.Prize_Name = prizeItemsName[i] + "等奖";
                        if (!string.IsNullOrEmpty(gift.Alias_Name))
                        {
                            gift.Prize_Name = gift.Alias_Name;
                        }
                        gift.Has_Gift = 1;
                        gift.Users = ViewData.GetString("Cst_Plugin_Items_SiteUsers" + i.ToString());
                        gift.UsersName = ViewData.GetString("Cst_Plugin_Items_SiteUsersName" + i.ToString());
                        prizeItems.Add(gift.Odds);
                        prizeGift.Add(gift);
                    }
                    int averageAngle = 360 / prizeCount;
                    Log4NetImpl.Write("抽奖开始：概率与礼物设置");
                    if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_Prize_Multi) && ViewBag.Cst_Plugin_Prize_Multi == "1")
                    {
                        //多结果奖项
                        prizeGift.Add(new PrizeGift() { ID = prizeItemCount, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 1000000, Angle = 0, Prize_Name = "继续加油" });
                    }
                    else
                    {
                        if (prizeCount - prizeItemCount > 0)
                        {
                            for (int j = prizeItemCount; j < prizeCount; j++)
                            {
                                int angleNoUse = 0;
                                for (int a = 0; a < 12; a++)
                                {
                                    IEnumerable<int> angleTemp = prizeAngle.Where(an => an == a * averageAngle);
                                    if (angleTemp.Count() > 0)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        angleNoUse = a * averageAngle;
                                        prizeAngle.Add(angleNoUse);
                                        break;
                                    }
                                }
                                prizeGift.Add(new PrizeGift() { ID = j, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = angleNoUse, Prize_Name = "继续加油" });
                                //prizeGift.Add(new PrizeGift() { ID = j, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = 30, Prize_Name = "继续加油" });
                            }
                        }
                    }
                    prizeItems.OrderBy(d => d);

                    //Log4NetImpl.Write("抽奖开始：设置未中奖项");

                    int result = -1;
                    string resultStr = "";

                    pluginUsed.HasUseCount = pluginUsed.HasUseCount - 1;
                    _pluginUsedService.Update(pluginUsed);

                    IList<PrizeSiteUser> prizeSiteUserList = GetPrizeUsers(Id);

                    if (prizeGift.Where(g => g.Has_Gift == 1 && g.Leavings_Quantity > 0).Count() == 0)
                    {
                        //return JsonMessage(false, "对不起，已经没有奖品！");
                        return Json(new PrizeGift() { ID = -1, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = 300, Prize_Name = "继续加油" });
                    }

                    if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_Prize_Multi) && ViewBag.Cst_Plugin_Prize_Multi == "1")
                    {
                        if (prizeItems.Count < 3)
                        {
                            return JsonMessage(false, "对不起，系统配置错误，中奖奖项必须3项或以上！");
                        }
                        result = LotteryUtils.LotteryMulti(prizeItems, prizeGift, prizeItems.Count, this.MpUserID, Id, ViewData.GetInt("Cst_Plugin_ZeroPrize"), out resultStr, prizeSiteUserList);
                    }
                    else
                    {
                        result = LotteryUtils.Lottery2(prizeItems, prizeGift, prizeCount, this.MpUserID, Id, ViewData.GetInt("Cst_Plugin_ZeroPrize"), prizeSiteUserList, 10);
                    }
                    //Log4NetImpl.Write("抽奖开始：抽奖");
                    if (result == -1)
                    {
                        return Json(new PrizeGift() { ID = -1, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = 240, Prize_Name = "继续加油" });
                        //return JsonMessage(false, "对不起，根据系统的配置，已无法再次抽奖！");
                    }
                    else if (result == -2)
                    {
                        return Json(new PrizeGift() { ID = -2, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = 180, Prize_Name = "继续加油" });
                        //return JsonMessage(false, "对不起，根据系统的配置，已无法再次抽奖！");
                    }
                    else
                    {
                        PrizeGift hasPrizeGift = prizeGift[result];
                        if (string.IsNullOrEmpty(ViewBag.Cst_Plugin_Prize_Multi) || ViewBag.Cst_Plugin_Prize_Multi != "1")
                        {
                            hasPrizeGift.Rate_Array = "";
                        }
                        hasPrizeGift.Alias_Name = hasPrizeGift.Alias_Name ?? "";
                        if (hasPrizeGift.Has_Gift == 1 && hasPrizeGift.Leavings_Quantity > 0)
                        {
                            //中奖业务处理
                            PluginResult pluginResult = new PluginResult();
                            pluginResult.Address = "";
                            pluginResult.CreateDate = DateTime.Now;
                            pluginResult.Email = "";
                            pluginResult.IsUse = 0;
                            pluginResult.MobilePhone = "";
                            pluginResult.Name = "";
                            pluginResult.Phone = "";
                            pluginResult.PluginId = Id;
                            pluginResult.MpUserId = this.MpUserID;
                            pluginResult.SN = "";
                            pluginResult.UserName = (string.IsNullOrEmpty(this.Name) ? this.NickName : this.Name);

                            //增加中奖用户列表
                            PrizeSiteUser prizeSiteUser = prizeSiteUserList.Where(u => u.Prize_Index == result).FirstOrDefault();
                            if (prizeSiteUser == null)
                            {
                                prizeSiteUser = new PrizeSiteUser();
                                prizeSiteUser.Prize_Index = result;
                                prizeSiteUser.Users.Add(this.MpUserID);
                                prizeSiteUserList.Add(prizeSiteUser);
                            }
                            else
                            {
                                prizeSiteUser.Users.Add(this.MpUserID);
                            }

                            if (this.MpUserArr != null)
                            {
                                pluginResult.Summary = "恭喜用户[" + (string.IsNullOrEmpty(this.Name) ? this.NickName : this.Name) + "]获得" + hasPrizeGift.Prize_Name + "[" + hasPrizeGift.Name + "]";
                            }
                            else
                            {
                                pluginResult.Summary = "恭喜匿名用户获得" + hasPrizeGift.Prize_Name + "[" + hasPrizeGift.Name + "]";
                            }
                            pluginResult.Value = "<?xml version=\"1.0\" encoding=\"utf-8\"?><PluginSubmit><Cst_Plugin_ItemIndex>" + result.ToString() + "</Cst_Plugin_ItemIndex><Cst_Plugin_PrizeLevel>" + hasPrizeGift.Prize_Name + "</Cst_Plugin_PrizeLevel><Cst_Plugin_PrizeName>" + hasPrizeGift.Name + "</Cst_Plugin_PrizeName></PluginSubmit>";

                            #region 更新中奖数量
                            XmlNode itemsNode = pluginNode.SelectSingleNode("Cst_Plugin_Items");
                            XmlNode node = itemsNode.SelectSingleNode("Cst_Plugin_Items_PrizeUsedNum" + result.ToString());
                            if (node == null)
                            {
                                node = xmlDoc.CreateElement("Cst_Plugin_Items_PrizeUsedNum" + result.ToString());
                                node.InnerText = (hasPrizeGift.Quantity - 1).ToString();
                                itemsNode.AppendChild(node);
                            }
                            else
                            {
                                node.InnerText = (hasPrizeGift.Leavings_Quantity - 1).ToString();
                            }
                            StringWriter sw = new StringWriter();
                            XmlTextWriter tx = new XmlTextWriter(sw);
                            xmlDoc.WriteTo(tx);
                            string strXmlText = sw.ToString();
                            plugin.Value = strXmlText;
                            _pluginService.Update(plugin);
                            #endregion

                            _pluginResultService.Insert(pluginResult);
                            hasPrizeGift.ResultID = pluginResult.Id;
                            Log4NetImpl.Write("抽奖开始：中奖");
                        }
                        return Content(JsonConvert.SerializeObject(hasPrizeGift), "text/javascript");
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetImpl.Write(ex.Message);
                throw new OceanException(ex.Message, ex);
            }
        }
        #endregion
        #endregion

        public class LotteryUtils
        {
            private static Dictionary<Guid, Random> randomList = new Dictionary<Guid, Random>();
            private static Random randomin = new Random();
            /// <summary>
            /// 获取当前站点的抽奖器
            /// </summary>
            /// <param name="siteID"></param>
            /// <returns></returns>
            public static Random GetRandom(Guid pluginID)
            {
                Random random;
                if (randomList.Count > 0)
                {
                    if (randomList.TryGetValue(pluginID, out random))
                    {
                        return random;
                    }
                    else
                    {
                        random = new Random(unchecked((int)DateTime.Now.Ticks));
                        randomList[pluginID] = random;
                        return random;
                    }
                }
                else
                {
                    random = new Random(unchecked((int)DateTime.Now.Ticks));
                    randomList[pluginID] = random;
                    return random;
                }
            }
            public static int Lottery(IList<Double> orignalRates, IList<PrizeGift> prizeGifts, int ratesTotal, Guid userId, Guid pluginId, int zeroPrize, IList<PrizeSiteUser> prizeSiteUserList)
            {
                if (orignalRates == null || orignalRates.Count == 0)
                {
                    return -1;
                }
                int size = orignalRates.Count();
                List<Double> sortOrignalRates = new List<Double>(size);
                sortOrignalRates.AddRange(orignalRates.Where(d => d > 0d));
                //获取大于0的概率最小值
                double minOdds = sortOrignalRates.Min();

                #region 计算概率总数-
                string[] totalArr = ((decimal)minOdds).ToString().Split('.');
                int len = totalArr.Length;//长度,ToString("G")对于<1的多个小数点无作用，使用decimal]
                if (len == 1)
                {
                    len = 2;
                }
                else
                {

                    len = totalArr[1].Length < 2 ? 2 : totalArr[1].Length; //最小100里去随机
                }
                string prizeTotalStr = "1";
                for (int i = 0; i < len; i++)
                {
                    prizeTotalStr += 0;
                }
                int prizeTotal = TypeConverter.StrToInt(prizeTotalStr, int.MaxValue);//概率值总数
                #endregion

                #region 得到概率值列表
                List<int> rates = new List<int>(ratesTotal);
                int noPrize = ratesTotal - orignalRates.Count;
                int noPrizeValue = 0;
                int maxRateValue = 0;
                for (int x = 0; x < orignalRates.Count; x++)
                {
                    int rate = TypeConverter.StrToInt((prizeTotal * orignalRates[x]).ToString());
                    maxRateValue += rate;
                    if (rate == 0)
                    {
                        rates.Add(rate);
                    }
                    else
                    {
                        rates.Add(maxRateValue);
                    }
                    if (x == orignalRates.Count - 1)
                    {
                        noPrizeValue = prizeTotal - maxRateValue;
                    }
                }
                if (noPrize > 0)
                {
                    int noPrizeAverage = noPrizeValue / noPrize;//剩余未中奖奖项、平均分配概率
                    for (int y = 0; y < noPrize; y++)
                    {
                        if (y == noPrize - 1)
                        {
                            noPrizeAverage += noPrizeValue % noPrize;
                        }
                        maxRateValue += noPrizeAverage;
                        rates.Add(maxRateValue);
                    }
                }
                #endregion

                #region 抽奖
                int result = -1;
                #region 设置固定中奖者
                bool randomTrue = true;//设置固定中奖者

                int isUserCount = 0;
                foreach (PrizeGift gift in prizeGifts)
                {
                    if (!string.IsNullOrEmpty(gift.Users) && gift.Users != ",")
                    {
                        if (gift.Users.IndexOf("," + userId.ToString() + ",") >= 0)//中奖者用户
                        {
                            int rate = rates[gift.ID];
                            int index = rates.IndexOf(rate);
                            gift.Rate = rate;
                            result = index;
                            PrizeSiteUser prizeSiteUser = prizeSiteUserList.Where(u => u.Prize_Index == index && (u.Users != null && u.Users.Where(_u => _u == userId).Count() > 0)).FirstOrDefault();//该用户还未中奖
                            foreach (PrizeSiteUser tempUser in prizeSiteUserList)
                            {
                                Log4NetImpl.Write("prizeSiteUserList-" + tempUser.Prize_Index.ToString() + ":" + string.Join(",", tempUser.Users.Select(u => u).ToArray()));
                            }
                            if (prizeSiteUser == null)
                            {
                                prizeSiteUser = new PrizeSiteUser();
                                prizeSiteUser.Prize_Index = index;
                                prizeSiteUser.Users.Add(userId);
                                prizeSiteUserList.Add(prizeSiteUser);//添加中奖列表
                                randomTrue = false;//不随机了
                            }
                            break;
                        }
                        else
                        {
                            isUserCount++;
                        }
                    }
                }
                #endregion
                ////当全部设置了中奖者时-避免死循环
                #region 手动创建未中奖奖项
                //if (isUserCount == prizeGifts.Where(g => g.Has_Gift == 1).Count())
                //{
                //    if (noPrize > 0)
                //    {
                //        prizeTotal = rates[rates.Count - 1];//最大值
                //    }
                //    else
                //    {
                //        if (isUserCount == rates.Count) //默认为12项奖项
                //        {
                //            noPrize = (12 - rates.Count);
                //            int noPrizeAverage = prizeTotal /noPrize;//剩余未中奖奖项、平均分配概率
                //            for (int y = 0; y < noPrize; y++)
                //            {
                //                if (y == noPrize - 1)
                //                {
                //                    noPrizeAverage += prizeTotal % noPrize;
                //                    prizeTotal = maxRateValue + noPrizeAverage;//未中奖奖项
                //                }
                //                maxRateValue += noPrizeAverage;
                //                rates.Add(maxRateValue);
                //                if (prizeGifts.Count == isUserCount)
                //                {
                //                    for (int a = 0; a < 12; a++)
                //                    {
                //                        IEnumerable<int> angleTemp = prizeGifts.Where(an => an == a * averageAngle);
                //                        if (angleTemp.Count() > 0)
                //                        {
                //                            continue;
                //                        }
                //                        else
                //                        {
                //                            angleNoUse = a * averageAngle;
                //                            prizeAngle.Add(angleNoUse);
                //                            break;
                //                        }
                //                    }
                //                    prizeGift.Add(new PrizeGift() { ID = j, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = angleNoUse, Prize_Name = "继续加油" });
                //                }
                //            }
                //        }
                //    }
                //}
                #endregion

                int randomCount = 0;
                while (randomTrue && randomCount < 5)
                {
                    Log4NetImpl.Write("抽奖进行循环模式：" + randomCount.ToString());
                    Random random = GetRandom(pluginId);
                    int randomValue = random.Next(1, prizeTotal + 1);
                    #region //第5次直接设置为未中奖并退出
                    if (randomCount == 4)
                    {
                        IList<PrizeGift> noGifts = prizeGifts.Where(g => g.Has_Gift == 0).ToList();
                        if (noGifts.Count > 0)
                        {
                            //从未中奖项随机取出一组
                            int noPrizeIndex = randomin.Next(0, noGifts.Count());
                            //得到奖项图片索引
                            prizeGifts[noPrizeIndex].Rate = randomValue;
                            randomTrue = false;
                            result = noGifts[noPrizeIndex].ID;//索引即ID
                        }
                        else //直接提醒配置问题
                        {
                            result = -2;
                            randomTrue = false;
                        }
                        break;
                    }
                    #endregion
                    foreach (int rate in rates)
                    {
                        if (randomValue > rate)
                        {
                            continue;
                        }
                        result = rate;
                        break;
                    }
                    if (result >= 0)
                    {
                        int index = rates.IndexOf(result);
                        prizeGifts[index].Rate = randomValue;
                        result = index;
                        if (zeroPrize == 1 && prizeGifts[index].Has_Gift == 1 && prizeGifts[index].Leavings_Quantity == 0)
                        {
                            //剩余0不再中奖-奖概率重新分配
                            prizeGifts[index].Odds = 0.00d;
                            ////全部都等0时-或者，设置固定中奖者-已抽中的个数
                            //var zeroCount = prizeGifts.Where(g => g.Leavings_Quantity == 0).Count();
                            var zeroCount = 0;
                            for (int _z = 0; _z < prizeGifts.Count(); _z++)
                            {
                                PrizeSiteUser prizeSiteUser = prizeSiteUserList.Where(u => u.Prize_Index == prizeGifts[_z].ID).FirstOrDefault();
                                if (prizeGifts[_z].Leavings_Quantity == 0 || (!string.IsNullOrEmpty(prizeGifts[_z].Users) && prizeSiteUser != null && prizeSiteUser.Users.Count > 0 && ((prizeGifts[_z].Users.Split(',').Length - 2) - prizeSiteUser.Users.GroupBy(u => u).Where(u => prizeGifts[_z].Users.Contains(u.Key.ToString())).Count()) >= prizeGifts[_z].Leavings_Quantity))
                                {
                                    zeroCount++;
                                }

                            }
                            if (zeroCount == prizeGifts.Count)
                            {
                                IList<PrizeGift> noGifts = prizeGifts.Where(g => g.Has_Gift == 0).ToList();
                                if (noGifts.Count > 0)
                                {
                                    //从未中奖项随机取出一组
                                    int noPrizeIndex = randomin.Next(0, noGifts.Count() + 1);
                                    //得到奖项图片索引
                                    prizeGifts[noPrizeIndex].Rate = randomValue;
                                    randomTrue = false;
                                    result = noGifts[noPrizeIndex].ID;//索引即ID
                                }
                                else
                                {
                                    result = -2;
                                    randomTrue = false;
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(prizeGifts[index].Users) && prizeGifts[index].Users != ",")
                            {
                                if (prizeGifts[index].Users.IndexOf("," + userId.ToString() + ",") >= 0)
                                {//如果当前用户是固定中奖者，则直接中奖
                                    randomTrue = false;
                                }
                                else
                                {//如果不是固定中奖者
                                    PrizeSiteUser prizeSiteUser = prizeSiteUserList.Where(u => u.Prize_Index == index).FirstOrDefault();
                                    string[] users = prizeGifts[index].Users.Split(',');
                                    if ((users.Length - 2) < prizeGifts[index].Leavings_Quantity)
                                    {//如果不是固定中奖者，但是已经中奖，并且有足够余留奖项则中奖
                                        randomTrue = false;
                                    }
                                    else
                                    {//没有足够余留奖项
                                        if (prizeSiteUser != null && prizeSiteUser.Users != null && prizeSiteUser.Users.Count > 0)
                                        {//如果已经有中了此奖的人
                                            if (((users.Length - 2) - prizeSiteUser.Users.Where(u => users.Contains(u.ToString())).Count()) < prizeGifts[index].Leavings_Quantity || prizeGifts[index].Leavings_Quantity <= 0)
                                            {//固定中奖者 - 已经中奖的固定中奖者个数  < 余留奖项，或者没有余留奖项
                                                randomTrue = false;
                                            }
                                            else
                                            {
                                                ////当全部设置了中奖者时||处理中奖奖项数量是否已完-避免死循环
                                                var zeroCount = 0;//prizeGifts.Where(g => g.Leavings_Quantity == 0).Count();
                                                for (int _z = 0; _z < prizeGifts.Count(); _z++)
                                                {
                                                    PrizeSiteUser prizeSiteUserTemp = prizeSiteUserList.Where(u => u.Prize_Index == prizeGifts[_z].ID).FirstOrDefault();
                                                    if (prizeGifts[_z].Leavings_Quantity == 0 || (!string.IsNullOrEmpty(prizeGifts[_z].Users) && prizeSiteUserTemp != null && prizeSiteUserTemp.Users.Count > 0 && ((prizeGifts[_z].Users.Split(',').Length - 2) - prizeSiteUserTemp.Users.GroupBy(u => u).Where(u => prizeGifts[_z].Users.Contains(u.Key.ToString())).Count()) >= prizeGifts[_z].Leavings_Quantity))
                                                    {
                                                        zeroCount++;
                                                    }

                                                }
                                                if (zeroCount == prizeGifts.Count)
                                                {
                                                    IList<PrizeGift> noGifts = prizeGifts.Where(g => g.Has_Gift == 0).ToList();
                                                    if (noGifts.Count > 0)
                                                    {
                                                        //从未中奖项随机取出一组
                                                        int noPrizeIndex = randomin.Next(0, noGifts.Count() + 1);
                                                        //得到奖项图片索引
                                                        prizeGifts[noPrizeIndex].Rate = randomValue;
                                                        randomTrue = false;
                                                        result = noGifts[noPrizeIndex].ID;//索引即ID
                                                    }
                                                    else //直接提醒配置问题
                                                    {
                                                        result = -2;
                                                        randomTrue = false;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {//该奖项还没有人中
                                            ////当全部设置了中奖者时||处理中奖奖项数量是否已完-避免死循环
                                            var zeroCount = 0;//prizeGifts.Where(g => g.Leavings_Quantity == 0).Count();
                                            for (int _z = 0; _z < prizeGifts.Count(); _z++)
                                            {
                                                PrizeSiteUser prizeSiteUserTemp = prizeSiteUserList.Where(u => u.Prize_Index == prizeGifts[_z].ID).FirstOrDefault();
                                                if (prizeGifts[_z].Leavings_Quantity == 0 || (!string.IsNullOrEmpty(prizeGifts[_z].Users) && prizeSiteUserTemp != null && prizeSiteUserTemp.Users.Count > 0 && ((prizeGifts[_z].Users.Split(',').Length - 2) - prizeSiteUserTemp.Users.GroupBy(u => u).Where(u => prizeGifts[_z].Users.Contains(u.Key.ToString())).Count()) >= prizeGifts[_z].Leavings_Quantity))
                                                {
                                                    zeroCount++;
                                                }

                                            }
                                            if (zeroCount == prizeGifts.Count)
                                            {
                                                IList<PrizeGift> noGifts = prizeGifts.Where(g => g.Has_Gift == 0).ToList();
                                                if (noGifts.Count > 0)
                                                {
                                                    //从未中奖项随机取出一组
                                                    int noPrizeIndex = randomin.Next(0, noGifts.Count() + 1);
                                                    //得到奖项图片索引
                                                    prizeGifts[noPrizeIndex].Rate = randomValue;
                                                    randomTrue = false;
                                                    result = noGifts[noPrizeIndex].ID;//索引即ID
                                                }
                                                else //直接提醒配置问题
                                                {
                                                    result = -2;
                                                    randomTrue = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                randomTrue = false;
                            }
                        }
                    }
                    randomCount++;
                }
                #endregion

                return result;
            }

            public static int Lottery2(IList<Double> orignalRates, IList<PrizeGift> prizeGifts, int ratesTotal, Guid userId, Guid pluginId, int zeroPrize, IList<PrizeSiteUser> prizeSiteUserList, int tradeNo)
            {
                if (orignalRates == null || orignalRates.Count == 0)
                {
                    return -1;
                }
                int size = orignalRates.Count();
                List<Double> sortOrignalRates = new List<Double>(size);
                sortOrignalRates.AddRange(orignalRates.Where(d => d > 0d));
                //获取大于0的概率最小值
                double minOdds = sortOrignalRates.Min();

                #region 计算概率总数-
                string[] totalArr = ((decimal)minOdds).ToString().Split('.');
                int len = totalArr.Length;//长度,ToString("G")对于<1的多个小数点无作用，使用decimal]
                if (len == 1)
                {
                    len = 2;
                }
                else
                {

                    len = totalArr[1].Length < 2 ? 2 : totalArr[1].Length; //最小100里去随机
                }
                string prizeTotalStr = "1";
                for (int i = 0; i < len; i++)
                {
                    prizeTotalStr += 0;
                }
                int prizeTotal = TypeConverter.StrToInt(prizeTotalStr, int.MaxValue);//概率值总数
                #endregion

                #region 得到概率值列表
                List<int> rates = new List<int>(ratesTotal);
                int noPrize = ratesTotal - orignalRates.Count;
                int noPrizeValue = 0;
                int maxRateValue = 0;
                for (int x = 0; x < orignalRates.Count; x++)
                {
                    int rate = TypeConverter.StrToInt((prizeTotal * orignalRates[x]).ToString());
                    maxRateValue += rate;
                    if (rate == 0)
                    {
                        rates.Add(rate);
                    }
                    else
                    {
                        rates.Add(maxRateValue);
                    }
                    if (x == orignalRates.Count - 1)
                    {
                        noPrizeValue = prizeTotal - maxRateValue;
                    }
                }
                if (noPrize > 0)
                {
                    int noPrizeAverage = noPrizeValue / noPrize;//剩余未中奖奖项、平均分配概率
                    for (int y = 0; y < noPrize; y++)
                    {
                        if (y == noPrize - 1)
                        {
                            noPrizeAverage += noPrizeValue % noPrize;
                        }
                        maxRateValue += noPrizeAverage;
                        rates.Add(maxRateValue);
                    }
                }
                #endregion

                #region 抽奖
                int result = -1;
                #region 设置固定中奖者
                bool randomTrue = true;//设置固定中奖者

                int isUserCount = 0;
                foreach (PrizeGift gift in prizeGifts)
                {
                    if (!string.IsNullOrEmpty(gift.Users) && gift.Users != ",")
                    {
                        if (gift.Users.IndexOf("," + userId.ToString() + ",") >= 0)//中奖者用户
                        {
                            int rate = rates[gift.ID];
                            int index = rates.IndexOf(rate);
                            gift.Rate = rate;
                            result = index;
                            PrizeSiteUser prizeSiteUser = prizeSiteUserList.Where(u => u.Prize_Index == index && (u.Users != null && u.Users.Where(_u => _u == userId).Count() > 0)).FirstOrDefault();//该用户还未中奖
                            foreach (PrizeSiteUser tempUser in prizeSiteUserList)
                            {
                                Log4NetImpl.Write("prizeSiteUserList-" + tempUser.Prize_Index.ToString() + ":" + string.Join(",", tempUser.Users.Select(u => u).ToArray()));
                            }
                            if (prizeSiteUser == null)
                            {
                                prizeSiteUser = new PrizeSiteUser();
                                prizeSiteUser.Prize_Index = index;
                                prizeSiteUser.Users.Add(userId);
                                prizeSiteUserList.Add(prizeSiteUser);//添加中奖列表
                                randomTrue = false;//不随机了
                            }
                            break;
                        }
                        else
                        {
                            isUserCount++;
                        }
                    }
                }
                #endregion

                int randomCount = 0;
                while (randomTrue && randomCount < 5)
                {
                    Log4NetImpl.Write("抽奖进行循环模式：" + randomCount.ToString());
                    Random random = GetRandom(pluginId);
                    int randomValue = random.Next(1, prizeTotal + 1);
                    #region //第5次直接设置为未中奖并退出
                    if (randomCount == 4)
                    {
                        IList<PrizeGift> noGifts = prizeGifts.Where(g => g.Has_Gift == 0).ToList();
                        if (noGifts.Count > 0)
                        {
                            //从未中奖项随机取出一组
                            int noPrizeIndex = randomin.Next(0, noGifts.Count());
                            //得到奖项图片索引
                            prizeGifts[noPrizeIndex].Rate = randomValue;
                            randomTrue = false;
                            result = noGifts[noPrizeIndex].ID;//索引即ID
                        }
                        else //直接提醒配置问题
                        {
                            result = -2;
                            randomTrue = false;
                        }
                        break;
                    }
                    #endregion
                    foreach (int rate in rates)
                    {
                        if (randomValue > rate)
                        {
                            continue;
                        }
                        result = rate;
                        break;
                    }
                    if (result >= 0)
                    {
                        int index = rates.IndexOf(result);
                        prizeGifts[index].Rate = randomValue;
                        result = index;
                        if (result == 0 && tradeNo < 6)//仅仅交易笔数达到6笔才可以中一等奖
                        {
                            Log4NetImpl.Write("中了一等奖，但是交易笔数不够！" + userId.ToString());
                            randomCount++;
                            continue;
                        }
                        if (zeroPrize == 1 && prizeGifts[index].Has_Gift == 1 && prizeGifts[index].Leavings_Quantity == 0)
                        {//如果中奖之后不再中奖，并且现在中的奖有礼品，但是礼品用完了
                            //剩余0不再中奖-奖概率重新分配
                            prizeGifts[index].Odds = 0.00d;
                            ////全部都等0时-或者，设置固定中奖者-已抽中的个数
                            //var zeroCount = prizeGifts.Where(g => g.Leavings_Quantity == 0).Count();
                            var zeroCount = 0;
                            for (int _z = 0; _z < prizeGifts.Count(); _z++)
                            {
                                PrizeSiteUser prizeSiteUser = prizeSiteUserList.Where(u => u.Prize_Index == prizeGifts[_z].ID).FirstOrDefault();
                                if (prizeGifts[_z].Leavings_Quantity == 0 || (!string.IsNullOrEmpty(prizeGifts[_z].Users) && prizeSiteUser != null && prizeSiteUser.Users.Count > 0 && ((prizeGifts[_z].Users.Split(',').Length - 2) - prizeSiteUser.Users.GroupBy(u => u).Where(u => prizeGifts[_z].Users.Contains(u.Key.ToString())).Count()) >= prizeGifts[_z].Leavings_Quantity))
                                {//奖项剩余数目等于0，或者该奖项的所有固定中奖者 - 该奖项的已中奖的固定中奖者 >= 奖项剩余数目(不够分)
                                    zeroCount++;
                                }

                            }
                            //遍历所有的奖项，如果所有的奖项都不够用了，那么直接让他不中奖！
                            //如果不相等，那么表示 没有用完那么让他再抽一下其它奖项
                            if (zeroCount == prizeGifts.Count)
                            {
                                IList<PrizeGift> noGifts = prizeGifts.Where(g => g.Has_Gift == 0).ToList();
                                if (noGifts.Count > 0)
                                {
                                    //从未中奖项随机取出一组
                                    int noPrizeIndex = randomin.Next(0, noGifts.Count() + 1);
                                    //得到奖项图片索引
                                    prizeGifts[noPrizeIndex].Rate = randomValue;
                                    randomTrue = false;
                                    result = noGifts[noPrizeIndex].ID;//索引即ID
                                }
                                else
                                {
                                    result = -2;
                                    randomTrue = false;
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(prizeGifts[index].Users) && prizeGifts[index].Users != ",")
                            {
                                if (prizeGifts[index].Users.IndexOf("," + userId.ToString() + ",") >= 0)
                                {//如果当前用户是固定中奖者，则直接中奖
                                    randomTrue = false;
                                }
                                else
                                {//如果当前用户不是固定中奖者
                                    PrizeSiteUser prizeSiteUser = prizeSiteUserList.Where(u => u.Prize_Index == index).FirstOrDefault();
                                    string[] users = prizeGifts[index].Users.Split(',');
                                    if ((users.Length - 2) < prizeGifts[index].Leavings_Quantity)
                                    {//如果不是固定中奖者，但是已经中奖，并且有足够余留奖项则中奖
                                        randomTrue = false;
                                    }
                                    else
                                    {//没有足够余留奖项
                                        if (prizeSiteUser != null && prizeSiteUser.Users != null && prizeSiteUser.Users.Count > 0)
                                        {//如果已经有中了此奖的人
                                            if (((users.Length - 2) - prizeSiteUser.Users.Where(u => users.Contains(u.ToString())).Count()) < prizeGifts[index].Leavings_Quantity || prizeGifts[index].Leavings_Quantity <= 0)
                                            {//该奖项的所有固定中奖者 - 该奖项的已中奖的固定中奖者  < 余留奖项，或者没有余留奖项（外边会提示 没有奖品了？）
                                                randomTrue = false;
                                            }
                                            else
                                            {
                                                ////当全部设置了中奖者时||处理中奖奖项数量是否已完-避免死循环
                                                var zeroCount = 0;//prizeGifts.Where(g => g.Leavings_Quantity == 0).Count();
                                                for (int _z = 0; _z < prizeGifts.Count(); _z++)
                                                {
                                                    PrizeSiteUser prizeSiteUserTemp = prizeSiteUserList.Where(u => u.Prize_Index == prizeGifts[_z].ID).FirstOrDefault();
                                                    if (prizeGifts[_z].Leavings_Quantity == 0 || (!string.IsNullOrEmpty(prizeGifts[_z].Users) && prizeSiteUserTemp != null && prizeSiteUserTemp.Users.Count > 0 && ((prizeGifts[_z].Users.Split(',').Length - 2) - prizeSiteUserTemp.Users.GroupBy(u => u).Where(u => prizeGifts[_z].Users.Contains(u.Key.ToString())).Count()) >= prizeGifts[_z].Leavings_Quantity))
                                                    {
                                                        zeroCount++;
                                                    }

                                                }
                                                if (zeroCount == prizeGifts.Count)
                                                {
                                                    IList<PrizeGift> noGifts = prizeGifts.Where(g => g.Has_Gift == 0).ToList();
                                                    if (noGifts.Count > 0)
                                                    {
                                                        //从未中奖项随机取出一组
                                                        int noPrizeIndex = randomin.Next(0, noGifts.Count() + 1);
                                                        //得到奖项图片索引
                                                        prizeGifts[noPrizeIndex].Rate = randomValue;
                                                        randomTrue = false;
                                                        result = noGifts[noPrizeIndex].ID;//索引即ID
                                                    }
                                                    else //直接提醒配置问题
                                                    {
                                                        result = -2;
                                                        randomTrue = false;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {//该奖项是固定中奖者，但该奖项还没有人中，当前用户不是固定中奖者
                                            ////当全部设置了中奖者时||处理中奖奖项数量是否已完-避免死循环
                                            var zeroCount = 0;//prizeGifts.Where(g => g.Leavings_Quantity == 0).Count();
                                            for (int _z = 0; _z < prizeGifts.Count(); _z++)
                                            {
                                                PrizeSiteUser prizeSiteUserTemp = prizeSiteUserList.Where(u => u.Prize_Index == prizeGifts[_z].ID).FirstOrDefault();
                                                if (prizeGifts[_z].Leavings_Quantity == 0 || (!string.IsNullOrEmpty(prizeGifts[_z].Users) && prizeSiteUserTemp != null && prizeSiteUserTemp.Users.Count > 0 && ((prizeGifts[_z].Users.Split(',').Length - 2) - prizeSiteUserTemp.Users.GroupBy(u => u).Where(u => prizeGifts[_z].Users.Contains(u.Key.ToString())).Count()) >= prizeGifts[_z].Leavings_Quantity))
                                                {
                                                    zeroCount++;
                                                }

                                            }
                                            if (zeroCount == prizeGifts.Count)
                                            {
                                                IList<PrizeGift> noGifts = prizeGifts.Where(g => g.Has_Gift == 0).ToList();
                                                if (noGifts.Count > 0)
                                                {
                                                    //从未中奖项随机取出一组
                                                    int noPrizeIndex = randomin.Next(0, noGifts.Count() + 1);
                                                    //得到奖项图片索引
                                                    prizeGifts[noPrizeIndex].Rate = randomValue;
                                                    randomTrue = false;
                                                    result = noGifts[noPrizeIndex].ID;//索引即ID
                                                }
                                                else //直接提醒配置问题
                                                {
                                                    result = -2;
                                                    randomTrue = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                randomTrue = false;
                            }
                        }
                    }
                    randomCount++;
                }
                #endregion

                return result;
            }

            /// <summary>
            /// 多组概率相乘
            /// </summary>
            /// <param name="orignalRates">设置概率组</param>
            /// <param name="prizeGifts">奖项信息</param>
            /// <param name="ratesTotal"></param>
            /// <param name="siteID"></param>
            /// <param name="siteUserId"></param>
            /// <param name="pluginId"></param>
            /// <param name="zeroPrize"></param>
            /// <returns></returns>
            public static int LotteryMulti(IList<Double> orignalRates, IList<PrizeGift> prizeGifts, int ratesTotal, Guid userId, Guid pluginId, int zeroPrize, out string resultStr, IList<PrizeSiteUser> prizeSiteUserList)
            {
                resultStr = "";
                int result = -1;
                try
                {
                    if (orignalRates == null || orignalRates.Count == 0)
                    {
                        return result;
                    }

                    int size = orignalRates.Count();
                    List<Double> sortOrignalRates = new List<Double>(size);
                    sortOrignalRates.AddRange(orignalRates.Where(d => d > 0d));
                    //获取大于0的概率最小值
                    double minOdds = sortOrignalRates.Min();

                    #region 计算概率总数-
                    string[] totalArr = ((decimal)minOdds).ToString().Split('.');
                    int len = totalArr.Length;//长度,ToString("G")对于<1的多个小数点无作用，使用decimal]
                    if (len == 1)
                    {
                        len = 2;
                    }
                    else
                    {

                        len = totalArr[1].Length < 2 ? 2 : totalArr[1].Length; //最小100里去随机
                    }
                    string prizeTotalStr = "1";
                    for (int i = 0; i < len; i++)
                    {
                        prizeTotalStr += 0;
                    }
                    int prizeTotal = TypeConverter.StrToInt(prizeTotalStr, int.MaxValue);//概率值总数
                    #endregion

                    #region 得到概率值列表
                    List<int> rates = new List<int>(ratesTotal + 1);
                    int maxRateValue = 0;
                    for (int x = 0; x < orignalRates.Count; x++)
                    {
                        int rate = TypeConverter.StrToInt((prizeTotal * orignalRates[x]).ToString());
                        maxRateValue += rate;
                        if (rate == 0)
                        {
                            rates.Add(0);
                        }
                        else
                        {
                            rates.Add(maxRateValue);
                        }
                    }
                    //增加一项未中奖奖项
                    rates.Add(prizeTotal);
                    #endregion

                    #region 从最多12项中算出组合
                    int[] combinationArr = new int[orignalRates.Count];
                    for (int c = 0; c < orignalRates.Count; c++)
                    {
                        combinationArr[c] = c;
                    }
                    List<int[]> combinations = PermutationAndCombination<int>.GetCombination(combinationArr, 3);//得到组合
                    #endregion

                    #region 提取组合中三个相同的项-并得到索引
                    List<int[]> combinationRates = combinations.Where(c => c[0] == c[1] && c[0] == c[2]).ToList();//获取奖项的组合
                    combinationRates = combinationRates.OrderBy(c => c[0]).ToList();
                    for (int i = 0; i < combinationRates.Count; i++)
                    {
                        combinations.Remove(combinationRates[i]);//删除奖项组合
                    }
                    #endregion

                    #region 抽奖
                    bool randomTrue = true;//设置固定中奖者
                    foreach (PrizeGift gift in prizeGifts)
                    {
                        if (!string.IsNullOrEmpty(gift.Users) && gift.Users != ",")
                        {
                            if (gift.Users.IndexOf("," + userId.ToString() + ",") >= 0)//中奖者用户
                            {
                                int rate = rates[gift.ID];
                                int index = rates.IndexOf(rate);
                                gift.Rate = rate;
                                result = index;
                                PrizeSiteUser prizeSiteUser = prizeSiteUserList.Where(u => u.Prize_Index == index && (u.Users != null && u.Users.Where(_u => _u == userId).Count() > 0)).FirstOrDefault();//该用户还未中奖
                                if (prizeSiteUser == null)
                                {
                                    prizeSiteUser = new PrizeSiteUser();
                                    prizeSiteUser.Prize_Index = index;
                                    prizeSiteUser.Users.Add(userId);
                                    prizeSiteUserList.Add(prizeSiteUser);//添加中奖列表
                                    randomTrue = false;//不随机了
                                }
                                break;
                            }
                        }
                    }
                    int randomCount = 0;
                    while (randomTrue && randomCount < 5)
                    {
                        Log4NetImpl.Write("抽奖进行循环模式：" + randomCount.ToString());
                        Random random = GetRandom(pluginId);
                        int randomValue = random.Next(1, prizeTotal + 1);
                        if (randomCount == 4) //超过五次自动退出，设置为不中奖
                        {
                            //从未中奖项随机取出一组
                            int noPrizeIndex = randomin.Next(0, combinations.Count + 1);
                            //得到奖项图片索引
                            resultStr = string.Join<int>(",", combinations[noPrizeIndex].AsEnumerable());
                            prizeGifts[ratesTotal].Rate = randomValue;
                            prizeGifts[ratesTotal].Rate_Array = resultStr;
                            randomTrue = false;
                            result = combinationRates.Count;
                            break;
                        }
                        foreach (int rate in rates)
                        {
                            if (randomValue > rate)
                            {
                                continue;
                            }
                            result = rate;
                            break;
                        }
                        if (result >= 0)
                        {
                            int index = rates.IndexOf(result);
                            if (index < combinationRates.Count)
                            {
                                prizeGifts[index].Rate = randomValue;
                                //得到奖项图片索引
                                resultStr = string.Join<int>(",", combinationRates[index].AsEnumerable());
                                prizeGifts[index].Rate_Array = resultStr;
                                result = index;
                                if (zeroPrize == 1 && prizeGifts[index].Has_Gift == 1 && (prizeGifts[index].Leavings_Quantity == 0))
                                {
                                    //剩余0不再中奖-奖概率重新分配-未存储
                                    prizeGifts[index].Odds = 0.00d;
                                    //全部都等0时-或者，设置固定中奖者-已抽中的个数
                                    //var zeroCount =prizeGifts.Where(g => g.Leavings_Quantity == 0).Count();
                                    var zeroCount = 0;
                                    for (int _z = 0; _z < prizeGifts.Count(); _z++)
                                    {
                                        PrizeSiteUser prizeSiteUser = prizeSiteUserList.Where(u => u.Prize_Index == prizeGifts[_z].ID).FirstOrDefault();
                                        if (prizeGifts[_z].Leavings_Quantity == 0 || (!string.IsNullOrEmpty(prizeGifts[_z].Users) && prizeSiteUser != null && prizeSiteUser.Users.Count > 0 && ((prizeGifts[_z].Users.Split(',').Length - 2) - prizeSiteUser.Users.GroupBy(u => u).Where(u => prizeGifts[_z].Users.Contains(u.Key.ToString())).Count()) >= prizeGifts[_z].Leavings_Quantity))
                                        {
                                            zeroCount++;
                                        }

                                    }
                                    if (zeroCount == prizeGifts.Count)
                                    {
                                        //result = -2;
                                        //randomTrue = false;
                                        //从未中奖项随机取出一组
                                        int noPrizeIndex = randomin.Next(0, combinations.Count + 1);
                                        //得到奖项图片索引
                                        resultStr = string.Join<int>(",", combinations[noPrizeIndex].AsEnumerable());
                                        prizeGifts[ratesTotal].Rate = randomValue;
                                        prizeGifts[ratesTotal].Rate_Array = resultStr;
                                        randomTrue = false;
                                        result = combinationRates.Count;
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(prizeGifts[index].Users) && prizeGifts[index].Users != ",")
                                    {
                                        if (prizeGifts[index].Users.IndexOf("," + userId.ToString() + ",") >= 0)
                                        {
                                            randomTrue = false;
                                        }
                                        else
                                        {
                                            //得到当前中奖索引的固定中奖用户
                                            PrizeSiteUser prizeSiteUser = prizeSiteUserList.Where(u => u.Prize_Index == index).FirstOrDefault();
                                            string[] users = prizeGifts[index].Users.Split(',');
                                            if ((users.Length - 2) < prizeGifts[index].Leavings_Quantity) //数量大于设置的人数，可中奖
                                            {
                                                randomTrue = false;
                                            }
                                            else
                                            {
                                                if (prizeSiteUser != null && prizeSiteUser.Users != null && prizeSiteUser.Users.Count > 0) //
                                                {
                                                    if (((users.Length - 2) - prizeSiteUser.Users.GroupBy(u => u).Where(u => users.Contains(u.Key.ToString())).Count()) < prizeGifts[index].Leavings_Quantity || prizeGifts[index].Leavings_Quantity <= 0)
                                                    {
                                                        randomTrue = false;
                                                    }
                                                    else
                                                    {
                                                        //处理中奖奖项数量是否已完
                                                        var zeroCount = 0;//prizeGifts.Where(g => g.Leavings_Quantity == 0).Count();
                                                        for (int _z = 0; _z < prizeGifts.Count(); _z++)
                                                        {
                                                            PrizeSiteUser prizeSiteUserTemp = prizeSiteUserList.Where(u => u.Prize_Index == prizeGifts[_z].ID).FirstOrDefault();
                                                            if (prizeGifts[_z].Leavings_Quantity == 0 || (!string.IsNullOrEmpty(prizeGifts[_z].Users) && prizeSiteUserTemp != null && prizeSiteUserTemp.Users.Count > 0 && ((prizeGifts[_z].Users.Split(',').Length - 2) - prizeSiteUserTemp.Users.GroupBy(u => u).Where(u => prizeGifts[_z].Users.Contains(u.Key.ToString())).Count()) >= prizeGifts[_z].Leavings_Quantity))
                                                            {
                                                                zeroCount++;
                                                            }

                                                        }
                                                        if (zeroCount == prizeGifts.Count)
                                                        {
                                                            //从未中奖项随机取出一组
                                                            int noPrizeIndex = randomin.Next(0, combinations.Count + 1);
                                                            //得到奖项图片索引
                                                            resultStr = string.Join<int>(",", combinations[noPrizeIndex].AsEnumerable());
                                                            prizeGifts[ratesTotal].Rate = randomValue;
                                                            prizeGifts[ratesTotal].Rate_Array = resultStr;
                                                            randomTrue = false;
                                                            result = combinationRates.Count;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    //处理中奖奖项数量是否已完
                                                    var zeroCount = 0;//prizeGifts.Where(g => g.Leavings_Quantity == 0).Count();
                                                    for (int _z = 0; _z < prizeGifts.Count(); _z++)
                                                    {
                                                        PrizeSiteUser prizeSiteUserTemp = prizeSiteUserList.Where(u => u.Prize_Index == prizeGifts[_z].ID).FirstOrDefault();
                                                        if (prizeGifts[_z].Leavings_Quantity == 0 || (!string.IsNullOrEmpty(prizeGifts[_z].Users) && prizeSiteUserTemp != null && prizeSiteUserTemp.Users.Count > 0 && ((prizeGifts[_z].Users.Split(',').Length - 2) - prizeSiteUserTemp.Users.GroupBy(u => u).Where(u => prizeGifts[_z].Users.Contains(u.Key.ToString())).Count()) >= prizeGifts[_z].Leavings_Quantity))
                                                        {
                                                            zeroCount++;
                                                        }

                                                    }
                                                    if (zeroCount == prizeGifts.Count)
                                                    {
                                                        //从未中奖项随机取出一组
                                                        int noPrizeIndex = randomin.Next(0, combinations.Count + 1);
                                                        //得到奖项图片索引
                                                        resultStr = string.Join<int>(",", combinations[noPrizeIndex].AsEnumerable());
                                                        prizeGifts[ratesTotal].Rate = randomValue;
                                                        prizeGifts[ratesTotal].Rate_Array = resultStr;
                                                        randomTrue = false;
                                                        result = combinationRates.Count;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        randomTrue = false;
                                    }
                                }
                            }
                            else
                            {
                                //从未中奖项随机取出一组
                                int noPrizeIndex = randomin.Next(0, combinations.Count + 1);
                                //得到奖项图片索引
                                resultStr = string.Join<int>(",", combinations[noPrizeIndex].AsEnumerable());
                                prizeGifts[ratesTotal].Rate = randomValue;
                                prizeGifts[ratesTotal].Rate_Array = resultStr;
                                randomTrue = false;
                                result = combinationRates.Count;
                            }
                        }
                        else
                        {
                            //从未中奖项随机取出一组
                            int noPrizeIndex = randomin.Next(0, combinations.Count + 1);
                            //得到奖项图片索引
                            resultStr = string.Join<int>(",", combinations[noPrizeIndex].AsEnumerable());
                            prizeGifts[ratesTotal].Rate = randomValue;
                            prizeGifts[ratesTotal].Rate_Array = resultStr;
                            randomTrue = false;
                            result = combinationRates.Count;
                        }
                        randomCount++;
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    throw new OceanException(ex.Message, ex);
                }
                return result;
            }


            #region 排列组合算法-排列实现了，组合也就实现了组合C(N,R)就是P(N,R)/P(R,R)
            /// <summary>
            /// 排列循环方法
            /// </summary>
            /// <param name="N"></param>
            /// <param name="R"></param>
            /// <returns></returns>
            static long P1(int N, int R)
            {
                if (R > N || R <= 0 || N <= 0) throw new OceanException("参数有误!");
                long t = 1;
                int i = N;
                while (i != N - R)
                {
                    try
                    {
                        checked
                        {
                            t *= i;
                        }
                    }
                    catch
                    {
                        throw new OceanException("超出长整型最大值!");
                    }
                    --i;

                }
                return t;
            }
            /// <summary>
            /// 排列堆栈方法
            /// </summary>
            /// <param name="N"></param>
            /// <param name="R"></param>
            /// <returns></returns>
            public static long P2(int N, int R)
            {
                if (R > N || R <= 0 || N <= 0) throw new OceanException("参数有误!");
                Stack<int> s = new Stack<int>();
                long iRlt = 1;
                int t;
                s.Push(N);
                while ((t = s.Peek()) != N - R)
                {
                    try
                    {
                        checked
                        {
                            iRlt *= t;
                        }
                    }
                    catch
                    {
                        throw new OceanException("超出长整型最大值!");
                    }
                    s.Pop();
                    s.Push(t - 1);
                }
                return iRlt;
            }
            /// <summary>
            /// 组合
            /// </summary>
            /// <param name="N"></param>
            /// <param name="R"></param>
            /// <returns></returns>
            public static long C(int N, int R)
            {
                return P1(N, R) / P1(R, R);
            }
            public static List<string> GetCombinationF3(string[] data, int count)
            {
                Dictionary<string, int> dic = new Dictionary<string, int>();
                List<string> output = new List<string>();
                for (int i = 0; i < data.Length; i++)
                {
                    dic.Add(data[i], i);
                }
                SelectN(dic, data, count, 1, ref output);
                return output;
            }

            public static void SelectN(Dictionary<string, int> dd, string[] data, int count, int times, ref List<string> output)
            {
                Dictionary<string, int> dic = new Dictionary<string, int>();

                foreach (KeyValuePair<string, int> kv in dd)
                {
                    for (int i = kv.Value + 1; i < data.Length; i++)
                    {
                        if (times < count - 1)
                        {
                            dic.Add(kv.Key + "/t" + data[i], i);
                        }
                        else
                        {
                            output.Add(kv.Key + "/t" + data[i]);//不考虑输出，将此句注释掉  
                        }
                    }
                }
                times++;
                if (dic.Count > 0) SelectN(dic, data, count, times, ref output);
            }


            public static List<string> GetCombinationF1(string[] set, int m)
            {
                int n = set.Length;
                int min = (0x01 << m) - 1;//00111111  
                int max = min << (n - m);//11111100  
                int j;
                int k;
                List<string> output = new List<string>();
                string s;

                for (int i = min; i <= max; i++)
                {
                    j = 0;
                    k = i;
                    while (k > 0)
                    {
                        j += (int)(k & 0x01);
                        k >>= 1;
                        if (j > m)
                        {
                            break;
                        }
                    }
                    if (j == m)
                    {
                        s = "";
                        k = 0x01;
                        for (int l = n - 1; l >= 0; l--)
                        {
                            if ((k & i) == k)
                            {
                                s += set[l] + "/t";
                            }
                            k <<= 1;
                        }
                        output.Add(s);
                    }

                }
                return output;
            }


            public static List<string> GetCombinationF2(string[] strArray, int selectCount)
            {
                int totalCount = strArray.Length;
                int[] currentSelect = new int[selectCount];
                int last = selectCount - 1;
                List<string> output = new List<string>();
                string s;

                //付初始值  
                for (int i = 0; i < selectCount; i++)
                    currentSelect[i] = i;

                while (true)
                {
                    s = "";
                    //输出部分,生成的时候从0计数,所以输出的时候+1  
                    for (int i = 0; i < selectCount; i++)
                    {
                        s += strArray[currentSelect[i]] + "/t";
                    }
                    output.Add(s);

                    //如果不进位  
                    if (currentSelect[last] < totalCount - 1)
                        currentSelect[last]++;
                    else
                    {
                        //进位部分  
                        int position = last;

                        while (position > 0 && currentSelect[position - 1] == currentSelect[position] - 1)
                            position--;

                        if (position == 0)
                            break;

                        currentSelect[position - 1]++;

                        for (int i = position; i < selectCount; i++)
                            currentSelect[i] = currentSelect[i - 1] + 1;
                    }
                }
                return output;
            }

            public static List<string> GetCombinationF4(string[] data, int count)
            {
                List<string> output = new List<string>();
                int len = data.Length;
                string start = "1".PadRight(count, '1').PadRight(len, '0');
                string s;
                while (start != string.Empty)
                {
                    s = "";
                    for (int i = 0; i < len; i++)
                        if (start[i] == '1') s += data[i] + "/t";
                    output.Add(s);
                    start = GetNext(start);
                }
                return output;

            }

            public static string GetNext(string str)
            {
                string next = string.Empty;
                int pos = str.IndexOf("10");
                if (pos < 0) return next;
                else if (pos == 0) return "01" + str.Substring(2);
                else
                {
                    int len = str.Length;
                    next = str.Substring(0, pos).Replace("0", "").PadRight(pos, '0') + "01";
                    if (pos < len - 2) next += str.Substring(pos + 2);
                }
                return next;
            }
            #endregion

        }
        #region 排名组合算法
        public class PermutationAndCombination<T>
        {
            /// <summary>
            /// 交换两个变量
            /// </summary>
            /// <param name="a">变量1</param>
            /// <param name="b">变量2</param>
            public static void Swap(ref T a, ref T b)
            {
                T temp = a;
                a = b;
                b = temp;
            }

            /// <summary>
            /// 递归算法求数组的组合(私有成员)
            /// </summary>
            /// <param name="list">返回的范型</param>
            /// <param name="t">所求数组</param>
            /// <param name="n">辅助变量</param>
            /// <param name="m">辅助变量</param>
            /// <param name="b">辅助数组</param>
            /// <param name="M">辅助变量M</param>
            private static void GetCombination(ref List<T[]> list, T[] t, int n, int m, int[] b, int M)
            {
                for (int i = t.Length; i >= 1; i--)
                {
                    b[m - 1] = i - 1;
                    if (m > 1)
                    {
                        GetCombination(ref list, t, i, m - 1, b, M);
                    }
                    else
                    {
                        if (list == null)
                        {
                            list = new List<T[]>();
                        }
                        T[] temp = new T[M];
                        for (int j = 0; j < b.Length; j++)
                        {
                            temp[j] = t[b[j]];
                        }
                        list.Add(temp);
                    }
                }
                //for (int i = n; i >= m; i--)
                //{
                //    b[m - 1] = i - 1;
                //    if (m > 1)
                //    {
                //        GetCombination(ref list, t, i - 1, m - 1, b, M);
                //    }
                //    else
                //    {
                //        if (list == null)
                //        {
                //            list = new List<T[]>();
                //        }
                //        T[] temp = new T[M];
                //        for (int j = 0; j < b.Length; j++)
                //        {
                //            temp[j] = t[b[j]];
                //        }
                //        list.Add(temp);
                //    }
                //}
            }

            /// <summary>
            /// 递归算法求排列(私有成员)
            /// </summary>
            /// <param name="list">返回的列表</param>
            /// <param name="t">所求数组</param>
            /// <param name="startIndex">起始标号</param>
            /// <param name="endIndex">结束标号</param>
            private static void GetPermutation(ref List<T[]> list, T[] t, int startIndex, int endIndex)
            {
                if (startIndex == endIndex)
                {
                    if (list == null)
                    {
                        list = new List<T[]>();
                    }
                    T[] temp = new T[t.Length];
                    t.CopyTo(temp, 0);
                    list.Add(temp);
                }
                else
                {
                    for (int i = startIndex; i <= endIndex; i++)
                    {
                        Swap(ref t[startIndex], ref t[i]);
                        GetPermutation(ref list, t, startIndex + 1, endIndex);
                        Swap(ref t[startIndex], ref t[i]);
                    }
                }
            }

            /// <summary>
            /// 求从起始标号到结束标号的排列，其余元素不变
            /// </summary>
            /// <param name="t">所求数组</param>
            /// <param name="startIndex">起始标号</param>
            /// <param name="endIndex">结束标号</param>
            /// <returns>从起始标号到结束标号排列的范型</returns>
            public static List<T[]> GetPermutation(T[] t, int startIndex, int endIndex)
            {
                if (startIndex < 0 || endIndex > t.Length - 1)
                {
                    return null;
                }
                List<T[]> list = new List<T[]>();
                GetPermutation(ref list, t, startIndex, endIndex);
                return list;
            }

            /// <summary>
            /// 返回数组所有元素的全排列
            /// </summary>
            /// <param name="t">所求数组</param>
            /// <returns>全排列的范型</returns>
            public static List<T[]> GetPermutation(T[] t)
            {
                return GetPermutation(t, 0, t.Length - 1);
            }

            /// <summary>
            /// 求数组中n个元素的排列
            /// </summary>
            /// <param name="t">所求数组</param>
            /// <param name="n">元素个数</param>
            /// <returns>数组中n个元素的排列</returns>
            public static List<T[]> GetPermutation(T[] t, int n)
            {
                if (n > t.Length)
                {
                    return null;
                }
                List<T[]> list = new List<T[]>();
                List<T[]> c = GetCombination(t, n);
                for (int i = 0; i < c.Count; i++)
                {
                    List<T[]> l = new List<T[]>();
                    GetPermutation(ref l, c[i], 0, n - 1);
                    list.AddRange(l);
                }
                return list;
            }


            /// <summary>
            /// 求数组中n个元素的组合
            /// </summary>
            /// <param name="t">所求数组</param>
            /// <param name="n">元素个数</param>
            /// <returns>数组中n个元素的组合的范型</returns>
            public static List<T[]> GetCombination(T[] t, int n)
            {
                if (t.Length < n)
                {
                    return null;
                }
                int[] temp = new int[n];
                List<T[]> list = new List<T[]>();
                GetCombination(ref list, t, t.Length, n, temp, n);
                return list;
            }
            //int[] arr = new int[6];
            //for (int i = 0; i < arr.Length; i++)
            //{
            //    arr[i] = i + 1;
            //}
            ////求排列
            //List<int[]> lst_Permutation = Algorithms.PermutationAndCombination<int>.GetPermutation(arr, 3);
            ////求组合
            //List<int[]> lst_Combination = Algorithms.PermutationAndCombination<int>.GetCombination(arr, 3);

        }
        #endregion
    }
}