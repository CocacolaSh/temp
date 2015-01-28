using System.Collections.Generic;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.GoogleMap;
using Senparc.Weixin.MP.Helpers;
using Ocean.Entity;
using Ocean.Services;
using Ocean.Core.Infrastructure;
using System;
using Ocean.Entity.DTO;
using Ocean.Core.Utility;
using Ocean.Framework.Configuration.global.config;

namespace Senparc.Weixin.MP.CommonService
{
    public class LocationService
    {
        protected IBranchService BranchService
        {
            get
            {
                return EngineContext.Current.Resolve<IBranchService>();
            }
        }
        protected IMpUserService MpUserService
        {
            get
            {
                return EngineContext.Current.Resolve<IMpUserService>();
            }
        }
        public ResponseMessageNews GetResponseMessage(RequestMessageLocation requestMessage)
        {
            MpUser fromUser = MpUserService.GetByOpenID(requestMessage.FromUserName);

            if (fromUser != null)
            {
                fromUser.LocationX = requestMessage.Location_X;
                fromUser.LocationY = requestMessage.Location_Y;
                fromUser.LocationLabel = requestMessage.Label;
                fromUser.LastVisitDate = DateTime.Now;
                MpUserService.Update(fromUser);
            }

            double longitude = requestMessage.Location_Y;
            double latitude = requestMessage.Location_X;
            //修正坐标
            DistanceHelper.ConvertCoordinate(ref longitude, ref latitude);
            IList<BranchDTO> listBranch = BranchService.GetBranch(longitude, latitude);
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);
            responseMessage.Articles.Add(new Article()
            {
                Title = "石狮农商银行周边网点",
                Description = "",
                PicUrl = GlobalConfig.GetConfig()["ResourceDomain"] + "/funongbaotop.jpg",
                Url = "http://wx.ssrcb.com/Branch/BranchListMap?point=" + longitude + "," + latitude,
            });
            //responseMessage.Articles.Add(new Article()
            //{
            //    Title = "石狮农商银行周边网点",
            //    Description = "",
            //    PicUrl = GlobalConfig.GetConfig()["ResourceDomain"] + "/funongbaotop.jpg",
            //    Url = "http://wx.ssrcb.com/Branch/BranchList?point=" + longitude + "," + latitude,
            //});
            //foreach (var item in listBranch)
            //{                        
            //    responseMessage.Articles.Add(new Article()
            //    {
            //        Title = item.Name+"("+item.Distance+"千米)"+"\n"+item.Address,
            //        Description = item.Address,
            //        PicUrl = "",
            //        Url = "http://wx.ssrcb.com/Branch/BranchInfo?id=" + item.Id.ToString() + "&longitude=" + longitude + "&latitude=" + latitude
            //    });
            //}
            return responseMessage;
        }
    }
}