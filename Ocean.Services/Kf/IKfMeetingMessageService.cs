using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity;
using Ocean.Core;
using Ocean.Entity.DTO;

namespace Ocean.Services
{
    public interface IKfMeetingMessageService : IService<KfMeetingMessage>
    {
        void InsertKfMeetingMessage(Guid kfMeetingId, int whoIsSend, string messageContent);

        PagedList<KfMeetingMessage> GetPageList(int pageIndex, int pageSize, KfMeetingMessageDTO kfMeetingMessageDTO);
    }
}