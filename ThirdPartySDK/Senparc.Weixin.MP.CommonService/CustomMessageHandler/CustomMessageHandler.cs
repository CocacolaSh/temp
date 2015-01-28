using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web.Configuration;
using Senparc.Weixin.MP.Agent;
using Senparc.Weixin.MP.Context;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Helpers;
using Ocean.Services;
using Ocean.Core.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using Ocean.Entity;
using Ocean.Framework.Configuration.global.config;
using Ocean.Framework.Communication;
using Ocean.Core.Logging;

namespace Senparc.Weixin.MP.CommonService.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        protected IMpReplyService MpReplyService
        {
            get
            {
                return EngineContext.Current.Resolve<IMpReplyService>();
            }
        }
        protected IMpUserService MpUserService
        {
            get
            {
                return EngineContext.Current.Resolve<IMpUserService>();
            }
        }
#if DEBUG
        string agentUrl = "";
        string agentToken = "27C455F496044A87";
        string souideaKey = "CNadjJuWzyX5bz5Gn+/XoyqiqMa5DjXQ";
#else
        //下面的Url和Token可以用其他平台的消息，或者到注册微信用用
        private string agentUrl = WebConfigurationManager.AppSettings["WeixinAgentUrl"];//
        private string agentToken = WebConfigurationManager.AppSettings["WeixinAgentToken"];//Token
        private string souideaKey = WebConfigurationManager.AppSettings["WeixinAgentSouideaKey"];//code by 微虾米
#endif
        public CustomMessageHandler(Stream inputStream, int maxRecordCount = 0)
            : base(inputStream, maxRecordCount)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            WeixinContext.ExpireMinutes = 3;
        }

        public override void OnExecuting()
        {
            //测试MessageContext.StorageData
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }
            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }

        /// <summary>
        /// 处理文字请求
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            MpReply reply = MpReplyService.GetALL(k => k.KeywordName == requestMessage.Content.Trim()).FirstOrDefault();
            MpUser fromUser = MpUserService.GetByOpenID(requestMessage.FromUserName);
            if (fromUser != null)
            {
                if (reply == null)
                {
                    reply = MpReplyService.GetALL(k => k.Action == "autoreply").FirstOrDefault();
                }
                if (reply != null)
                {
                    fromUser.LastVisitDate = DateTime.Now;
                    fromUser.IsSubscribe = true;
                    MpUserService.Update(fromUser);
                    //用户接入
                    if (reply.MpMaterial.IsChat == 1)
                    {
                        Client.StartKfMeeting(fromUser.Id);
                        return null;
                    }
                    //用户断开
                    if (reply.MpMaterial.IsChat == 2 || fromUser.LastVisitDate.AddMinutes(120) < DateTime.Now)
                    {
                        fromUser.UserState = 0;
                        Client.EndKfMeeting(fromUser.Id);
                        return null;
                    }

                    if (fromUser.UserState == 1)
                    {
                        //发送消息给客服
                        Client.SendPrivateMessage(fromUser.Id, requestMessage.Content);
                        return null;
                    }
                    return ResponseMsg(reply.MpMaterial);
                }
            }

            return null;
        }

        private IResponseMessageBase ResponseMsg(MpMaterial MpMaterial)
        {
            if (MpMaterial != null && MpMaterial.MpMaterialItems != null)
            {
                if (MpMaterial.TypeName == "text")
                {
                    var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
                    responseMessage.Content = MpMaterial.MpMaterialItems.FirstOrDefault().ReplyContent;
                    return responseMessage;
                }
                else if (MpMaterial.TypeName == "news")
                {
                    var responseMessage = base.CreateResponseMessage<ResponseMessageNews>();
                    foreach (var item in MpMaterial.MpMaterialItems)
                    {
                        responseMessage.Articles.Add(new Article()
                        {
                            Title = item.Title,
                            Description = string.IsNullOrEmpty(item.Summary) ? (string.IsNullOrEmpty(item.Description) ? "暂无..." : "") : item.Summary,
                            PicUrl = GlobalConfig.GetConfig()["ResourceDomain"] + item.PicUrl,
                            Url = string.IsNullOrEmpty(item.Url) ? "http://wx.ssrcb.com/MpMaterial/MaterialDetail?id=" + item.Id.ToString() : item.Url
                        });
                    }
                    return responseMessage;
                }
            }
            return null;
        }

        /// <summary>
        /// 处理位置请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnLocationRequest(RequestMessageLocation requestMessage)
        {
            var locationService = new LocationService();
            var responseMessage = locationService.GetResponseMessage(requestMessage as RequestMessageLocation);
            return responseMessage;
        }

        /// <summary>
        /// 处理图片请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            MpReply reply = MpReplyService.GetALL(k => k.Action == "autoreply").FirstOrDefault();
            if (reply != null)
            {
                return ResponseMsg(reply.MpMaterial);
            }
            return null;
        }

        /// <summary>
        /// 处理语音请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnVoiceRequest(RequestMessageVoice requestMessage)
        {
            MpReply reply = MpReplyService.GetALL(k => k.Action == "autoreply").FirstOrDefault();
            if (reply != null)
            {
                return ResponseMsg(reply.MpMaterial);
            }
            return null;
        }

        /// <summary>
        /// 处理视频请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnVideoRequest(RequestMessageVideo requestMessage)
        {
            MpReply reply = MpReplyService.GetALL(k => k.Action == "autoreply").FirstOrDefault();
            if (reply != null)
            {
                return ResponseMsg(reply.MpMaterial);
            }
            return null;
        }

        /// <summary>
        /// 处理链接消息请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnLinkRequest(RequestMessageLink requestMessage)
        {
            MpReply reply = MpReplyService.GetALL(k => k.Action == "autoreply").FirstOrDefault();
            if (reply != null)
            {
                return ResponseMsg(reply.MpMaterial);
            }
            return null;
        }

        /// <summary>
        /// 处理事件请求（这个方法一般不用重写，这里仅作为示例出现。除非需要在判断具体Event类型以外对Event信息进行统一操作
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEventRequest(RequestMessageEventBase requestMessage)
        {
            var eventResponseMessage = base.OnEventRequest(requestMessage);//对于Event下属分类的重写方法，见：CustomerMessageHandler_Events.cs
            //TODO: 对Event信息进行统一操作
            return eventResponseMessage;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            /* 所有没有被处理的消息会默认返回这里的结果，
             * 因此，如果想把整个微信请求委托出去（例如需要使用分布式或从其他服务器获取请求），
             * 只需要在这里统一发出委托请求，如：
             * var responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
             * return responseMessage;
             */

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这条消息来自DefaultResponseMessage。";
            return responseMessage;
        }
    }
}
