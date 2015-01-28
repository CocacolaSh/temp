using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ocean.Entity;
using Ocean.Framework.Sms;
using Ocean.Core.Utility;
using Ocean.Framework.Configuration.global.config;
using Ocean.Services;
using Ocean.Core.Infrastructure;
using log4net;
using Ocean.Core.Logging;
using Ocean.Framework.Mvc.Handlers;

namespace Ocean.Page
{
    [OceanHandleError(ErrorType="Web")]
    public class WebBaseController : PageBaseController
    {
        private static ILog _log = LogManager.GetLogger(typeof(WebBaseController));
        protected readonly IMpUserService _mpUserService;
        private readonly IMobileCodeService _mobileCodeService;

        public WebBaseController()
        {
            this._mpUserService = EngineContext.Current.Resolve<IMpUserService>();
            this._mobileCodeService = EngineContext.Current.Resolve<IMobileCodeService>();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        #region 微信用户登录验证
        public virtual int UserLoginValidate(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            return 1;
        }

        #region MpUserCookie的操作
        protected string WriteMpUserCookie(MpUser user)
        {
            Session["LUserID"] = user.Id.ToString();//user.ID;
            StringBuilder userBuilder = new StringBuilder(Session.SessionID);
            userBuilder.Append("|");
            userBuilder.Append(user.Id.ToString());//user.ID
            userBuilder.Append("|");
            userBuilder.Append(user.OpenID.ToString());
            userBuilder.Append("|");
            userBuilder.Append(user.HeadImgUrl ?? "");
            userBuilder.Append("|");
            userBuilder.Append((!string.IsNullOrEmpty(user.NickName) ? Url.Encode(user.NickName) : ""));
            userBuilder.Append("|");
            userBuilder.Append(user.MpGroupID.ToString());
            userBuilder.Append("|");
            userBuilder.Append((!string.IsNullOrEmpty(user.Name) ? Url.Encode(user.Name) : ""));
            userBuilder.Append("|");
            userBuilder.Append(user.Sex);
            userBuilder.Append("|");
            userBuilder.Append(user.OrginID);
            userBuilder.Append("|");
            userBuilder.Append("");
            userBuilder.Append("|");
            userBuilder.Append("");
            userBuilder.Append("|");
            userBuilder.Append("");
            UserLogin.Instance.WriteCookie("LUser", userBuilder.ToString(), 0, "/", "localhost");            
            return userBuilder.ToString();
        }

        protected void ClearCookie()
        {
            MpUserID = Guid.Empty;
            Session.Remove("LUserID");
            UserLogin.Instance.ClearCookie("LUser", "/", "");
            OpenID = "";
            MpUserID = Guid.Empty;
        }
        #endregion

        #endregion

        #region 微信用户相关属性
        #region MpUserID
        private Guid mpuserid = Guid.Empty;
        protected Guid MpUserID
        {
            get
            {
                if (mpuserid == Guid.Empty)
                {
                    string mpuserCookie = UserLogin.Instance.GetCookie("LUser");
                    if (Session["LUserID"] == null)
                    {
                        if (!string.IsNullOrEmpty(mpuserCookie))
                        {
                            if (MpUserArr != null && MpUserArr.Length > 5)
                            {
                                mpuserid = Guid.Parse(MpUserArr[1]);
                                Session["LUserID"] = mpuserid.ToString();
                            }
                        }
                    }
                    else
                    {
                        mpuserid = Guid.Parse(Session["LUserID"].ToString());
                        if (string.IsNullOrEmpty(mpuserCookie))
                        {
                            MpUser mpuser = _mpUserService.GetById(mpuserid);
                            if (mpuser != null)
                            {
                                WriteMpUserCookie(mpuser);
                            }
                        }
                    }
                }
                return mpuserid;
            }
            set
            {
                mpuserid = value;
            }
        }
        #endregion

        #region OpenID
        private string openid = "";
        protected string OpenID
        {
            get
            {
                if (string.IsNullOrEmpty(openid))
                {
                    if (MpUserArr != null && MpUserArr.Length > 5)
                    {
                        openid = MpUserArr[2];
                    }
                }
                return openid;
            }
            set
            {
                openid = value;
            }
        }
        #endregion

        #region NickName
        private string nickname = "";
        protected string NickName
        {
            get
            {
                if (string.IsNullOrEmpty(nickname))
                {
                    if (MpUserArr != null && MpUserArr.Length > 5)
                    {
                        if (!string.IsNullOrEmpty(MpUserArr[4]))
                        {
                            nickname = Server.UrlDecode(MpUserArr[4]);
                        }
                    }
                }
                return nickname;
            }
            set
            {
                nickname = value;
            }
        }
        #endregion

        #region Name
        private string name = "";
        protected string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    if (MpUserArr != null && MpUserArr.Length > 5)
                    {
                        if (!string.IsNullOrEmpty(MpUserArr[6]))
                        {
                            name = Server.UrlDecode(MpUserArr[6]);
                        }
                    }
                }
                return name;
            }
            set
            {
                name = value;
            }
        }
        #endregion

        #region Sex
        private string sex = "";
        protected string Sex
        {
            get
            {
                if (string.IsNullOrEmpty(sex))
                {
                    if (MpUserArr != null && MpUserArr.Length > 5)
                    {
                        if (!string.IsNullOrEmpty(MpUserArr[7]))
                        {
                            sex = MpUserArr[7];
                        }
                    }
                }
                return sex == "1" ? "先生" : "女士";
            }
            set
            {
                sex = value;
            }
        }
        #endregion

        #region MpUsrArr
        private string[] mpuserarr;
        protected string[] MpUserArr
        {
            get
            {
                if (mpuserarr == null)
                {
                    string mpuserCookie = UserLogin.Instance.GetCookie("LUser");
                    if (!string.IsNullOrEmpty(mpuserCookie))
                    {
                        mpuserarr = mpuserCookie.Split(new char[] { '|' });
                    }
                }
                return mpuserarr;
            }
            set
            {
                mpuserarr = value;
            }
        }
        #endregion
        #endregion

        #region 验证码
        private const int disSecond = 120;
        public ActionResult GetMobileCodeSeconds(int operateType)
        {
            int second = disSecond;
            string mobile = UserLogin.Instance.GetCookie("mobilecode" + operateType.ToString());

            if (!string.IsNullOrEmpty(mobile) && mobile.Split(',').Length > 1)
            {
                string[] mobileArr = mobile.Split(',');
                MobileCode code = _mobileCodeService.GetMobileCode(this.MpUserID, TypeConverter.StrToInt(mobileArr[1]));

                if (code != null)
                {
                    TimeSpan tspan = System.DateTime.Now.Subtract(code.CreateDate);

                    if (tspan.TotalSeconds < disSecond && tspan.TotalSeconds > 0)
                    {
                        second = disSecond - int.Parse(tspan.TotalSeconds.ToString("#"));
                    }
                }
            }

            return Json(new { count = second });
        }

        public ActionResult GetMobileCode(string mobile, int operateType)
        {
            if (_mobileCodeService.GetMobileCodeCount(this.MpUserID, operateType) > 5)
            {
                return JsonMessage(false, "短信获取失败[您当天获取验证码的次数不能超过5次]");
            }
            else
            {
                UserLogin.Instance.WriteCookie("mobilecode" + operateType, mobile + "," + operateType.ToString(), 10, "/", "");
                //验证参数格式
                long lSMID = 0;
                string smID = GlobalConfig.GetConfig()["SMID"];

                if (!long.TryParse(smID, out lSMID))
                {
                    _log.Debug("SMID格式错误");
                    return JsonMessage(false, "验证码获取失败,请稍后重试");
                }
#if DEBUG
            //创建验证码
                MobileCode mobileCode = new MobileCode
                {
                    MpUserId = MpUserID,
                    Mobile = mobile,
                    Code = StringHelper.GetRandomInt(6),
                    Time = 0,
                    OperationType = operateType,
                    Status = 0
                };

                _mobileCodeService.Insert(mobileCode);
                return JsonMessage(true, string.Format("{0}", mobileCode.Code));
#else
                    string message = string.Empty;
                try
                {
                    //短信猫发送短信
                    IList<string> mobiles = new List<string>();
                    mobiles.Add(mobile);
                    string code = StringHelper.GetRandomInt(6);
                    string yzMsg = string.Empty;

                    if (operateType == 1)
                    {
                        yzMsg = string.Format("您本次申请福农宝额度调整的验证码为{0}，验证码10分钟内有效，详询4000896336。", code);
                    }
                    else if (operateType==2)
                    {
                        yzMsg = string.Format("您本次申请贷款的验证码为{0}，验证码10分钟内有效，详询4000896336。", code);
                    }
                    else if (operateType == 3)
                    {
                        yzMsg = string.Format("您本次申请POS的验证码为{0}，验证码10分钟内有效，详询4000896336。", code);
                    }
                    else if (operateType == 4)
                    {
                        yzMsg = string.Format("您本次申请摇奖的验证码为{0}，验证码10分钟内有效，详询4000896336。", code);
                    }
                    else
                    {
                        yzMsg = string.Format("您本次查询POS的验证码为{0}，验证码10分钟内有效，详询4000896336。", code);
                    }
                    SendSmsRequest sendSmsRequest = new SendSmsRequest(mobiles, yzMsg, lSMID);

                    //if(true)
                    if (new SmsClient().Execute(sendSmsRequest, ref message))
                    {
                        //创建验证码
                        MobileCode mobileCode = new MobileCode
                        {
                            MpUserId = MpUserID,
                            Mobile = mobile,
                            Code = code,
                            Time = 0,
                            OperationType = operateType,
                            Status = 0
                        };
                        _mobileCodeService.Insert(mobileCode);
                        //return JsonMessage(true, "验证码获取成功");
                        return JsonMessage(true, string.Format("{0}", string.Empty));
                    }
                    else
                    {
                        Log4NetImpl.Write("验证码获取失败:" + message);
                        SmsClient.ReleaseAPIClient();//释放资源
                        return JsonMessage(false, "验证码获取失败");
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = "";

                    if (string.IsNullOrWhiteSpace(message))
                        errMsg = string.Format("验证码获取失败[{0}]",ex.Message);
                    else
                        errMsg = string.Format("验证码获取失败[{0}]{1}", message, ex.Message);

                    Log4NetImpl.Write(errMsg);
                    SmsClient.ReleaseAPIClient();//释放资源
                    return JsonMessage(false, "验证码获取失败");
                }
#endif
            }
        }
        #endregion

    }
}