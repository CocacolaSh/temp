using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Entity.DTO
{
    public class KfMeetingDTO : BaseDTO
    {
        public KfMeetingDTO() { }

        public KfMeetingDTO(KfMeeting kfMeeting)
        {
            this.Id = kfMeeting.Id;
            this.CreateDate = kfMeeting.CreateDate;
            this.KfNumberId = kfMeeting.KfNumberId;

            if (kfMeeting.KfNumber != null)
            {
                this.NickName = kfMeeting.KfNumber.NickName;
            }
            else
            {
                this.NickName = string.Empty;
            }

            this.MpUserId = kfMeeting.MpUserId;

            if (kfMeeting.MpUser != null)
            {
                this.MpUserName = kfMeeting.MpUser.NickName;
            }
            else
            {
                this.MpUserName = string.Empty;
            }

            this.RecordCount = kfMeeting.RecordCount;
            this.BeginDate = kfMeeting.BeginDate;
            this.EndDate = kfMeeting.EndDate;
            this.MeetingPage = kfMeeting.MeetingPage;
            this.Explain = kfMeeting.Explain;
            this.IsEnd = kfMeeting.IsEnd;

            if (EndDate.HasValue)
            {
                TimeSpan ts = EndDate.Value.Subtract(BeginDate);
                this.Duration = ts.Hours + "小时" + ts.Minutes + "分钟" + ts.Seconds + "秒";
            }
            else
            {
                this.Duration = string.Empty;
            }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? MeetingStartDate { set; get; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? MeetingEndDate { set; get; }

        /// <summary>
        /// 客服工号Id
        /// </summary>
        public Guid KfNumberId { set; get; }

        /// <summary>
        /// 客服昵称
        /// </summary>
        public string NickName { set; get; }

        /// <summary>
        /// 访客Id
        /// </summary>
        public Guid MpUserId { set; get; }

        /// <summary>
        /// 访客名称
        /// </summary>
        public string MpUserName { set; get; }

        /// <summary>
        /// 记录总数
        /// </summary>
        public int RecordCount { set; get; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginDate { set; get; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { set; get; }

        /// <summary>
        /// 时长
        /// </summary>
        public string Duration { set; get; }

        /// <summary>
        /// 咨询页面
        /// </summary>
        public string MeetingPage { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Explain { set; get; }

        /// <summary>
        /// 是否结束
        /// </summary>
        public bool IsEnd { set; get; }
    }
}