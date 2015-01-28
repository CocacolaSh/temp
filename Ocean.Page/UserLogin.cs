using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Ocean.Core.Caching;
using Ocean.Entity;
using System.Globalization;
using Ocean.Core.Utility;
using System.Web;
using Ocean.Core.Common;
using Ocean.Core.Infrastructure;
using Ocean.Services;

namespace Ocean.Page
{
    public class UserLogin
    {
        private readonly ILog log = LogManager.GetLogger(typeof(UserLogin));
        private static object objLock = new object();
        private static ICacheManager _cacheManager = new MemoryCacheManager();

        #region private static UserLogin instance 用户登录实例

        private static UserLogin instance = null;

        /// <summary>
        /// 用户登录实例
        /// </summary>
        public static UserLogin Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (objLock)
                    {
                        if (instance == null)
                        {
                            instance = new UserLogin();
                        }

                        return instance;
                    }
                }

                return instance;
            }
        }
        #endregion

        #region [储蓄会员登录信息Cookie名称]
        /// <summary>
        /// 储蓄会员登录信息Cookie名称
        /// </summary>
        private const string CookieUserName = "ocean.user";
        #endregion

        #region [创建用户的缓存key]
        /// <summary>
        /// 创建用户的缓存key
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns></returns>
        private static string GetUserCacheIdentification(Guid id)
        {
            const string UserCacheIdentification = "OnlineUserItems";
            return string.Format("{0}{1}", UserCacheIdentification, id.ToString().ToLower());
        }
        #endregion

        #region [将用户数据缓存在列表中] public void CacheUser(MpUser mpUser)
        /// <summary>
        /// 将用户数据缓存在列表中
        /// </summary>
        /// <param name="mpUser">MpUser</param>
        public void CacheUser(MpUser mpUser)
        {
            #region [设置唯一登录]
            if (this.CheckOnlyLogin())
            {
                mpUser.SessionId = Guid.NewGuid().ToString();
                HttpCookie cookie = HttpContext.Current.Request.Cookies["SessionId"] ?? new HttpCookie("SessionId");
                cookie.Value = mpUser.SessionId;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            #endregion

            if (null == mpUser)
            {
                throw new ArgumentNullException();
            }

            string userCacheIdentification = GetUserCacheIdentification(mpUser.Id);
            _cacheManager.Set(userCacheIdentification, mpUser, 10080);//10080=1周
            this.CacheLocalUser(mpUser);
        }
        #endregion

        #region [更新用户在本地队列的缓存] public void CacheLocalUser(MpUser mpUser)
        /// <summary>
        /// 更新用户在本地队列的缓存
        /// </summary>
        /// <param name="mpUser"></param>
        public void CacheLocalUser(MpUser mpUser)
        {
            ///保存一份登录到本服务器的在线用户队列
            if (_cacheManager.Get<Dictionary<Guid, MpUser>>("OnlineUserItems") == null) //检测缓存是否存在,新增缓存并增加用户信息
            {
                Dictionary<Guid, MpUser> dictionaryUsers = new Dictionary<Guid, MpUser>();
                dictionaryUsers.Add(mpUser.Id, mpUser);
                _cacheManager.Set("OnlineUserItems", dictionaryUsers, 10080);//10080=1周
            }
            else if (CheckOnlineUserCacheIsEmpty(mpUser.Id)) //检测缓存中是否存在当前用户的数据,更新用户信息
            {
                _cacheManager.Get<Dictionary<Guid, MpUser>>("OnlineUserItems")[mpUser.Id] = mpUser;
            }
            else //向缓存中新增用户信息 
            {
                _cacheManager.Get<Dictionary<Guid, MpUser>>("OnlineUserItems").Add(mpUser.Id, mpUser);
            }
        }
        #endregion

        #region 缓存公众账号

        #endregion

        #region [检测指定用户信息是否缓存在本服务器Cache队列中] public bool CheckOnlineUserCacheIsEmpty(Guid id)
        /// <summary>
        /// 检测指定用户信息是否缓存在本服务器Cache队列中
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns>bool</returns>
        public bool CheckOnlineUserCacheIsEmpty(Guid id)
        {
            Dictionary<Guid, MpUser> dictionaryUsers = _cacheManager.Get<Dictionary<Guid, MpUser>>("OnlineUserItems");

            if (dictionaryUsers == null || !dictionaryUsers.ContainsKey(id))
            {
                return false;
            }

            return true;
        }
        #endregion

        #region [创建Cookie] public void CreateCookie(Guid id, string openId, string passwordkey)
        /// <summary>
        /// 创建Cookie
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="openId">openId</param>
        /// <param name="passwordkey">密钥</param>
        public void CreateCookie(Guid id, string openId, string passwordkey)
        {
            CreateCookie(id, openId, passwordkey, 0);
        }
        #endregion

        #region [创建Cookie] public void CreateCookie(Guid id, string openId, string passwordkey, int minutes)
        /// <summary>
        /// 创建Cookie
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="openId">openId</param>
        /// <param name="passwordkey">密钥</param>
        /// <param name="minutes">保存时间,单位:分</param>
        public void CreateCookie(Guid id, string openId, string passwordkey, int minutes)
        {
            string ip = IpHelper.UserHostAddress;

            if (ip == "0.0.0.0")
            {
                return;
            }

            HttpCookie user = HttpContext.Current.Request.Cookies[CookieUserName];

            if (user != null)
            {
                this.UpdateCookie(id, openId, passwordkey, minutes);
                return;
            }

            string value = StrCrypt.EncryptDes(string.Format("{0}|{1}", id, StrCrypt.EncryptDes(openId, passwordkey)), ip);
            user = new HttpCookie(CookieUserName, value);

            if (minutes > 0)
            {
                user.Expires = DateTime.Now.AddMinutes(minutes);
            }

            HttpContext.Current.Response.Cookies.Add(user);
        }

        #endregion

        #region [更新Cookie] public void UpdateCookie(Guid id, string openId, string passwordkey)
        /// <summary>
        /// 更新Cookie
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="openId">openId</param>
        /// <param name="passwordkey">密钥</param>
        public void UpdateCookie(Guid id, string openId, string passwordkey)
        {
            this.UpdateCookie(id, openId, passwordkey, 0);
        }
        #endregion

        #region [更新Cookie] public void UpdateCookie(Guid id, string openId, string passwordkey, int minutes)
        /// <summary>
        /// 更新 Cookie
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="openId">openId</param>
        /// <param name="passwordkey">密钥</param>
        /// <param name="minutes">保存时间,单位:分</param>
        public void UpdateCookie(Guid id, string openId, string passwordkey, int minutes)
        {
            string ip = IpHelper.UserHostAddress;

            if (ip == "0.0.0.0")
            {
                return;
            }

            HttpCookie user = HttpContext.Current.Request.Cookies[CookieUserName];

            if (user == null)
            {
                CreateCookie(id, openId, passwordkey, minutes);
                return;
            }

            user.Value = StrCrypt.EncryptDes(string.Format("{0}|{1}", id, StrCrypt.EncryptDes(openId, passwordkey)), ip);

            if (minutes > 0)
            {
                user.Expires = DateTime.Now.AddMinutes(minutes);
            }

            HttpContext.Current.Response.Cookies.Set(user);
        }
        #endregion

        #region [获取当前登录用户信息] public UsersModel LoginUser
        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        public MpUser LoginUser
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return null;
                }

                MpUser mpUser;

                const string key = "Ocean_UserID";

                if (HttpContext.Current.Items.Contains(key))
                {
                    mpUser = HttpContext.Current.Items[key] as MpUser;
                    return mpUser;
                }

                string ip = IpHelper.UserHostAddress;

                if (ip == "0.0.0.0")
                {
                    this.log.Warn("登陆失败[701]：异常IP");
                    return null;
                }

                if (HttpContext.Current.Request.Cookies[CookieUserName] == null)
                {
                    this.log.Warn("登陆失败[702]：Cookie为空");
                    return null;
                }

                string value = HttpContext.Current.Request.Cookies[CookieUserName].Value;

                if (!StrCrypt.DecryptDes(ref value, ip))
                {
                    this.log.Warn("登陆失败[703]：DES解密出错");
                    return null;
                }

                string[] obj = value.Split('|');

                if (!StringValidate.IsGuid(obj[0]) || Guid.Parse(obj[0]) == Guid.Empty)
                {
                    this.log.Warn("登陆失败[704]：GUID格式解析错误");
                    return null;
                }

                mpUser = GetOnlineUserInfo(Guid.Parse(obj[0]));

                if (mpUser == null)
                {
                    var mpUserService = EngineContext.Current.Resolve<IMpUserService>();
                    MpUser mpUser2 = mpUserService.GetById(new Guid(obj[0]));

                    if (mpUser2 == null)
                    {
                        this.log.Warn("登陆失败[705]：找不到指定用户");
                        return null;
                    }

                    CacheUser(mpUser2);
                }

                if (StrCrypt.EncryptDes(mpUser.OpenID, mpUser.PasswordKey) != obj[1])
                {
                    this.log.Warn("登陆失败[706]：OpenId验证失败");
                    return null;
                }

                #region [检测是否唯一登录]
                if (this.CheckOnlyLogin())
                {
                    HttpCookie SessionId = HttpContext.Current.Request.Cookies["SessionId"];

                    if (SessionId == null || string.IsNullOrEmpty(SessionId.Value) || SessionId.Value != mpUser.SessionId)
                    {
                        this.log.Warn("登陆失败[707]：唯一登录失败");
                        return null;
                    }
                }
                #endregion

                HttpContext.Current.Items[key] = mpUser;
                return mpUser;
            }
        }
        #endregion

        #region [唯一登陆] public bool CheckOnlyLogin()
        /// <summary>
        /// 唯一登陆
        /// </summary>
        /// <returns></returns>
        private bool CheckOnlyLogin()
        {
            return false;
        }
        #endregion

        #region [获取指定在线用户信息] public MpUser GetOnlineUserInfo(Guid id)
        /// <summary>
        /// 获取在线用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MpUser GetOnlineUserInfo(Guid id)
        {
            Dictionary<Guid, MpUser> dictionaryUsers = _cacheManager.Get<Dictionary<Guid, MpUser>>("OnlineUserItems");

            if (dictionaryUsers == null || !dictionaryUsers.ContainsKey(id))
            {
                return null;
            }
            else
            {
                return dictionaryUsers[id];
            }
        }
        #endregion

        #region [退出登录] public void Logout()
        /// <summary>
        /// 会员退出登录
        /// </summary>
        public void Logout()
        {
            MpUser mpUser = LoginUser;

            if (mpUser != null)
            {
                string userCacheIdentification = GetUserCacheIdentification(mpUser.Id);
                _cacheManager.Remove(userCacheIdentification);
                UpdateCookie(Guid.Empty, "0", "00000000", 1);
                //移除出本地用户列表
                RemoveCacheUser(mpUser.Id);
            }
        }
        #endregion

        #region [移除执行在线用户的缓存信息] public void RemoveCacheUser(Guid id)
        /// <summary>
        /// 移除缓存中的会员
        /// </summary>
        /// <param name="uId"></param>
        public void RemoveCacheUser(Guid id)
        {
            //处理本服务器用户缓存队列
            if (CheckOnlineUserCacheIsEmpty(id))
            {
                try
                {
                    _cacheManager.Get<Dictionary<Guid, MpUser>>("OnlineUserItems").Remove(id);
                }
                catch { }
            }
        }
        #endregion

        //---------------------------------------------------------------------------------------------

        #region 获取COOKIE  public string GetCookie(string strName)
        public string GetCookie(string strName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
                return HttpContext.Current.Request.Cookies[strName].Value.ToString();
            return "";
        }
        #endregion

        #region 写入WriteCookie
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="strValue">过期时间(分钟)</param>
        public void WriteCookie(string strName, string strValue, int expires, string path, string domain)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            //cookie.HttpOnly = true;
            cookie.Value = strValue;
            if (expires != 0)
            {
                cookie.Expires = DateTime.Now.AddMinutes(expires);
            }
            if (path != "")
            {
                cookie.Path = path;
            }
            if (domain != "" && domain.ToLower() != "localhost")
            {
                cookie.Domain = domain;
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        #endregion

        #region 清除Cookie
        public void ClearCookie(string strName, string path, string domain)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                if (path != "")
                {
                    cookie.Path = path;
                }
                if (domain != "")
                {
                    cookie.Domain = domain;
                }
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        #endregion
    }
}