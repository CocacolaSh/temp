using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Communication.Comet;
using System.ServiceModel;
using Ocean.Communication.Model;
using Ocean.Communication.Common;

namespace Ocean.Communication.Services
{
    public class InMemoryRepository : ICommunicationRepository
    {
        public void Dispose()
        {
            ;
        }

        #region 发送消息
        /// <summary>
        /// 发送消息
        /// </summary>
        public bool SendPrivateMessage(Guid sendUserId, Guid receiveUserId, string message)
        {
            try
            {
                //InstanceContext instanceContext = new InstanceContext(new CommunicationCallback());

                var messageModel = new MessageModel
                {
                    SendUserId = sendUserId,
                    ReceiveUserId = receiveUserId,
                    Message = message,
                    SendTimeOfService = DateTime.Now
                };

                //处理用户消息
                SafeCollection<MessageModel> listMessageModel = new SafeCollection<MessageModel>();
                listMessageModel.Add(messageModel);
                ICommunicationCallback _client = new CommunicationCallback();
                //向服务器发送消息
                _client.SendMessage(listMessageModel);

                //using (DuplexChannelFactory<ICommunication> channelFactory = new DuplexChannelFactory<ICommunication>(instanceContext, "CommunicationService"))
                //{
                //    ICommunication proxy = channelFactory.CreateChannel();

                //    using (proxy as IDisposable)
                //    {
                //        proxy.InsertMessage(message);
                //    }
                //}
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region 发送通知
        /// <summary>
        /// 发送通知
        /// </summary>
        public bool SendNotice(Guid sendUserId, string sendNickName, Guid receiveUserId, int noticeType, int status, string args)
        {
            try
            {
                //InstanceContext instanceContext = new InstanceContext(new CommunicationCallback());

                var noticeModel = new NoticeModel
                {
                    SendUserId = sendUserId,
                    SendNickName = sendNickName,
                    ReceiveUserId = receiveUserId,
                    NoticeType = noticeType,
                    Status = status,
                    Args = args,
                    LastActiveDate = DateTime.Now
                };

                //处理用户消息
                SafeCollection<NoticeModel> listNoticeModel = new SafeCollection<NoticeModel>();
                listNoticeModel.Add(noticeModel);
                ICommunicationCallback _client = new CommunicationCallback();
                //向服务器发送通知
                _client.SendNotice(listNoticeModel);

                //using (DuplexChannelFactory<ICommunication> channelFactory = new DuplexChannelFactory<ICommunication>(instanceContext, "CommunicationService"))
                //{
                //    ICommunication proxy = channelFactory.CreateChannel();

                //    using (proxy as IDisposable)
                //    {
                //        proxy.InsertNotice(noticeModel);
                //    }
                //}

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}