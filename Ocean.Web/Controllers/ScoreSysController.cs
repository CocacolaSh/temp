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
using Ocean.Entity.Enums.ScoreSys;
using Ocean.Page;
using Ocean.Core.Utility;
using Ocean.Core.Logging;
using Ocean.Entity.DTO.Plugins;
using Ocean.Framework.Mvc.Extensions;
using System.Xml;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Ocean.Core.Data.DynamicQuery;
namespace Ocean.Web.Controllers
{
    public class ScoreSysController : WebBaseController
    {
        //
        // GET: /ScoreSys/

        private readonly IScoreConsumeInfoService _scoreConsumeInfoService;
        private readonly IScorePluginResultService _scorePluginResultService;
        private readonly IScoreStoreItemService _scoreStoreItemServiceService;
        private readonly IScoreTradeInfoService _scoreTradeInfoService;
        private readonly IScoreUserService _scoreUserService;
        private readonly IPluginService _pluginService;
        private readonly IMpUserService _mpUserService;

        private static int recommendScore = 10;
        private static int bindScore = 10;
        public ScoreSysController(IMpUserService mpUserService
            , IScoreConsumeInfoService scoreConsumeInfoService
            , IScorePluginResultService scorePluginResultService
            , IScoreStoreItemService scoreStoreItemServiceService
            , IScoreTradeInfoService scoreTradeInfoService
            , IScoreUserService scoreUserService
            , IPluginService pluginService)
        {
            _mpUserService = mpUserService;
            _scoreConsumeInfoService = scoreConsumeInfoService;
            _scorePluginResultService = scorePluginResultService;
            _scoreStoreItemServiceService = scoreStoreItemServiceService;
            _scoreTradeInfoService = scoreTradeInfoService;
            _scoreUserService = scoreUserService;
            _pluginService = pluginService;
        }


