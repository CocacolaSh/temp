using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Communication.Services
{
    public interface ICommunicationRepository : IDisposable
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        bool SendPrivateMessage(Guid sendUserId, Guid receiveUserId, string messageText);

        /// <summary>
        /// 发送通知
        /// </summary>
        bool SendNotice(Guid sendUserId, string sendNickName, Guid receiveUserId, int noticeType, int status, string args);
    }
}