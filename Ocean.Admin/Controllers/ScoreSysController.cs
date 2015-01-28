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
using System.Xml;
using Ocean.Core.Utility;
using Ocean.Core.Npoi;
using System.Data;
using Ocean.Core.Logging;

namespace Ocean.Admin.Controllers
{
    public class ScoreSysController : AdminBaseController
    {
        private readonly IMpUserService _mpUserService;

        private readonly IScoreConsumeInfoService _scoreConsumeInfoService;
        private readonly IScorePluginResultService _scorePluginResultService;
        private readonly IScoreStoreItemService _scoreStoreItemServiceService;
        private readonly IScoreTradeInfoService _scoreTradeInfoService;
        private readonly IScoreUserService _scoreUserService;
        private readonly IPluginService _pluginService;

        public ScoreSysController(IScoreConsumeInfoService scoreConsumeInfoService, 
            IScorePluginResultService scorePluginResultService,
            IScoreStoreItemService scoreStoreItemServiceService,
            IScoreTradeInfoService scoreTradeInfoService,
            IScoreUserService scoreUserService,
            IPluginService pluginService,
            IMpUserService mpUserService)
        {
            _scoreConsumeInfoService = scoreConsumeInfoService;
            _scorePluginResultService = scorePluginResultService;
            _scoreStoreItemServiceService = scoreStoreItemServiceService;
            _scoreTradeInfoService = scoreTradeInfoService;
            _scoreUserService = scoreUserService;
            _pluginService = pluginService;
            this._mpUserService = mpUserService;
        }

        

        //客服会话管理

