using System;
using System.Collections.Generic;
using System.Threading;
using log4net;
using Ocean.Communication.Model;
using Ocean.Communication.Common;

namespace Ocean.Communication.Comet
{
    public class CometWaitThread
    {
        private readonly ILog log = LogManager.GetLogger(typeof(CometWaitThread));
        private object state = new object();
        private List<CometWaitRequest> waitRequests = new List<CometWaitRequest>();
        private bool started = false;
        //每个线程7个消息分组
        private static int groups = 7;

        #region 当前线程上的消息队列
        /// <summary>
        /// 当前线程上的消息队列
        /// </summary>
        public SafeCollection<MessageModel>[] listMessage = null;
        #endregion

        #region 当前线程上的通知队列
        /// <summary>
        /// 当前线程上的通知队列
        /// </summary>
        public SafeCollection<NoticeModel>[] listNotice = null;
        #endregion

        #region 获取当前线程上的监听客户端
        /// <summary>
        /// 获取当前线程上的监听客户端
        /// </summary>
        public List<CometWaitRequest> WaitRequests
        {
            get { return this.waitRequests; }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public CometWaitThread()
        {
            if (!started)
            {
                listMessage = new SafeCollection<MessageModel>[groups];
                listNotice = new SafeCollection<NoticeModel>[groups];
                started = true;
                Thread t = new Thread(new ThreadStart(QueueCometWaitRequest_WaitCallback));
                t.IsBackground = false;
                t.Start();
            }
        }
        #endregion

        #region 添加用户消息 public void AddMessage(FriendChatMessageModel model)
        /// <summary>
        /// 添加用户消息
        /// </summary>
        public void AddMessage(MessageModel model)
        {
            try
            {
                if (model == null)
                {
                    return;
                }

                int group = GuidCoveter.CoveterFirstChat(model.ReceiveUserId) % groups;

                if (listMessage[group] == null)
                {
                    listMessage[group] = new SafeCollection<MessageModel>();
                }

                listMessage[group].Add(model);
            }
            catch (Exception exception)
            {
                log.Warn("发送消息给线程内队列出现异常：" + exception.Message);
            }
        }
        #endregion

        #region 获取用户消息
        /// <summary>
        /// 获取用户消息
        /// </summary>
        public List<MessageModel> GetMessage(Guid receiveUserId)
        {
            try
            {
                List<MessageModel> listMessageModel = new List<MessageModel>();
                int nGroup = GuidCoveter.CoveterFirstChat(receiveUserId) % groups;

                Thread.Sleep(3);

                if (listMessage[nGroup] == null)
                {
                    listMessage[nGroup] = new SafeCollection<MessageModel>();
                }

                foreach (MessageModel model in listMessage[nGroup])
                {
                    //清理掉超时5分钟没被领走的消息
                    if (DateTime.Now.Subtract(model.SendTimeOfService).TotalSeconds > 300)
                    {
                        listMessage[nGroup].Remove(model);
                        continue;
                    }

                    if (model.ReceiveUserId == receiveUserId)
                    {
                        listMessageModel.Add(model);
                        listMessage[nGroup].Remove(model);
                    }
                }

                return listMessageModel;
            }
            catch (Exception exception)
            {
                log.Warn("获取线程内消息队列出现异常：" + exception.Message);
                return new List<MessageModel>();
            }
        }
        #endregion

        #region 添加用户通知 public void AddNotice(NoticeModel model)
        /// <summary>
        /// 添加用户通知
        /// </summary>
        public void AddNotice(NoticeModel model)
        {
            try
            {
                if (model == null)
                {
                    return;
                }

                int nGroup = GuidCoveter.CoveterFirstChat(model.ReceiveUserId) % groups;

                if (listNotice[nGroup] == null)
                {
                    listNotice[nGroup] = new SafeCollection<NoticeModel>();
                }

                listNotice[nGroup].Add(model);
            }
            catch (Exception exception)
            {
                log.Warn("发送通知给线程内队列出现异常：" + exception.Message);
            }
        }
        #endregion

        #region 取用户的通知
        /// <summary>
        /// 取用户的通知
        /// </summary>
        /// <param name="receiveUserId"></param>
        /// <returns></returns>
        public List<NoticeModel> GetNotice(Guid receiveUserId)
        {
            try
            {
                List<NoticeModel> listNoticeModel = new List<NoticeModel>();
                int nGroup = GuidCoveter.CoveterFirstChat(receiveUserId) % groups;

                Thread.Sleep(3);

                if (listNotice[nGroup] == null)
                {
                    listNotice[nGroup] = new SafeCollection<NoticeModel>();
                }

                foreach (NoticeModel model in listNotice[nGroup])
                {
                    //清理掉超时50秒没被领走的消息
                    if (DateTime.Now.Subtract(model.LastActiveDate).Seconds > 50)
                    {
                        listNotice[nGroup].Remove(model);
                        continue;
                    }

                    if (model.ReceiveUserId == receiveUserId)
                    {
                        listNoticeModel.Add(model);
                        listNotice[nGroup].Remove(model);
                    }
                }

                return listNoticeModel;
            }
            catch (Exception exception)
            {
                log.Warn("获取线程内通知队列出现异常：" + exception.Message);
                return new List<NoticeModel>();
            }
        }
        #endregion

        #region 增加监听客户端到当前线程队列
        /// <summary>
        /// 增加监听客户端到当前线程队列
        /// </summary>
        /// <param name="request"></param>
        internal void QueueCometWaitRequest(CometWaitRequest request)
        {
            lock (this.state)
            {
                waitRequests.Add(request);
            }
        }
        #endregion

        #region 从当前线程队列移除监听客户端
        /// <summary>
        /// 从当前线程队列移除监听客户端
        /// </summary>
        /// <param name="request"></param>
        internal void DequeueCometWaitRequest(CometWaitRequest request)
        {
            lock (this.state)
            {
                this.waitRequests.Remove(request);
                CometWaitRequest.RequestCount--;
            }
        }
        #endregion

        #region 根据用户Id从当前线程队列移除监听客户端
        /// <summary>
        /// 根据用户Id从当前线程队列移除监听客户端
        /// </summary>
        /// <param name="sendUserId"></param>
        internal void DequeueCometWaitRequest(Guid sendUserId)
        {
            lock (this.state)
            {
                for (int i = 0; i < this.waitRequests.Count; i++)
                {
                    CometWaitRequest request = (CometWaitRequest)this.waitRequests[i];

                    if (request.SendUserId == sendUserId)
                    {
                        this.waitRequests.Remove(request);
                        CometWaitRequest.RequestCount--;
                    }
                }
            }
        }
        #endregion

        #region 当前客户端监听任务完成执行函数
        /// <summary>
        /// 当前客户端监听任务完成执行函数
        /// </summary>
        /// <param name="target"></param>
        private void QueueCometWaitRequest_Finished(object target, ArgumentModel modelArgument)
        {
            CometWaitRequest request = target as CometWaitRequest;
            request.execute.BeginInvoke(modelArgument, null, null);
        }
        #endregion

        #region 开始线程处理
        /// <summary>
        /// 开始线程处理
        /// </summary>
        private void QueueCometWaitRequest_WaitCallback()
        {
            try
            {
                while (true)
                {
                    CometWaitRequest[] processRequest;

                    lock (this.state)
                    {
                        processRequest = waitRequests.ToArray();
                    }

                    Thread.Sleep(500);

                    for (int i = 0; i < processRequest.Length; i++)
                    {
                        CometWaitRequest cometWaitRequest = processRequest[i];

                        //  超时则从线程队列中移除对该客户端的监听
                        if (DateTime.Now.Subtract(cometWaitRequest.DateTimeAdded).TotalSeconds >= 20)
                        {
                            //从当前线程队列移除监听客户端
                            DequeueCometWaitRequest(cometWaitRequest);
                            this.QueueCometWaitRequest_Finished(cometWaitRequest, null);
                        }
                        else
                        {
                            object serverPushEvent = this.CheckForServerPushEvent(processRequest[i], false);

                            if (serverPushEvent != null)
                            {
                                ArgumentModel modelArgument = (ArgumentModel)serverPushEvent;
                                this.QueueCometWaitRequest_Finished(cometWaitRequest, modelArgument);
                                //从当前线程队列移除监听客户端
                                DequeueCometWaitRequest(cometWaitRequest);
                            }
                        }

                        Thread.Sleep(5);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Warn(ex);
                QueueCometWaitRequest_WaitCallback();
            }
        }
        #endregion

        #region 处理服务器请求
        /// <summary>
        /// 处理服务器请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private object CheckForServerPushEvent(CometWaitRequest request, bool timeout)
        {
            if (timeout)
            {
                return "Response from: " + Thread.CurrentThread.ManagedThreadId.ToString();
            }

            List<MessageModel> listMessage = new List<MessageModel>();
            List<NoticeModel> listNotice = new List<NoticeModel>();
            //取用户消息信息
            listMessage = GetMessage(request.SendUserId);
            //取用户好友相关通知
            listNotice = GetNotice(request.SendUserId);

            if (listMessage.Count == 0 && listNotice.Count == 0)
            {
                return null;
            }

            ArgumentModel modelArgument = new ArgumentModel();
            modelArgument.listMessage = listMessage;
            modelArgument.listNotice = listNotice;
            return modelArgument;
        }
        #endregion
    }
}