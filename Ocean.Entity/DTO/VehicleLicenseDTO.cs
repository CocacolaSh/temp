using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Entity.DTO
{
    public class VehicleLicenseDTO : BaseDTO
    {
        public VehicleLicenseDTO() { }

        public VehicleLicenseDTO(VehicleLicense vehicleLicense)
        {
            this.Id = vehicleLicense.Id;
            this.CreateDate = vehicleLicense.CreateDate;
            this.Owner = vehicleLicense.Owner;
            this.PlateNo = vehicleLicense.PlateNo;
        }

        /// <summary>
        /// 车主姓名
        /// </summary>
        public string Owner { set; get; }
        /// <summary>
        /// 车牌号码
        /// </summary>
        public string PlateNo { set; get; }
        /// <summary>
        /// 业务员
        /// </summary>
        public string MpUser { set; get; }

        /// <summary>
        /// 上传的MP
        /// </summary>
        public Guid ?MpUserId { set; get; }
        /// <summary>
        /// 发证日期开始时间
        /// </summary>
        public DateTime? StartDate { set; get; }

        /// <summary>
        /// 发证日期结束时间
        /// </summary>
        public DateTime? EndDate { set; get; }

        
    }
}