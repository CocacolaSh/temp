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
    public class KfMeetingService : ServiceBase<KfMeeting>, IKfMeetingService
    {
        public KfMeetingService(IRepository<KfMeeting> kfMeetingRepository, IDbContext context)
            : base(kfMeetingRepository, context)
        {
            
        }

        /// <summary>
        /// 根据搜索条件获取分页数据
        /// </summary>
        public PagedList<KfMeeting> GetPageList(int pageIndex, int pageSize, KfMeetingDTO kfMeetingDTO)
        {
            IQueryable<KfMeeting> query = base.Table;

            if (kfMeetingDTO.MeetingStartDate.HasValue)
            {
                query = query.Where(k => k.CreateDate >= kfMeetingDTO.MeetingStartDate);
            }

            if (kfMeetingDTO.MeetingEndDate.HasValue)
            {
                DateTime dtEndDate = ((DateTime)kfMeetingDTO.MeetingEndDate).AddDays(1);
                query = query.Where(k => k.CreateDate <= dtEndDate);
            }

            if (!string.IsNullOrWhiteSpace(kfMeetingDTO.NickName))
            {
                query = query.Where(k => k.KfNumber.NickName == kfMeetingDTO.NickName);
            }

            if (kfMeetingDTO.KfNumberId != null && kfMeetingDTO.KfNumberId != Guid.Empty)
            {
                query = query.Where(k => k.KfNumberId == kfMeetingDTO.KfNumberId);
            }

            query = query.OrderByDescending("CreateDate");
            int count = query.Count();

            if (pageIndex == 1)
            {
                query = query.Take<KfMeeting>(pageSize);
            }
            else
            {
                query = query.Skip<KfMeeting>((pageIndex - 1) * pageSize).Take<KfMeeting>(pageSize);
            }

            PagedList<KfMeeting> pageItems = new PagedList<KfMeeting>(query.ToList(), pageIndex, pageSize);
            pageItems.TotalItemCount = count;
            return pageItems;
        }
    }
}