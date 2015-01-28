using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Framework.Configuration.reg.config
{
    [Serializable]
    public class RegConfigInfo
    {
        private bool openReg;

        #region 是否开启会员注册
        /// <summary>
        /// 是否开启会员注册
        /// </summary>
        public bool OpenReg
        {
            get
            {
                return this.openReg;
            }
            set
            {
                this.openReg = value;
            }
        }
        #endregion
    }
}
