﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="MpUser.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-02-10 16:38
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity;
using Ocean.Core.Data;
using Ocean.Data;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.CommonAPIs;
using Ocean.Core;
using Ocean.Entity.DTO;
using Ocean.Core.Logging;
using Ocean.Core.Infrastructure;
using System.Web;
using Ocean.Core.Caching;

namespace Ocean.Services
{
    public class MpUserService : ServiceBase<MpUser>, IMpUserService
    {
        protected static ICacheManager _cacheManager = new MemoryCacheManager();
        public MpUserService(IRepository<MpUser> mpUserRepository, IDbContext context)
            : base(mpUserRepository, context)
        {
        }

        /// <summary>
        /// WebHelper
        /// </summary>
        protected IMpCenterService MpCenterService
        {
            get
            {
                return EngineContext.Current.Resolve<IMpCenterService>();
            }
        }

        public void GetUserFromApi(string token)
        {
            ////AccessTokenResult token = CommonApi.GetToken("wx8228154e6fa07a5f", "28dff7e4ba00f9a005a9b81bbf10f9be");
            ////string str=HttpHelper.GetHtml("https://api.weixin.qq.com/cgi-bin/menu/create?access_token="+token.access_token, "{\"button\":[{\"name\":\"福农宝\",\"sub_button\":[{\"type\":\"view\",\"name\":\"产品介绍\",\"url\":\"http://wx.ssrcb.com/funongbao\"},{\"type\":\"view\",\"name\":\"我的福农宝\",\"url\":\"http://wx.ssrcb.com/funongbao\"},{\"type\":\"view\",\"name\":\"调额申请\",\"url\":\"http://wx.ssrcb.com/apply\"}]},{\"type\":\"view\",\"name\":\"贷款申请\",\"\"url\":\"http://wx.ssrcb.com/bankloan/loanapply\"},{\"name\":\"客户服务\",\"sub_button\":[{	\"type\":\"view\",\"name\":\"人工客服\",\"url\":\"http://wx.ssrcb.com\"},{\"type\":\"view\",\"name\":\"便民服务\",\"url\":\"http://wx.ssrcb.com\"},{\"type\":\"view\",\"name\":\"周边网点\",\"url\":\"http://wx.ssrcb.com\"}]}]}", true);
            ////token = "9xUfNRNvDlP11NUJL8ZA0372j505i2YLcdjh_kMZLZjvA6wwXWRgUnyHnLjHACUij94jdayhXUNraYiIm15XR4042ZBgkD8C3NlucUipad6GcAECRI-FVRycrPZVPAiiE4Ft_JGo_LhBKoDUtsyfJw";
            ////string str = HttpHelper.GetHtml("https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token, "{\"button\":[{\"name\":\"福农宝\",\"sub_button\":[{\"type\":\"view\",\"name\":\"产品介绍\",\"url\":\"http://wx.ssrcb.com\"},{\"type\":\"view\",\"name\":\"我的福农宝\",\"url\":\"http://wx.ssrcb.com/funongbao/myfnb\"},{\"type\":\"view\",\"name\":\"调额申请\",\"url\":\"http://wx.ssrcb.com/funongbao/apply\"}]},{\"type\":\"view\",\"name\":\"贷款申请\",\"url\":\"http://wx.ssrcb.com/bankloan/loanapply\"},{\"name\":\"客户服务\",\"sub_button\":[{\"type\":\"click\",\"name\":\"人工客服\",\"key\":\"ContactUs\"},{	\"type\":\"view\",\"name\":\"便民服务\",\"url\":\"http://wx.ssrcb.com/Life/Search\"},{	\"type\":\"view\",\"name\":\"手机银行下载\",\"url\":\"http://wx.ssrcb.com/MpMaterial/App\"}]}]}", true);
            //使用这个
            //string str = HttpHelper.GetHtml("https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token, "{\"button\":[{\"name\":\"福农宝\",\"sub_button\":[{\"type\":\"view\",\"name\":\"产品介绍\",\"url\":\"http://wx.ssrcb.com\"},{\"type\":\"view\",\"name\":\"我的福农宝\",\"url\":\"http://wx.ssrcb.com/funongbao/myfnb\"},{\"type\":\"view\",\"name\":\"调额申请\",\"url\":\"http://wx.ssrcb.com/funongbao/apply\"}]},{\"name\":\"贷款/POS\",\"sub_button\":[{\"type\":\"view\",\"name\":\"贷款申请\",\"url\":\"http://wx.ssrcb.com/bankloan/loanapply\"},{\"type\":\"view\",\"name\":\"POS\",\"url\":\"http://wx.ssrcb.com/pos/index\"}]},{\"name\":\"客户服务\",\"sub_button\":[{\"type\":\"click\",\"name\":\"人工客服\",\"key\":\"ContactUs\"},{\"type\":\"view\",\"name\":\"他行汇入我行\",\"url\":\"http://wx.ssrcb.com/MpMaterial/App\"},{	\"type\":\"view\",\"name\":\"客户中心\",\"url\":\"http://wx.ssrcb.com/mpuser/ucenter\"},{\"type\":\"view\",\"name\":\"机具维护人员\",\"url\":\"http://wx.ssrcb.com/Operator/Index\"}, {\"type\":\"click\",\"name\":\"附近网点\",\"key\":\"NearBy\"}]}]}", true);
            string str = HttpHelper.GetHtml("https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token, "{\"button\":[{\"name\":\"福农宝\",\"sub_button\":[{\"type\":\"view\",\"name\":\"产品介绍\",\"url\":\"http://wx.ssrcb.com\"},{\"type\":\"view\",\"name\":\"我的福农宝\",\"url\":\"http://wx.ssrcb.com/funongbao/myfnb\"},{\"type\":\"view\",\"name\":\"调额申请\",\"url\":\"http://wx.ssrcb.com/funongbao/apply\"}]},{\"name\":\"贷款/POS\",\"sub_button\":[{\"type\":\"view\",\"name\":\"贷款申请\",\"url\":\"http://wx.ssrcb.com/bankloan/loanapply\"},{\"type\":\"view\",\"name\":\"POS\",\"url\":\"http://wx.ssrcb.com/pos/index\"}]},{\"name\":\"客户/抽奖\",\"sub_button\":[{\"type\":\"click\",\"name\":\"人工客服\",\"key\":\"ContactUs\"},{\"type\":\"view\",\"name\":\"他行汇入我行\",\"url\":\"http://wx.ssrcb.com/MpMaterial/App\"},{	\"type\":\"view\",\"name\":\"客户中心\",\"url\":\"http://wx.ssrcb.com/mpuser/ucenter\"},{\"type\":\"view\",\"name\":\"机具维护人员\",\"url\":\"http://wx.ssrcb.com/Operator/Index\"}, {\"type\":\"view\",\"name\":\"手机银行抽奖\",\"url\":\"http://wx.ssrcb.com/plugins/lottery?id=9063c7f9-70b3-4b93-b005-e2941fdc791e\"}]}]}", true);
            ////OpenIdResultJson openlist = User.Get("", "");
        }

        public PagedList<MpUserDTO> GetUsers(int pageIndex, int pageSize, MpUserDTO mpDto)
        {
            string sql = "select mug.*,mq.title , a.Name as LoginName from (select mp.*,(isnull(mp.Country,'')+','+isnull(mp.Province,'')+','+isnull(mp.City,'')) as Area,mpg.Name as CateName";
            sql += " from mpuser mp,mpusergroup mpg where mp.MpGroupID=mpg.Id ";
            Dictionary<string, object> parms = new Dictionary<string, object>();
            if (mpDto != null)
            {
                if (mpDto.MpGroupID != Guid.Empty)
                {
                    sql += " and mp.MpGroupID=@gId";
                    parms.Add("gId", mpDto.MpGroupID.ToString());
                }

                if (!string.IsNullOrEmpty(mpDto.NickName) && !string.IsNullOrEmpty(mpDto.NickName.Trim()))
                {
                    sql += " and mp.NickName like @nickName";
                    parms.Add("nickName", "%" + mpDto.NickName.ToString() + "%");
                }

                if (mpDto.IsAuth == 1)
                {
                    sql += " and mp.IsAuth =1";
                }
            }
            sql += ") as mug left join dbo.MpQrScene mq on mug.sceneid=mq.sceneid left join dbo.Admin a on mug.adminid = a.Id ";
            if (mpDto != null)
            {
                if (!string.IsNullOrEmpty(mpDto.LoginName) && !string.IsNullOrEmpty(mpDto.LoginName.Trim()))
                {
                    sql += " where a.Name like @LoginName";
                    parms.Add("LoginName", "%" + mpDto.LoginName.Trim() + "%");
                }
                if (!string.IsNullOrEmpty(mpDto.Qrcode))
                {
                    sql += " and mq.Title like @qrcode";
                    parms.Add("qrcode", "%" + mpDto.Qrcode.Trim() + "%");
                }
            }
            sql += " order by mug.LastVisitDate desc";
            return this.GetPageList<MpUserDTO>(sql, parms, pageIndex, pageSize);
        }

        public PagedList<MpUser> GetUnderUsers(int pageIndex, int pageSize, MyUnderDTO mpUnderDto, int isAll)
        {
            string sql = "select * from MpUser ";

            string condition = "where ParentPhone = (select MobilePhone from MpUser where Id = '" + mpUnderDto.MpUserId + "')";
            Dictionary<string, object> parms = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(mpUnderDto.Name))
            {
                parms.Add("Name", mpUnderDto.Name);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "Name like '%'+@Name+'%'";
            }
            if (!string.IsNullOrEmpty(mpUnderDto.Phone))
            {
                parms.Add("MobilePhone", mpUnderDto.Phone);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "MobilePhone like '%'+@MobilePhone+'%'";
            }

            if ((mpUnderDto.IsAuth.HasValue))
            {
                parms.Add("IsAuth", mpUnderDto.IsAuth);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "IsAuth = @IsAuth";
            }
            if (isAll == 0)
            {
                parms.Add("MpUserId", mpUnderDto.MpUserId);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "ParentPhone = (select MobilePhone from MpUser where Id = @MpUserId)";
            }
            else
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "ParentPhone != ''";
            }

