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
using Ocean.Core.Plugins.Upload;
using System.Web.Helpers;
using System.Text;
using Ocean.Framework.Configuration.global.config;


namespace Ocean.Admin.Controllers
{
    [ValidateInput(false)]
    public class MpMaterialController : AdminBaseController
    {
        private readonly IMpMaterialService MpMaterialService;
        private readonly IMpMaterialItemService MpMaterialItemService;

        public MpMaterialController(IMpMaterialService MpMaterialService, IMpMaterialItemService MpMaterialItemService)
        {
            this.MpMaterialService = MpMaterialService;
            this.MpMaterialItemService = MpMaterialItemService;
        }

        /// <summary>
        /// 添加素材列表
        /// </summary>
        [HttpGet]
        public ActionResult Add(int? isMul,Guid? id)
        {
            if (!base.HasPermission("mpmaterial", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            MpMaterial material = new MpMaterial();
            if (id == null || id == Guid.Empty)
            {
                material.IsDynamic = false;
                material.ApiUrl = "";
                material.CreateDate = DateTime.Now;
                material.CreateUser = Guid.NewGuid();
                material.DynamicType = "";
                material.MedeaID = 0;
                material.MpID = MpCenterCache.Id;
                material.PostData = "";
                material.TypeID = 10;
                material.TypeName = "news";
                material.UpateDate = DateTime.Now;
                material.IsMul = isMul.HasValue?isMul.Value:0;
                material.UpdateUser = Guid.NewGuid();

                List<MpMaterialItem> items = new List<MpMaterialItem> { 
                    new MpMaterialItem{
                        CreateDate=DateTime.Now,
                        Description="",
                        Title="标题",
                        Url="",
                        Summary="",
                        MusicUrl="",
                        PicUrl="",
                        HQMusicUrl="",
                        MpMaterial=material,
                        ReplyContent=""
                    }
                };
                if (material.IsMul == 1)
                {
                    items.Add(new MpMaterialItem
                    {
                        CreateDate = DateTime.Now,
                        Description = "",
                        Title = "标题",
                        Url = "",
                        Summary = "",
                        MusicUrl = "",
                        PicUrl = "",
                        HQMusicUrl = "",
                        MpMaterial = material,
                        ReplyContent = ""
                    });
                }
                material.MpMaterialItems = items;

            }
            else
            {
                material = MpMaterialService.GetById(id);
            }

            string strLbz = JsonConvert.SerializeObject(material);
            ViewBag.strLbz = strLbz;
            return View();
        }

        /// <summary>
        /// 素材库列表页面
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            if (!base.HasPermission("mpmaterial", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            PagedList<Ocean.Entity.MpMaterial> list = MpMaterialService.GetPageList(x => x.TypeName == "news", "CreateDate", PageIndex, PageSize, false);
            ViewBag.strList = JsonConvert.SerializeObject(list);
            ViewBag.totalCount = list.TotalItemCount;
            return View(list);
        }

        [HttpPost]
        public ActionResult SaveMaterial(string data)
        {
            MpMaterialService.SaveMaterial(data);
            return JsonMessage(true, "添加成功");
        }

        [HttpPost]
        public ActionResult DeleteMaterial(Guid id)
        {
            if (MpMaterialService.DelMaterial(id))
            {
                return JsonMessage(true, "");
            }
            else
            {
                return JsonMessage(false, "删除失败");
            }
        }

        [HttpPost]
        public JsonResult UploadPhoto()
        {
            HttpPostedFileBase uploadFiles0 = Request.Files["picUpload"];

            if (uploadFiles0 != null && uploadFiles0.ContentLength > 0 && !string.IsNullOrEmpty(uploadFiles0.FileName))
            {
                UpFileEntity fileEntity0 = new UpFileEntity() { Size = 2048 };
                fileEntity0.Dir = "/MpImages/";
                fileEntity0.AllowType = ".jpg,.jpeg,.gif,.png,.bmp";

                AttachmentInfo attch0 = UploadProvider.GetInstance("Common").UploadFile(uploadFiles0, fileEntity0);
                if (string.IsNullOrEmpty(attch0.Error))
                {
                    return JsonMessage(true, attch0.FileName);
                }
                else
                {
                    return JsonMessage(false, attch0.Error);
                }
            }
            return JsonMessage(false, "上传不成功...");
        }

        public ActionResult Test()
        {
            return View();
        }

        /// <summary>
        /// 添加素材列表
        /// </summary>
        [HttpPost]
        public ActionResult GetMaterialData()
        {
            IList<Ocean.Entity.MpMaterial> list = MpMaterialService.GetPageList(x => x.TypeName == "news", "CreateDate", PageIndex, 50, false);
            string strLbz = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new ExcludePropertiesContractResolver(new List<string> { "UpateDate", "Description", "Summary" })
            });
            return JsonMessage(true, strLbz);
        }

                /// <summary>
        /// 富文本图片上传
        /// </summary>
        [HttpPost]
        public JsonResult Uploadxh()
        {
            try
            {
                HttpPostedFileBase uploadImage = Request.Files[0];
                string imgFileName = String.Empty;  //返回的图片路径
                string imgErr = String.Empty;   //返回的上传错误信息
                if (uploadImage != null && uploadImage.ContentLength > 0)
                {
                    IUpload upload = UploadProvider.GetInstance();
                    UpFileEntity fileEntity = new UpFileEntity() { Size = 4096, AllowType = ".jpg,.jpeg,.bmp,.gif,.png" };
                    fileEntity.Dir = "/MpImages/";
                    fileEntity.AllowType = ".jpg,.jpeg,.gif,.png,.bmp";

                    AttachmentInfo attch = upload.UploadFile(uploadImage, fileEntity);

                    if (attch != null && string.IsNullOrEmpty(attch.Error))
                    {
                        imgFileName = GlobalConfig.GetConfig()["ResourceDomain"]+attch.FileName;
                    }
                    else
                    {
                        imgErr = attch.Error;
                    }
                }
                else
                {
                    imgErr = "请选择上传文件";
                }
                var res = new JsonResult();
                var person = new { err = imgErr, msg = imgFileName };
                res.Data = person;//返回单个对象
                res.ContentType = "application/javascript";
                res.ContentEncoding = Encoding.UTF8;
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception("上传文件测试", ex);
            }
        }

    }
}