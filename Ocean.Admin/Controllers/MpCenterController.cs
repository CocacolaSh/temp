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
using Ocean.Core.Infrastructure;
using Senparc.Weixin.MP.AdvancedAPIs;
using System.IO;
using Ocean.Framework.Configuration.global.config;
using Ocean.Core.Logging;

namespace Ocean.Admin.Controllers
{
    public class MpCenterController : AdminBaseController
    {
        private readonly IMpCenterService MpCenterService;
        /// <summary>
        /// MpQrSceneService
        /// </summary>
        protected IMpQrSceneService MpQrSceneService
        {
            get
            {
                return EngineContext.Current.Resolve<IMpQrSceneService>();
            }
        }
        public MpCenterController(IMpCenterService MpCenterService)
        {
            this.MpCenterService = MpCenterService;
        }

        /// <summary>
        /// 微信绑定页面
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            if (!base.HasPermission("mpcenterindex", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return View();
        }

        /// <summary>
        /// 获取管理员列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_MpList")]
        public ActionResult MpProvide()
        {
            PagedList<Ocean.Entity.MpCenter> list = MpCenterService.GetPageList(PageIndex, PageSize);
            return JsonList<MpCenter>(list, list.TotalItemCount);
        }

        /// <summary>
        /// 初始化MpEdit页面
        /// </summary>
        [HttpGet]
        public ActionResult MpEdit()
        {
            string id = RQuery["Id"];
            MpCenter mp = string.IsNullOrWhiteSpace(id) ? null : MpCenterService.GetById(new Guid(id));
            return AdminView(mp);
        }
        /// <summary>
        /// 初始化MpEdit页面
        /// </summary>
        [HttpPost]
        [ActionName("_MpEdit")]
        public ActionResult MpEditProvide()
        {
            Ocean.Entity.MpCenter mp = new Entity.MpCenter();
            TryUpdateModel<MpCenter>(mp);
            if (mp.Id.ToString().StartsWith("0000"))
            {
                MpCenterService.Insert(mp);
                MpCenterCache = mp;
                return JsonMessage(true, "绑定公众账号成功");
            }
            else
            {
                Ocean.Entity.MpCenter oldMp = MpCenterService.GetById(mp.Id);
                if (oldMp != null)
                {
                    oldMp.MpName = mp.MpName;
                    oldMp.OriginID = mp.OriginID;
                    oldMp.Token = mp.Token;
                    oldMp.AppID = mp.AppID;
                    oldMp.AppSecret = mp.AppSecret;
                    oldMp.UpdateUser = mp.Id;
                    oldMp.UpdateDate = DateTime.Now;
                    MpCenterService.Update(oldMp);
                    MpCenterCache = mp;
                    return JsonMessage(true, "公众账号绑定信息修改成功");
                }
            }
            return JsonMessage(false, "公众账号绑定失败");
        }

        /// <summary>
        /// 解除公众账号的绑定
        /// </summary>
        [HttpPost]
        [ActionName("_MpDelete")]
        public ActionResult MpDeleteProvide(Guid id)
        {
            MpCenterService.Delete(id.ToString());
            return JsonMessage(true, "删除成功");
        }

        [ActionName("QrCode")]
        public ActionResult QrCodeList()
        {
            if (!base.HasPermission("mpcenterqrcode", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return View();
        }
        /// <summary>
        /// 获取管理员列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_MpQrCodeList")]
        public ActionResult MpQrCodeProvide()
        {
            PagedList<Ocean.Entity.MpQrScene> list = MpQrSceneService.GetPageList(PageIndex, PageSize);
            return JsonList<MpQrScene>(list, list.TotalItemCount);
        }

        /// <summary>
        /// 初始化MpEdit页面
        /// </summary>
        [HttpGet]
        public ActionResult MpQrCodeEdit()
        {
            string id = RQuery["Id"];
            MpQrScene mp = string.IsNullOrWhiteSpace(id) ? null : MpQrSceneService.GetById(new Guid(id));
            return View(mp);
        }
        /// <summary>
        /// 初始化MpEdit页面
        /// </summary>
        [HttpPost]
        [ActionName("_MpQrCodeDelete")]
        public ActionResult MpQrCodeDelete(Guid id)
        {
            if (id != Guid.Empty)
            {
                    MpQrScene qr = MpQrSceneService.GetById(id);
                try
                {
                    if (qr != null)
                    {
                        MpQrSceneService.BeginTransaction();
                        MpQrSceneService.ExcuteSql("update MpUser set SceneId=0 where SceneId>0 and SceneId=" + qr.SceneId.ToString());
                        MpQrSceneService.Delete(qr);
                        MpQrSceneService.Commit();
                    }
                }
                catch (Exception ex)
                {
                    if (qr != null)
                    {
                        MpQrSceneService.Rollback();
                    }
                    Log4NetImpl.Write("delete qrcode:" + ex.ToString());
                    return JsonMessage(false, "删除失败");
                }
            }
            return JsonMessage(true);
        }
        /// <summary>
        /// 初始化MpEdit页面
        /// </summary>
        [HttpPost]
        [ActionName("_MpQrCodeEdit")]
        public ActionResult MpQrCodeEditProvide()
        {
            Ocean.Entity.MpQrScene mp = new Entity.MpQrScene();
            TryUpdateModel<MpQrScene>(mp);
            if (mp.Id.ToString().StartsWith("0000"))
            {
                string token = GetAccessToken();
                int iTemp = MpQrSceneService.GetMaxSceneId(mp.ActionName) + 1;
                try
                {
                    if (mp.ActionName == 0)
                    {
                        mp.TimeoutTicks = 1800;
                        mp.SceneId = iTemp;
                        CreateQrCodeResult result = QrCode.Create(token, 1800, iTemp);
                        //QrCode.ShowQrCode(result.ticket, new MemoryStream(), WebHelper.MapPaths("~/images/Qrimg/Qr_" + iTemp.ToString() + ".jpg"));
                        //mp.ImgUrl = GlobalConfig.GetConfig()["ResourceDomain"] + "/Qrimg/Qr_" + iTemp.ToString() + ".jpg";
                        mp.ImgUrl = QrCode.ShowQrCode(result.ticket);
                    }
                    else
                    {
                        mp.TimeoutTicks = 0;
                        mp.SceneId = iTemp;
                        CreateQrCodeResult result = QrCode.Create(token, 0, iTemp);
                        //QrCode.ShowQrCode(result.ticket, new MemoryStream(), WebHelper.MapPaths("~/images/Qrimg/Qr_" + iTemp.ToString() + ".jpg"));
                        //mp.ImgUrl = GlobalConfig.GetConfig()["ResourceDomain"] + "/Qrimg/Qr_" + iTemp.ToString() + ".jpg";
                        mp.ImgUrl = QrCode.ShowQrCode(result.ticket);

                    }
                    MpQrSceneService.Insert(mp);
                    return JsonMessage(true, "生成二维码成功");
                }
                catch (Exception e)
                {
                    Log4NetImpl.Write("MpQrCodeEditProvide失败:" + e.Message);
                    return JsonMessage(false, "生成二维码失败");
                }


            }
            else
            {

            }
            return JsonMessage(false, "生成二维码失败");
        }

        /// <summary>
        /// 获网点类型
        /// </summary>
        [HttpPost]
        [ActionName("_QrType")]
        public ActionResult QrTypeProvide()
        {
            string strType = "[";
            strType += "{\"id\":\"\",\"text\":\"--请选择--\"},";
            foreach (var item in EnumExtensions.ToListEnumItem<Ocean.Entity.Enums.Branch.QrEnum>())
            {
                strType += "{\"id\":\"" + item.EnumValue.ToString() + "\",\"text\":\"" + item.EnumDescript + "\"},";
            }
            strType = strType.Substring(0, strType.Length - 1);
            strType += "]";
            return Content(strType);
        }
    }
}