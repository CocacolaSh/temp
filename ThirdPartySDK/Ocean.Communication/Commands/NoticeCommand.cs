using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Communication.Commands
{
    [Command("Notice", "发送通知", "[SendUserId],[SendNickName],[ReceiveUserId],[NoticeType],[Status],[Args]", "communication")]
    public class NoticeCommand : SendNoticeCommand
    {
        public override void ExecuteNotice(CallerContext callerContext, string[] args, ref string message)
        {
            this.SendNotice(new Guid(args[0]),args[1], new Guid(args[2]), int.Parse(args[3]), int.Parse(args[4]), args[5]);
        }
    }
}