            sql += condition;
            return this.GetPageList<MpUser>(sql, parms, pageIndex, pageSize);
        }
        public void UpdateGroup(Guid[] ids, Guid gid)
        {
            IList<MpUser> mpusers = this.GetALL(p => ids.Contains(p.Id));
            if (mpusers != null && mpusers.Count > 0)
            {
                foreach (var item in mpusers)
                {
                    item.MpGroupID = gid;
                    this.Update(item);
                }
            }
        }

        public int GetCountByGroup(Guid groupId)
        {
            string sql = "select * from MpUser where MpGroupID=@gid";
            Dictionary<string, object> parms = new Dictionary<string, object>();
            parms.Add("gid", groupId);
            return this.GetCount(sql, parms);
        }

        public MpUser GetByOpenID(string openid)
        {
            return this.GetALL(u => u.OpenID == openid).FirstOrDefault();
        }

        public MpUser AddUser(MpUser MpUser)
        {
            this.Insert(MpUser);
            return MpUser;
        }

        public MpUser UpdateUserInfo(string OpenId, string token)
        {
            MpUser currUser = this.GetByOpenID(OpenId);
            if (currUser != null && currUser.IsSubscribe)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        UserInfoJson info = Senparc.Weixin.MP.AdvancedAPIs.User.Info(token, OpenId);
                        if (info != null)
                        {
                            currUser.City = info.city;
                            currUser.Country = info.country;
                            currUser.HeadImgUrl = info.headimgurl;
                            currUser.Language = info.language;
                            currUser.NickName = info.nickname;
                            currUser.Province = info.province;
                            currUser.Sex = info.sex;
                            this.Update(currUser);
                        }
                    }
                    catch (Exception e)
                    {
                        Log4NetImpl.Write("UpdateUserInfo失败:" + e.Message);
                    }
                }
            }
            return currUser;
        }

        public bool SendMessage(string openId, string content, string token = "")
        {
            if (string.IsNullOrEmpty(token))
            {
                token = GetAccessToken();
            }
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    WxJsonResult errorResult = Custom.SendText(token, openId, content);
                    if (errorResult.errcode != ReturnCode.请求成功)
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Log4NetImpl.Write("SendMessage：失败" + e.Message);
                    return false;
                }

                return true;
            }
            return false;
        }

        public string GetAccessToken()
        {
            MpCenter mp = MpCenterService.GetALL().FirstOrDefault();
            try
            {
                if (mp != null && !string.IsNullOrEmpty(mp.AppID) && !string.IsNullOrEmpty(mp.AppSecret))
                {
                    var timespan = DateTime.Now - mp.GetTokenDate.Value;
                    if (timespan.TotalMinutes > 15 || string.IsNullOrEmpty(mp.AccessToken))
                    {
                        AccessTokenResult token = CommonApi.GetToken(mp.AppID, mp.AppSecret);
                        if (token != null && !string.IsNullOrEmpty(token.access_token))
                        {
                            Log4NetImpl.Write("GetAccessToken更新:not null");
                            mp.GetTokenDate = DateTime.Now;
                            mp.AccessToken = token.access_token;
                            MpCenterService.ExcuteSql("update MpCenter set GetTokenDate='" + DateTime.Now + "',AccessToken='" + token.access_token + "' where Id='" + mp.Id.ToString() + "'");
                            MpCenter mpCache = _cacheManager.Get<MpCenter>("MpCenter");
                            mpCache = mp;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log4NetImpl.Write("GetAccessToken失败:" + e.Message);
                return "";
            }

            return mp.AccessToken;
        }
        public OceanDynamicList<object> GetMyBusiness(Guid mpUserID)
        {
            string sql = @"select * from (
select Id,1 as ApplyType,CreateDate,ApplyLimit,ApplyRates,'' as VendorName,ProcessStatus,ProcessResult from FunongbaoApply where FunongbaoId=(select top 1 Id from Funongbao where MpuserId='" + mpUserID.ToString() + @"') and ApplyDate>'1900-01-01'
union all
select Id,2,CreateDate,ApplyMoney as ApplyLimit,0.00,'',ProcessStatus,ProcessResult from Loan where MpUserId='" + mpUserID.ToString() + @"'
union all
select Id,3,CreateDate,0.00,0.00,case when isnull(VendorName,'')='' then VendorAddress else VendorName end,ProcessStatus,ProcessResult from PosApply where MpUserId='" + mpUserID.ToString() + @"'
) t order by CreateDate";
            return this.GetDynamicList(sql);
        }
        

        #region plugin
        public PagedList<MpUser> GetMpUsers(int pageIndex, int pageSize, string whereStr, bool isFunongbao)
        {
            if (isFunongbao)
            {
                return this.GetPageList("select u.ID,u.OpenID,u.MpID,u.MpGroupID,f.Name ad NickName,u.OrginID,u.RemarkName,u.Sex,u.Language,u.City,u.Province,u.Country,u.HeadImgUrl,u.IsSubscribe,u.UserState,u.LocationX,u.LocationY,u.LocationLabel,u.SubscribeDate,u.LastVisitDate,u.CreateDate,u.PasswordKey,u.SessionId,u.PassportNO,u.Name,u.MobilePhone,u.IsAuth from MpUser u inner join Funongbao f on u.Id=f.MpUserId where isnull(u.OpenId,'')!='' and f.IsAuth=1 order by CreateDate desc " + whereStr, pageIndex, pageSize);
            }
            else
            {
                return this.GetPageList("from MpUser where isnull(OpenId,'')!=''" + whereStr, pageIndex, pageSize);
            }
        }
        #endregion

        #region Task
        public PagedList<MpUserDTO> GetForOutTaskUserList(int pageIndex, int pageSize)
        {
            int currentMonth = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            DateRule dateRule = DateRuleList.DateRules.Where(d => d.Months.Where(m => m == currentMonth).Count() > 0).First();
            string sql = "select u.* from FunongbaoApply fa inner join Funongbao f on fa.FunongbaoId=f.Id inner join MpUser u on f.MpUserId=u.Id where u.IsSubscribe=1 and fa.CreateDate>='" + (new DateTime(year, dateRule.ApplyMonth, 1)) + "' order by u.CreateDate desc ";//and datediff(hour,u.lastvisitdate,getdate())>=48 

            return this.GetPageList<MpUserDTO>(sql, pageIndex, pageSize);
        }
        public PagedList<MpUserDTO> GetForOutAuthUserList(int pageIndex, int pageSize)
        {
            int currentMonth = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            DateRule dateRule = DateRuleList.DateRules.Where(d => d.Months.Where(m => m == currentMonth).Count() > 0).First();
            string sql = "select u.* from Funongbao f inner join MpUser u on f.MpUserId=u.Id where u.IsSubscribe=1 order by u.CreateDate desc ";

            return this.GetPageList<MpUserDTO>(sql, pageIndex, pageSize);
        }
        public void ChangeMpUserState(Guid id, int state)
        {
            //MpUser user = this.GetById(id);
            //if (user != null)
            //{
            //    user.UserState = state;
            //    this.Update(user);
            //}
            this.ExcuteSql("update MpUser set UserState=" + state.ToString() + " where Id='" + id.ToString() + "'");
            
        }
        #endregion
    }
}
