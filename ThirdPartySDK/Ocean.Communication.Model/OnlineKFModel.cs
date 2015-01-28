using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Communication.Model
{
    [Serializable]
    public class OnlineKFModel
    {
        /// <summary>
        /// 客服工号Id
        /// </summary>
        public Guid KfNumberId { set; get; }

        #region 唯一标识
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Hash { set; get; }
        #endregion 

        #region 最后活动时间
        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime LastActiveDate { set; get; }
        #endregion
    }
}