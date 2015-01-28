using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Page;
using Ocean.Services;
using Ocean.Entity;
using Ocean.Core;
using Newtonsoft.Json;
using Ocean.Entity.DTO;
using Ocean.Core.Utility;
using Ocean.Core.Logging;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Helpers;

namespace Ocean.Admin.Controllers
{
    public class MpUserController : AdminBaseController
    {
        private readonly IMpUserGroupService MpUserGroupService;
        private readonly IMpUserService MpUserService;
        public MpUserController(IMpUserGroupService MpUserGroupService, IMpUserService MpUserService)
        {
            this.MpUserGroupService = MpUserGroupService;
            this.MpUserService = MpUserService;
        }

        /// <summary>
        /// 微信用户分组页面
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            if (!base.HasPermission("mpgroup", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return View();
        }

        /// <summary>
        /// 获取用户分组列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_MpUserGroupList")]
        public ActionResult MpUserGroupProvide()
        {
            PagedList<Ocean.Entity.MpUserGroup> list = MpUserGroupService.GetPageList(PageIndex, PageSize);
            return JsonList<MpUserGroup>(list, list.TotalItemCount);
        }

        /// <summary>
        /// 初始化编辑用户分组页面
        /// </summary>
        [HttpGet]
        public ActionResult MpUserGroupEdit()
        {
            string id = RQuery["Id"];
            MpUserGroup mp = string.IsNullOrWhiteSpace(id) ? null : MpUserGroupService.GetById(new Guid(id));
            return AdminView(mp);
        }

        /// <summary>
        ///编辑用户分组提交
        /// </summary>
        [HttpPost]
        [ActionName("_MpUserGroupEdit")]
        public ActionResult MpUserGroupEditProvide()
        {
            Ocean.Entity.MpUserGroup mp = new Entity.MpUserGroup();
            TryUpdateModel<MpUserGroup>(mp);
            if (mp.Id == Guid.Empty)
            {
                mp.CreateDate = DateTime.Now;
                MpUserGroupService.Insert(mp);
                return JsonMessage(true, "添加用户分组成功");
            }
            else
            {
                Ocean.Entity.MpUserGroup oldMp = MpUserGroupService.GetById(mp.Id);
                if (oldMp != null)
                {
                    oldMp.Name = mp.Name;
                    oldMp.OrderIndex = mp.OrderIndex;
                    oldMp.Description = mp.Description;
                    oldMp.IsSystem = false;
                    oldMp.GID = 0;
                    oldMp.UpdateUser = mp.Id;
                    oldMp.UpdateDate = DateTime.Now;
                    MpUserGroupService.Update(oldMp);
                    return JsonMessage(true, "修改用户分组成功");
                }
            }
            return JsonMessage(false, "修改用户分组失败！");
        }

        /// <summary>
        /// 解除公众账号的绑定
        /// </summary>
        [HttpPost]
        [ActionName("_MpUserGroupDelete")]
        public ActionResult MpUserGroupDeleteProvide(Guid id)
        {
            //先查询该分组下是否有会员
            if (MpUserService.GetCountByGroup(id) == 0)
            {
                MpUserGroupService.Delete(id.ToString());
                return JsonMessage(true, "删除成功");
            }
            else
            {
                return JsonMessage(false, "该分组下尚有用户，先将用户移入其他分组，再执行此操作");
            }

        }


        /// <summary>
        /// 微信用户列表页面
        /// </summary>
        [HttpGet]
        public ActionResult UserIndex()
        {
            if (!base.HasPermission("mpuser", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return View();
        }

        /// <summary>
        /// 获取用户分组列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_MpUserList")]
        public ActionResult MpUserListProvide(MpUserDTO mpDto)
        {
            PagedList<MpUserDTO> list = MpUserService.GetUsers(PageIndex, PageSize, mpDto);
            return JsonList<MpUserDTO>(list, list.TotalItemCount);
        }

        /// <summary>
        /// 获取会员分组左边的列表
        /// </summary>
        [HttpPost]
        [ActionName("_MpUserGroupJson")]
        public ActionResult MpUserGroupJson()
        {
            string strGroup = "[{\"id\":\"00000\",\"text\":\"全部分组\",\"children\":[";
            IList<Ocean.Entity.MpUserGroup> list = MpUserGroupService.GetALL();
            foreach (var item in list)
            {
                strGroup += "{\"id\":\"" + item.Id.ToString() + "\",\"text\":\"" + item.Name + "\"},";
            }
            strGroup = strGroup.Substring(0, strGroup.Length - 1);
            strGroup += "]}]";
            return Content(strGroup);
        }

        public ActionResult UpdateUserGroup()
        {
            IList<MpUserGroup> grouplist = MpUserGroupService.GetALL();
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (var item in grouplist)
            {
                selectList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            return View(selectList);
        }

        /// <summary>
        /// 获取用户分组列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_UpdateUserGroup")]
        public ActionResult UserGroupUpdate(string ids, Guid MpUserGroup)
        {

            string[] idsArr = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (idsArr != null && idsArr.Length > 0)
            {
                Guid[] gids = TypeConverter.StrsToGuids(idsArr);
                MpUserService.UpdateGroup(gids, MpUserGroup);
            }
            return JsonMessage(true, "添加成功");
        }


        /// <summary>
        /// 移到黑名单
        /// </summary>
        [HttpPost]
        [ActionName("_MoveToBack")]
        public ActionResult MoveToBack(string ids)
        {

            string[] idsArr = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (idsArr != null && idsArr.Length > 0)
            {
                Guid[] gids = TypeConverter.StrsToGuids(idsArr);
                MpUserGroup MpUserGroup = MpUserGroupService.GetSystemGroup("黑名单");
                MpUserService.UpdateGroup(gids, MpUserGroup.Id);
            }
            return JsonMessage(true, "移除成功");
        }


        /// <summary>
        /// 根据API获取会员数据
        /// </summary>
        [ActionName("_GetUserFromApi")]
        public ActionResult GetUserFromApi()
        {
            string accessToken = "w7HIAWRKthV8FfEZpGPsutRhm3Hps6tmg5QiTE3Aw4cshDv89cu3UfHBLZEUUrJV2J30Z8088heeOZK-HwkTwg";
            MpUserService.GetUserFromApi(accessToken);
            return View();
        }

        /// <summary>
        /// 根据API给用户发消息
        /// </summary>
        [HttpPost]
        [ActionName("_SendMsg")]
        public ActionResult SendMsg(string OpenId, string content)
        {
            bool bflag = true;
            bool allFlag = true;
            string openlist = "";
            string[] idsArr = OpenId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (idsArr != null && idsArr.Length > 0)
            {
                for (int i = 0; i < idsArr.Length; i++)
                {
                    bflag = SendMessage(idsArr[i], content);
                    if (!bflag)
                    {
                        allFlag = false;
                        openlist += idsArr[i]+",";
                    }
                }
            }
            return JsonMessage(allFlag, openlist.Length>0?"有部分OpenId没有发送成功\r\n:"+openlist:"消息发送成功");
        }
        [HttpPost]
        [ActionName("_UpdateUserInfo")]
        public ActionResult UpdateUserInfo(string ids)
        {
            string[] idsArr = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (idsArr != null && idsArr.Length > 0)
            {
                Guid[] gids = TypeConverter.StrsToGuids(idsArr);
                IList<MpUser> mpusers = MpUserService.GetALL(p => gids.Contains(p.Id));
                string token = GetAccessToken();

                if (mpusers != null && mpusers.Count > 0 && !string.IsNullOrEmpty(token))
                {

                    foreach (MpUser currUser in mpusers)
                    {
                        try
                        {

                            UserInfoJson info = Senparc.Weixin.MP.AdvancedAPIs.User.Info(token, currUser.OpenID);
                            if (info != null)
                            {
                                currUser.City = info.city;
                                currUser.Country = info.country;
                                currUser.HeadImgUrl = info.headimgurl;
                                currUser.Language = info.language;
                                currUser.NickName = info.nickname;
                                currUser.Province = info.province;
                                currUser.Sex = info.sex;
                                currUser.SubscribeDate = DateTimeHelper.GetDateTimeFromXml(info.subscribe_time);
                                MpUserService.Update(currUser);
                            }
                        }
                        catch (Exception e)
                        {
                            Log4NetImpl.Write("AysnUser失败:" + e.Message);
                        }
                    }
                }
            }
            return JsonMessage(true, "同步资料成功");
        }

    }
}