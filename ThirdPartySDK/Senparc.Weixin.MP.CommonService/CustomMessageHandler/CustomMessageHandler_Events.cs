using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Senparc.Weixin.MP.Agent;
using Senparc.Weixin.MP.Context;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.MessageHandlers;
using Ocean.Services;
using Ocean.Core.Infrastructure;
using System.Collections.Generic;
using Ocean.Entity;
using Ocean.Core.Logging;
using Ocean.Framework.Communication;

namespace Senparc.Weixin.MP.CommonService.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// </summary>
    public partial class CustomMessageHandler
    {
        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            IResponseMessageBase reponseMessage = null;
            //菜单点击，需要跟创建菜单时的Key匹配
            switch (requestMessage.EventKey)
            {
                case "Near":
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                        reponseMessage = strongResponseMessage;
                        strongResponseMessage.Content = "您点击了底部按钮。\r\n为了测试微信软件换行bug的应对措施，这里做了一个——\r\n换行";
                    }
                    break;
                case "ContactUs":
                    {
                        MpUser fromUser = MpUserService.GetByOpenID(requestMessage.FromUserName);
                        if (fromUser != null)
                        {
                            Client.StartKfMeeting(fromUser.Id);
                        }
                        return null;
                    }
                    break;
                case "NearBy":
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                        reponseMessage = strongResponseMessage;
                        strongResponseMessage.Content = "首先切换到对话模式，看到输入框旁边那个“+”了没，选择“位置”后点击“发送”，即可查询周边网点信息。";
                    }
                    break;
                case "SubClickRoot_Music":
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageMusic>();
                        reponseMessage = strongResponseMessage;
                        strongResponseMessage.Music.MusicUrl = "http://weixin.senparc.com/Content/music1.mp3";
                    }
                    break;
                case "SubClickRoot_Agent"://代理消息
                    {
                        //获取返回的XML
                        DateTime dt1 = DateTime.Now;
                        reponseMessage = MessageAgent.RequestResponseMessage(this, agentUrl, agentToken, RequestDocument.ToString());
                        //上面的方法也可以使用扩展方法：this.RequestResponseMessage(this,agentUrl, agentToken, RequestDocument.ToString());

                        DateTime dt2 = DateTime.Now;

                        if (reponseMessage is ResponseMessageNews)
                        {
                            (reponseMessage as ResponseMessageNews)
                                .Articles[0]
                                .Description += string.Format("\r\n\r\n代理过程总耗时：{0}毫秒", (dt2 - dt1).Milliseconds);
                        }
                    }
                    break;
                case "Member"://托管代理会员信息
                    {
                        //原始方法为：MessageAgent.RequestXml(this,agentUrl, agentToken, RequestDocument.ToString());//获取返回的XML
                        reponseMessage = this.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
                    }
                    break;
                case "OAuth"://OAuth授权测试
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
                        strongResponseMessage.Articles.Add(new Article()
                        {
                            Title = "OAuth2.0测试",
                            Description = "点击【查看全文】进入授权页面。\r\n注意：此页面仅供测试，测试号随时可能过期。请将此DEMO部署到您自己的服务器上，并使用自己的appid和secret。",
                            Url = "http://weixin.senparc.com/oauth2",
                        });
                        reponseMessage = strongResponseMessage;
                    }
                    break;
            }

            return reponseMessage;
        }

        public override IResponseMessageBase OnEvent_EnterRequest(RequestMessageEvent_Enter requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = "您刚才发送了ENTER事件请求。";
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {
            MpUser fromUser = MpUserService.GetByOpenID(requestMessage.FromUserName);
            if (fromUser != null)
            {
                fromUser.LocationX = requestMessage.Latitude;
                fromUser.LocationY = requestMessage.Longitude;
                fromUser.LocationLabel = string.IsNullOrEmpty(fromUser.LocationLabel) ? "" : fromUser.LocationLabel;
                fromUser.LastVisitDate = DateTime.Now;
                MpUserService.Update(fromUser);
            }
            return null;
        }

        /// <summary>
        /// 订阅（关注）事件
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            Log4NetImpl.Write("关注事件触发：");
            MpUser fromUser = MpUserService.GetByOpenID(requestMessage.FromUserName);
            if (fromUser != null)
            {
                Log4NetImpl.Write("EventKey:" + requestMessage.EventKey);
                fromUser.IsSubscribe = true;
                if (!string.IsNullOrEmpty(requestMessage.EventKey))
                {
                    fromUser.SceneId = Convert.ToInt32(requestMessage.EventKey.Replace("qrscene_", ""));
                    Log4NetImpl.Write("OnEvent_SubscribeRequest:" + fromUser.SceneId.ToString());
                }
                fromUser.LastVisitDate = DateTime.Now;
                MpUserService.Update(fromUser);
            }
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            MpReply reply = MpReplyService.GetALL(k => k.Action == "beadded").FirstOrDefault();
            if (reply != null)
            {
                return ResponseMsg(reply.MpMaterial);
            }
            return responseMessage;
        }

        /// <summary>
        /// 订阅（关注）事件
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScanRequest(RequestMessageEvent_Scan requestMessage)
        {
            Log4NetImpl.Write("Scan关注事件触发：");
            MpUser fromUser = MpUserService.GetByOpenID(requestMessage.FromUserName);
            if (fromUser != null)
            {
                if (!string.IsNullOrEmpty(requestMessage.EventKey))
                {
                    fromUser.SceneId = Convert.ToInt32(requestMessage.EventKey);
                    Log4NetImpl.Write("OnEvent_ScanRequest:" + fromUser.SceneId.ToString());
                }
                fromUser.LastVisitDate = DateTime.Now;
                MpUserService.Update(fromUser);
            }
            return null;
        }

        /// <summary>
        /// 退订
        /// 实际上用户无法收到非订阅账号的消息，所以这里可以随便写。
        /// unsubscribe事件的意义在于及时删除网站应用中已经记录的OpenID绑定，消除冗余数据。并且关注用户流失的情况。
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        {
            MpUser fromUser = MpUserService.GetByOpenID(requestMessage.FromUserName);
            if (fromUser != null)
            {
                fromUser.IsSubscribe = false;
                fromUser.LastVisitDate = DateTime.Now;
                MpUserService.Update(fromUser);
            }
            return null;
        }
    }
}