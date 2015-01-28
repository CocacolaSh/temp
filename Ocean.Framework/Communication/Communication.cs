using System;
using Ocean.Core.Utility;
using Ocean.Framework.Configuration.global.config;
using Newtonsoft.Json;
using Ocean.Core.Logging;

namespace Ocean.Framework.Communication
{
    [Serializable]
    public class data
    {
        public string model { set;get; }
        public string code { set; get; }
    }

    [Serializable]
    public class CommunicationObject
    {
        public data data { set; get; }
        public string dataExt { set; get; }
    }

    public class Client
    {
        private static readonly string token = "ocean";

        #region 通信post目标地址
        /// <summary>
        /// 通信post目标地址
        /// </summary>
        public static string CommunicationPostUrl
        {
            get
            {
                return string.Format("{0}{1}{2}", GlobalConfig.GetConfig()["CommunicationDomain"].TrimEnd('/'), "/", "Client/");
            }
        }
        #endregion

        //聊天接口实现

        #region[发送私有消息] public static string SendPrivateMessage()
        /// <summary>
        /// 发送私有消息
        /// </summary>
        public static CommunicationObject SendPrivateMessage(Guid sendUserId, string messageText)
        {
            HttpRequests httpRequests = new HttpRequests();
            string url = string.Format("{0}{1}", CommunicationPostUrl, "SendPrivateMessage");
            string strPostData = httpRequests.Analysis(new string[] { "SendUserId", "messageText", "token" }, new string[] { sendUserId.ToString(), messageText, token });
            string jsonText = httpRequests.DownloadHtml(url, strPostData);
            //反序列化JSON字符串  
            return JsonConvert.DeserializeObject<CommunicationObject>(jsonText);
        }
        #endregion

        #region [离线] public static string OffLine()
        /// <summary>
        /// [离线]
        /// </summary>
        public static string OffLine()
        {
            return string.Empty;
        }
        #endregion

        #region [开始会话(等待客服接入)] public static string StartKfMeeting()
        /// <summary>
        /// [开始会话(等待客服接入)]
        /// </summary>
        public static CommunicationObject StartKfMeeting(Guid sendUserId)
        {
            Log4NetImpl.Write("等待客服接入");
            HttpRequests httpRequests = new HttpRequests();
            string url = string.Format("{0}{1}", CommunicationPostUrl, "StartKfMeeting");
            string strPostData = httpRequests.Analysis(new string[] { "SendUserId", "token" }, new string[] { sendUserId.ToString(), token });
            string jsonText = httpRequests.DownloadHtml(url, strPostData);
            //反序列化JSON字符串  
            return JsonConvert.DeserializeObject<CommunicationObject>(jsonText);
        }
        #endregion

        #region [结束会话] public static string EndKfMeeting()
        /// <summary>
        /// [结束会话]
        /// </summary>
        public static CommunicationObject EndKfMeeting(Guid sendUserId)
        {
            HttpRequests httpRequests = new HttpRequests();
            string url = string.Format("{0}{1}", CommunicationPostUrl, "EndKfMeeting");
            string strPostData = httpRequests.Analysis(new string[] { "SendUserId", "token" }, new string[] { sendUserId.ToString(), token });
            string jsonText = httpRequests.DownloadHtml(url, strPostData);
            //反序列化JSON字符串  
            return JsonConvert.DeserializeObject<CommunicationObject>(jsonText);
        }
        #endregion
    }
}