        public ActionResult Index()
        {
            try
            {
                //页面上的Plugins
                IList<Plugin> plugins = _pluginService.GetALL();
                //用isopen 来区分 是传统的抽奖还是 积分系统
                Plugin curPlugin = plugins.Where(p => p.IsOpen == 1).FirstOrDefault();
                ViewBag.CurPlugin = curPlugin;
                if (curPlugin == null)
                {
                    throw new OceanException("对不起，不存在该插件或未开通，请检查！");
                }
                ViewBag.Title = curPlugin.RPluginBase.Name;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(curPlugin.Value);
                XmlNode pluginNode = xmlDoc.SelectSingleNode("plugin");
                GetDataToViewData(pluginNode);

                ViewBag.MpUserArr = this.MpUserArr;
                ViewBag.MpUserID = this.MpUserID;
                ViewBag.CurrentScore = ViewBag.RecommendScore = ViewBag.DealScore = 0;
                if (MpUserID != Guid.Empty)
                {
                    PagedList<ScoreConsumeInfo> scoreConsumeInfoList = _scoreConsumeInfoService.GetConsumeScoreList(MpUserID, 1, 30);//"from SceneResult where PluginId='" + Id.ToString() + "'");
                    ViewBag.ScoreConsumeInfoList = scoreConsumeInfoList;

                    ScoreUser scoreUser = _scoreUserService.GetUnique("from ScoreUser where (MpUserId ='" + this.MpUserID + "')");

                    if (scoreUser != null)
                    {
                        ViewBag.CurrentScore = scoreUser.CurrentScore;
                        ViewBag.RecommendScore = scoreUser.RecommendScore;
                        ViewBag.DealScore = scoreUser.DealScore;
                    }
                    IList<ScorePluginResult> scorePluginResultList = _scorePluginResultService.GetList("from ScorePluginResult where MpUserId='" + MpUserID + "' order by createdate desc");
                    ViewBag.MyPlugin_SubmitList = scorePluginResultList;
                }
                bindScore = ViewData.GetInt("Score_Bind");// ViewBag.Score_Bind;
                recommendScore = ViewData.GetInt("Score_Recommend");// ViewBag.Score_Recommend;
                TempData["Id"] = curPlugin.Id;
                TempData["Score_Plugin_Title"] = ViewBag.Score_Plugin_Title;
                TempData["Score_Plugin_Summary"] = ViewBag.Score_Plugin_Summary;
                TempData["Score_Plugin_StartDate"] = ViewBag.Score_Plugin_StartDate;
                TempData["Score_Plugin_EndDate"] = ViewBag.Score_Plugin_EndDate;
                TempData["Score_Plugin_ZeroPrize"] = ViewBag.Score_Plugin_ZeroPrize;
                TempData["Score_Plugin_UsePoint"] = ViewData.GetInt("Score_Plugin_UsePoint");

                TempData["Score_Plugin_FailsTimes"] = ViewBag.Score_Plugin_FailsTimes;
                TempData["Score_Plugin_FailsAddOdds"] = ViewBag.Score_Plugin_FailsAddOdds; 
                return View("Index");
            }
            catch (Exception ex)
            {
                throw new OceanException(ex.Message, ex);
            }
        }
        public ActionResult Bind(Guid id, string Name, string Phone, string RecomendName, string RecomendPhone)
        {
            try
            {
                if (MpUserID == Guid.Empty)
                {
                    string rawUrl = "http://wx.ssrcb.com/ScoreSys/Index";
                    if (string.IsNullOrEmpty(RQuery["openid"]))
                    {
                        return Json(new { isOK = false, isLogin = true, error_code = "ERR_NotLogin", message = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri=http://wx.ssrcb.com/mpuser/autologin?refUrl={1}&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect", MpCenterCache.AppID, rawUrl) });
                    }
                }
                ScoreUser scoreUser = _scoreUserService.GetUnique("from ScoreUser where (ClientPhone='" + Phone.Trim() + "' and ClientName = '" + Name + "') or (MpUserId ='" + this.MpUserID.ToString() + "')");

                if (scoreUser != null)
                {
                    return Json(new { isOK = false, isVertfy = true, error_code = "ERR_UserHadBind" });
                }
                scoreUser = new ScoreUser();
                scoreUser.ClientName = Name.Trim();
                scoreUser.ClientPhone = Phone.Trim();
                scoreUser.RecommendName = RecomendName.Trim();
                scoreUser.RecommendPhone = RecomendPhone.Trim();
                scoreUser.MpUserId = MpUserID;
                scoreUser.RecommendScore = 0;
                scoreUser.CurrentScore = bindScore;
                scoreUser.DealScore = 0;
                _scoreUserService.Insert(scoreUser);

                //首次绑定获得积分
                ScoreConsumeInfo scoreConsumeDetail = new ScoreConsumeInfo();
                scoreConsumeDetail.MpUserId = this.MpUserID;
                scoreConsumeDetail.ConsumeType = (int)ScoreConsume.Bind;
                scoreConsumeDetail.ConsumePoints = bindScore;
                scoreConsumeDetail.CreateDate = DateTime.Now;
                scoreConsumeDetail.ConsumeDateTime = DateTime.Now;
                scoreConsumeDetail.Summary = "首次绑定获得" + bindScore + "积分";
                _scoreConsumeInfoService.Insert(scoreConsumeDetail);

                ScoreUser recommendScoreUser = _scoreUserService.GetUnique("from ScoreUser where ClientPhone='" + RecomendPhone.Trim() + "'");

                if (recommendScoreUser != null)
                {
                    ScoreConsumeInfo recommendScoreConsumeDetail = new ScoreConsumeInfo();
                    recommendScoreConsumeDetail.MpUserId = recommendScoreUser.MpUserId;
                    recommendScoreConsumeDetail.ConsumeType = (int)ScoreConsume.Recommend;
                    recommendScoreConsumeDetail.ConsumePoints = recommendScore;
                    recommendScoreConsumeDetail.CreateDate = DateTime.Now;
                    recommendScoreConsumeDetail.ConsumeDateTime = DateTime.Now;
                    recommendScoreConsumeDetail.Summary = "推荐用户[" + Phone + "]获得" + recommendScore + "积分";
                    _scoreConsumeInfoService.Insert(recommendScoreConsumeDetail);

                    recommendScoreUser.RecommendScore += recommendScore;
                    recommendScoreUser.CurrentScore += recommendScore;
                    _scoreUserService.Update(recommendScoreUser);
                }
                return Json(new { isOK = true, error_code = "ERR_OK", currentScore = scoreUser.CurrentScore, scoreDetail = scoreConsumeDetail.ConsumeDateTime + scoreConsumeDetail.Summary });
            }
            catch (System.Exception ex)
            {
                Log4NetImpl.Write("ScoreSys_Bind:" + ex.Message);
            }
            return View();
        }

        public ActionResult Trans(Guid id, string TransName, string TransPhone, int TransScore)
        {
            try
            {
                if (MpUserID == Guid.Empty)
                {
                    string rawUrl = "http://wx.ssrcb.com/ScoreSys/Index";
                    if (string.IsNullOrEmpty(RQuery["openid"]))
                    {
                        return Json(new { isOK = false, isLogin = true, error_code = "ERR_NotLogin", message = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri=http://wx.ssrcb.com/mpuser/autologin?refUrl={1}&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect", MpCenterCache.AppID, rawUrl) });
                    }
                }
                ScoreUser scoreUser = _scoreUserService.GetUnique("from ScoreUser where (MpUserId ='" + this.MpUserID.ToString() + "')");

                if (scoreUser == null)
                {
                    return Json(new { isOK = false, error_code = "ERR_UserHadBind" });
                }

                ScoreUser scoreTransUser = _scoreUserService.GetUnique("from ScoreUser where (ClientPhone='" + TransPhone.Trim() + "' and ClientName = '" + TransName + "')");

                if (scoreTransUser == null)
                {
                    return Json(new { isOK = false, error_code = "ERR_UserNotExist" });
                }
                if (scoreUser.ClientPhone == scoreTransUser.ClientPhone)
                {
                    return Json(new { isOK = false, error_code = "ERR_TransSamePerson" });
                }
                if (scoreUser.CurrentScore < TransScore)
                {
                    return Json(new { isOK = false, error_code = "ERR_ScoreNotEnough" });
                }
                scoreUser.CurrentScore -= TransScore;
                _scoreUserService.Update(scoreUser);

                //转出积分记录
                ScoreConsumeInfo scoreConsumeDetail = new ScoreConsumeInfo();
                scoreConsumeDetail.MpUserId = this.MpUserID;
                scoreConsumeDetail.ConsumeType = (int)ScoreConsume.Trans;
                scoreConsumeDetail.ConsumePoints = -TransScore;
                scoreConsumeDetail.CreateDate = DateTime.Now;
                scoreConsumeDetail.ConsumeDateTime = DateTime.Now;
                scoreConsumeDetail.Summary = "转给[" +TransName+TransPhone+"]"+ TransScore + "积分";
                _scoreConsumeInfoService.Insert(scoreConsumeDetail);

                scoreTransUser.CurrentScore += TransScore;
                _scoreUserService.Update(scoreTransUser);

                //转入积分记录
                ScoreConsumeInfo transScoreConsumeDetail = new ScoreConsumeInfo();
                transScoreConsumeDetail.MpUserId = scoreTransUser.MpUserId;
                transScoreConsumeDetail.ConsumeType = (int)ScoreConsume.Trans;
                transScoreConsumeDetail.ConsumePoints = TransScore;
                transScoreConsumeDetail.CreateDate = DateTime.Now;
                transScoreConsumeDetail.ConsumeDateTime = DateTime.Now;
                transScoreConsumeDetail.Summary = "从[" + scoreUser.ClientName + scoreUser.ClientPhone + "]获得转入" + TransScore + "积分";
                _scoreConsumeInfoService.Insert(transScoreConsumeDetail);

                return Json(new { isOK = true, error_code = "ERR_OK", currentScore = scoreUser.CurrentScore, scoreDetail = scoreConsumeDetail.ConsumeDateTime + scoreConsumeDetail.Summary });
            }
            catch (System.Exception ex)
            {
                Log4NetImpl.Write("ScoreSys_Trans:" + ex.Message);
            }
            return View();
        }

        #region 大转盘
        public ActionResult PluginLottery(Guid Id)
        {
            try
            {
                if (Id == Guid.Empty)
                {
                    throw new OceanException("参数有误，请检查！");
                }
                //if (MpUserID == Guid.Empty)
                //{
                //    string rawUrl = "http://wx.ssrcb.com/ScoreSys/PluginLottery?id=" + TempData["Id"];
                //    if (string.IsNullOrEmpty(RQuery["openid"]))
                //    {
                //        return Json(new { isOK = false, isLogin = true, error_code = "ERR_NotLogin", message = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri=http://wx.ssrcb.com/mpuser/autologin?refUrl={1}&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect", MpCenterCache.AppID, rawUrl) });
                //    }
                //}
                ViewBag.Score_Plugin_Title = TempData["Score_Plugin_Title"];
                ViewBag.Score_Plugin_StartDate = TempData["Score_Plugin_StartDate"];
                ViewBag.Score_Plugin_EndDate = TempData["Score_Plugin_EndDate"];
                ViewBag.Score_Plugin_Summary = TempData["Score_Plugin_Summary"];
                ViewBag.Score_Plugin_ZeroPrize = TempData["Score_Plugin_ZeroPrize"];
                ViewBag.Score_Plugin_UsePoint = TempData["Score_Plugin_UsePoint"];
                ViewBag.Score_Plugin_FailsTimes = TempData["Score_Plugin_FailsTimes"];
                ViewBag.Score_Plugin_FailsAddOdds = TempData["Score_Plugin_FailsAddOdds"];

                IList<ScorePluginResult> scorePluginResultList = _scorePluginResultService.GetList("from ScorePluginResult where PluginId='" + Id.ToString() + "'");
                ViewBag.ScorePluginResultList = scorePluginResultList;
                if (scorePluginResultList != null)
                {
                    IEnumerable<ScorePluginResult> myplugin_SubmitList = _scorePluginResultService.GetList("from ScorePluginResult where PluginId='" + Id.ToString() + "' and MpUserId='" + this.MpUserID + "' order by CreateDate desc");
                    ViewBag.MyPlugin_SubmitList = myplugin_SubmitList;
                }
            }
            catch (System.Exception ex)
            {
                Log4NetImpl.Write("ScoreSys_PluginLottery:" + ex.Message);
                return View("Index");
            }

            return View("Lottery");
        }


        [HttpPost]
        public ActionResult PluginsLottery(Guid Id, string t)
        {
            try
            {
                if (Id == Guid.Empty)
                {
                    Log4NetImpl.Write("ScoreSYS-PluginLottery:ID is NULL");
                    return Json(new { isOK = false, error_code = "ERR_ArgNotExist" }); //message = "参数有误，请检查！" });
                }

                if (MpUserID == Guid.Empty)
                {
                    string rawUrl = "http://wx.ssrcb.com/ScoreSys/PluginLottery?id=" + WebHelper.GetGuid("Id", Guid.Empty);
                    if (string.IsNullOrEmpty(RQuery["openid"]))
                    {
                        Log4NetImpl.Write("ScoreSYS-PluginLottery:Redirect");
                        return Json(new { isOK = false, isLogin = true, error_code = "ERR_NotLogin", message = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri=http://wx.ssrcb.com/mpuser/autologin?refUrl={1}&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect", MpCenterCache.AppID, rawUrl) });
                    }
                }

                string Score_Plugin_Title = Request.Form["Score_Plugin_Title"];
                string Score_Plugin_StartDate = Request.Form["Score_Plugin_StartDate"];
                string Score_Plugin_EndDate = Request.Form["Score_Plugin_EndDate"];
                string Score_Plugin_UsePoint = Request.Form["Score_Plugin_UsePoint"];
                string Score_Plugin_FailsTimes = Request.Form["Score_Plugin_FailsTimes"];
                string Score_Plugin_FailsAddOdds = Request.Form["Score_Plugin_FailsAddOdds"];
                if (Score_Plugin_Title == "")
                {
                    return Json(new { isOK = false, error_code = "ERR_PluginNotExits" });
                }
                if (!string.IsNullOrEmpty(Score_Plugin_StartDate))
                {
                    DateTime startDate = TypeConverter.StrToDateTime(Score_Plugin_StartDate);
                    if (startDate > DateTime.Now)
                    {
                        return Json(new { isOK = false, error_code = "ERR_PluginNotStart" });
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Score_Plugin_EndDate))
                        {
                            DateTime endDate = TypeConverter.StrToDateTime(Score_Plugin_EndDate);
                            if (endDate < DateTime.Now)
                            {
                                return Json(new { isOK = false, error_code = "ERR_PluginIsEnd" });// message = "对不起，抽奖已经结束！" });
                            }
                        }
                    }
                }

                IList<ScoreStoreItem> scoreStoreItems = _scoreStoreItemServiceService.GetList("from ScoreStoreItem where BaseId = '" + Id.ToString() + "'");
                scoreStoreItems.OrderBy(storeItems => storeItems.PluginName);
                ScoreUser scoreUser = _scoreUserService.GetUnique("from ScoreUser where (MpUserId ='" + this.MpUserID.ToString() + "')");

                if (scoreUser == null)
                {
                    return Json(new { isOK = false, isVertfy = true, error_code = "ERR_UserNotVerify" });
                }
                if (scoreUser.CurrentScore < ViewData.GetInt("Score_Plugin_UsePoint"))
                {
                    return Json(new { isOK = false, error_code = "ERR_UserUseOutChance" });
                }

                int prizeItemCount = scoreStoreItems.Count;// TypeConverter.StrToInt(ViewBag.Cst_Plugin_PrizeCount, 12);
               
                IList<PrizeGift> prizeGift = new List<PrizeGift>(prizeItemCount);
                IList<Double> prizeItems = new List<Double>(prizeItemCount);
                IList<string> prizeItemsName = new List<string>(prizeItemCount);
                IList<int> prizeAngle = new List<int>(prizeItemCount);
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
                    ScoreStoreItem storeItem = scoreStoreItems[i];
                    PrizeGift gift = new PrizeGift();
                    gift.ID = i;
                    gift.Name = storeItem.PluginName;
                    gift.Alias_Name = storeItem.AliasName;
                    gift.Odds = Convert.ToDouble(storeItem.PrizePercent)/100;
                    gift.Pic = storeItem.ItemPic;
                    gift.Quantity = storeItem.TotalCount;
                    gift.Leavings_Quantity = storeItem.LeftCount;

                    gift.Angle = Convert.ToInt32(storeItem.PrizeAngle);
                    prizeAngle.Add(gift.Angle);
                    gift.Prize_Name = prizeItemsName[i] + "等奖";
                    if (!string.IsNullOrEmpty(gift.Alias_Name))
                    {
                        gift.Prize_Name = gift.Alias_Name;
                    }
                    gift.Has_Gift = 1;// ViewData.GetInt("Cst_Plugin_Items_Has_Gift" + i.ToString());
                    //gift.Users = ViewData.GetString("Cst_Plugin_Items_SiteUsers" + i.ToString());
                    //gift.UsersName = ViewData.GetString("Cst_Plugin_Items_SiteUsersName" + i.ToString());
                    prizeItems.Add(gift.Odds);
                    prizeGift.Add(gift);
                }

                prizeItems.OrderBy(d => d);

                
                //verifyCodeDetail.LeaveCount = verifyCodeDetail.LeaveCount - 1;
                //verifyCodeDetail.UseDate = DateTime.Now;
                //_pluginSceneVerifyCodeDetailService.Update(verifyCodeDetail);

                int count = _scorePluginResultService.GetCount("from ScorePluginResult where pluginId='" + Id.ToString() + "' and MpUserId='" + this.MpUserID.ToString() + "'");
                int tryCount = _scoreConsumeInfoService.GetCount(" from ScoreConsumeInfo where ConsumeType = " +(int)ScoreConsume.Plugins + "");
                if ( count == 0 &&　tryCount >  Convert.ToInt32(Score_Plugin_FailsTimes))
                {
                    PrizeGift _lastGift = prizeGift.Last();

                    double finalOdds = _lastGift.Odds * (1 + Convert.ToInt32(Score_Plugin_FailsAddOdds));
                    _lastGift.Odds = finalOdds;
                    prizeItems.RemoveAt(prizeItems.Count - 1);
                    prizeItems.Add(finalOdds);
                }

                int result = -1;

                scoreUser.CurrentScore = scoreUser.CurrentScore - Convert.ToInt32(Score_Plugin_UsePoint);// ViewData.GetInt("Score_Plugin_UsePoint");
                scoreUser.LastUpdateDateTime = DateTime.Now;
                _scoreUserService.Update(scoreUser);

                #region 更新积分消耗纪录
                ScoreConsumeInfo scoreConSumeInfo = new ScoreConsumeInfo();
                scoreConSumeInfo.ConsumeDateTime = scoreConSumeInfo.CreateDate = DateTime.Now;
                scoreConSumeInfo.MpUserId = this.MpUserID;
                scoreConSumeInfo.ConsumeType = (int)(ScoreConsume.Plugins);
                scoreConSumeInfo.ConsumePoints = -Convert.ToInt32(Score_Plugin_UsePoint);
                scoreConSumeInfo.Summary = "抽奖消耗了" + Convert.ToInt32(Score_Plugin_UsePoint) + "积分";
                _scoreConsumeInfoService.Insert(scoreConSumeInfo);
                #endregion
                result = LotteryUtils.Lottery(prizeItems, prizeGift, prizeItemCount, this.MpUserID, Id);


                if (result == -1)
                {
                    return Json(new { isOK = true, error_code = "ERR_RunOutOfGift" });//Json(new PrizeGift() { ID = -1, Name = "未中奖", Odds = 0.00d, Pic = "", Quantity = 100000, Angle = 240, Prize_Name = "继续加油" });
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
                    ScoreStoreItem scoreStoreItem = scoreStoreItems[result];
                    //ScoreStoreItem scoreStoreItem = _scoreStoreItemServiceService.GetUnique("from ScoreStoreItem where (Id ='" + this.MpUserID.ToString() + "')");
                    //score
                    hasPrizeGift.Alias_Name = hasPrizeGift.Alias_Name ?? "";
                    if (scoreStoreItem.LeftCount > 0)
                    {
                        //中奖业务处理
                        ScorePluginResult pluginSceneResult = new ScorePluginResult();
                        pluginSceneResult.Address = "";
                        pluginSceneResult.CreateDate = DateTime.Now;
                        pluginSceneResult.Type = 1;
                        pluginSceneResult.IsUse = 0;
                        pluginSceneResult.MobilePhone = "";
                        pluginSceneResult.Name = "";
                        pluginSceneResult.Phone = "";
                        pluginSceneResult.PluginId = Id;
                        pluginSceneResult.MpUserId = this.MpUserID;
                        pluginSceneResult.SN = "";
                        pluginSceneResult.UserName = (string.IsNullOrEmpty(this.Name) ? this.NickName : this.Name);

                        if (this.MpUserArr != null)
                        {
                            pluginSceneResult.Summary = "恭喜用户[" + (string.IsNullOrEmpty(this.Name) ? this.NickName : this.Name) + "]获得" + hasPrizeGift.Prize_Name + "[" + hasPrizeGift.Name + "]";
                        }
                        else
                        {
                            pluginSceneResult.Summary = "恭喜匿名用户获得" + hasPrizeGift.Prize_Name + "[" + hasPrizeGift.Name + "]";
                        }
                        //pluginSceneResult.Value = "<?xml version=\"1.0\" encoding=\"utf-8\"?><PluginSubmit><Cst_Plugin_ItemIndex>" + result.ToString() + "</Cst_Plugin_ItemIndex><Cst_Plugin_PrizeLevel>" + hasPrizeGift.Prize_Name + "</Cst_Plugin_PrizeLevel><Cst_Plugin_PrizeName>" + hasPrizeGift.Name + "</Cst_Plugin_PrizeName></PluginSubmit>";
                        pluginSceneResult.Value = hasPrizeGift.Prize_Name + "++" + hasPrizeGift.Name;

                        #region 更新商城数量
                        scoreStoreItem.LeftCount -= 1;
                        _scoreStoreItemServiceService.Update(scoreStoreItem);
                        #endregion
                        #region 更新中奖纪录
                        _scorePluginResultService.Insert(pluginSceneResult);
                        #endregion
                        #region 更新虚拟积分纪录
                        if (scoreStoreItem.VirtualGift > 0)
                        {
                            scoreUser.CurrentScore += scoreStoreItem.VirtualGift;
                            _scoreUserService.Update(scoreUser);


                            ScoreConsumeInfo scoreConSumeInfo2 = new ScoreConsumeInfo();
                            scoreConSumeInfo2.ConsumeDateTime = scoreConSumeInfo.CreateDate = DateTime.Now;
                            scoreConSumeInfo2.MpUserId = this.MpUserID;
                            scoreConSumeInfo2.ConsumeType = (int)(ScoreConsume.Plugins);
                            scoreConSumeInfo2.ConsumePoints = scoreStoreItem.VirtualGift;
                            scoreConSumeInfo2.Summary = "抽奖中了" + scoreStoreItem.VirtualGift + "积分";
                            _scoreConsumeInfoService.Insert(scoreConSumeInfo2);
                        }
                        #endregion
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

        #region 商城兑换
        public ActionResult StoreItem(Guid Id)
        {
            try
            {
                if (Id == Guid.Empty)
                {
                    throw new OceanException("参数有误，请检查！");
                }
                //if (MpUserID == Guid.Empty)
                //{
                //    string rawUrl = "http://wx.ssrcb.com/ScoreSys/StoreItem?Id=" + WebHelper.GetGuid("Id", Guid.Empty);
                //    if (string.IsNullOrEmpty(RQuery["openid"]))
                //    {
                //        return Json(new { isOK = false, isLogin = true, error_code = "ERR_NotLogin", message = WebHelper.GetUrl() });
                //        //return Json(new { isOK = false, isLogin = true, error_code = "ERR_NotLogin", message = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri=http://wx.ssrcb.com/mpuser/autologin?refUrl={1}&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect", MpCenterCache.AppID, rawUrl) });
                //    }
                //}

                TempData["Id"] = Id;
                //ViewBag.Score_Plugin_Title = TempData["Score_Plugin_Title"].ToString();
                //ViewBag.Score_Plugin_EndDate = TempData["Score_Plugin_EndDate"].ToString();
                //ViewBag.Score_Plugin_EndDate = TempData["Score_Plugin_EndDate"].ToString();
                //ViewBag.Score_Plugin_Summary = TempData["Score_Plugin_Summary"];
                //ViewBag.Score_Plugin_ZeroPrize = TempData["Score_Plugin_ZeroPrize"];
                //ViewBag.Score_Plugin_UsePoint = TempData["Score_Plugin_UsePoint"];

                ScoreUser scoreUser = _scoreUserService.GetUnique("from ScoreUser where MpUserId = '" + MpUserID + "'");
                ViewBag.CurrentScore = 0;
                if (scoreUser != null || scoreUser.ClientPhone != "")
                {
                    ViewBag.CurrentScore = scoreUser.CurrentScore;
                    ViewBag.RecvName = scoreUser.RecvName;
                    ViewBag.RecvPhone = scoreUser.RecvPhone;
                    ViewBag.RecvAddress = scoreUser.RecvAddress;
                }
                IList<ScoreStoreItem> scoreStoreItemList = _scoreStoreItemServiceService.GetList("from ScoreStoreItem where baseId='" + Id.ToString() + "' and VirtualGift = 0");
                ViewBag.ScoreStoreItemList = scoreStoreItemList;

                IList<ScorePluginResult> scorePluginResultList = _scorePluginResultService.GetList("from ScorePluginResult where PluginId='" + Id.ToString() + "' and MpUserId = '" + MpUserID + "'and type = 2");
                ViewBag.ScorePluginResultList = scorePluginResultList;
            }
            catch (System.Exception ex)
            {
                return View("Index");
            }
            return View("StoreItem");
        }


        [HttpPost]
        public ActionResult StoreItemExchange(Guid baseId, Guid itemId, string t)
        {
            try
            {
                if (baseId == Guid.Empty)
                {
                    return Json(new { isOK = false, error_code = "ERR_ArgNotExist" });
                }

                if (MpUserID == Guid.Empty)
                {
                    string rawUrl = "http://wx.ssrcb.com/ScoreSys/StoreItem?id=" + WebHelper.GetGuid("baseId", Guid.Empty);
                    if (string.IsNullOrEmpty(RQuery["openid"]))
                    {
                        return Json(new { isOK = false, isLogin = true, error_code = "ERR_NotLogin", message = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri=http://wx.ssrcb.com/mpuser/autologin?refUrl={1}&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect", MpCenterCache.AppID, rawUrl) });
                    }
                }

                ScoreStoreItem scoreStoreItem = _scoreStoreItemServiceService.GetUnique("from ScoreStoreItem where Id='" + itemId.ToString() + "'");
                if (scoreStoreItem == null)
                {
                    return Json(new { isOK = false, error_code = "ERR_StoreItemNotExist" });
                }
                if (scoreStoreItem.LeftCount <= 0)
                {
                    return Json(new { isOK = false, error_code = "ERR_StoreItemScoreRunOut" });
                }
                ScoreUser scoreUser = _scoreUserService.GetUnique("from ScoreUser where MpUserId = '" + MpUserID + "'");
                if (scoreUser == null)
                {
                    return Json(new { isOK = false, error_code = "ERR_StoreItemUserNotExist" });
                }
                if (scoreUser.CurrentScore < scoreStoreItem.NeedPoint)
                {
                    return Json(new { isOK = false, error_code = "ERR_StoreItemScoreNotEnough" });
                }
                string Name = Request.Form["Name"];
                string Phone = Request.Form["Phone"];
                string Address = Request.Form["Address"];
                string ResultID = Request.Form["resultId"];
                string isDefaultAddr = Request.Form["DefaultAddr"];
                if (isDefaultAddr == "true")
                {
                    scoreUser.RecvName = Name;
                    scoreUser.RecvPhone = Phone;
                    scoreUser.RecvAddress = Address;
                    _scoreUserService.Update(scoreUser);
                }
                ScorePluginResult scorePluginResult = null;
                if (ResultID == "0")
                {
                    scorePluginResult = new ScorePluginResult();
                    scorePluginResult.MobilePhone = Phone;
                    scorePluginResult.Address = Address;
                    scorePluginResult.Name = Name;
                    scorePluginResult.Type = 2;
                    scorePluginResult.CreateDate = DateTime.Now;
                    scorePluginResult.MpUserId = this.MpUserID;
                    scorePluginResult.PluginId = baseId;
                    scorePluginResult.Summary = "申请兑奖【" + scoreStoreItem.PluginName + "】" + scoreStoreItem.AliasName;
                    scorePluginResult.Value = "" + scoreStoreItem.PluginName + "++" + scoreStoreItem.AliasName;
                    _scorePluginResultService.Insert(scorePluginResult);
                    
                    ScoreConsumeInfo scoreConSumeInfo = new ScoreConsumeInfo();
                    scoreConSumeInfo.ConsumeDateTime = scoreConSumeInfo.CreateDate = DateTime.Now;
                    scoreConSumeInfo.MpUserId = this.MpUserID;
                    scoreConSumeInfo.ConsumeType = (int)(ScoreConsume.Exchange);
                    scoreConSumeInfo.ConsumePoints = -Convert.ToInt32(scoreStoreItem.NeedPoint);
                    scoreConSumeInfo.Summary = "兑换消耗了" + Convert.ToInt32(scoreStoreItem.NeedPoint) + "积分";
                    _scoreConsumeInfoService.Insert(scoreConSumeInfo);

                    scoreUser.CurrentScore -= scoreStoreItem.NeedPoint;
                    _scoreUserService.Update(scoreUser);

                    return Json(new { isOK = true, error_code = "ERR_RequireSuccess" });
                }
                else
                {
                    scorePluginResult = _scorePluginResultService.GetUnique("from ScorePluginResult where MpUserId='" + this.MpUserID.ToString() + "' and Id='" + ResultID.ToString() + "'");
                    if (scorePluginResult == null)
                    {
                        return Json(new { isOK = false, error_code = "ERR_ScorePluginResultNotExist" });
                    }
                    if (scorePluginResult.IsUse == 1)
                    {
                        return Json(new { isOK = false, error_code = "ERR_AlreadySend" });//对不起，您已经领取完奖！
                    }
                    scorePluginResult.MobilePhone = Phone;
                    scorePluginResult.Address = Address;
                    scorePluginResult.Name = Name;
                    _scorePluginResultService.Update(scorePluginResult);
                    return Json(new { isOK = true, error_code = "ERR_ModifySuccess" });
                }
            }
            catch (Exception ex)
            {
                throw new OceanException(ex.Message, ex);
            }
        }

        #endregion
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


        #endregion
    }
}
