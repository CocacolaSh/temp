using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Entity.Enums;
using Ocean.Page;
using Ocean.Framework.Caching.Cache;
using Ocean.Services;
using Ocean.Entity;
using Ocean.Entity.DTO;
using Ocean.Core;
using Ocean.Core.Utility;
using Ocean.Core.Logging;
using Ocean.Core.Plugins.Upload;

namespace Ocean.Web.Controllers
{
    public class PosController : WeixinOAuthController
    {
        private readonly IPosAuthService _posAuthService;
        private readonly IPosApplyService _posApplyService;
        private readonly IPosService _posService;
        private readonly IMobileCodeService _mobileCodeService;
        public PosController(IPosAuthService posAuthService,IPosApplyService posApplyService,
            IMobileCodeService mobileCodeService, IPosService posService)
        {
            _posAuthService = posAuthService;
            _posApplyService = posApplyService;
            _mobileCodeService = mobileCodeService;
            _posService = posService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PosApply(string isEdit)
        {
            //当前登录用户
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            PosAuth posAuth = _posAuthService.GetUnique(pos => pos.MpUserId == mpUser.Id);
            ViewBag.IsAuth =false;
            if (posAuth != null)
            {
                ViewBag.IsAuth = posAuth.IsAuth > 0;
            }
            ViewBag.Name = mpUser.Name;

            if (mpUser.Sex == 1)
            {
                ViewBag.Sex = "先生";
            }
            else if (mpUser.Sex == 2)
            {
                ViewBag.Sex = "女士";
            }
            else
            {
                ViewBag.Sex = "";
            }

            //用户最近的一笔贷款申请[未撤销]
            PosApply posApply = _posApplyService.GetUnique(pos => pos.MpUserId == MpUserID && pos.Status == 0 && pos.ProcessStatus == 0, "CreateDate", false);
            //有未过期的贷款申请
            if (posApply != null && !(isEdit == "1" && (DateTime.Now - posApply.CreateDate).Hours <= 24))
            {
                return RedirectToAction("PosMessage", new { });
            }
            if (posApply == null)
            {
                posApply = new PosApply();
            }
            return View(posApply);
        }


        /// <summary>
        /// 验证
        /// </summary>
        private bool ValidatorPos(ref string message)
        {
            if (string.IsNullOrWhiteSpace(RQuery["VendorAddress"]))
            {
                message = "经营地址不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(RQuery["VendorPic"]))
            {
                message = "请上传门店照片";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 提交申请
        /// </summary>
        [HttpPost]
        [ActionName("_PosApply")]
        public ActionResult PosApplyProvide(string isEdit)
        {
            string message = string.Empty;
            bool isSuccess = true;
            if (!ValidatorPos(ref message))
            {
                return JsonMessage(false, message);
            }

            //用户最近的一笔申请[未撤销]
            PosApply apply2 = _posApplyService.GetUnique(pos => pos.MpUserId == MpUserID && pos.Status == 0, "CreateDate", false);
            //有未过期的贷款申请
            if (apply2 != null && !(isEdit == "1" && (DateTime.Now - apply2.CreateDate).Hours <= 24))
            {
                return RedirectToAction("PosMessage", new { });
            }

            try
            {
                _posApplyService.BeginTransaction();
                //当前登录用户
                MpUser mpUser = _mpUserService.GetById(MpUserID);
                PosAuth posAuth = _posAuthService.GetUnique(pos => pos.MpUserId == mpUser.Id);
                ViewBag.IsAuth = false;
                if (posAuth != null)
                {
                    ViewBag.IsAuth = posAuth.IsAuth > 0;
                }
                //进行手机验证
                if (!ViewBag.IsAuth)
                {
                    int useCount = _mobileCodeService.GetMobileCodeCount(this.MpUserID, 3);//当天使用次数
                    if (useCount <= 5)
                    {
                        MobileCode lastCode = _mobileCodeService.GetMobileCode(this.MpUserID, 3);//是否存在未使用的证码
                        if (lastCode != null)
                        {
                            //验证
                            MobileCode mobileCode = _mobileCodeService.GetMobileCode(RQuery["MobilePhone"], RQuery["Code"], MpUserID, 3);

                            if (mobileCode != null)
                            {
                                TimeSpan ts = DateTime.Now.Subtract(mobileCode.CreateDate).Duration();

                                if (ts.Minutes > 10)
                                {
                                    ViewBag.CodeError = "验证码已经失效，请重新获取";
                                isSuccess = false;
                                }
                                PosAuth auth = new PosAuth();
                                auth.MpUserId = MpUserID;
                                auth.Name = RQuery["Name"];
                                auth.MobilePhone = RQuery["MobilePhone"];
                                auth.IsAuth = 1;
                                _posAuthService.Insert(auth);
                            }
                            else
                            {
                                lastCode.Time += 1;
                                _mobileCodeService.Update(lastCode);
                                ViewBag.CodeError = "对不起，验证码有误，请检查！";
                                isSuccess = false;
                            }
                        }
                        else
                        {
                            ViewBag.CodeError = "对不起，您还未点击发送验证码！";
                            isSuccess = false;
                        }
                    }
                    else
                    {
                        ViewBag.CodeError = "对不起，您今天最多只能发起5次验证码！";
                        isSuccess = false;
                    }
                }
                //处理贷款业务
                if (isEdit == "1")
                {
                    if (apply2!=null)
                    {
                        TryUpdateModel<PosApply>(apply2);
                        if (isSuccess)
                        {
                            _posApplyService.Update(apply2);
                        }
                    }
                    else
                    {
                        throw new OceanException("对不起，您的参数有误，请联系客服！");
                    }

                }
                else
                {
                    apply2 = new PosApply();
                    TryUpdateModel<PosApply>(apply2);
                    apply2.MpUserId = MpUserID;
                    apply2.Status = apply2.AssignStatus = apply2.ProcessStatus = 0;
                    if (isSuccess)
                    {
                        _posApplyService.Insert(apply2);
                    }
                }
            }
            catch (Exception ex)
            {
                _posApplyService.Rollback();
                throw new OceanException(string.Format("对不起，{0}！", ex.Message));
            }
            finally
            {
                _posApplyService.Commit();
            }
            if (isSuccess)
            {
                return RedirectToAction("PosApply");
            }
            else
            {
                return View("PosApply", apply2);
            }
        }

        /// <summary>
        /// 是否能够编辑
        /// </summary>
        private bool CanEditApply(PosApply pos, ref string message)
        {
            //后台已进行相关处理,不能进行编辑操作
            if (pos.AssignStatus > 0 || pos.ProcessStatus > 0)
            {
                message = "后台已进行相关处理,不能进行编辑操作！";
                return false;
            }

            //修改信息只限申请的当天24点前
            if (DateTime.Now > DateTime.Parse(pos.CreateDate.AddDays(1).ToShortDateString()))
            {
                message = "修改信息只限申请的当天24点前！";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 能否编辑贷款
        /// </summary>
        [HttpPost]
        [ActionName("_CanEditApply")]
        public ActionResult CanEditApply(Guid id)
        {
            string message = string.Empty;
            PosApply pos = _posApplyService.GetById(id);

            if (!CanEditApply(pos, ref message))
            {
                return JsonMessage(false, message);
            }

            return JsonMessage(true, "");
        }


        /// <summary>
        /// 存量客户再次进入
        /// </summary>
        [HttpGet]
        public ActionResult PosMessage()
        {
            //当前登录用户
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            //用户最近的一笔贷款申请[未撤销]
            PosApply pos = _posApplyService.GetUnique(p => p.MpUserId == MpUserID && p.Status == 0, "CreateDate", false);
            ViewBag.PosApply = pos;
            return View(mpUser);
        }

        /// <summary>
        /// 撤销申请
        /// </summary>
        [HttpPost]
        [ActionName("_CancelApply")]
        public ActionResult CancelApply()
        {
            PosApply pos = _posApplyService.GetById(new Guid(RQuery["Id"]));

            //后台已进行相关处理,不能进行编辑操作
            if (pos.AssignStatus > 0 || pos.ProcessStatus > 0)
            {
                return JsonMessage(false, "撤销失败，本次申请已被受理");
            }

            pos.Status = 1;
            pos.RepealDate = DateTime.Now;
            _posApplyService.Update(pos);
            return JsonMessage(true, "撤销成功");
        }

        [HttpPost]
        public JsonResult UploadPhoto()
        {
            HttpPostedFileBase uploadFiles0 = Request.Files["picUpload"];

            if (uploadFiles0 != null && uploadFiles0.ContentLength > 0 && !string.IsNullOrEmpty(uploadFiles0.FileName))
            {
                UpFileEntity fileEntity0 = new UpFileEntity() { Size = 2048 };
                fileEntity0.Dir = "/VendorImages/";
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


        [HttpGet]
        public ActionResult MyPos()
        {
            //int count=_posService.GetCount("sele count(*) from Pos where MpUserIds like '%"+MpUserID.ToString()+"%'");
            //if (count > 0)
            //{
            //    return RedirectToAction("MyPosBindList");
            //}
            //else
            //{
                //当前登录用户
                MpUser mpUser = _mpUserService.GetById(MpUserID);
                PosAuth posAuth = _posAuthService.GetUnique(pos => pos.MpUserId == mpUser.Id);
                //if (posAuth == null)
                //{
                //    return RedirectToAction("PosApply");
                //}
                if (posAuth != null)
                {
                    ViewBag.Name = posAuth.Name;
                }
                else
                {
                    ViewBag.Name = mpUser.NickName;
                }
                if (mpUser.Sex == 1)
                {
                    ViewBag.Sex = "先生";
                }
                else if (mpUser.Sex == 2)
                {
                    ViewBag.Sex = "女士";
                }
                else
                {
                    ViewBag.Sex = "";
                }
                return View();
            //}
        }
        public ActionResult MyPosNo() {

            //当前登录用户
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            PosAuth posAuth = _posAuthService.GetUnique(pos => pos.MpUserId == mpUser.Id);
            //if (posAuth == null)
            //{
            //    return RedirectToAction("PosApply");
            //}
            if (posAuth != null)
            {
                ViewBag.Name = posAuth.Name;
            }
            else
            {
                ViewBag.Name = mpUser.NickName;
            }

            if (mpUser.Sex == 1)
            {
                ViewBag.Sex = "先生";
            }
            else if (mpUser.Sex == 2)
            {
                ViewBag.Sex = "女士";
            }
            else
            {
                ViewBag.Sex = "";
            }
            return View();
        }
        [HttpPost]
        public ActionResult MyPos(Pos pos)
        {
            //当前登录用户
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            PosAuth posAuth = _posAuthService.GetUnique(p => p.MpUserId == mpUser.Id);
            //if (posAuth == null)
            //{
            //    return JsonMessage(false, "对不起，您还未申请POS，请先申请！");
            //}
            if (posAuth != null)
            {
                ViewBag.Name = posAuth.Name;
            }
            else
            {
                ViewBag.Name = mpUser.NickName;
            }

            if (mpUser.Sex == 1)
            {
                ViewBag.Sex = "先生";
            }
            else if (mpUser.Sex == 2)
            {
                ViewBag.Sex = "女士";
            }
            else
            {
                ViewBag.Sex = "";
            }

            int useCount = _mobileCodeService.GetMobileCodeCount(this.MpUserID, 4);//当天使用次数
            if (useCount <= 5)
            {
                MobileCode lastCode = _mobileCodeService.GetMobileCode(this.MpUserID, 4);//是否存在未使用的证码
                if (lastCode != null)
                {
                    //验证
                    MobileCode mobileCode = _mobileCodeService.GetMobileCode(RQuery["MobilePhone"], RQuery["Code"], MpUserID, 4);

                    if (mobileCode != null)
                    {
                        TimeSpan ts = DateTime.Now.Subtract(mobileCode.CreateDate).Duration();
                        mobileCode.Status = 1;
                        if (ts.Minutes > 10)
                        {
                            return JsonMessage(false, "验证码已经失效，请重新获取");
                        }
                        _mobileCodeService.Update(mobileCode);
                        if (_posService.PosBind(pos) == 1)
                        {
                            if (!string.IsNullOrEmpty(pos.MobilePhones) && pos.MobilePhones.Contains(RQuery["MobilePhone"]))
                            {
                                return JsonMessage(false, "您已查询过当前POS信息，您可以点击我的POS查看详细！");
                            }
                            else
                            {
                                _posService.ExcuteSql("update Pos set MobilePhones=isnull(MobilePhones,'')+'," + RQuery["MobilePhone"] + "',MpUserIds=isnull(MpUserIds,'')+'," + MpUserID.ToString() + "' where Id='" + pos.Id.ToString() + "'");
                                return JsonMessage(true, pos.Id.ToString());
                            }
                        }
                        else
                        {
                            return JsonMessage(false, "-1");
                        }
                    }
                    else
                    {
                        lastCode.Time += 1;
                        _mobileCodeService.Update(lastCode);
                        return JsonMessage(false, "对不起，验证码有误，请检查！");
                    }
                }
                else
                {
                    return JsonMessage(false, "对不起，您还未点击发送验证码！");
                }
            }
            else
            {
                return JsonMessage(false, "对不起，您今天最多只能发起5次验证码");
            }
        }
        [HttpPost]
        public ActionResult MyPosUnbind(Guid id)
        {
            //当前登录用户
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            PosAuth posAuth = _posAuthService.GetUnique(p => p.MpUserId == mpUser.Id);
                
                //if (posAuth == null)
                //{
                //    return JsonMessage(false, "对不起，您还未申请POS，请先申请！");
                //}
                if (posAuth != null)
                {
                    ViewBag.Name = posAuth.Name;
                }
                else
                {
                    ViewBag.Name = mpUser.NickName;
                }

            if (mpUser.Sex == 1)
            {
                ViewBag.Sex = "先生";
            }
            else if (mpUser.Sex == 2)
            {
                ViewBag.Sex = "女士";
            }
            else
            {
                ViewBag.Sex = "";
            }
            if (id != Guid.Empty)
            {
                _posService.ExcuteSql("update Pos set MpUserIds=replace(MpUserIds,'," + MpUserID.ToString() + "',''),MobilePhones=replace(MobilePhones,'," + posAuth.MobilePhone + "','') where id='" + id.ToString() + "'");
            }
            else
            {
                _posService.ExcuteSql("update Pos set MpUserIds=replace(MpUserIds,'," + MpUserID.ToString() + "',''),MobilePhones=replace(MobilePhones,'," + posAuth.MobilePhone + "','') where  MpUserIds like '%" + MpUserID.ToString() + "%'");
            }
            return JsonMessage(true);
        }
        [HttpGet]
        public ActionResult MyPosBind(Guid id)
        {
            //当前登录用户
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            PosAuth posAuth = _posAuthService.GetUnique(p => p.MpUserId == mpUser.Id);
            //if (posAuth == null)
            //{
            //    return RedirectToAction("PosApply");
            //}
            if (posAuth != null)
            {
                ViewBag.Name = posAuth.Name;
            }
            else
            {
                ViewBag.Name = mpUser.NickName;
            }

            if (mpUser.Sex == 1)
            {
                ViewBag.Sex = "先生";
            }
            else if (mpUser.Sex == 2)
            {
                ViewBag.Sex = "女士";
            }
            else
            {
                ViewBag.Sex = "";
            }

            string mpuserId = MpUserID.ToString();
            Pos pos = _posService.GetUnique(p => p.Id == id && p.MpUserIds.Contains(mpuserId));
            if (pos == null)
            {
                throw new OceanException("对不起,参数有误，请检查！");
            }
            return View(pos);
        }
        [HttpGet]
        public ActionResult MyPosBindList()
        {
            string mpuserid = MpUserID.ToString();
            IList<Pos> pos = _posService.GetALL(p => p.MpUserIds.Contains(mpuserid));
            if (pos == null || pos.Count == 0)
            {
                return RedirectToAction("MyPos");
            }
            //当前登录用户
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            PosAuth posAuth = _posAuthService.GetUnique(p => p.MpUserId == mpUser.Id);
            //if (posAuth == null)
            //{
            //    return RedirectToAction("PosApply");
            //}
            if (posAuth != null)
            {
                ViewBag.Name = posAuth.Name;
            }
            else
            {
                ViewBag.Name = mpUser.NickName;
            }

            if (mpUser.Sex == 1)
            {
                ViewBag.Sex = "先生";
            }
            else if (mpUser.Sex == 2)
            {
                ViewBag.Sex = "女士";
            }
            else
            {
                ViewBag.Sex = "";
            }
            return View(pos);
        }
    }
}
