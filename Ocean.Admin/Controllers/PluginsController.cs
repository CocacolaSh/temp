using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Core.Utility;
using Ocean.Core.Common;
using Ocean.Page;
using Ocean.Framework.Configuration.global.config;
using Ocean.Entity.Enums.AdminLoggerModule;
using Ocean.Entity;
using Ocean.Services;
using System.Xml;
using Ocean.Core;
using Ocean.Core.Plugins.Upload;
using System.Text;

namespace Ocean.Admin.Controllers
{
    public class PluginsController : AdminBaseController
    {
        private readonly IPluginService _pluginService;
        private readonly IPluginBaseStyleService _pluginBaseStyleService;
        private readonly IPluginResultService _pluginResultService;
        private readonly IPluginUsedService _pluginUsedService;
        private readonly IMpUserService _mpUserService;
        private readonly IPluginBaseService _pluginBaseService;
        public PluginsController(IPluginService pluginService, IPluginBaseStyleService pluginBaseStyleService
            , IPluginResultService pluginResultService, IPluginUsedService pluginUsedService
            ,IMpUserService mpUserService,IPluginBaseService pluginBaseService)
        {
            _pluginService = pluginService;
            _pluginBaseStyleService = pluginBaseStyleService;
            _pluginResultService = pluginResultService;
            _pluginUsedService = pluginUsedService;
            _mpUserService = mpUserService;
            _pluginBaseService = pluginBaseService;
        }


        #region 共用方法

        /// <summary>
        /// 将数据塞到ViewData
        /// </summary>
        /// <param name="viewdata"></param>
        /// <param name="widgetNode"></param>
        private void GetDataToViewData(XmlNode pluginNode)
        {
            if (pluginNode == null)
            {
                return;
            }
            foreach (XmlNode propNode in pluginNode.ChildNodes)
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

        private void UpdateFormToXmlDoc(XmlDocument xmlDoc)
        {
            //TODO:这里必须用正则处理掉参数

            XmlNode widgetNode = xmlDoc.SelectSingleNode("plugin");
            bool hasCData = false;
            string value = string.Empty;
            bool updateItems = false;

            XmlNode itemsNode = widgetNode.SelectSingleNode("Cst_Plugin_Items");
            //处理所有的Form参数
            foreach (string key in Request.Form)
            {
                //不属于系统需要管理的参数范围
                if (!key.ToLower().StartsWith("sys_") && !key.ToLower().StartsWith("cst_"))
                {
                    continue;
                }

                //集合
                if (key.ToLower().StartsWith("cst_plugin_items"))
                {
                    if (!updateItems)
                    {
                        updateItems = true;
                        if (itemsNode == null)
                        {
                            itemsNode = xmlDoc.CreateElement("Cst_Plugin_Items");
                            widgetNode.AppendChild(itemsNode);
                        }
                        else
                        {
                            itemsNode.RemoveAll();
                        }
                    }
                    //string itemkey = key.Substring("items_".Length);
                    XmlNode xml_Item = xmlDoc.CreateElement(key);
                    //为空就不创建CDATA元素
                    if (string.IsNullOrWhiteSpace(Request.Form[key]))
                    {
                        xml_Item.InnerText = Request.Form[key];
                    }
                    else
                    {
                        /*处理MVC中的Html.CheckBox生成的特殊代码*/
                        value = Request.Form[key];
                        if (value.Trim() == "true,false")
                        {
                            value = "true";
                        }
                        XmlNode xml_Cdata = xmlDoc.CreateCDataSection(value);
                        xml_Item.AppendChild(xml_Cdata);
                    }
                    itemsNode.AppendChild(xml_Item);
                    continue;
                }

                XmlNode xmlItem = widgetNode.SelectSingleNode(key);

                //不存在则创建，存在应用升级的情况，有些参数有可能在新版本被新增的
                if (xmlItem == null)
                {
                    xmlItem = xmlDoc.CreateElement(key);
                    widgetNode.AppendChild(xmlItem);
                }
                //是否存在CDATA
                hasCData = xmlItem.ChildNodes.Count > 0 && xmlItem.ChildNodes[0].NodeType == XmlNodeType.CDATA;

                //Form元素的文本为空时，移除所有的节点，包括CDATA
                if (string.IsNullOrWhiteSpace(Request.Form[key]))
                {
                    if (hasCData)
                    {
                        xmlItem.RemoveAll();
                    }

                    continue;
                }

                if (!hasCData)
                {
                    XmlNode xmlCData = xmlDoc.CreateCDataSection(string.Empty);

                    xmlItem.AppendChild(xmlCData);
                }

                /*处理MVC中的Html.CheckBox生成的特殊代码*/
                value = Request.Form[key];
                if (value.Trim() == "true,false")
                {
                    value = "true";
                }
                XmlNode xmlCdata = xmlDoc.CreateCDataSection(value);

                xmlItem.ChildNodes[0].InnerText = value;
            }
        }

        #endregion

        /// <summary>
        /// 编辑插件设计器
        /// </summary>
        [HttpGet]
        public ActionResult EditPlugin(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                throw new OceanException("参数有误，请检查！");
            }
            //设置布局页
            XmlDocument xmlDoc = new XmlDocument();

            Plugin plugin = _pluginService.GetById(Id);
            if (plugin == null)
            {
                throw new OceanException("不存在该插件，请检查！");
            }
            xmlDoc.LoadXml(plugin.Value);

            XmlNode pluginNode = xmlDoc.SelectSingleNode("plugin");
            GetDataToViewData(pluginNode);

            ViewBag.Field_Style_ID = plugin.RPluginBaseStyle.Id;
            ViewBag.Field_Style_Folder = plugin.StyleFolder;
            ViewBag.Field_Plugin = plugin.RPluginBase;

            ViewBag.PluginStyleList = _pluginBaseStyleService.GetStyleList(plugin.RPluginBase.Id);

            ViewBag.PluginName = plugin.RPluginBase.Name;
            ViewBag.IsPost = false;
            return View(plugin.RPluginBase.FolderName + "/design/edit");
        }

