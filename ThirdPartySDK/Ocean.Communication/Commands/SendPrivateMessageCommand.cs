using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Communication.Services;

namespace Ocean.Communication.Commands
{
    [Command("SendPrivateMessage", "发送私有消息", "[sendUserId],[receiveUserId],[messageText]", "communication")]
    public class SendPrivateMessageCommand : ICommand
    {
        public void Execute(CallerContext callerContext, string[] args, ref string message)
        { 
            // 当前登录客服
            this.SendMessage(new Guid(args[0]), new Guid(args[1]), args[2]);
        }

        #region [把消息存入数据库和进行内存分发] private bool SendMessage(Guid sendUserId, Guid receiveUserId, string messageText)
        /// <summary>
        /// 把消息存入数据库和进行内存分发
        /// </summary>
        private bool SendMessage(Guid sendUserId, Guid receiveUserId, string messageText)
        {
            ICommunicationRepository inMemoryRepository = new InMemoryRepository();
            return inMemoryRepository.SendPrivateMessage(sendUserId, receiveUserId, messageText);
        }
        #endregion
    }
}