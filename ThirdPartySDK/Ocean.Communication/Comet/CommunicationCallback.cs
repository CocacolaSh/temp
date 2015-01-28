using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Communication.Model;
using Ocean.Communication.Common;

namespace Ocean.Communication.Comet
{
    public class CommunicationCallback : ICommunicationCallback
    {
        #region 把WCF的消息分发到本地
        /// <summary>
        /// 把WCF的消息分发到本地
        /// </summary>
        /// <param name="listMessage"></param>
        public void SendMessage(SafeCollection<MessageModel> listMessage)
        {
            if (listMessage != null && listMessage.Count > 0)
            {
                MessageManager.Instance.AddMessageFromServer(listMessage);
            }
        }
        #endregion

        #region 把WCF的通知分发到本地
        /// <summary>
        /// 把WCF的通知分发到本地
        /// </summary>
        /// <param name="listNotice"></param>
        public void SendNotice(SafeCollection<NoticeModel> listNotice)
        {
            if (listNotice != null && listNotice.Count > 0)
            {
                MessageManager.Instance.AddNoticeFromServer(listNotice);
            }
        }
        #endregion
    }
}