using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Entity.DTO
{
    public class DrivingLicenseDTO : BaseDTO
    {
        public DrivingLicenseDTO() { }

        public DrivingLicenseDTO(DrivingLicense drivingLicense)
        {
            this.Id = drivingLicense.Id;
            this.CreateDate = drivingLicense.CreateDate;
            this.Name = drivingLicense.Name;
            this.CertNo = drivingLicense.CertNo;
        }

        /// <summary>
        /// 车主姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 号码
        /// </summary>
        public string CertNo { set; get; }
        /// <summary>
        /// 业务员
        /// </summary>
        public string BussinesName { set; get; }

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