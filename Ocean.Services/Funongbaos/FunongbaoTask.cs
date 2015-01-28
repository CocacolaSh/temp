using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Services.Tasks;
using Ocean.Core.Logging;
using Ocean.Entity;
using Ocean.Core;
using Ocean.Entity.DTO;
using Ocean.Core.Configuration;

namespace Ocean.Services
{
    public partial class FunongbaoTask : ITask
    {
        private readonly IFunongbaoApplyService _funongbaoApplyService;
        private readonly IMpUserService _mpUserService;
        public FunongbaoTask(IFunongbaoApplyService funongbaoApplyService,IMpUserService mpUserService)
        {
            this._funongbaoApplyService = funongbaoApplyService; 
            _mpUserService = mpUserService;
        }

        /// <summary>
        /// 实现自己的任务逻辑
        /// </summary>
        public void Execute()
        {
            ////测试
            //FunongbaoApply apply = _funongbaoApplyService.GetUnique(f => f.Id == new Guid("9A904BD7-5556-4EB2-A493-6C3DC85CB4BD") && f.NoticeStatus == 0);
            //if (apply != null)
            //{
            //    _mpUserService.SendMessage(apply.RFunongbao.OpenId, "尊敬的" + apply.RFunongbao.Name + "，您本季度有新的额度可供调整。\r\n" + "<a href=\"" + BaseConfigs.GetConfig().WebUrl + "/funongbao/apply\">点击这里，立即申请</a>");//"点击菜单->福农宝->调额申请");
            //    apply.NoticeStatus = 1;
            //    _funongbaoApplyService.Update(apply);
            //}
            //尊敬的**，您本季度有新的额度可供调整。
            //点击这里，立即申请
            if (DateRuleList.DateRules.Where(d => d.ApplyMonth == DateTime.Now.Month).Count() > 0 && DateTime.Now.Day == DateRule.ApplyStartDay)
            {
                Log4NetImpl.Write("执行任务！" + DateTime.Now);
                //PagedList<FunongbaoApply> applys = _funongbaoApplyService.GetForTaskUserList(1, 100);
                //if (applys != null)
                //{
                //    foreach (FunongbaoApply apply in applys)
                //    {
                //        apply.NoticeStatus = 1;
                //        _mpUserService.SendMessage(apply.RFunongbao.OpenId, "尊敬的" + apply.RFunongbao.Name + "，您本季度有新的额度可供调整。\r\n" + "点击菜单->福农宝->调额申请");//"<a href=\"" + BaseConfigs.GetConfig().WebUrl + "/funongbao/apply\">点击这里，立即申请</a>");
                //        _funongbaoApplyService.Update(apply);
                //    }
                //}
                //for (int i = 1; i < applys.TotalPageCount; i++)
                //{
                //    applys = _funongbaoApplyService.GetForTaskUserList((i + 1), 100);
                //    if (applys != null)
                //    {
                //        foreach (FunongbaoApply apply1 in applys)
                //        {
                //            apply1.NoticeStatus = 1;
                //            _mpUserService.SendMessage(apply1.RFunongbao.OpenId, "尊敬的" + apply1.RFunongbao.Name + "，您本季度有新的额度可供调整。\r\n" + "点击菜单->福农宝->调额申请");//"<a href=\"" + BaseConfigs.GetConfig().WebUrl + "/funongbao/apply\">点击这里，立即申请</a>");
                //            _funongbaoApplyService.Update(apply1);
                //        }
                //    }
                //}
            }
        }
    }
}