        /// <summary>
        /// 初始化KfMeetingList页面
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            if (!base.HasPermission("guaguaka", PermissionOperate.view))
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
        [ActionName("_ScoreSysUserList")]
        public ActionResult ScoreSysUserListProvide(ScoreUser scoreUserDTO)
        {
            if (!base.HasPermission("guaguaka", PermissionOperate.view))
            {
                if (base.LoginAdmin.KfNumbers.Count == 0)
                {
                    return null;
                }

            }

            PagedList<ScoreUser> list = _scoreUserService.GetPageList(PageIndex, PageSize, scoreUserDTO);

            var json = new
            {
                total = list.TotalItemCount,
                rows = (from r in list
                        select new ScoreUser()
                        {
                            MpUserId = r.MpUserId,
                            ClientName = r.ClientName,
                            ClientPhone = r.ClientPhone,
                            RecommendName = r.RecommendName,
                            RecommendPhone = r.RecommendPhone,
                            CurrentScore = r.CurrentScore,
                            RecommendScore = r.RecommendScore,
                            DealScore = r.DealScore,
                            IsEmployee = r.IsEmployee,
                            CreateDate = r.CreateDate
                        }).ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
            //return SwitchJsonList<ScoreUser, ScoreUser>(list, list.TotalItemCount);
        }
        [HttpGet]
        public ActionResult ScoreSysUserConsumeList()
        {
            if (!base.HasPermission("guaguaka", PermissionOperate.view))
            {
                if (base.LoginAdmin.KfNumbers.Count == 0)
                {
                    return Content("当前用户不被允许查看所有记录，您必须先被分配一个工号");
                }
            }

            string mpUserId = RQuery["MpUserId"];

            if (!string.IsNullOrWhiteSpace(mpUserId))
                ViewBag.MpUserId = mpUserId;
            else
                ViewBag.MpUserId = string.Empty;

            return AdminView();
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        [ActionName("_ScoreSysUserConsumeList")]
        public ActionResult ScoreSysUserConsumeListProvide(ScoreConsumeInfo scoreConsumeInfo)
        {
            if (!base.HasPermission("guaguaka", PermissionOperate.view))
            {
                if (base.LoginAdmin.KfNumbers.Count == 0)
                {
                    return null;
                }
            }


            PagedList<ScoreConsumeInfo> list = _scoreConsumeInfoService.GetConsumeScoreList(scoreConsumeInfo.MpUserId, PageIndex, PageSize);
            var json = new
            {
                total = list.TotalItemCount,
                rows = (from r in list
                        select new ScoreConsumeInfo()
                        {
                            MpUserId = r.MpUserId,
                            ConsumeType = r.ConsumeType,
                            ConsumePoints = r.ConsumePoints,
                            Summary = r.Summary,
                            ConsumeDateTime = r.ConsumeDateTime,
                        }).ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }


        #region Submit
        [HttpGet]
        /// <summary>
        /// 插件表单提交管理
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        public ActionResult SubmitEdit()
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



            ViewBag.Field_Style_ID = curPlugin.RPluginBaseStyle.Id;
            ViewBag.Field_Style_Folder = curPlugin.StyleFolder;
            ViewBag.Field_Plugin = curPlugin.RPluginBase;
            ViewBag.pluginId = curPlugin.Id;

            //ViewBag.PluginStyleList = _pluginBaseStyleService.GetStyleList(plugin.RPluginBase.Id);

            ViewBag.PluginName = curPlugin.RPluginBase.Name;
            ViewBag.IsPost = false;

            return AdminView(); 
        }
        /// <summary>
        /// 插件表单提交管理
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        public ActionResult ResultList(Guid pluginId, string skey)
        {
            PagedList<ScorePluginResult> list = _scorePluginResultService.GetPageList(PageIndex, PageSize);

            var json = new
            {
                total = list.TotalItemCount,
                rows = (from r in list
                        select new ScorePluginResult()
                        {
                            PluginId = r.PluginId,
                            Id = r.Id,
                            MpUserId = r.MpUserId,
                            Name = r.Name,
                            MobilePhone = r.MobilePhone,
                            Address = r.Address,
                            IsUse = r.IsUse,
                            Summary = r.Summary,
                            CreateDate = r.CreateDate
                        }).ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
            //if (StringHelper.IsZeroGuid(pluginId))
            //{
            //    return Json(new { message = "参数有误，请检查！" });
            //}
            //else
            //{
            //    Plugin plugin = _pluginService.GetById(pluginId);
            //    if (plugin == null)
            //    {
            //        return Json(new { message = "对不起，不存在该插件或未开通，请检查！" });
            //    }
            //    else
            //    {
            //        PagedList<ScorePluginResult> ScorePluginResultList = _scorePluginResultService.GetPageList("from ScorePluginResult where PluginId='" + pluginId + "'", PageIndex, 10);
            //        ViewBag.PluginResultList = ScorePluginResultList;
            //        //return Json(pluginResultList);
            //        return View(plugin.RPluginBase.FolderName + "/submit/list");
            //    }
            //}
        }

        /// <summary>
        /// 插件表单提交管理
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResultEdit(Guid pluginId, Guid id)
        {
            if (StringHelper.IsZeroGuid(pluginId))
            {
                return Json(new { message = "参数有误，请检查！" });
            }
            else
            {
                Plugin plugin = _pluginService.GetById(pluginId);
                if (plugin == null)
                {
                    return Json(new { message = "对不起，不存在该插件或未开通，请检查！" });
                }
                else
                {
                    ScorePluginResult pluginResult = _scorePluginResultService.GetById(id);
                    if (pluginResult == null)
                    {
                        return Json(new { message = "对不起，不存在该中奖项，请检查！" });
                    }
                    TryUpdateModel<ScorePluginResult>(pluginResult);
                    _scorePluginResultService.Update(pluginResult);
                    return Json(new { isOK = false, isVertfy = true, message = "已发放" });
                }
            }

        }
        #endregion


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

        [HttpGet]
        [ActionName("_ImportPhoneExcel")]
        public ActionResult ImportPhoneExcel()
        {
            ViewBag.Method = "get";
            return View();
        }
        /// <summary>
        /// 导入福农宝客户
        /// </summary>
        [HttpPost]
        [ActionName("_ImportPhoneExcel")]
        public ActionResult ImportPhoneExcel(HttpPostedFileBase phoneExcel, DateTime importTime)
        {
            ViewBag.Method = "post";
            ViewBag.Success = true;
            int countPhone = 0;
            int countPhoneUpdate = 0;
            if (importTime == null)
            {
                ViewBag.Success = false;
                ViewBag.Message = "请选择导入日期";
                return View("_ImportPhoneExcel");
            }
            if (!phoneExcel.FileName.Contains(importTime.ToString("yyyyMMdd")))
            {
                ViewBag.Success = false;
                ViewBag.Message = "导入日期不正确";
                return View("_ImportPhoneExcel");
            }
            if (phoneExcel != null)
            {
                DataTable dt = null;
                if (phoneExcel.FileName.ToLower().EndsWith(".xlsx"))
                {
                    dt = NPOIHelper.ExcelToTableForXLSX(phoneExcel.InputStream, "客户名称");
                }
                else
                {
                    dt = NPOIHelper.ExcelToTableForXLS(phoneExcel.InputStream, "客户名称", string.Empty);
                }
                var columns = dt.Columns;

                //DateRule rule = DateRuleList.DateRules.Where(d => d.Months.Where(m => m == DateTime.Now.Month).Count() > 0).First();
                //int month = rule.ApplyMonth;
                //int year = DateTime.Now.Year;
                try
                {
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        ScoreTradeInfo tradeInfo = new ScoreTradeInfo();
                        tradeInfo.ClientName = dt.Rows[i]["客户名称"].ToString();
                        if (tradeInfo.ClientName == "")
                        {
                            Log4NetImpl.Write("Line:" + i);
                            continue;
                        }
                        tradeInfo.ClientPhone = dt.Rows[i]["签约手机"].ToString();
                        tradeInfo.TradeCount = Convert.ToInt32(dt.Rows[i]["业务笔数"].ToString());
                        tradeInfo.TradeTime = importTime;
                        tradeInfo.IsStatic = 0;
                        ScoreTradeInfo oldTradeInfo;
                        int exits = _scoreTradeInfoService.ExistTradeInfo(tradeInfo, out oldTradeInfo, false);
                        if (exits == 1 && oldTradeInfo.IsStatic == 0)//已存在，并且未跑
                        {
                            countPhoneUpdate++;
                            oldTradeInfo.TradeCount += tradeInfo.TradeCount;
                            _scoreTradeInfoService.Update(oldTradeInfo);
                        }
                        else
                        {
                            countPhone++;
                            _scoreTradeInfoService.Insert(tradeInfo);
                        }
                    }
                }
                catch (Exception ex)
                {

                    Log4NetImpl.Write("导入异常"+ex.Message);
                    throw new OceanException("导入失败，具体原因，请查看日志！", ex);
                }
                ViewBag.countPhone = countPhone;
                ViewBag.countPhoneUpdate = countPhoneUpdate;
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = "请选择导入文件";
            }
            return View("_ImportPhoneExcel");
        }
    }
}