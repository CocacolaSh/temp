using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Communication.Services;

namespace Ocean.Communication.Commands
{
    public abstract class SendNoticeCommand : ICommand
    {
        void ICommand.Execute(CallerContext callerContext, string[] args, ref string message)
        {
            ExecuteNotice(callerContext, args, ref message);
        }

        public abstract void ExecuteNotice(CallerContext callerContext, string[] args, ref string message);

        #region [把通知进行分发] protected bool SendNotice(Guid SendUserId, Guid ReceiveUserId, int NoticeType, int Status)
        /// <summary>
        /// [把通知进行分发]
        /// </summary>
        /// <param name="sendUserId">发送者Id</param>
        /// <param name="receiveUserId">接收者Id</param>
        /// <param name="noticeType">通知类型</param>
        /// <param name="status">当前在线状态</param>
        /// <param name="args">附带参数</param>
        /// <returns></returns>
        protected bool SendNotice(Guid sendUserId, string sendNickName, Guid receiveUserId, int noticeType, int status, string args)
        {
            //把通知进行分发
            ICommunicationRepository inMemoryRepository = new InMemoryRepository();
            return inMemoryRepository.SendNotice(sendUserId, sendNickName, receiveUserId, noticeType, status, args);
        }
        #endregion

        #region 把消息进行分发
        /// <summary>
        /// 把消息进行分发
        /// </summary>
        /// <param name="sendUserId">发送者</param>
        /// <param name="receiveUserId">接受者</param>
        /// <param name="messageText">消息内容</param>
        /// <returns></returns>
        protected bool SendPrivateMessage(Guid sendUserId, Guid receiveUserId, string messageText)
        {
            //把消息进行分发
            ICommunicationRepository inMemoryRepository = new InMemoryRepository();
            return inMemoryRepository.SendPrivateMessage(sendUserId, receiveUserId, messageText);
        }
        #endregion
    }
}