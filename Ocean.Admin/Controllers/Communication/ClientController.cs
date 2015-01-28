using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Diagnostics;
using Ocean.Communication.Services;
using Ocean.Communication.ContentProviders.Core;
using log4net;
using Ocean.Communication.Comet;
using Ocean.Communication.Model;
using Ocean.Communication.Commands;
using Microsoft.Security.Application;
using Ocean.Entity;
using Ocean.Communication.Common;
using Ocean.Services;
using Ocean.Core.Logging;
using Ocean.Core;

namespace WebSite.Controllers.Communication
{
    /// <summary>
    /// 访客端通信控制器
    /// </summary>
    public class ClientController : AsyncController, ICommunicationService
    {
        private readonly IKfNumberService _kfNumberService;
        private readonly IKfMeetingService _kfMeetingService;
        private readonly IKfMeetingMessageService _kfMeetingMessageService;
        private readonly IMpUserService _mpUserService;

        public ClientController(IKfNumberService kfNumberService, 
            IKfMeetingService kfMeetingService, 
            IKfMeetingMessageService kfMeetingMessageService,
            IMpUserService mpUserService)
        {
            this._kfNumberService = kfNumberService;
            this._kfMeetingService = kfMeetingService;
            this._kfMeetingMessageService = kfMeetingMessageService;
            this._mpUserService = mpUserService;
        }

        #region 资源处理器
        /// <summary>
        /// 资源处理器
        /// </summary>
        private readonly IResourceProcessor _resourceProcessor = new ResourceProcessor();
        #endregion

        #region [创建日志记录组件实例]
        /// <summary>
        /// 创建日志记录组件实例
        /// <remarks>
        /// private ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// </remarks>
        /// </summary>        
        private readonly ILog log = LogManager.GetLogger(typeof(ClientController));

        #endregion

        #region [发送者Id] public Guid SendUserId
        /// <summary>
        /// 发送者Id
        /// </summary>
        public Guid SendUserId
        {
            get
            {
                return new Guid(Request["SendUserId"]);
            }
        }
        #endregion

