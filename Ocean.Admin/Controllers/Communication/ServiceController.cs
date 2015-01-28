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
using Ocean.Page;

namespace WebSite.Controllers.Communication
{
    /// <summary>
    /// 客服端通信控制器
    /// </summary>
    public class ServiceController : AsyncController, ICommunicationService
    {
        private readonly IKfNumberService _kfNumberService;
        private readonly IKfMeetingService _kfMeetingService;
        private readonly IKfMeetingMessageService _kfMeetingMessageService;
        private readonly IMpUserService _mpUserService;
        private readonly IFunongbaoService _funongbaoService;

        public ServiceController(IKfNumberService kfNumberService, 
            IKfMeetingService kfMeetingService, 
            IKfMeetingMessageService kfMeetingMessageService,
            IMpUserService mpUserService,
            IFunongbaoService funongbaoService)
        {
            this._kfNumberService = kfNumberService;
            this._kfMeetingService = kfMeetingService;
            this._kfMeetingMessageService = kfMeetingMessageService;
            this._mpUserService = mpUserService;
            this._funongbaoService = funongbaoService;
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
        private readonly ILog log = LogManager.GetLogger(typeof(ServiceController));

        #endregion

        #region [客服工号Id] public Guid KfNumberId
        /// <summary>
        /// 客服工号Id
        /// </summary>
        public Guid KfNumberId
        {
            get
            {
                return ((KfNumber)AdminLogin.Instance.Admin.KfNumbers.ToList()[0]).Id;
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
                return new Guid(Request["ReceiveUserId"]);
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
                return new Guid(Request["KfMeetingId"]);
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
            CometWaitRequest cometWaitRequest = new CometWaitRequest(KfNumberId);
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
            try
            {
                KfNumber kfNumber = _kfNumberService.GetById(KfNumberId);

                if (kfNumber == null)
                {
                    return JsonHelper.AnalysisJson<string>("KfNumber对象为null", "error", null);
                }

                KfMeeting kfMeeting = _kfMeetingService.GetById(KfMeetingId);

                if (kfMeeting.IsEnd)
                {
                    return JsonHelper.AnalysisJson<string>(string.Format("本条消息发送失败[{0}]", "本次会话已经结束"), "error", null);
                }

                string messageText = Request["messageText"];

                if (string.IsNullOrWhiteSpace(messageText))
                {
                    return JsonHelper.AnalysisJson<string>("消息不能为空", "error", null);
                }

                messageText = messageText.Trim();
                // 防止XSS攻击
                //messageText = Sanitizer.GetSafeHtmlFragment(messageText);
                //messageText = HttpUtility.HtmlEncode(StringHelper.StripHtml(messageText));
                messageText = HttpUtility.HtmlEncode(messageText);
                //把消息存入数据库
                //ICommunicationRepository iCommunicationRepository = new PersistedRepository();
                _kfMeetingMessageService.InsertKfMeetingMessage(KfMeetingId, 1, messageText);
                //把消息发送给微信端客户
                MpUser mpUser = _mpUserService.GetById(ReceiveUserId);

                if (mpUser != null && !string.IsNullOrWhiteSpace(mpUser.OpenID))
                {
                    _mpUserService.SendMessage(mpUser.OpenID, messageText);
                }
                //把消息发给通信线程进行中转（适用于web聊天）
                string message2 = string.Empty;

                if (new CommandManager(null).TryHandleCommand("SendPrivateMessage", new string[] { KfNumberId.ToString(), ReceiveUserId.ToString(), messageText }, ref message2) && string.IsNullOrWhiteSpace(message2))
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
            KfNumber kfNumber = _kfNumberService.GetById(KfNumberId);

            if (kfNumber == null)
            {
                return JsonHelper.AnalysisJson<string>("KfNumber对象为null", "error", null);
            }

            //发送客服离线通知
            if (new CommandManager(null).TryHandleCommand("OffLine", new string[] { KfNumberId.ToString(), "1" }, ref message))
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

        #region [接入会话(接待客户)] public string ReceptionKfMeeting()
        /// <summary>
        /// [接入会话(接待客户)]
        /// </summary>
        [HttpPost]
        public string ReceptionKfMeeting()
        {
            KfMeeting kfMeeting = _kfMeetingService.GetById(KfMeetingId);

            if (kfMeeting.IsEnd)
            {
                return JsonHelper.AnalysisJson<string>("当前会话已被对方结束", "error", null);
            }

            kfMeeting.BeginDate = DateTime.Now;
            kfMeeting.KfNumberId = KfNumberId;
            _kfMeetingService.Update(kfMeeting);
            //向其他客服发送当前客户已经接管的通知
            string message = string.Empty;
            IList<KfNumber> listKfNumber = _kfNumberService.GetALL().Where(m => m.Id != KfNumberId && m.IsOnline == true).ToList();

            foreach (KfNumber model in listKfNumber)
            {
                new CommandManager(null).TryHandleCommand("Notice", new string[] { KfNumberId.ToString(), "客服接入", model.Id.ToString(), "2", "0", kfMeeting.MpUserId.ToString() }, ref message);
            }
            //向用户发送已经接入成功的消息
            string message2 = string.Empty;
            new CommandManager(null).TryHandleCommand("SendPrivateMessage", new string[] { KfNumberId.ToString(), kfMeeting.MpUserId.ToString(), "接入成功,现在可以与客服对话了" }, ref message2);
            //把消息发送给微信端客户
            MpUser mpUser = _mpUserService.GetById(kfMeeting.MpUserId);

            if (mpUser != null && !string.IsNullOrWhiteSpace(mpUser.OpenID))
            {
                _mpUserService.SendMessage(mpUser.OpenID, "接入成功,现在可以与客服对话了");
            }
            //更新用户状态
            _mpUserService.ChangeMpUserState(kfMeeting.MpUserId, 1);

            return JsonHelper.AnalysisJson<string>(string.Empty, "ok", null);
        }
        #endregion

        #region [拒绝会话] public string RejectKfMeeting()
        /// <summary>
        /// [拒绝会话]
        /// </summary>
        [HttpPost]
        public string RejectKfMeeting()
        {
            KfMeeting kfMeeting = _kfMeetingService.GetById(KfMeetingId);

            if (!kfMeeting.IsEnd)
            {
                kfMeeting.IsEnd = true;
                _kfMeetingService.Update(kfMeeting);
                //向其他客服发送当前客户已经拒绝的通知
                string message = string.Empty;
                IList<KfNumber> listKfNumber = _kfNumberService.GetALL().Where(m => m.Id != KfNumberId && m.IsOnline == true).ToList();

                foreach (KfNumber model in listKfNumber)
                {
                    new CommandManager(null).TryHandleCommand("Notice", new string[] { KfNumberId.ToString(), "客服拒绝", model.Id.ToString(), "4", "0", kfMeeting.MpUserId.ToString() }, ref message);
                }
                //向用户发送已经拒绝的消息
                string message2 = string.Empty;
                new CommandManager(null).TryHandleCommand("SendPrivateMessage", new string[] { KfNumberId.ToString(), kfMeeting.MpUserId.ToString(), "客服拒绝您的本次请求，请再次输入【客服】或【kf】转人工客服" }, ref message2);
                //把消息发送给微信端客户
                MpUser mpUser = _mpUserService.GetById(kfMeeting.MpUserId);

                if (mpUser != null && !string.IsNullOrWhiteSpace(mpUser.OpenID))
                {
                    _mpUserService.SendMessage(mpUser.OpenID, "客服连接超时，您可以拨打我行客服热线40008-96336");
                }
            }
            else
            {
                //向其他客服发送当前客户已经拒绝的通知
                string message = string.Empty;
                IList<KfNumber> listKfNumber = _kfNumberService.GetALL().Where(m => m.Id != KfNumberId && m.IsOnline == true).ToList();

                foreach (KfNumber model in listKfNumber)
                {
                    new CommandManager(null).TryHandleCommand("Notice", new string[] { KfNumberId.ToString(), "客服拒绝", model.Id.ToString(), "4", "0", kfMeeting.MpUserId.ToString() }, ref message);
                }
            }
            //更新用户状态
            _mpUserService.ChangeMpUserState(kfMeeting.MpUserId, 0);

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
            KfMeeting kfMeeting = _kfMeetingService.GetById(KfMeetingId);
            if (!kfMeeting.IsEnd)
            {
                kfMeeting.RecordCount = _kfMeetingMessageService.GetCount(" where KfMeetingId='" + kfMeeting.Id + "'");
                kfMeeting.EndDate = DateTime.Now;
                kfMeeting.IsEnd = true;
                _kfMeetingService.Update(kfMeeting);
                //向用户发送会话断开的消息
                string message2 = string.Empty;
                new CommandManager(null).TryHandleCommand("SendPrivateMessage", new string[] { KfNumberId.ToString(), kfMeeting.MpUserId.ToString(), "本次会话已被断开" }, ref message2);
                //把消息发送给微信端客户
                MpUser mpUser = _mpUserService.GetById(kfMeeting.MpUserId);

                if (mpUser != null && !string.IsNullOrWhiteSpace(mpUser.OpenID))
                {
                    _mpUserService.SendMessage(mpUser.OpenID, "本次会话已被断开");
                }
                //断开当前用户的所有连接
                _kfMeetingService.ExcuteSql("update KfMeeting set IsEnd=1 where MpUserId='" + kfMeeting.MpUserId + "'");
                //更新用户状态
                _mpUserService.ChangeMpUserState(kfMeeting.MpUserId, 0);
            }
            return JsonHelper.AnalysisJson<string>(string.Empty, "ok", null);
        }
        #endregion

        #region [监听是否有多个客服端,并保持客服在线]
        //监听是否有多个客服端,并保持客服在线
        [HttpPost]
        public string Monitor()
        {
            string hash = Request["Hash"];
            //更新客服在线缓存列表
            OnlineKFModel onlineKFModel = MessageManager.Instance.GetOnlineKFInfo(KfNumberId);

            if (onlineKFModel == null)
            {
                //更新客服在线缓存列表
                onlineKFModel = new OnlineKFModel();
                onlineKFModel.Hash = hash;
                onlineKFModel.KfNumberId = KfNumberId;
                onlineKFModel.LastActiveDate = DateTime.Now;
                MessageManager.Instance.CacheLocalKF(onlineKFModel);
            }
            else
            {
                //判断hash
                if(!string.Equals(hash,onlineKFModel.Hash,StringComparison.OrdinalIgnoreCase))
                {
                    return "0";
                }
                onlineKFModel.LastActiveDate = DateTime.Now;
                MessageManager.Instance.CacheLocalKF(onlineKFModel);
            }

            return "1";
        }
        #endregion

        #region 设置在线状态[0:正常,1:离开]
        /// <summary>
        /// 设置在线状态[0:正常,1:离开]
        /// </summary>
        /// <returns></returns>
        public string SetOnlineStatus()
        {
            int onlineStatus = int.Parse(Request["OnlineStatus"]);
            KfNumber kfNumber = _kfNumberService.GetById(KfNumberId);
            kfNumber.OnlineStatus = onlineStatus;
            _kfNumberService.Update(kfNumber);
            return JsonHelper.AnalysisJson<string>(onlineStatus.ToString(), "ok", null);
        }
        #endregion

        #region [获取用户信息] public string GetMpUserInfo()
        /// <summary>
        /// [获取用户信息]
        /// </summary>
        [HttpPost]
        public string GetMpUserInfo()
        {
            MpUser mpUser = _mpUserService.GetById(ReceiveUserId);
            string isFnb = _funongbaoService.GetCount(" where MpUserId='" + ReceiveUserId + "'").ToString();
            string extData = mpUser.CreateDate.ToString("yyyy-MM-dd") + "|" + mpUser.LastVisitDate.ToString("yyyy-MM-dd") + "|" + isFnb;
            return JsonHelper.AnalysisJson<MpUser>(mpUser, "ok", extData);
        }
        #endregion

        #region [获取会话摘要] public string GetMeetingExplain()
        /// <summary>
        /// [获取会话摘要]
        /// </summary>
        [HttpPost]
        public string GetMeetingExplain()
        {
            KfMeeting kfMeeting = _kfMeetingService.GetById(KfMeetingId);
            return JsonHelper.AnalysisJson<string>(kfMeeting.Explain, "ok", null);
        }
        #endregion

        #region [编辑会话摘要] public string UpdateMeetingExplain()
        /// <summary>
        /// [编辑会话摘要]
        /// </summary>
        [HttpPost]
        public string UpdateMeetingExplain()
        {
            string explain = Request["explain"];

            if(string.IsNullOrWhiteSpace(explain))
            {
                return JsonHelper.AnalysisJson<string>("请填写摘要", "ok", null);
            }

            KfMeeting kfMeeting = _kfMeetingService.GetById(KfMeetingId);
            kfMeeting.Explain = explain;
            _kfMeetingService.Update(kfMeeting);
            return JsonHelper.AnalysisJson<string>(kfMeeting.Explain, "ok", null);
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