using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Communication.Services
{
    public class PersistedRepository : ICommunicationRepository
    {
        public void Dispose()
        {
            ;
        }

        #region 发送消息
        /// <summary>
        /// 发送消息
        /// </summary>
        public bool SendPrivateMessage(Guid sendUserId, Guid receiveUserId, string messageText)
        {
            try
            {
                return false;
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
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            //No need to do anything here
            throw new NotImplementedException();
        }
        #endregion
    }
}