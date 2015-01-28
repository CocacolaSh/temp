using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity;
using Ocean.Core.Data;
using Ocean.Data;
using Ocean.Entity.DTO;
using Ocean.Core;
using Ocean.Core.Data.OrderBy;

namespace Ocean.Services
{
    public class KfMeetingMessageService : ServiceBase<KfMeetingMessage>, IKfMeetingMessageService
    {
        public KfMeetingMessageService(IRepository<KfMeetingMessage> kfMeetingMessageRepository, IDbContext context)
            : base(kfMeetingMessageRepository, context)
        {
            
        }

        public void InsertKfMeetingMessage(Guid kfMeetingId, int whoIsSend, string messageContent)
        {
            KfMeetingMessage kfMeetingMessage = new KfMeetingMessage();
            kfMeetingMessage.KfMeetingId = kfMeetingId;
            kfMeetingMessage.WhoIsSend = whoIsSend;
            kfMeetingMessage.MessageContent = messageContent;
            base.Insert(kfMeetingMessage);
        }

        /// <summary>
        /// 根据搜索条件获取分页数据
        /// </summary>
        public PagedList<KfMeetingMessage> GetPageList(int pageIndex, int pageSize, KfMeetingMessageDTO kfMeetingMessageDTO)
        {
            IQueryable<KfMeetingMessage> query = base.Table;

            if (kfMeetingMessageDTO.StartDate.HasValue)
            {
                query = query.Where(k => k.CreateDate >= kfMeetingMessageDTO.StartDate);
            }

            if (kfMeetingMessageDTO.EndDate.HasValue)
            {
                DateTime dtEndDate = ((DateTime)kfMeetingMessageDTO.EndDate).AddDays(1);
                query = query.Where(k => k.CreateDate <= dtEndDate);
            }

            if (!string.IsNullOrWhiteSpace(kfMeetingMessageDTO.MessageContent))
            {
                query = query.Where(k => k.MessageContent.Contains(kfMeetingMessageDTO.MessageContent));
            }

            if (kfMeetingMessageDTO.KfMeetingId != null && kfMeetingMessageDTO.KfMeetingId != Guid.Empty)
            {
                query = query.Where(k => k.KfMeetingId == kfMeetingMessageDTO.KfMeetingId);
            }

            if (kfMeetingMessageDTO.MpUserId != null && kfMeetingMessageDTO.MpUserId != Guid.Empty)
            {
                query = query.Where(k => k.KfMeeting.MpUserId == kfMeetingMessageDTO.MpUserId && k.KfMeeting.KfNumberId == kfMeetingMessageDTO.KfNumberId);
            }

            if (kfMeetingMessageDTO.KfNumberId != null && kfMeetingMessageDTO.KfNumberId != Guid.Empty)
            {
                query = query.Where(k => k.KfMeeting.KfNumberId == kfMeetingMessageDTO.KfNumberId);
            }

            query = query.OrderByDescending("CreateDate");
            int count = query.Count();

            if (pageIndex == 1)
            {
                query = query.Take<KfMeetingMessage>(pageSize);
            }
            else
            {
                query = query.Skip<KfMeetingMessage>((pageIndex - 1) * pageSize).Take<KfMeetingMessage>(pageSize);
            }

            PagedList<KfMeetingMessage> pageItems = new PagedList<KfMeetingMessage>(query.ToList(), pageIndex, pageSize);
            pageItems.TotalItemCount = count;
            return pageItems;
        }
    }
}