using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ocean.Framework.Sms
{
    public class SendSmsRequest
    {
        /// <summary>
        /// 手机号码，多个手机用逗号隔开
        /// </summary>
        public IList<string> Mobiles
        {
            get;
            private set;
        }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// SMID
        /// </summary>
        public long SMID
        {
            get;
            private set;
        }

        /// <summary>
        /// SrcID
        /// </summary>
        public long? SrcID
        {
            get;
            set;
        }

        /// <summary>
        /// 定时发送时间
        /// </summary>
        public DateTime? SendTime
        {
            get;
            set;
        }

        public SendSmsRequest(IList<string> mobiles, string message, long smID)
        {
            Mobiles = mobiles;
            Message = message;
            SMID = smID;
        }
    }
}