using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Communication.Model;

namespace Ocean.Communication.Comet
{
    public class ArgumentModel
    {
        #region 消息列表
        /// <summary>
        /// 消息列表
        /// </summary>
        public List<MessageModel> listMessage
        {
            set;
            get;
        }
        #endregion

        #region 通知列表
        /// <summary>
        /// 通知列表
        /// </summary>
        public List<NoticeModel> listNotice
        {
            set;
            get;
        }
        #endregion
    }
}