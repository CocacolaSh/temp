using System;
using System.Web.Mvc;
using Ocean.Framework.Caching.Cache;
using Ocean.Services;
using Ocean.Entity;
using Ocean.Core.ExceptionHandling;
using Ocean.Entity.Enums.TypeIdentifying;
using Ocean.Page;
using Ocean.Core.Utility;
using Ocean.Core.Logging;

namespace Ocean.Web.Controllers
{
    public class BankLoanController : WeixinOAuthController
    {
        private readonly ILoanService _loanService;
        private readonly IMpUserService _mpUserService;
        private readonly IMobileCodeService _mobileCodeService;

        public BankLoanController(ILoanService loanService, IMpUserService mpUserService, IMobileCodeService mobileCodeService)
        {
            this._loanService = loanService;
            this._mpUserService = mpUserService;
            this._mobileCodeService = mobileCodeService;
        }

        /// <summary>
        /// 贷款申请页面
        /// </summary>
        [HttpGet]
        public ActionResult LoanApply()
        {
            //当前登录用户
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            ViewBag.IsAuth = mpUser.IsAuth > 0;
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
            Loan loan = _loanService.GetNewestLoanApplyByMpUserId(MpUserID);
            //有未过期的贷款申请
            //有未过期的贷款申请
            if (loan != null && DateTime.Now < loan.CreateDate.AddMonths(1) && loan.LoanCategoryId == 4)
            {
                return RedirectToAction("GongzhiBaoMessage", new { });
            }
            else if (loan != null && DateTime.Now < loan.CreateDate.AddMonths(1))
            {
                return RedirectToAction("LoanMessage", new { });
            }
            
            //贷款种类
            ViewBag.ListLoanCategory = EnumDataCache.Instance.GetList(TypeIdentifyingEnum.LoanCategory);
            //贷款期限
            ViewBag.ListLoanDeadline = EnumDataCache.Instance.GetList(TypeIdentifyingEnum.LoanDeadline);
            //还款方式
            ViewBag.ListRepaymentMode = EnumDataCache.Instance.GetList(TypeIdentifyingEnum.RepaymentMode);
            return View();
        }

