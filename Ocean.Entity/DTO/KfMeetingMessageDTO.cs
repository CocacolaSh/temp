using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Entity.DTO
{
    public class KfMeetingMessageDTO : BaseDTO
    {
        public KfMeetingMessageDTO()
        { 
            
        }

        public KfMeetingMessageDTO(KfMeetingMessage kfMeetingMessage)
        {
            this.Id = kfMeetingMessage.Id;
            this.CreateDate = kfMeetingMessage.CreateDate;
            this.KfMeetingId = kfMeetingMessage.KfMeetingId;
            this.WhoIsSend = kfMeetingMessage.WhoIsSend;
            this.MessageContent = kfMeetingMessage.MessageContent;
            this.IsRead = kfMeetingMessage.IsRead;

            if (kfMeetingMessage.KfMeeting.KfNumber != null)
            {
                KfNickName = kfMeetingMessage.KfMeeting.KfNumber.NickName;
            }
            else
            {
                KfNickName = string.Empty;
            }

            if (kfMeetingMessage.KfMeeting.MpUser != null)
            {
                MpUserNickName = kfMeetingMessage.KfMeeting.MpUser.NickName;
            }
            else
            {
                MpUserNickName = string.Empty;
            }

            if (kfMeetingMessage.WhoIsSend == 1)
            {
                this.SendNickName = KfNickName;
                this.ReceiveNickName = MpUserNickName;
            }
            else if (kfMeetingMessage.WhoIsSend == 2)
            {
                this.SendNickName = MpUserNickName;
                this.ReceiveNickName = KfNickName;
            }
            else
            {
                this.SendNickName = string.Empty;
                this.ReceiveNickName = string.Empty;
            }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDate { set; get; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { set; get; }

        /// <summary>
        /// 客服昵称
        /// </summary>
        public string KfNickName { set; get; }

        /// <summary>
        /// 访客昵称
        /// </summary>
        public string MpUserNickName { set; get; }

        /// <summary>
        /// 发送方昵称
        /// </summary>
        public string SendNickName { set; get; }

        /// <summary>
        /// 接收方昵称
        /// </summary>
        public string ReceiveNickName { set; get; }

        /// <summary>
        /// 会话Id
        /// </summary>
        public Guid KfMeetingId { set; get; }

        /// <summary>
        /// 公众号用户Id
        /// </summary>
        public Guid MpUserId { set; get; }

        /// <summary>
        /// 客服工号Id
        /// </summary>
        public Guid KfNumberId { set; get; }

        /// <summary>
        /// 会话实体
        /// </summary>
        public virtual KfMeeting KfMeeting { set; get; }

        /// <summary>
        /// 发送方[1:客服,2:访客]
        /// </summary>
        public int WhoIsSend { set; get; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string MessageContent { set; get; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsRead { set; get; }
    }
}