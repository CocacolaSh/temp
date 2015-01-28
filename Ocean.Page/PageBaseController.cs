using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ocean.Core.Infrastructure;
using Ocean.Core.Common;
using Ocean.Services;
using Ocean.Core;
using Ocean.Core.Utility;
using System.Reflection;
using Ocean.Framework.Caching.Cache;
using Ocean.Core.Caching;
using Ocean.Entity.Enums.TypeIdentifying;
using Ocean.Entity;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.AdvancedAPIs;
using Ocean.Core.Logging;
using Senparc.Weixin.MP;
using System.Web;

namespace Ocean.Page
{
    [ValidateInput(false)]
    public class PageBaseController : Controller
    {
        /// <summary>
        /// WebHelper
        /// </summary>
        protected IWebHelper WebHelpers
        {
            get
            {
                return EngineContext.Current.Resolve<IWebHelper>();
            }
        }

        protected static ICacheManager _cacheManager = new MemoryCacheManager();
        private readonly IMpCenterService _mpCenterService;

        public PageBaseController()
        {
            this._mpCenterService = EngineContext.Current.Resolve<IMpCenterService>();
        }

        #region 查询参数集
        /// <summary>
        /// 查询参数集
        /// </summary>
        public QueryStringCollection RQuery
        {
            private set;
            get;
        }
        #endregion

        #region 当前页码
        /// <summary>
        /// 当前页码
        /// </summary>
        protected int PageIndex
        {
            get
            {
                return RQuery["page", 1];
            }
        }
        #endregion

        #region 当前每页显示数量
        /// <summary>
        /// 当前每页显示数量
        /// </summary>
        public int PageSize
        {
            get
            {
                return RQuery["rows", 10];
            }
        }
        #endregion

        #region JSON解析函数
        /*************************************************JSON解析函数**************************************************/

        protected JsonResult JsonMessage(bool success)
        {
            return JsonMessage(success, null);
        }

        protected JsonResult JsonMessage(bool success, string message)
        {
            var result = new { success = success, message = message };
            return Json(result);
        }