        #region [接收者Id] public Guid ReceiveUserId
        /// <summary>
        /// 接收者Id
        /// </summary>
        public Guid ReceiveUserId
        {
            get
            {
                KfMeeting kfMeeting = _kfMeetingService.GetUnique(m => m.MpUserId == SendUserId && !m.IsEnd, "CreateDate", false);

                if (kfMeeting != null)
                {
                    return kfMeeting.KfNumberId;
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }
        #endregion

        #region [会话Id] KfMeetingId
        /// <summary>
        /// [会话Id] KfMeetingId
        /// </summary>
        public Guid KfMeetingId
        {
            get
            {
                KfMeeting kfMeeting = _kfMeetingService.GetUnique(m => m.MpUserId == SendUserId && !m.IsEnd, "CreateDate", false);

                if (kfMeeting != null)
                {
                    return kfMeeting.Id;
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }
        #endregion

        #region 凭证
        /// <summary>
        /// 凭证
        /// </summary>
        public string Token
        {
            get
            {
                return Request["token"];
            }
        }
        #endregion

        #region[开始异步调用消息] public void MonitoringItemAsync()
        /// <summary>
        /// 开始异步调用消息
        /// </summary>
        [AsyncTimeout(60000)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "/Views/Shared/Error.cshtml")]
        public void MonitoringItemAsync()
        {
            AsyncManager.OutstandingOperations.Increment();
            //初始化
            CometWaitRequest cometWaitRequest = new CometWaitRequest(SendUserId);
            cometWaitRequest.execute = new CometWaitRequest.Execute(EndExecute);
            CometThreadPool.QueueCometWaitRequest(cometWaitRequest);
        }
        #endregion

        #region[异步调用消息,实现委托] public bool EndExecute(ArgumentModel modelArgument)
        /// <summary>
        /// 异步调用消息,实现委托
        /// </summary>
        public bool EndExecute(ArgumentModel modelArgument)
        {
            if (modelArgument == null)
            {
                AsyncManager.Parameters["modelArgument"] = null;
                AsyncManager.OutstandingOperations.Decrement();
                return false;
            }
            else
            {
                AsyncManager.Parameters["modelArgument"] = modelArgument;
                AsyncManager.OutstandingOperations.Decrement();
                return true;
            }
        }
        #endregion

        #region 对消息按时间进行排序
        /// <summary>
        /// 对消息按时间进行排序
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        private static int CompareMessage(MessageModel m1, MessageModel m2)
        {
            int flag = m1.SendTimeOfService.CompareTo(m2.SendTimeOfService);
            return flag;
        }
        #endregion

        #region 对通知按时间进行排序
        /// <summary>
        /// 对通知按时间进行排序
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        private static int CompareNotice(NoticeModel m1, NoticeModel m2)
        {
            int flag = m1.LastActiveDate.CompareTo(m2.LastActiveDate);
            return flag;
        }
        #endregion

        #region[异步调用信息完成] public string MonitoringItemCompleted(ArgumentModel modelArgument)
        /// <summary>
        /// 异步调用信息完成
        /// </summary>
        public string MonitoringItemCompleted(ArgumentModel modelArgument)
        {
            try
            {
                if (modelArgument == null)
                {
                    return string.Empty;
                }

                //解析聊天内容
                foreach (MessageModel model in modelArgument.listMessage)
                {
                    //model.Message = Regex.Replace(model.Message, @"\[img:([0-9]+)\]", "<img src=\"/Content/Images/Chat/Smilies/Default/$1.gif\" />");
                    //处理换行
                    model.Message = model.Message.Replace("\r\n", "<br />").Replace("\n", "<br />");
                    //处理聊天内容里的链接
                    HashSet<string> links;
                    model.Message = ExtractUrls.TransformAndExtractUrls(model.Message, out links);

                    if (links.Count > 0)
                    {
                        IList<string> listContentExt = ProcessUrls(links);
                        foreach (string value in listContentExt)
                        {
                            model.Message = model.Message + "<br />" + value;
                        }
                    }
                }

                //组装返回到客户端的json数据
                JsonObject json = new JsonObject((a) =>
                {
                    a["dataMessage"] = new JsonProperty();
                    modelArgument.listMessage.Sort(CompareMessage);//对数组进行排序
                    foreach (MessageModel model in modelArgument.listMessage)
                    {
                        a["dataMessage"].Add(new JsonObject((b) =>
                        {
                            b["SendUserId"] = new JsonProperty(model.SendUserId);
                            b["Message"] = new JsonProperty(model.Message);
                            b["SendTimeOfService"] = new JsonProperty(model.SendTimeOfService.ToString("yyyy-MM-dd HH:mm:ss"));
                        }));
                    }
                    a["dataNotice"] = new JsonProperty();
                    modelArgument.listNotice.Sort(CompareNotice);
                    foreach (NoticeModel model in modelArgument.listNotice)
                    {
                        a["dataNotice"].Add(new JsonObject((b) =>
                        {
                            b["SendUserId"] = new JsonProperty(model.SendUserId);
                            b["SendNickName"] = new JsonProperty(model.SendNickName);
                            b["NoticeType"] = new JsonProperty(model.NoticeType);
                            b["Status"] = new JsonProperty(model.Status);
                            b["Args"] = new JsonProperty(model.Args);
                        }));
                    }
                });

                return json.ToString();
            }
            catch (Exception ex)
            {
                this.log.Warn(ex);
                return string.Empty;
            }
        }
        #endregion

        #region [解析消息里的Url] private void ProcessUrls(IEnumerable<string> links)
        /// <summary>
        /// 解析消息里的Url
        /// </summary>
        private IList<string> ProcessUrls(IEnumerable<string> links)
        {
            IList<string> listContent = new List<string>();
            var contentTasks = links.Select(this._resourceProcessor.ExtractResource).ToArray();

            Task.Factory.ContinueWhenAll(contentTasks, tasks =>
            {
                foreach (var task in tasks)
                {
                    if (task.IsFaulted)
                    {
                        Trace.TraceError(task.Exception.GetBaseException().Message);
                        continue;
                    }

                    if (task.Result == null || String.IsNullOrEmpty(task.Result.Content))
                    {
                        continue;
                    }

                    // 尝试从所请求的url里面解析出内容并发给目标用户
                    string extractedContent = "<p>" + task.Result.Content + "</p>";
                    listContent.Add(extractedContent);
                }
            });

            return listContent;
        }
        #endregion

        //聊天接口实现

        #region[发送私有消息] public string SendPrivateMessage()
        /// <summary>
        /// 发送私有消息
        /// </summary>
        [HttpPost]
        public string SendPrivateMessage()
        {
            if (!string.IsNullOrWhiteSpace(Token))
            {
                if (Token != "ocean")
                {
                    return JsonHelper.AnalysisJson<string>("凭证错误", "error", null);
                }
            }

            try
            {
                string messageText = Request["messageText"];

                if (string.IsNullOrWhiteSpace(messageText))
                {
                    return JsonHelper.AnalysisJson<string>("消息不能为空", "error", null);
                }

                messageText = messageText.Trim();

                if (messageText == "接入")
                {
                    return StartKfMeeting();
                }

                if (messageText == "断开")
                {
                    return EndKfMeeting();
                }

                // 防止XSS攻击
                //messageText = Sanitizer.GetSafeHtmlFragment(messageText);
                //messageText = HttpUtility.HtmlEncode(StringHelper.StripHtml(messageText));
                messageText = HttpUtility.HtmlEncode(messageText);
                //把消息存入数据库
                //ICommunicationRepository iCommunicationRepository = new PersistedRepository();

                if (KfMeetingId == Guid.Empty)
                {
                    return JsonHelper.AnalysisJson<string>(string.Format("本条消息发送失败[{0}]", "您还没有建立会话"), "error", null);
                }

                _kfMeetingMessageService.InsertKfMeetingMessage(KfMeetingId, 2, messageText);

                string message2 = string.Empty;

                if (new CommandManager(null).TryHandleCommand("SendPrivateMessage", new string[] { SendUserId.ToString(), ReceiveUserId.ToString(), messageText }, ref message2) && string.IsNullOrWhiteSpace(message2))
                {
                    return JsonHelper.AnalysisJson<string>(string.Empty, "ok", null);
                }

                this.log.Warn(string.Format("消息发送失败，{0}", message2));
                return JsonHelper.AnalysisJson<string>(string.Format("本条消息发送失败[{0}]", message2), "error", null);
            }
            catch (Exception ex)
            {
                this.log.Warn(ex.Message);
                return JsonHelper.AnalysisJson<string>(string.Format("本条消息发送失败[{0}]", ex.Message), "error", null);
            }
        }
        #endregion

        #region [离线] public string OffLine()
        /// <summary>
        /// [离线]
        /// </summary>
        [HttpPost]
        public string OffLine()
        {
            string message = string.Empty;

            if (new CommandManager(null).TryHandleCommand("OffLine", new string[] { SendUserId.ToString(), "2" }, ref message))
            {

                return JsonHelper.AnalysisJson<string>(string.Empty, "ok", null);
            }
            else
            {
                this.log.Error(message);
                return JsonHelper.AnalysisJson<string>(message, "error", null);
            }
        }
        #endregion

        #region [开始会话(等待客服接入)] public string StartKfMeeting()
        /// <summary>
        /// [开始会话(等待客服接入)]
        /// </summary>
        [HttpPost]
        public string StartKfMeeting()
        {
            if (!string.IsNullOrWhiteSpace(Token))
            {
                if (Token != "ocean")
                {
                    return JsonHelper.AnalysisJson<string>("凭证错误", "error", null);
                }
            }
            KfMeeting kfMeeting2 = _kfMeetingService.GetUnique(m => m.MpUserId == SendUserId && !m.IsEnd, "CreateDate", false);

            if (kfMeeting2 != null && kfMeeting2.KfNumberId != Guid.Empty)
            {
                string message2 = string.Empty;

                Log4NetImpl.Write("SendPrivateMessage:" + kfMeeting2.MpUserId.ToString(), kfMeeting2.KfNumberId.ToString());
                new CommandManager(null).TryHandleCommand("SendPrivateMessage", new string[] { kfMeeting2.MpUserId.ToString(), kfMeeting2.KfNumberId.ToString(), "客服" }, ref message2);
                return JsonHelper.AnalysisJson<string>("已经接入", "error", null);
            }

            string message = string.Empty;
            IList<KfNumber> listKfNumber = _kfNumberService.GetALL().Where(m => m.IsOnline == true && m.OnlineStatus == 0).ToList();

            if (listKfNumber.Count == 0)
            {
                //把消息发送给微信端客户
                MpUser mpUser2 = _mpUserService.GetById(SendUserId);

                if (mpUser2 != null && !string.IsNullOrWhiteSpace(mpUser2.OpenID))
                {
                    _mpUserService.SendMessage(mpUser2.OpenID, "当前没有客服在线");
                }

                return JsonHelper.AnalysisJson<string>("当前没有客服在线", "error", null);
            }

            var kfMeeting = new KfMeeting()
            {
                BeginDate = DateTime.Now,
                IsEnd = false,
                MpUserId = SendUserId
            };

            _kfMeetingService.Insert(kfMeeting);

            //向所有在线客服发送接入请求
            //访客名称
            string visitName = string.Empty;
            MpUser mpUser = _mpUserService.GetById(kfMeeting.MpUserId);
            if (mpUser != null)
            {
                visitName = mpUser.NickName;

                if (!string.IsNullOrWhiteSpace(mpUser.OpenID))
                {
                    _mpUserService.SendMessage(mpUser.OpenID, "等待客服接入，输入【退出】或【tc】退出聊天状态");
                }
            }
            else
            {
                visitName = "访客_" + new Random().Next(100);
            }
            foreach (KfNumber model in listKfNumber)
            {
                new CommandManager(null).TryHandleCommand("Notice", new string[] { SendUserId.ToString(), visitName, model.Id.ToString(), "1", "0", kfMeeting.Id.ToString() }, ref message);
            }

            return JsonHelper.AnalysisJson<string>(string.Empty, "ok", null);
        }
        #endregion

        #region [结束会话] public string EndKfMeeting()
        /// <summary>
        /// [结束会话]
        /// </summary>
        [HttpPost]
        public string EndKfMeeting()
        {
            if (!string.IsNullOrWhiteSpace(Token))
            {
                if (Token != "ocean")
                {
                    return JsonHelper.AnalysisJson<string>("凭证错误", "error", null);
                }
            }

            KfMeeting kfMeeting = _kfMeetingService.GetById(KfMeetingId);

            if (!kfMeeting.IsEnd)
            {
                kfMeeting.RecordCount = _kfMeetingMessageService.GetCount(" where KfMeetingId='" + kfMeeting.Id + "'");
                kfMeeting.EndDate = DateTime.Now;
                kfMeeting.IsEnd = true;
                _kfMeetingService.Update(kfMeeting);
                //向客服发送断开通知
                string message = string.Empty;
                //访客名称
                string visitName = string.Empty;
                MpUser mpUser = _mpUserService.GetById(kfMeeting.MpUserId);
                if (mpUser != null)
                {
                    visitName = mpUser.NickName;

                    if (!string.IsNullOrWhiteSpace(mpUser.OpenID))
                    {
                        _mpUserService.SendMessage(mpUser.OpenID, "成功退出聊天，输入【客服】或【kf】转人工客服");
                    }
                }
                else
                {
                    visitName = "访客_" + new Random().Next(100);
                }
                new CommandManager(null).TryHandleCommand("Notice", new string[] { kfMeeting.MpUserId.ToString(), visitName, kfMeeting.KfNumberId.ToString(), "3", "0", kfMeeting.MpUserId.ToString() }, ref message);
                //断开当前用户的所有连接
                _kfMeetingService.ExcuteSql("update KfMeeting set IsEnd=1 where MpUserId='" + kfMeeting.MpUserId + "'");
            }

            //更新用户状态
            _mpUserService.ChangeMpUserState(kfMeeting.MpUserId, 0);
            return JsonHelper.AnalysisJson<string>(string.Empty, "ok", null);
        }
        #endregion

        //微信接口

        #region [接收消息接口]
        /// <summary>
        /// [接收消息接口]
        /// </summary>
        public void AcceptMessage(string token, Guid sendUserId, Guid receiveUserId, string message)
        {
            return;

            var listMessageModel = new Ocean.Communication.Common.SafeCollection<MessageModel>();

            var model = new MessageModel
            {
                SendUserId = sendUserId,
                ReceiveUserId = receiveUserId,
                Message = message,
                SendTimeOfService = DateTime.Now
            };

            listMessageModel.Add(model);
            MessageManager.Instance.AddMessageFromServer(listMessageModel);
        }
        #endregion

        #region [接收通知接口]
        /// <summary>
        /// [接收通知接口]
        /// </summary>
        public void AcceptNotice(string token, Guid sendUserId, string sendNickeName, Guid receiveUserId, int noticeType, int status, string args)
        {
            return;
            
            var listNoticeModel = new Ocean.Communication.Common.SafeCollection<NoticeModel>();

            var model = new NoticeModel
            {
                SendUserId = sendUserId,
                SendNickName = sendNickeName,
                ReceiveUserId = receiveUserId,
                NoticeType = noticeType,
                Status = status,
                Args = args
            };

            listNoticeModel.Add(model);
            MessageManager.Instance.AddNoticeFromServer(listNoticeModel);
        }
        #endregion
    }
}