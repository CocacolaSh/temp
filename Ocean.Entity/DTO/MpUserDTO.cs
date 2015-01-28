using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;

namespace Ocean.Entity.DTO
{
    public class MpUserDTO : BaseDTO
    {
        public MpUserDTO()
        {

        }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { set; get; }

        /// <summary>
        /// 登陆名
        /// </summary>
        public string LoginName { set; get; }
        /// <summary>
        /// 备注名
        /// </summary>
        public string RemarkName { set; get; }

        /// <summary>
        /// OpenID
        /// </summary>
        public string OpenID { set; get; }

        /// <summary>
        /// 地区
        /// </summary>
        public string Area { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string City { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string Province { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string Country { set; get; }



        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImgUrl { set; get; }

        /// <summary>
        /// 分类
        /// </summary>
        public string CateName { set; get; }

        /// <summary>
        /// 分类
        /// </summary>
        public Guid MpGroupID { set; get; }

        public int Sex { set; get; }

        /// <summary>
        /// 关注
        /// </summary>
        public bool IsSubscribe { set; get; }

        public DateTime SubscribeDate { set; get; }

        public DateTime LastVisitDate { set; get; }

        public string Title { set; get; }

        public string Name { get; set; }

        public string MobilePhone { get; set; }

        public Guid? FnbID { get; set; }

        public int IsAuth { get; set; }

        public Guid? POSID { get; set; }

        public int SceneId { get; set; }
        public string Qrcode { get; set; }
    }
}