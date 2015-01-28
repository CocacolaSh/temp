using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Services.Tasks;
using Ocean.Core.Logging;

namespace Ocean.Services
{
    public partial class LoanTask : ITask
    {
        private readonly ILoanService _loanService;

        public LoanTask(ILoanService loanService)
        {
            this._loanService = loanService;
        }

        /// <summary>
        /// 实现自己的任务逻辑
        /// </summary>
        public void Execute()
        {
            int count = _loanService.GetCount(" where 1=1");
            Log4NetImpl.Write(string.Format("贷款任务调度测试，当前贷款总条数{0}，任务调度执行时间{1}", count, DateTime.Now), Log4NetImpl.ErrorLevel.Info);
        }
    }
}