using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Communication.Services
{
    /// <summary>
    /// 通讯接口
    /// </summary>
    public interface ICommunicationService
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        string SendPrivateMessage();
    }
}