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
using Ocean.Core.Data.DynamicQuery;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace Ocean.Web.Controllers
{
    public class XYPluginsController : WebBaseController
    {
        private readonly IPluginService _pluginService;
        private readonly IPluginSceneResultService _pluginSceneResultService;
        private readonly IXYPluginUserService _xyPluginUserService;
        public XYPluginsController(IXYPluginUserService xyPluginUserService
            , IPluginService pluginService
            , IPluginSceneResultService pluginSceneResultService)
        {
            _pluginService = pluginService;
            _pluginSceneResultService = pluginSceneResultService;
            _xyPluginUserService = xyPluginUserService;
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
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
                else if (propNode.Name.Trim().ToLower() == "cst_plugin_guaitems")
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


        #endregion


        #region 刮验证码
        [HttpGet]
        public ActionResult PluginScratch(Guid Id)
        {
            try
            {
                TempData["OpenID"] = "";
                if (TempData["OpenID"].ToString() == "")
                {
                    string code = RQuery["code"];
                    string state = RQuery["state"];
                    string refUrl = RQuery["refUrl"];
                    string openid = RQuery["openid"];
                    if (code == "")
                    {
                        Log4NetImpl.Write("PluginScratch:redirectUrl");
                        string redirectUrl = "http://wx.ssrcb.com/xyplugins/PluginScratch?id=" + Id.ToString();
                        string reUrl = (OAuth.GetAuthorizeUrl(MpCenterCache.AppID, redirectUrl, "STATE", OAuthScope.snsapi_base));
                        Response.Redirect(reUrl);
                        return View("Scratch");
                    }
                    if (string.IsNullOrEmpty(openid) || openid == "")
                    {
                        OAuthAccessTokenResult authResult = OAuth.GetAccessToken(MpCenterCache.AppID, MpCenterCache.AppSecret, code);
                        openid = authResult.openid;
                    }
                    TempData["OpenID"] = openid;
                }
                ViewBag.OpenID = TempData["OpenID"].ToString();
                //页面上的Plugins
                IList<Plugin> plugins = _pluginService.GetALL();
                ViewBag.ContentPlugins = plugins;
                Plugin curPlugin = plugins.Where(p => p.Id == Id).FirstOrDefault();
                ViewBag.CurPlugin = curPlugin;
                if (curPlugin == null)
                {
                    throw new OceanException("对不起，不存在该插件或未开通，请检查！");
                }
                ViewBag.Title = curPlugin.RPluginBase.Name + "";
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(curPlugin.Value);
                XmlNode pluginNode = xmlDoc.SelectSingleNode("plugin");
                GetDataToViewData(pluginNode);

                return View("Scratch");
            }
            catch (Exception ex)
            {
                throw new OceanException(ex.Message, ex);
            }
        }

        [HttpPost]
        public ActionResult ScratchVerifyCode(Guid Id, string t, string OpenID)
        {
            try
            {
                if (Id == Guid.Empty)
                {
                    return Json(new { message = "参数有误，请检查！" });
                }
                
                Plugin plugin = _pluginService.GetById(Id);
                if (plugin == null)
                {
                    return Json(new { isOK = false, error_code = "ERR_PluginNotExits" });//message = "对不起，不存在该插件或未开通，请检查！" });
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(plugin.Value);
                XmlNode pluginNode = xmlDoc.SelectSingleNode("plugin");
                GetDataToViewData(pluginNode);
                if (ViewBag.Cst_Plugin_Title == null)
                {
                    return Json(new { isOK = false, error_code = "ERR_PluginNotExits" });// message = "对不起，您还未配置或配置错误，请到插件中心进行配置下！" });
                }
                if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_StartDate))
                {
                    DateTime startDate = TypeConverter.StrToDateTime(ViewBag.Cst_Plugin_StartDate.ToString());
                    if (startDate > DateTime.Now)
                    {
                        return Json(new { isOK = false, error_code = "ERR_PluginNotStart" });// message = "对不起，抽奖还未开始！" });
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_EndDate))
                        {
                            DateTime endDate = TypeConverter.StrToDateTime(ViewBag.Cst_Plugin_EndDate.ToString());
                            if (endDate < DateTime.Now)
                            {
                                return Json(new { isOK = false, error_code = "ERR_PluginIsEnd" });// message = "对不起，抽奖已经结束！" });
                            }
                        }
                    }
                }

                XYPluginUser xyPluginUser = _xyPluginUserService.GetUnique("from XYPluginUser where OpenId='" + OpenID + "'");
                if (xyPluginUser == null)
                {
                    return Json(new { isOK = false, isVertfy = true, error_code = "ERR_UserNotVerify" });
                }
                else if (xyPluginUser.GuaGuaKa == 0)
                {
                    return Json(new { isOK = false, isVertfy = true, error_code = "ERR_UserNotChance" });
                }

                int prizeCount = TypeConverter.StrToInt(ViewBag.Cst_Plugin_GuaPrizeCount, 12);
                int prizeItemCount = TypeConverter.StrToInt(ViewBag.Cst_Plugin_GuaItems_Count, 1);
                if (prizeCount != prizeItemCount)
                {
                    Log4NetImpl.Write("奖项设置不正确");
                    return Json(new { isOK = false, error_code = "ERR_ArgNotExist" });
                }
                IList<PrizeGift> prizeGift = new List<PrizeGift>(prizeItemCount);
                IList<Double> prizeItems = new List<Double>(prizeItemCount);
                IList<string> prizeItemsName = new List<string>(prizeCount);
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
                    gift.Name = ViewData.GetString("Cst_Plugin_GuaItems_PrizeName" + i.ToString());
                    gift.Alias_Name = ViewData.GetString("Cst_Plugin_GuaItems_PrizeAliasName" + i.ToString());
                    gift.Odds = ViewData.GetDouble("Cst_Plugin_GuaItems_PrizePercent" + i.ToString(), 0.00) / 100.00;
                    gift.Pic = ViewData.GetString("Cst_Plugin_GuaItems_PrizePic" + i.ToString());
                    gift.Quantity = ViewData.GetInt("Cst_Plugin_GuaItems_PrizeNum" + i.ToString());
                    if (ViewData["Cst_Plugin_GuaItems_PrizeUsedNum" + i.ToString()] != null)
                    {
                        gift.Leavings_Quantity = ViewData.GetInt("Cst_Plugin_GuaItems_PrizeUsedNum" + i.ToString());
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
                    gift.Has_Gift = ViewData.GetInt("Cst_Plugin_GuaItems_Has_Gift" + i.ToString());//>1为奖品，2为刮刮卡
                    prizeItems.Add(gift.Odds);
                    prizeGift.Add(gift);
                }
                int averageAngle = 360 / prizeCount;

                prizeItems.OrderBy(d => d);

                int result = -1;

                xyPluginUser.GuaGuaKa = xyPluginUser.GuaGuaKa - 1;
                _xyPluginUserService.Update(xyPluginUser);

                int count = _pluginSceneResultService.GetCount("from SceneResult where pluginId='" + Id.ToString() + "' and MpUserId='" + xyPluginUser.Id + "'");

                if (prizeGift.Where(g => g.Has_Gift == 1 && g.Leavings_Quantity > 0).Count() == 0)
                {
                    return Json(new { isOK = true, error_code = "ERR_RunOutOfGift" });
                }

                result = LotteryUtils.Lottery(prizeItems, prizeGift, prizeCount, xyPluginUser.Id, Id);


                if (result == -1)
                {
                    return Json(new { isOK = true, error_code = "ERR_RunOutOfGift" });
                }
                else if (result == -2)
                {
                    return Json(new { isOK = true, error_code = "ERR_RunOutOfGift" });// Json(new PrizeGift() { ID = -2, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = 180, Prize_Name = "继续加油" });

                }
                else if (result == -3)
                {
                    return Json(new { isOK = true, error_code = "ERR_RunOutOfGift" });// Json(new PrizeGift() { ID = -2, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = 180, Prize_Name = "继续加油" });

                }
                else
                {
                    PrizeGift hasPrizeGift = prizeGift[result];

                    hasPrizeGift.Alias_Name = hasPrizeGift.Alias_Name ?? "";
                    if (hasPrizeGift.Has_Gift == 1 && hasPrizeGift.Leavings_Quantity > 0)
                    {
                        //中奖业务处理
                        PluginSceneResult pluginSceneResult = new PluginSceneResult();
                        pluginSceneResult.Address = "";
                        pluginSceneResult.CreateDate = DateTime.Now;
                        pluginSceneResult.Email = "";
                        pluginSceneResult.IsUse = 0;
                        pluginSceneResult.MobilePhone = "";
                        pluginSceneResult.Name = "";
                        pluginSceneResult.Phone = "";
                        pluginSceneResult.PluginId = Id;
                        pluginSceneResult.MpUserId = xyPluginUser.Id;
                        pluginSceneResult.SN = "";
                        pluginSceneResult.UserName = (string.IsNullOrEmpty(this.Name) ? this.NickName : this.Name);

                        
                        if (this.MpUserArr != null)
                        {
                            pluginSceneResult.Summary = "恭喜用户[" + (string.IsNullOrEmpty(this.Name) ? this.NickName : this.Name) + "]获得" + hasPrizeGift.Prize_Name + "[" + hasPrizeGift.Name + "]";
                        }
                        else
                        {
                            pluginSceneResult.Summary = "恭喜用户获得" + hasPrizeGift.Prize_Name + "[" + hasPrizeGift.Name + "]";
                        }
                        pluginSceneResult.Value = "<?xml version=\"1.0\" encoding=\"utf-8\"?><PluginSubmit><Cst_Plugin_ItemIndex>" + result.ToString() + "</Cst_Plugin_ItemIndex><Cst_Plugin_PrizeLevel>" + hasPrizeGift.Prize_Name + "</Cst_Plugin_PrizeLevel><Cst_Plugin_PrizeName>" + hasPrizeGift.Name + "</Cst_Plugin_PrizeName></PluginSubmit>";

                        #region 更新中奖数量
                        XmlNode itemsNode = pluginNode.SelectSingleNode("Cst_Plugin_GuaItems");
                        XmlNode node = itemsNode.SelectSingleNode("Cst_Plugin_GuaItems_PrizeUsedNum" + result.ToString());
                        if (node == null)
                        {
                            node = xmlDoc.CreateElement("Cst_Plugin_GuaItems_PrizeUsedNum" + result.ToString());
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

                        _pluginSceneResultService.Insert(pluginSceneResult);
                        hasPrizeGift.ResultID = pluginSceneResult.Id;
                    }

                    return Content(JsonConvert.SerializeObject(hasPrizeGift), "text/javascript");
                }
                
            }
            catch (Exception ex)
            {
                throw new OceanException(ex.Message, ex);
            }
        }

        #endregion

        #region 用户验证
        public ActionResult VerifyUserSubmit(Guid pluginId, string CardNo, string TradeSeqNo, string OpenID)
        {
            //string OpenID = TempData["OpenID"].ToString();
            if (OpenID == "")
            {
                return Json(new { isOK = false, error_code = "ERR_NotLogin" }); //JsonMessage(false, "你还未关注本行微信或者未登陆！");
            }
            else
            {
                if (pluginId == Guid.Empty)
                {
                    return Json(new { isOK = false, error_code = "ERR_ArgNotExist" }); //JsonMessage(false, "参数有误，请检查！");
                }
                else
                {
                    try
                    {
                        XYPluginUser xyPluginUser = _xyPluginUserService.GetUnique("from XYPluginUser where OpenId='" + OpenID + "'");
                        if (xyPluginUser != null)
                        {
                            return Json(new { isOK = false, error_code = "ERR_AlreadyVerify" });
                        }
                        xyPluginUser = new XYPluginUser();
                        xyPluginUser.OpenId = OpenID;
                        xyPluginUser.CardNo = CardNo;
                        xyPluginUser.TradeSeqNo = TradeSeqNo;
                        xyPluginUser.GuaGuaKa = 0;
                        xyPluginUser.LeftCount = 1;

                        _xyPluginUserService.Insert(xyPluginUser);
                    }
                    catch (Exception ex)
                    {
                        Log4NetImpl.Write("VerifyUserSubmit" + ex.Message);
                        return Json(new { isOK = false, error_code = "ERR_UnKnown" }); //JsonMessage(false, "对不起，出现异常，请联系我行工作人员！");
                    }
                }
                return Json(new { isOK = true, error_code = "ERR_OK" }); //JsonMessage(true, "恭喜您，可以摇奖了!");
            }

        }
        #endregion

        [HttpGet]
        public ActionResult GetSeqNo()
        {
            return View("GetSeqNo");
        }
        #region 仙游抽奖
        [HttpGet]
        public ActionResult PluginsLottery(Guid Id)
        {
            try
            {
                if (Id == Guid.Empty)
                {
                    Log4NetImpl.Write("PluginsLottery:参数有误");
                    throw new OceanException("参数有误，请检查！");
                }
                TempData["OpenID"] = "";
                if (TempData["OpenID"].ToString() == "")
                {
                    string code = RQuery["code"];
                    string state = RQuery["state"];
                    string refUrl = RQuery["refUrl"];
                    string openid = RQuery["openid"];
                    if (code == "")
                    {
                        string redirectUrl = "http://wx.ssrcb.com/xyplugins/PluginsLottery?id=" + Id.ToString();
                        string reUrl = (OAuth.GetAuthorizeUrl(MpCenterCache.AppID, redirectUrl, "STATE", OAuthScope.snsapi_base));
                        Response.Redirect(reUrl);
                        return View("Lottery");
                    }
                    if (string.IsNullOrEmpty(openid) || openid == "")
                    {
                        OAuthAccessTokenResult authResult = OAuth.GetAccessToken(MpCenterCache.AppID, MpCenterCache.AppSecret, code);
                        openid = authResult.openid;
                    }
                    TempData["OpenID"] = openid;
                }
                //页面上的Plugins
                IList<Plugin> plugins = _pluginService.GetALL();
                ViewBag.ContentPlugins = plugins;
                Plugin curPlugin = plugins.Where(p => p.Name.Contains("仙游")).FirstOrDefault();
                ViewBag.CurPlugin = curPlugin;
                ViewBag.OpenID = TempData["OpenID"].ToString();
                if (curPlugin == null)
                {
                    throw new OceanException("对不起，不存在该插件或未开通，请检查！");
                }
                ViewBag.Title = curPlugin.RPluginBase.Name;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(curPlugin.Value);
                XmlNode pluginNode = xmlDoc.SelectSingleNode("plugin");
                GetDataToViewData(pluginNode);

                ViewBag.LeaveCount = 0;
                XYPluginUser xyPluginUser = _xyPluginUserService.GetUnique("from XYPluginUser where OpenId='" + OpenID + "'");
                Guid mpID = Guid.Empty;
                if (xyPluginUser != null)
                {
                    ViewBag.LeaveCount = xyPluginUser.LeftCount;
                    mpID = xyPluginUser.Id;
                }

                IList<PluginSceneResult> pluginSceneResultList = _pluginSceneResultService.GetList("from SceneResult where PluginId='" + Id.ToString() + "' and MpUserId='" + mpID.ToString() + "'");
                ViewBag.PluginResultList = pluginSceneResultList;

                ViewBag.MpUserArr = this.MpUserArr;
                ViewBag.MpUserID = mpID;

                return View("Lottery");
            }
            catch (Exception ex)
            {
                Log4NetImpl.Write("PluginsLottery:" + ex.Message);
                throw new OceanException(ex.Message, ex);
            }
        }

        [HttpPost]
        public ActionResult PluginsLottery(Guid Id, string t, string OpenID)
        {
            try
            {
                if (Id == Guid.Empty)
                {
                    Log4NetImpl.Write("PluginsLottery:ajax-ERR_ArgNotExist" );
                    return Json(new { isOK = false, error_code = "ERR_ArgNotExist" });
                }

                Plugin plugin = _pluginService.GetById(Id);
                if (plugin == null)
                {
                    return Json(new { isOK= false, error_code = "ERR_PluginNotExits"});
                }

                //设置布局页
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(plugin.Value);
                XmlNode pluginNode = xmlDoc.SelectSingleNode("plugin");
                GetDataToViewData(pluginNode);
                if (ViewBag.Cst_Plugin_Title == null)
                {
                    return Json(new { isOK= false, error_code = "ERR_PluginNotExits"});// message = "对不起，您还未配置或配置错误，请到插件中心进行配置下！" });
                }
                if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_StartDate))
                {
                    DateTime startDate = TypeConverter.StrToDateTime(ViewBag.Cst_Plugin_StartDate.ToString());
                    if (startDate > DateTime.Now)
                    {
                        return Json(new { isOK = false, error_code = "ERR_PluginNotStart" });// message = "对不起，抽奖还未开始！" });
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(ViewBag.Cst_Plugin_EndDate))
                        {
                            DateTime endDate = TypeConverter.StrToDateTime(ViewBag.Cst_Plugin_EndDate.ToString());
                            if (endDate < DateTime.Now)
                            {
                                return Json(new { isOK = false, error_code = "ERR_PluginIsEnd" });// message = "对不起，抽奖已经结束！" });
                            }
                        }
                    }
                }

                XYPluginUser xyPluginUser = _xyPluginUserService.GetUnique("from XYPluginUser where OpenId='" + OpenID + "'");
                if (xyPluginUser == null)
                {
                    Log4NetImpl.Write("PluginsLottery:ajax-ERR_UserNotVerify");
                    return Json(new { isOK = false, isVertfy = true, error_code = "ERR_UserNotVerify" });
                }

                if (xyPluginUser.LeftCount <= 0)
                {
                    Log4NetImpl.Write("PluginsLottery:ajax-ERR_UserUseOutChance");
                    return Json(new { isOK = false, error_code = "ERR_UserUseOutChance" });
                }
                

                int prizeCount = TypeConverter.StrToInt(ViewBag.Cst_Plugin_PrizeCount, 12);
                int prizeItemCount = TypeConverter.StrToInt(ViewBag.Cst_Plugin_Items_Count, 1);
                if (prizeCount != prizeItemCount)
                {
                    Log4NetImpl.Write("PluginsLottery:ajax-奖项设置不正确");
                    return Json(new { isOK = false, error_code = "ERR_ArgNotExist" });
                }
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
                    gift.Has_Gift = ViewData.GetInt("Cst_Plugin_Items_Has_Gift" + i.ToString());//>1为奖品，2为刮刮卡
                    gift.Users = ViewData.GetString("Cst_Plugin_Items_SiteUsers" + i.ToString());
                    gift.UsersName = ViewData.GetString("Cst_Plugin_Items_SiteUsersName" + i.ToString());
                    prizeItems.Add(gift.Odds);
                    prizeGift.Add(gift);
                }
                int averageAngle = 360 / prizeCount;

                prizeItems.OrderBy(d => d);

                int result = -1;

                xyPluginUser.LeftCount = xyPluginUser.LeftCount - 1;
                _xyPluginUserService.Update(xyPluginUser);

                int count = _pluginSceneResultService.GetCount("from SceneResult where pluginId='" + Id.ToString() + "' and MpUserId='" + xyPluginUser.Id + "'");
                if (ViewData.GetInt("Cst_Plugin_ZeroPrize") == 1 && count > 0)
                {
                    Log4NetImpl.Write("PluginsLottery:ajax-ERR_RunOutOfGift");
                    return Json(new { isOK = true, error_code = "ERR_RunOutOfGift" });
                }
                if (prizeGift.Where(g => g.Has_Gift > 1 && g.Leavings_Quantity > 0).Count() == 0)
                {
                    Log4NetImpl.Write("PluginsLottery:ajax-ERR_RunOutOfGift");
                    return Json(new { isOK = true, error_code = "ERR_RunOutOfGift" });
                }

                result = LotteryUtils.Lottery(prizeItems, prizeGift, prizeCount, xyPluginUser.Id, Id);


                if (result == -1)
                {
                    return Json(new { isOK = true, error_code = "ERR_RunOutOfGift" });
                }
                else if (result == -2)
                {
                    return Json(new { isOK = true, error_code = "ERR_RunOutOfGift" });// Json(new PrizeGift() { ID = -2, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = 180, Prize_Name = "继续加油" });

                }
                else if (result == -3)
                {
                    return Json(new { isOK = true, error_code = "ERR_RunOutOfGift" });// Json(new PrizeGift() { ID = -2, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = 180, Prize_Name = "继续加油" });

                }
                else
                {
                    PrizeGift hasPrizeGift = prizeGift[result];

                    hasPrizeGift.Alias_Name = hasPrizeGift.Alias_Name ?? "";
                    if (hasPrizeGift.Has_Gift > 0 && hasPrizeGift.Leavings_Quantity > 0)
                    {
                        //中奖业务处理
                        PluginSceneResult pluginSceneResult = new PluginSceneResult();
                        pluginSceneResult.Address = "";
                        pluginSceneResult.CreateDate = DateTime.Now;
                        pluginSceneResult.Email = "";
                        pluginSceneResult.IsUse = 0;
                        pluginSceneResult.MobilePhone = "";
                        pluginSceneResult.Name = "";
                        pluginSceneResult.Phone = "";
                        pluginSceneResult.PluginId = Id;
                        pluginSceneResult.MpUserId = xyPluginUser.Id;
                        pluginSceneResult.SN = "";
                        pluginSceneResult.UserName = (string.IsNullOrEmpty(this.Name) ? this.NickName : this.Name);

                        if (hasPrizeGift.Has_Gift == 2)//刮刮卡
                        {
                            xyPluginUser.GuaGuaKa = 1;
                            _xyPluginUserService.Update(xyPluginUser);
                        }

                        if (this.MpUserArr != null)
                        {
                            pluginSceneResult.Summary = "恭喜用户[" + (string.IsNullOrEmpty(this.Name) ? this.NickName : this.Name) + "]获得" + hasPrizeGift.Prize_Name + "[" + hasPrizeGift.Name + "]";
                        }
                        else
                        {
                            pluginSceneResult.Summary = "恭喜匿名用户获得" + hasPrizeGift.Prize_Name + "[" + hasPrizeGift.Name + "]";
                        }
                        pluginSceneResult.Value = "<?xml version=\"1.0\" encoding=\"utf-8\"?><PluginSubmit><Cst_Plugin_ItemIndex>" + result.ToString() + "</Cst_Plugin_ItemIndex><Cst_Plugin_PrizeLevel>" + hasPrizeGift.Prize_Name + "</Cst_Plugin_PrizeLevel><Cst_Plugin_PrizeName>" + hasPrizeGift.Name + "</Cst_Plugin_PrizeName></PluginSubmit>";

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

                        _pluginSceneResultService.Insert(pluginSceneResult);
                        hasPrizeGift.ResultID = pluginSceneResult.Id;
                    }

                    return Content(JsonConvert.SerializeObject(hasPrizeGift), "text/javascript");
                }

            }
            catch (Exception ex)
            {
                throw new OceanException(ex.Message, ex);
            }
        }
        [HttpGet]
        public ActionResult PluginsLotteryResult(Guid Id)
        {
            try
            {
                if (Id == Guid.Empty)
                {
                    Log4NetImpl.Write("PluginsLottery:参数有误");
                    throw new OceanException("参数有误，请检查！");
                }

                //页面上的Plugins
                IList<Plugin> plugins = _pluginService.GetALL();
                ViewBag.ContentPlugins = plugins;
                Plugin curPlugin = plugins.Where(p => p.Name.Contains("仙游")).FirstOrDefault();
                ViewBag.CurPlugin = curPlugin;
                if (curPlugin == null)
                {
                    throw new OceanException("对不起，不存在该插件或未开通，请检查！");
                }
                ViewBag.Title = curPlugin.RPluginBase.Name;

                OceanDynamicList<object> list = _pluginSceneResultService.GetPageDynamicList(Id);
                ViewBag.PluginResultList = list;

                return View("LotteryResult");
            }
            catch (Exception ex)
            {
                Log4NetImpl.Write("PluginsLotteryResult:" + ex.Message);
                throw new OceanException(ex.Message, ex);
            }
        }
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

            public static int Lottery(IList<Double> orignalRates, IList<PrizeGift> prizeGifts, int ratesTotal, Guid userId, Guid pluginId)
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
                if (noPrizeValue > 0)
                {
                    //补充最后一个
                    rates.Add(prizeTotal);
                }
                //if (noPrize > 0)
                //{
                //    int noPrizeAverage = noPrizeValue / noPrize;//剩余未中奖奖项、平均分配概率
                //    for (int y = 0; y < noPrize; y++)
                //    {
                //        if (y == noPrize - 1)
                //        {
                //            noPrizeAverage += noPrizeValue % noPrize;
                //        }
                //        maxRateValue += noPrizeAverage;
                //        rates.Add(maxRateValue);
                //    }
                //}
                #endregion

                #region 抽奖
                int result = -1;
                bool randomTrue = true;
                int randomCount = 0;
                while (randomTrue && randomCount < 5)
                {
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
                        if (index >= size)
                        {//如果抽到的是 超出奖项设置的，直接未抽中
                            result = -3;
                            return result;
                        }
                        prizeGifts[index].Rate = randomValue;
                        result = index;


                        if (prizeGifts[index].Has_Gift == 1 && prizeGifts[index].Leavings_Quantity == 0)
                        {
                            //剩余0不再中奖-奖概率重新分配
                            prizeGifts[index].Odds = 0.00d;
                            ////全部都等0时-或者，设置固定中奖者-已抽中的个数
                            //var zeroCount = prizeGifts.Where(g => g.Leavings_Quantity == 0).Count();
                            var zeroCount = 0;
                            for (int _z = 0; _z < prizeGifts.Count(); _z++)
                            {
                                if (prizeGifts[_z].Leavings_Quantity == 0 )
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
                            randomTrue = false;
                        }
                    }
                    randomCount++;
                }
                #endregion

                return result;
            }

        }
    }
}