        [ValidateInput(false)]
        public ActionResult SavePlugin()
        {
            if (Request.Form.Keys.Count == 0)
            {
                throw new OceanException("对不起，您在保存过程中遇到错误，请重新提交数据！");
            }
            else
            {
                #region 公用
                XmlDocument xmlDoc = new XmlDocument();

                //创建XML顶部
                XmlNode xmlRoot = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes");
                xmlDoc.AppendChild(xmlRoot);
                //创建根节点
                XmlNode rootWidget = xmlDoc.CreateElement("plugin");
                xmlDoc.AppendChild(rootWidget);

                //TODO:要对item进行名称过滤，比如<、>、$等之类的特殊符号及数字开头
                //参考 http://xml.jz123.cn/xml_elements.asp.htm 《XML元素命名》
                //使用正则是个不错的选择
                string value = string.Empty;
                bool updateItems = false;
                XmlNode itemsNode = null;
                foreach (string key in Request.Form.Keys)
                {
                    //不属于系统需要管理的参数范围
                    if (!key.ToLower().StartsWith("sys_") && !key.ToLower().StartsWith("cst_p_") && !key.ToLower().StartsWith("items_"))
                    {
                        continue;
                    }

                    //集合
                    if (key.ToLower().StartsWith("items_"))
                    {
                        if (!updateItems)
                        {
                            updateItems = true;
                            if (itemsNode == null)
                            {
                                itemsNode = xmlDoc.CreateElement("items");
                                rootWidget.AppendChild(itemsNode);
                            }
                            else
                            {
                                itemsNode.RemoveAll();
                            }
                        }
                        //string itemkey = key.Substring("items_".Length);
                        XmlNode xml_Item = xmlDoc.CreateElement(key);
                        //为空就不创建CDATA元素
                        if (string.IsNullOrWhiteSpace(Request.Form[key]))
                        {
                            xml_Item.InnerText = Request.Form[key];
                        }
                        else
                        {
                            /*处理MVC中的Html.CheckBox生成的特殊代码*/
                            value = Request.Form[key];
                            if (value.Trim() == "true,false")
                            {
                                value = "true";
                            }
                            XmlNode xmlCdata = xmlDoc.CreateCDataSection(value);
                            xml_Item.AppendChild(xmlCdata);
                        }
                        itemsNode.AppendChild(xml_Item);

                        continue;
                    }

                    XmlNode xmlItem = xmlDoc.CreateElement(key);
                    //为空就不创建CDATA元素
                    if (string.IsNullOrWhiteSpace(Request.Form[key]))
                    {
                        xmlItem.InnerText = Request.Form[key];
                    }
                    else
                    {
                        /*处理MVC中的Html.CheckBox生成的特殊代码*/
                        value = Request.Form[key];
                        if (value.Trim() == "true,false")
                        {
                            value = "true";
                        }
                        XmlNode xmlCdata = xmlDoc.CreateCDataSection(value);
                        xmlItem.AppendChild(xmlCdata);
                    }
                    rootWidget.AppendChild(xmlItem);
                }
                #endregion
                Plugin pluginDto = new Plugin();
                //被过滤的名称不存
                this.TryUpdateModel(pluginDto);
                //存入XML值
                pluginDto.Value = xmlDoc.OuterXml;

                pluginDto = _pluginService.SavePlugin(pluginDto);

                ViewBag.PluginID = pluginDto.Id;

                return RedirectToAction("PluginSet", new { pluginId = pluginDto.PluginBaseId });
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditPlugin()
        {
            if (Request.Form.Keys.Count == 0)
            {
                throw new OceanException("对不起，您在保存过程中遇到错误，请重新提交数据!");
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                Plugin pluginDto = new Plugin();
                //被过滤的名称不存 
                this.TryUpdateModel(pluginDto);
                Plugin plugin = _pluginService.GetById(pluginDto.Id);
                if (plugin == null)
                {
                    throw new OceanException("不存在该插件，请检查！");
                }
                //开始载入XML文档
                xmlDoc.LoadXml(plugin.Value);
                //更新Form
                UpdateFormToXmlDoc(xmlDoc);

                //存入XML值
                pluginDto.Value = xmlDoc.OuterXml;

                pluginDto.Name = RQuery["Cst_Plugin_Title"];
                pluginDto.StartDate = RQuery["Cst_Plugin_StartDate", DateTime.Now];
                pluginDto.EndDate = RQuery["Cst_Plugin_EndDate", DateTime.Now];
                pluginDto.StyleId = new Guid(RQuery["Field_Style_ID"]);
                pluginDto.StyleFolder = RQuery["Field_Style_Folder"];
                _pluginService.UpdatePlugin(pluginDto,plugin);
                ViewBag.IsPost = true;
                ViewBag.Success = true;
                return View(plugin.RPluginBase.FolderName + "/design/edit");
                //return JsonMessage(true, pluginDto.Id.ToString());
            }
        }

        public ActionResult Index(int? page)
        {
            return View("index");
        }
        [ActionName("_pluginList")]
        [HttpPost]
        public ActionResult PluginList()
        {
            OceanDynamicList<object> list = _pluginBaseService.GetPluginList(PageIndex, 15, new Guid(RQuery["cid",Guid.Empty.ToString()]));
            XmlDocument xmlDoc = new XmlDocument();
            DateTime now = DateTime.Now;
            DateTime startDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now.AddDays(1);
            foreach (dynamic item in list)
            {
                item.IsOpen = 1;
                if (item == null || string.IsNullOrWhiteSpace(item.Value.ToString())) { continue; }
                string value = item.Value;
                xmlDoc.LoadXml(value);
                XmlNode node = xmlDoc.SelectSingleNode("plugin/Cst_Plugin_StartDate");
                if (node != null && node.HasChildNodes)
                {
                    if (!string.IsNullOrWhiteSpace(node.InnerText))
                    {
                        startDate = TypeConverter.StrToDateTime(node.InnerText, DateTime.Now).AddDays(-1);
                    }
                }
                node = xmlDoc.SelectSingleNode("plugin/Cst_Plugin_EndDate");
                if (node != null && node.HasChildNodes)
                {
                    if (!string.IsNullOrWhiteSpace(node.InnerText))
                    {
                        endDate = TypeConverter.StrToDateTime(node.InnerText, DateTime.Now).AddDays(1);
                    }
                }
                item.StartDate = startDate;
                item.EndDate = endDate;
                if (now < startDate)
                {
                    item.IsOpen = 2;
                }
                else if (now > endDate)
                {
                    item.IsOpen = 0;
                }
            }
            return Content(list.ToJson(), "text/javascript");
        }

        public ActionResult PluginSet(Guid Id)
        {
            string msg = TempData["MSG"] as string;
            if (!string.IsNullOrEmpty(msg))
            {
                //this.AppendError("plugin",msg);
                TempData.Clear();
            }
            if (Id==Guid.Empty)
                throw new OceanException("参数有误，请检查！");
            XmlDocument xmlDoc = new XmlDocument();

            Plugin plugin = _pluginService.GetById(Id);
            if (plugin != null)
            {
                xmlDoc.LoadXml(plugin.Value);
            }
            else
            {
                throw new OceanException("插件加载失败！");
            }
            XmlNode widgetNode = xmlDoc.SelectSingleNode("plugin");
            GetDataToViewData(widgetNode);
            return View("pluginSet", plugin);
        }

        /// <summary>
        /// 开通微站
        /// </summary>
        /// <param name="pluginid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OpenPlugin(Guid pluginid,string isMulti)
        {

            if (!StringHelper.IsZeroGuid(pluginid))
            {
                Guid plid = _pluginService.OpenPlugin(pluginid,isMulti);
                if (StringHelper.IsZeroGuid(pluginid))
                {
                    throw new OceanException("不存在该系统插件，请检查参数！");
                }
            }
            else
            {
                throw new OceanException("参数有误！");
            }
           return JsonMessage(true, "开通成功！");
        }


        [HttpPost]
        public ActionResult DelPlugin(Guid pluginId)
        {
            _pluginService.DelPlugin(pluginId);
            return JsonMessage(true);
        }
        [HttpPost]
        public ActionResult ResetPlugin(Guid pluginId)
        {
            _pluginService.ResetPlugin(pluginId);
            return JsonMessage(false, "对不起，您不存在该插件！");
        }

        [HttpPost]
        public ActionResult PluginUseCount(Guid pluginId, int subtractUseCount)
        {
            Plugin plugin = _pluginService.GetById(pluginId);
            if (plugin != null)
            {
                _pluginUsedService.ExcuteSql("update PluginUsed set HasUseCount=case when HasUseCount+" + subtractUseCount + ">0 then HasUseCount+" + subtractUseCount + " else 0 end where PluginId='" + pluginId.ToString() + "'");
                return JsonMessage(true);
            }
            else
            {
                return JsonMessage(false, "不存在该插件！");
            }
        }


        /// <summary>
        /// 插件搜索
        /// </summary>
        /// <returns></returns>
        public ActionResult SeachPlugin()
        {
            string name = RQuery["Name"];
            ViewBag.Name = name;
            return View("/index", _pluginService.SeachPluginList(name,PageIndex, 10));
        }

        [HttpPost]
        public JsonResult UploadBanner(string id)
        {
            try
            {
                HttpPostedFileBase uploadImage = Request.Files[0];
                string imgFileName = String.Empty;  //返回的图片路径
                string imgErr = String.Empty;   //返回的上传错误信息
                if (uploadImage != null && uploadImage.ContentLength > 0)
                {
                    IUpload upload = UploadProvider.GetInstance();
                    UpFileEntity fileEntity = new UpFileEntity() { Size = 4096, AllowType = ".jpg,.jpeg,.bmp,.gif,.png" };
                    fileEntity.Dir = "/Plugin/";
                    fileEntity.AllowType = ".jpg,.jpeg,.gif,.png,.bmp";

                    AttachmentInfo attch = upload.UploadFile(uploadImage, fileEntity);

                    if (attch != null && string.IsNullOrEmpty(attch.Error))
                    {
                        imgFileName = attch.FileName;
                    }
                    else
                    {
                        imgErr = attch.Error;
                    }
                }
                else
                {
                    imgErr = "请选择上传文件";
                }
                var res = new JsonResult();
                var person = new { err = imgErr, msg = imgFileName };
                res.Data = person;//返回单个对象
                res.ContentType = "application/javascript";
                res.ContentEncoding = Encoding.UTF8;
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception("上传文件测试", ex);
            }
        }

        #region Submit
        /// <summary>
        /// 插件表单提交管理
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        public ActionResult SubmitEdit(Guid pluginId)
        {
            if (pluginId == Guid.Empty)
            {
                throw new OceanException("参数有误，请检查！");
            }
            //设置布局页
            XmlDocument xmlDoc = new XmlDocument();

            Plugin plugin = null;
            //系统设计
            plugin = _pluginService.GetById(pluginId);
            if (plugin == null)
            {
                throw new OceanException("不存在该插件，请检查！");
            }
            xmlDoc.LoadXml(plugin.Value);

            XmlNode pluginNode = xmlDoc.SelectSingleNode("plugin");
            GetDataToViewData(pluginNode);

            ViewBag.Field_Style_ID = plugin.RPluginBaseStyle.Id;
            ViewBag.Field_Style_Folder = plugin.StyleFolder;
            ViewBag.Field_Plugin = plugin.RPluginBase;

            ViewBag.PluginStyleList = _pluginBaseStyleService.GetStyleList(plugin.RPluginBase.Id);

            ViewBag.PluginName = plugin.RPluginBase.Name;
            ViewBag.IsPost = false;

            return View(plugin.RPluginBase.FolderName + "/submit/submitEdit");
        }
        /// <summary>
        /// 插件表单提交管理
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        public ActionResult ResultList(Guid pluginId,string skey)
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
                    PagedList<PluginResult> pluginResultList = _pluginResultService.GetPageList("from PluginResult where PluginId='" + pluginId + "'", PageIndex, 10);
                    ViewBag.PluginResultList = pluginResultList;
                    //return Json(pluginResultList);
                    return View(plugin.RPluginBase.FolderName + "/submit/list");
                }
            }
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
                    PluginResult pluginResult = _pluginResultService.GetById(id);
                    if (pluginResult == null)
                    {
                        return Json(new { message = "对不起，不存在该中奖项，请检查！" });
                    }
                    TryUpdateModel<PluginResult>(pluginResult);
                    _pluginResultService.Update(pluginResult);
                    PagedList<PluginResult> pluginResultList = _pluginResultService.GetPageList("from PluginResult where PluginId='" + pluginId + "'", PageIndex, 10);
                    ViewBag.PluginResultList = pluginResultList;
                    return View(plugin.RPluginBase.FolderName + "/submit/list");
                }
            }

        }
        #endregion

        #region 数据获取
        /// <summary>
        /// 获取站点用户列表
        /// </summary>
        /// <param name="skey"></param>
        /// <returns></returns>
        public ActionResult GetMpUsers(string skey,string utype)
        {
            string condition = "";
            if (!string.IsNullOrEmpty(skey))
            {
                condition += " and UserName like '%" + StringHelper.SqlEscape(skey,true) + "%'";
            }
            PagedList<MpUser> mpUsers = _mpUserService.GetMpUsers(PageIndex, 10, condition, utype == "1" ? true : false);
            return View("lottery/design/mpusers", mpUsers);
        }

        #endregion
    }
}