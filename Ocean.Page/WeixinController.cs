using System;
using System.Web.Mvc;
using Ocean.Services;
using System.IO;
using Ocean.Core.Infrastructure;
using Senparc.Weixin.MP.MvcExtension;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.CommonService.CustomMessageHandler;
using Ocean.Entity;
using Senparc.Weixin.MP.AdvancedAPIs;
using Ocean.Core.Logging;
using System.Threading.Tasks;
using Senparc.Weixin.MP.Helpers;


namespace Ocean.Page
{
    public class WeixinController : WebBaseController
    {
        public readonly string Token = "ssrcb";//与微信公众账号后台的Token设置保持一致，区分大小写。

        protected readonly IMpUserService MpUserService;
        protected readonly IMpUserGroupService MpUserGroupService;

        public WeixinController(IMpUserService MpUserService, IMpUserGroupService MpUserGroupService)
        {
            this.MpUserService = MpUserService;
            this.MpUserGroupService = MpUserGroupService;
            Token = MpCenterCache.Token;
        }

        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(string signature, string timestamp, string nonce, string echostr)
        {
            string content = Request.QueryString["content"];


            string postdata = "<xml><ToUserName><![CDATA[gh_b96e4d6d32ee]]></ToUserName><FromUserName>";
            postdata += "<![CDATA[oM-9at0Axxp8bHTZfPfja3hMG4Mg]]></FromUserName> <CreateTime>1348831860</CreateTime><MsgType>";
            postdata += "<![CDATA[event]]></MsgType><Event><![CDATA[VIEW]]></Event><EventKey><![CDATA[www.qq.com]]></EventKey></xml>";
            

            //语音
            //图片
            //string result = HttpHelper.GetHtml("http://localhost:1102/weixin/index?signature=785a198b81b94ec5c81a7ee768b1f5ce4c9aae7a&timestamp=1384222258&nonce=1384547483", postdata, true); 


            if (CheckSignature.Check(signature, timestamp, nonce, Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("");
                //return Content("failed:" + signature + "," + CheckSignature.GetSignature(timestamp, nonce, Token) + "。" +
                //    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }

        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// PS：此方法为简化方法，效果与OldPost一致。
        /// v0.8之后的版本可以结合Senparc.Weixin.MP.MvcExtension扩展包，使用WeixinResult，见MiniPost方法。
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(string signature, string timestamp, string nonce, string echostr)
        {
            if (!CheckSignature.Check(signature, timestamp, nonce, Token))
            {
                Log4NetImpl.Write("WeixinController-Post失败:参数错误");
                return Content("参数错误！");
            }

            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
            var maxRecordCount = 3;

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new CustomMessageHandler(Request.InputStream, maxRecordCount);

            try
            {
                //测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。
                //messageHandler.RequestDocument.Save(Server.MapPath("~/App_Data/" + DateTime.Now.Ticks + "_Request_" + messageHandler.RequestMessage.FromUserName + ".txt"));
                //执行微信处理过程
                AysnUser(messageHandler.RequestMessage.FromUserName);
                messageHandler.Execute();
                //测试时可开启，帮助跟踪数据
                //messageHandler.ResponseDocument.Save(Server.MapPath("~/App_Data/" + DateTime.Now.Ticks + "_Response_" + messageHandler.ResponseMessage.ToUserName + ".txt"));
                
                //return Content(messageHandler.ResponseDocument.ToString());//v0.7-
                if (messageHandler.ResponseDocument == null)
                {
                    return Content("");
                }
                //return new FixWeixinBugWeixinResult(messageHandler);//为了解决官方微信5.0软件换行bug暂时添加的方法，平时用下面一个方法即可
                return new WeixinResult(messageHandler);//v0.8+
            }
            catch (Exception ex)
            {
                //using (TextWriter tw = new StreamWriter(Server.MapPath("~/App_Data/Error_" + DateTime.Now.Ticks + ".txt")))
                //{
                //    tw.WriteLine("ExecptionMessage:" + ex.Message);
                //    tw.WriteLine(ex.Source);
                //    tw.WriteLine(ex.StackTrace);
                //    //tw.WriteLine("InnerExecptionMessage:" + ex.InnerException.Message);

                //    if (messageHandler.ResponseDocument != null)
                //    {
                //        tw.WriteLine(messageHandler.ResponseDocument.ToString());
                //    }
                //    tw.Flush();
                //    tw.Close();
                //}
                Log4NetImpl.Write("Post失败:" + ex.Message);
                return Content("");
            }
        }

        public MpUser AysnUser(string OpenId)
        {
            MpUser currUser = MpUserService.GetByOpenID(OpenId);
            if (currUser == null && !string.IsNullOrEmpty(OpenId))
            {
                currUser = new MpUser();
                currUser.MpID = MpCenterCache.Id;
                currUser.CreateDate = DateTime.Now;
                currUser.MpGroupID = MpUserGroupService.GetSystemGroup("默认分组").Id;
                currUser.IsSubscribe = true;
                currUser.LastVisitDate = DateTime.Now;
                currUser.OpenID = OpenId;
                currUser.OrginID = MpCenterCache.OriginID;
                currUser.UserState = 0;
                currUser.SceneId = 0;
                string token = GetAccessToken();
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        UserInfoJson info = Senparc.Weixin.MP.AdvancedAPIs.User.Info(token, OpenId);
                        if (info != null)
                        {
                            currUser.City = info.city;
                            currUser.Country = info.country;
                            currUser.HeadImgUrl = info.headimgurl;
                            currUser.Language = info.language;
                            currUser.NickName = info.nickname;
                            currUser.Province = info.province;
                            currUser.SubscribeDate = DateTimeHelper.GetDateTimeFromXml(info.subscribe_time);
                            currUser.Sex = info.sex;
                        }
                    }
                    catch (Exception e)
                    {
                        Log4NetImpl.Write("AysnUser失败:" + e.Message);
                    }
                }
                MpUser currUserCopy = MpUserService.GetByOpenID(OpenId);
                if (currUserCopy==null||currUserCopy.Id == Guid.Empty)
                {
                    MpUserService.Insert(currUser);
                    Log4NetImpl.Write("AysnUserInsert结束");

                }
                else
                {
                    currUser.Id = currUserCopy.Id;
                    MpUserService.Update(currUser);
                    Log4NetImpl.Write("AysnUserUpdate结束");

                }
                
            }
            else
            {
                if (!currUser.IsSubscribe)
                {
                    currUser.IsSubscribe = true;
                    currUser.SubscribeDate = DateTime.Now;
                    MpUserService.Update(currUser);
                }
            }
            return currUser;
        }
    }
}