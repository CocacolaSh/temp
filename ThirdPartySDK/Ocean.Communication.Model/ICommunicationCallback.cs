using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Ocean.Communication.Common;

namespace Ocean.Communication.Model
{
    /// <summary>
    /// WCF回调函数
    /// </summary>
    public interface ICommunicationCallback
    {
        [OperationContract(IsOneWay = true)]
        void SendMessage(SafeCollection<MessageModel> listMessage);

        [OperationContract(IsOneWay = true)]
        void SendNotice(SafeCollection<NoticeModel> listNotice);
    }
}