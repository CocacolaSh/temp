using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Entity.DTO
{
    public class KfNumberDTO : BaseDTO
    {
        public KfNumberDTO(KfNumber kfNumber)
        {
            this.Id = kfNumber.Id;
            this.CreateDate = kfNumber.CreateDate;
            this.Number = kfNumber.Number;
            this.NickName = kfNumber.NickName;
            this.IsOnline = kfNumber.IsOnline;
            this.OnlineStatus = kfNumber.OnlineStatus;
            this.AdminName = kfNumber.Admin.Name;
        }

        /// <summary>
        /// 工号
        /// </summary>
        public string Number { set; get; }

        /// <summary>
        /// 客服昵称
        /// </summary>
        public string NickName { set; get; }

        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsOnline { set; get; }

        /// <summary>
        /// 在线状态[0:正常,1:忙碌,2:离开,3:隐身]
        /// </summary>
        public int OnlineStatus { set; get; }

        /// <summary>
        /// 管理员名称
        /// </summary>
        public string AdminName { set; get; }
    }
}