        protected JsonResult JsonOperationResult(string message, bool success, bool timeout)
        {
            var result = new { message = message, success = success, timeout = timeout };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected JsonResult JsonList<T>(IList<T> list)
        {
            var result = new { rows = list };
            return Json(result);
        }

        protected JsonResult JsonList<T>(IList<T> list, int total)
        {
            var result = new { rows = list, total = total };
            return Json(result);
        }

        protected JsonResult SwitchJsonList<T, U>(IList<T> list, int? total)
        {
            IList<U> jsonList = new List<U>();

            if (list != null && list.Count > 0)
            {
                foreach (T model in list)
                {
                    U jsonModel = (U)Activator.CreateInstance(typeof(U), new object[] { model });
                    jsonList.Add(jsonModel);
                }
            }

            return total.HasValue ? JsonList<U>(jsonList, total.Value) : JsonList<U>(jsonList);
        }

        protected JsonResult JsonList<T>(IList<T> list, string valueFeild, string textFeild)
        {
            IList<object> objs = new List<object>();

            Type modelType = typeof(T);

            if (list != null && list.Count > 0)
            {
                PropertyInfo textProperty = modelType.GetProperty(textFeild);
                PropertyInfo valueProperty = modelType.GetProperty(valueFeild);

                if (textProperty == null || valueProperty == null)
                {
                    throw new ArgumentException("提供的TextFeild或ValueFeild无效！");
                }

                foreach (T m in list)
                {
                    objs.Add(new
                    {
                        text = textProperty.GetValue(m, null).ToString(),
                        value = valueProperty.GetValue(m, null).ToString()
                    });
                }
            }
            return JsonList<object>(objs);
        }

        /***************************************************************************************************************/
        #endregion

        #region 封装组装GridTree或Tree
        /// <summary>
        /// 封装组装GridTree或Tree
        /// </summary>
        protected void BuildGridTree<T>(T t, IService<T> service, JsonObject jsonObject, string parentIdField, List<string> listField, List<T> listSource = null) where T : BaseEntity, new()
        {
            IList<T> listChild = new List<T>();

            if (listSource == null)
            {
                listSource = service.Table.OrderBy(p => p.CreateDate).ToList();
            }

            foreach (T item in listSource)
            {
                if (item.GetFieldGuid(parentIdField) == t.Id)
                    listChild.Add(item);
            }

            jsonObject["id"] = new JsonProperty(t.Id.ToString());

            foreach (string field in listField)
                jsonObject[field] = new JsonProperty(t.GetField<string>(field));

            if (listChild != null && listChild.Count > 0)
            {
                jsonObject["children"] = new JsonProperty();

                foreach (T item in listChild)
                {
                    JsonObject jsonObject2 = new JsonObject();
                    BuildGridTree(item, service, jsonObject2, parentIdField, listField, listSource);
                    jsonObject["children"].Add(jsonObject2);
                }
            }
        }

        protected void BuildTree<T>(T t, IService<T> service, JsonObject jsonObject, string parentIdField, string nameField, List<T> listSource = null) where T : BaseEntity, new()
        {
            IList<T> listChild = new List<T>();

            if (listSource == null)
            {
                listSource = service.Table.OrderBy(p => p.CreateDate).ToList();
            }

            foreach (T item in listSource)
            {
                if (item.GetFieldGuid(parentIdField) == t.Id)
                    listChild.Add(item);
            }

            jsonObject["id"] = new JsonProperty(t.Id.ToString());
            jsonObject["text"] = new JsonProperty(t.GetField<string>(nameField));

            if (listChild != null && listChild.Count > 0)
            {
                jsonObject["children"] = new JsonProperty();

                foreach (T item in listChild)
                {
                    JsonObject jsonObject2 = new JsonObject();
                    BuildTree(item, service, jsonObject2, parentIdField, nameField, listSource);
                    jsonObject["children"].Add(jsonObject2);
                }
            }
        }
        #endregion

        #region 获取枚举数据源
        /// <summary>
        /// 获取枚举数据源
        /// </summary>
        public static List<SelectListItem> GetEnumDataSource(TypeIdentifyingEnum enumTypeIdentifying, bool isShowPleaseSelect, string pleaseSelectValue = "0")
        {
            IEnumerable<SelectListItem> listSource = from a in EnumDataCache.Instance.GetList(enumTypeIdentifying)
                                                     select new SelectListItem
                                                     {
                                                         Text = a.Name,
                                                         Value = a.Value
                                                     };

            List<SelectListItem> list = new List<SelectListItem>();

            if (isShowPleaseSelect)
            {
                list.Add(new SelectListItem { Value = pleaseSelectValue, Text = "--请选择--" });
            }

            list.AddRange(listSource);
            return list;
        }

        public static List<SelectListItem> GetEnumDataSource(TypeIdentifyingEnum enumTypeIdentifying, bool isShowPleaseSelect, string defaultValue, string pleaseSelectValue = "0")
        {
            IEnumerable<SelectListItem> listSource = from a in EnumDataCache.Instance.GetList(enumTypeIdentifying)
                                                     select new SelectListItem
                                                     {
                                                         Selected = a.Value == defaultValue,
                                                         Text = a.Name,
                                                         Value = a.Value
                                                     };

            List<SelectListItem> list = new List<SelectListItem>();

            if (isShowPleaseSelect)
            {
                list.Add(new SelectListItem { Value = pleaseSelectValue, Text = "--请选择--" });
            }

            list.AddRange(listSource);
            return list;
        }
        #endregion

        #region 微信公众账号缓存
        /// <summary>
        /// 设置公众账号的缓存
        /// </summary>
        private MpCenter mpcentercache;
        public MpCenter MpCenterCache
        {
            get
            {
                if (mpcentercache == null)
                {
                    if (_cacheManager.Get<MpCenter>("MpCenter") == null) //检测缓存是否存在,新增缓存并增加公众账户信息
                    {
                        MpCenter mp = _mpCenterService.GetALL().FirstOrDefault();

                        if (mp != null)
                        {
                            _cacheManager.Set("MpCenter", mp, 10080);
                            mpcentercache = mp;
                        }
                    }
                    else
                    {
                        mpcentercache = _cacheManager.Get<MpCenter>("MpCenter");
                    }
                }

                return mpcentercache;

            }
            set
            {
                mpcentercache = value;
            }

        }
        public string GetAccessToken()
        {
            MpCenter mp = MpCenterCache;
            try
            {
                if (mp != null && !string.IsNullOrEmpty(mp.AppID) && !string.IsNullOrEmpty(mp.AppSecret))
                {
                    var timespan = DateTime.Now - mp.GetTokenDate.Value;
                    if (timespan.TotalMinutes > 15 || string.IsNullOrEmpty(mp.AccessToken))
                    {
                        AccessTokenResult token = CommonApi.GetToken(mp.AppID, mp.AppSecret);
                        if (token != null && !string.IsNullOrEmpty(token.access_token))
                        {
                            mp.GetTokenDate = DateTime.Now;
                            mp.AccessToken = token.access_token;
                            _mpCenterService.Update(mp);
                            MpCenterCache = mp;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log4NetImpl.Write("GetAccessToken失败:" + e.Message);
                return "";
            }

            return mp.AccessToken;
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="OpenId">OpenId</param>
        /// <param name="content">发送的内容</param>
        /// <param name="token">不填则自动获取</param>
        /// <returns></returns>
        public bool SendMessage(string OpenId, string content, string token = "")
        {
            if (string.IsNullOrEmpty(token))
            {
                token = GetAccessToken();
            }
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    WxJsonResult errorResult = Custom.SendText(token, OpenId, content);
                    if (errorResult.errcode != ReturnCode.请求成功)
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Log4NetImpl.Write("SendMessage：失败" + e.Message);
                    return false;                    
                }

                return true;
            }
            return false;
        }
        #endregion

        #region Error
        Dictionary<string, string> ErrorDic = new Dictionary<string, string>();
        protected void AppendError(string key, string message)
        {
            ErrorDic[key] = message;
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            ViewBag.ErrorDic = ErrorDic;
        }
        #endregion

        #region protected override void OnActionExecuting(ActionExecutingContext filterContext)
        /// <summary>
        /// protected override void OnActionExecuting(ActionExecutingContext filterContext)
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //初始化参数集合
            RQuery = new QueryStringCollection(Request);
            base.OnActionExecuting(filterContext);
        }
        #endregion
    }
}