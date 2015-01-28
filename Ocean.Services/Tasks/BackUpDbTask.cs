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
    public partial class BackUpDbTask : ITask
    {
        private readonly IMpUserService _mpUserService;
        public BackUpDbTask(IMpUserService mpUserService)
        {
            _mpUserService = mpUserService;
        }

        /// <summary>
        /// 实现自己的任务逻辑
        /// </summary>
        public void Execute()
        {
            if (DateTime.Now.Hour == 1)
            {
                Log4NetImpl.Write("执行任务！" + DateTime.Now);
                Dictionary<string, object> parms = new Dictionary<string, object>();
                parms.Add("dbname", "ocean");
                parms.Add("bkpath", "D:\\Db\\");
                parms.Add("bkfname", "db_\\DATE\\_db.bak");
                _mpUserService.ExcuteProc<int>("p_backupdb", parms);
            }
            
        }
    }
}