        /// <summary>
        /// 贷款申请页面
        /// </summary>
        [HttpGet]
        public ActionResult GongzhiBaoApply()
        {
            //当前登录用户
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            ViewBag.IsAuth = mpUser.IsAuth > 0;
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
            try
            {
                //用户最近的一笔贷款申请[未撤销]
                Loan loan = _loanService.GetNewestLoanApplyByMpUserId(MpUserID);
                //有未过期的贷款申请
                if (loan != null && DateTime.Now < loan.CreateDate.AddMonths(1) && loan.LoanCategoryId == 4)
                {
                    return RedirectToAction("GongzhiBaoMessage", new { });
                }
                else if (loan != null && DateTime.Now < loan.CreateDate.AddMonths(1))
                {
                    return RedirectToAction("LoanMessage", new { });
                }

            }
            catch (System.Exception ex)
            {
                Log4NetImpl.Write(ex.Message);
            }
            return View();
        }
        /// <summary>
        /// 提交宫之宝贷款申请
        /// </summary>
        [HttpPost]
        [ActionName("_GongzhiBaoApply")]
        public ActionResult GongzhiBaoApplyProvide(Loan loan)
        {
            string message = string.Empty;

            //用户最近的一笔贷款申请[未撤销]
            Loan loan2 = _loanService.GetNewestLoanApplyByMpUserId(MpUserID);
            //有未过期的贷款申请
            if (loan2 != null && DateTime.Now < loan2.CreateDate.AddMonths(1))
            {
                return JsonMessage(false, "您当前已经申请过贷款");
            }

            try
            {
                _loanService.BeginTransaction();
                //当前登录用户
                MpUser mpUser = _mpUserService.GetById(MpUserID);
                //进行手机验证
                if (mpUser.IsAuth == 0)
                {
                    int useCount = _mobileCodeService.GetMobileCodeCount(this.MpUserID, 2);//当天使用次数
                    if (useCount <= 5)
                    {
                        MobileCode lastCode = _mobileCodeService.GetMobileCode(this.MpUserID, 2);//是否存在未使用的证码
                        if (lastCode != null)
                        {
                            //验证
                            MobileCode mobileCode = _mobileCodeService.GetMobileCode(loan.Phone, RQuery["Code"], MpUserID, 2);

                            if (mobileCode != null)
                            {
                                TimeSpan ts = DateTime.Now.Subtract(mobileCode.CreateDate).Duration();

                                if (ts.Minutes > 10)
                                {
                                    return JsonMessage(false, "验证码已经失效，请重新获取");
                                }

                                mpUser.Name = loan.LoanName;
                                mpUser.MobilePhone = loan.Phone;
                                mpUser.IsAuth = 1;
                                _mpUserService.Update(mpUser);
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
                //处理贷款业务
                loan.MpUserId = MpUserID;
                loan.Status = loan.AssignStatus = loan.ProcessStatus = 0;
                _loanService.Insert(loan);
            }
            catch (Exception ex)
            {
                _loanService.Rollback();
                throw new OceanException(string.Format("对不起，{0}！", ex.Message));
            }
            finally
            {
                _loanService.Commit();
            }

            return JsonMessage(true, "提交成功,等待管理员审核");
        }
        /// <summary>
        /// 贷款编辑页面
        /// </summary>
        [HttpGet]
        public ActionResult GongzhiBaoEdit(Guid id)
        {
            Loan loan = _loanService.GetById(id);
            ViewBag.LoanName = loan.LoanName;

            //后台已进行相关处理,不能进行编辑操作
            if (loan.AssignStatus > 0 || loan.ProcessStatus > 0)
            {
                throw new OceanException("后台已进行相关处理,不能进行编辑操作！");
            }

            //修改信息只限申请的当天24点前
            if (DateTime.Now > DateTime.Parse(loan.CreateDate.AddDays(1).ToShortDateString()))
            {
                throw new OceanException("修改信息只限申请的当天24点前！");
            }

            return View(loan);
        }

        /// <summary>
        /// 编辑贷款申请
        /// </summary>
        [HttpPost]
        [ActionName("_GongzhiBaoEdit")]
        public ActionResult GongzhiBaoEditProvide()
        {
            Loan loan = _loanService.GetById(new Guid(RQuery["Id"]));

            string message = string.Empty;


            if (!CanEditLoan(loan, ref message))
            {
                return JsonMessage(false, message);
            }

            if (TryUpdateModel<Loan>(loan))
            {
                _loanService.Update(loan);
                return JsonMessage(true, "修改信息成功");
            }
            else
            {
                return JsonMessage(false, "修改信息失败");
            }
        }
        /// <summary>
        /// 验证
        /// </summary>
        private bool ValidatorLoan(ref string message)
        {
            if (string.IsNullOrWhiteSpace(RQuery["ApplyMoney"]))
            {
                message = "申请金额不能为空";
                return false;
            }

            if (!Validator.IsInt(RQuery["ApplyMoney"]))
            {
                message = "申请金额请填写整数，不能带小数点";
                return false;
            }

            if (int.Parse(RQuery["ApplyMoney"]) <= 0)
            {
                message = "请输入大于0的申请金额";
                return false;
            }

            if (string.IsNullOrWhiteSpace(RQuery["LoanCategoryId"]))
            {
                message = "请选择贷款种类";
                return false;
            }

            if (string.IsNullOrWhiteSpace(RQuery["DeadlineId"]))
            {
                message = "请选择贷款期限";
                return false;
            }

            if (string.IsNullOrWhiteSpace(RQuery["RepaymentModeId"]))
            {
                message = "请选择还款方式";
                return false;
            }

            if (string.IsNullOrWhiteSpace(RQuery["GuaranteeWayId"]))
            {
                message = "请选择担保方式";
                return false;
            }

            if (string.IsNullOrWhiteSpace(RQuery["AssetSituation"]))
            {
                message = "请选择资产情况";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 提交贷款申请
        /// </summary>
        [HttpPost]
        [ActionName("_LoanApply")]
        public ActionResult LoanApplyProvide(Loan loan)
        {
            string message = string.Empty;

            if (!ValidatorLoan(ref message))
            {
                return JsonMessage(false, message);
            }

            //用户最近的一笔贷款申请[未撤销]
            Loan loan2 = _loanService.GetNewestLoanApplyByMpUserId(MpUserID);
            //有未过期的贷款申请
            if (loan2 != null && DateTime.Now < loan2.CreateDate.AddMonths(1))
            {
                return JsonMessage(false, "您当前已经申请过贷款");
            }

            try
            {
                _loanService.BeginTransaction();
                //当前登录用户
                MpUser mpUser = _mpUserService.GetById(MpUserID);
                //进行手机验证
                if (mpUser.IsAuth == 0)
                {
                    int useCount = _mobileCodeService.GetMobileCodeCount(this.MpUserID, 2);//当天使用次数
                    if (useCount <= 5)
                    {
                        MobileCode lastCode = _mobileCodeService.GetMobileCode(this.MpUserID, 2);//是否存在未使用的证码
                        if (lastCode != null)
                        {
                            //验证
                            MobileCode mobileCode = _mobileCodeService.GetMobileCode(loan.Phone, RQuery["Code"], MpUserID, 2);

                            if (mobileCode != null)
                            {
                                TimeSpan ts = DateTime.Now.Subtract(mobileCode.CreateDate).Duration();

                                if (ts.Minutes > 10)
                                {
                                    return JsonMessage(false, "验证码已经失效，请重新获取");
                                }

                                mpUser.Name = loan.LoanName;
                                mpUser.MobilePhone = loan.Phone;
                                mpUser.IsAuth = 1;
                                _mpUserService.Update(mpUser);
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
                //处理贷款业务
                loan.MpUserId = MpUserID;
                loan.Status = loan.AssignStatus = loan.ProcessStatus = 0;
                _loanService.Insert(loan);
            }
            catch(Exception ex)
            {
                _loanService.Rollback();
                throw new OceanException(string.Format("对不起，{0}！", ex.Message));
            }
            finally
            {
                _loanService.Commit();
            }

            return JsonMessage(true, "提交成功,等待管理员审核");
        }

        /// <summary>
        /// 是否能够编辑
        /// </summary>
        private bool CanEditLoan(Loan loan, ref string message)
        {
            //后台已进行相关处理,不能进行编辑操作
            if (loan.AssignStatus > 0 || loan.ProcessStatus > 0)
            {
                message = "后台已进行相关处理,不能进行编辑操作！";
                return false;
            }

            //修改信息只限申请的当天24点前
            if (DateTime.Now > DateTime.Parse(loan.CreateDate.AddDays(1).ToShortDateString()))
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
        [ActionName("_CanEditLoan")]
        public ActionResult CanEditLoan(Guid id)
        {
            string message = string.Empty;
            Loan loan = _loanService.GetById(id);

            if (!CanEditLoan(loan, ref message))
            {
                return JsonMessage(false, message);
            }

            return JsonMessage(true, "");
        }

        /// <summary>
        /// 贷款编辑页面
        /// </summary>
        [HttpGet]
        public ActionResult LoanEdit(Guid id)
        {
            Loan loan = _loanService.GetById(id);
            ViewBag.LoanName = loan.LoanName;

            //后台已进行相关处理,不能进行编辑操作
            if (loan.AssignStatus > 0 || loan.ProcessStatus > 0)
            {
                throw new OceanException("后台已进行相关处理,不能进行编辑操作！");
            }

            //修改信息只限申请的当天24点前
            if (DateTime.Now > DateTime.Parse(loan.CreateDate.AddDays(1).ToShortDateString()))
            {
                throw new OceanException("修改信息只限申请的当天24点前！");
            }

            //贷款种类
            ViewBag.ListLoanCategory = EnumDataCache.Instance.GetList(TypeIdentifyingEnum.LoanCategory);
            //贷款期限
            ViewBag.ListLoanDeadline = EnumDataCache.Instance.GetList(TypeIdentifyingEnum.LoanDeadline);
            //还款方式
            ViewBag.ListRepaymentMode = EnumDataCache.Instance.GetList(TypeIdentifyingEnum.RepaymentMode);
            return View(loan);
        }

        /// <summary>
        /// 编辑贷款申请
        /// </summary>
        [HttpPost]
        [ActionName("_LoanEdit")]
        public ActionResult LoanEditProvide()
        {
            Loan loan = _loanService.GetById(new Guid(RQuery["Id"]));

            string message = string.Empty;

            if (!ValidatorLoan(ref message))
            {
                return JsonMessage(false, message);
            }

            if (!CanEditLoan(loan, ref message))
            {
                return JsonMessage(false, message);
            }

            if (TryUpdateModel<Loan>(loan))
            {
                _loanService.Update(loan);
                return JsonMessage(true, "修改信息成功");
            }
            else
            {
                return JsonMessage(false, "修改信息失败");
            }
        }
        /// <summary>
        /// 存量客户再次进入
        /// </summary>
        [HttpGet]
        public ActionResult GongzhiBaoMessage()
        {
            //当前登录用户
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            //用户最近的一笔贷款申请[未撤销]
            Loan loan = _loanService.GetNewestLoanApplyByMpUserId(MpUserID);
            ViewBag.Loan = loan;
            return View(mpUser);
        }

        /// <summary>
        /// 存量客户再次进入
        /// </summary>
        [HttpGet]
        public ActionResult LoanMessage()
        {
            //当前登录用户
            MpUser mpUser = _mpUserService.GetById(MpUserID);
            //用户最近的一笔贷款申请[未撤销]
            Loan loan = _loanService.GetNewestLoanApplyByMpUserId(MpUserID);
            ViewBag.Loan = loan;
            return View(mpUser);
        }

        /// <summary>
        /// 撤销申请
        /// </summary>
        [HttpPost]
        [ActionName("_CancelApply")]
        public ActionResult CancelApply()
        {
            Loan loan = _loanService.GetById(new Guid(RQuery["Id"]));

            //后台已进行相关处理,不能进行编辑操作
            if (loan.AssignStatus > 0 || loan.ProcessStatus > 0)
            {
                return JsonMessage(false, "撤销失败，本次申请已被受理");
            }

            loan.Status = 1;
            loan.RepealDate = DateTime.Now;
            _loanService.Update(loan);
            return JsonMessage(true, "撤销成功");
        }
    }
}