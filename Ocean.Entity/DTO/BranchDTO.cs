using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Entity.DTO
{
    public class BranchDTO : BaseDTO
    {
         public BranchDTO() { }

         public BranchDTO(Branch branch)
         {
             this.Id = branch.Id;
             this.CreateDate = branch.CreateDate;
             this.Name = branch.Name;
             this.Logo = branch.Logo;
             this.Address = branch.Address;
             this.Phone = branch.Phone;
             this.MobilePhone = branch.MobilePhone;
             this.ContactName = branch.ContactName;
             this.Longitude = branch.Longitude;
             this.Latitude = branch.Latitude;
             this.Status = branch.Status;
             this.Discription = branch.Discription;
             this.Type = branch.Type;
         }

         /// <summary>
         /// 网点名称
         /// </summary>
         public string Name { set; get; }

         /// <summary>
         /// LOGO
         /// </summary>
         public string Logo { set; get; }

         /// <summary>
         /// 地址
         /// </summary>
         public string Address { set; get; }

         /// <summary>
         /// 联系电话
         /// </summary>
         public string Phone { set; get; }

         /// <summary>
         /// 手机号码
         /// </summary>
         public string MobilePhone { set; get; }

         /// <summary>
         /// 联系人
         /// </summary>
         public string ContactName { set; get; }

         /// <summary>
         /// 经度
         /// </summary>
         public string Longitude { set; get; }

         /// <summary>
         /// 纬度
         /// </summary>
         public string Latitude { set; get; }

         /// <summary>
         /// 状态[0:未审核,1:已审核,2:被冻结]
         /// </summary>
         public int Status { set; get; }

         /// <summary>
         /// 描术-预留
         /// </summary>
         public string Discription { set; get; }

         /// <summary>
         /// 网点类型 
         /// </summary>
         public int? Type { set; get; }

         /// <summary>
         /// 距离
         /// </summary>
         public double Distance { set; get; }
    }
}