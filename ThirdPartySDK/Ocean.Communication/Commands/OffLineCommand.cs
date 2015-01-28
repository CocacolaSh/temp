using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Communication.Comet;
using Ocean.Page;
using Ocean.Services;
using Ocean.Core.Infrastructure;
using Ocean.Entity;

namespace Ocean.Communication.Commands
{
    [Command("OffLine", "用户离线", "[userId],[type(1:客服,2:访客)]", "communication")]
    public class OffLineCommand : SendNoticeCommand
    {
        public override void ExecuteNotice(CallerContext callerContext, string[] args, ref string message)
        {
            int type = int.Parse(args[1]);

            //客服离线
            if (type == 1)
            {
                Guid kfNumberId = new Guid(args[0]);
                //移除缓存中的客服
                MessageManager.Instance.RemoveCacheKF(kfNumberId);
                //离线时清理线程中的客服端(防止第一条离线信息丢失)
                CometThreadPool.DequeueCometWaitRequest(kfNumberId);
                //断开与所有访客的会话
                IKfNumberService kfNumberService = EngineContext.Current.Resolve<IKfNumberService>();
                IKfMeetingService kfMeetingService = EngineContext.Current.Resolve<IKfMeetingService>();
                IKfMeetingMessageService kfMeetingMessageService = EngineContext.Current.Resolve<IKfMeetingMessageService>();
                IMpUserService mpUserService = EngineContext.Current.Resolve<IMpUserService>();
                IList<KfMeeting> listMeeting = kfMeetingService.GetALL().Where(m => m.KfNumberId == kfNumberId && m.IsEnd == false).ToList();

                foreach (KfMeeting kfMeeting in listMeeting)
                {
                    kfMeeting.RecordCount = kfMeetingMessageService.GetCount(" where KfMeetingId='" + kfMeeting.Id + "'");
                    kfMeeting.EndDate = DateTime.Now;
                    kfMeeting.IsEnd = true;
                    kfMeetingService.Update(kfMeeting);
                    base.SendPrivateMessage(kfNumberId, kfMeeting.MpUserId, "本次会话已被断开，您可以重新发起会话请求");
                    //发送给微信
                    MpUser mpUser = mpUserService.GetById(kfMeeting.MpUserId);

                    if (mpUser != null && !string.IsNullOrWhiteSpace(mpUser.OpenID))
                    {
                        mpUserService.SendMessage(mpUser.OpenID, "本次会话已被断开，您可以重新发起会话请求");
                    }
                    //更新用户状态
                    mpUserService.ChangeMpUserState(kfMeeting.MpUserId, 0);
                }
                //把客服设置成离线
                KfNumber kfNumber = kfNumberService.GetById(kfNumberId);
                kfNumber.IsOnline = false;
                kfNumberService.Update(kfNumber);
            }

            //访客离线
            if (type == 2)
            {
                Guid mpUserId = new Guid(args[0]);
                //离线时清理线程中的访客端(防止第一条离线信息丢失)
                CometThreadPool.DequeueCometWaitRequest(mpUserId);
                //断开与客服的会话
                IKfMeetingService kfMeetingService = EngineContext.Current.Resolve<IKfMeetingService>();
                IKfMeetingMessageService kfMeetingMessageService = EngineContext.Current.Resolve<IKfMeetingMessageService>();
                IList<KfMeeting> listMeeting = kfMeetingService.GetALL().Where(m => m.MpUserId == mpUserId && m.IsEnd == false).ToList();

                foreach (KfMeeting kfMeeting in listMeeting)
                {
                    kfMeeting.RecordCount = kfMeetingMessageService.GetCount(" where KfMeetingId='" + kfMeeting.Id + "'");
                    kfMeeting.EndDate = DateTime.Now;
                    kfMeeting.IsEnd = true;
                    kfMeetingService.Update(kfMeeting);
                    base.SendNotice(mpUserId, "访客_" + new Random().Next(100), kfMeeting.KfNumberId, 3, 0, mpUserId.ToString());
                }
            }
        }
    }
}