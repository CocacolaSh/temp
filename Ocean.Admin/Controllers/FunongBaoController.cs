using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Page;
using Ocean.Services;
using Ocean.Entity;
using Ocean.Core;
using Ocean.Entity.DTO;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using Ocean.Core.Logging;
using System.Data;
using Ocean.Core.Npoi;
using Newtonsoft.Json;
using Ocean.Core.Utility;
using System.Threading;
using Ocean.Entity.Enums.AdminLoggerModule;

namespace Ocean.Admin.Controllers
{
    public class FunongBaoController : AdminBaseController
    {
        private readonly IFunongbaoService _funongbaoService;
        private readonly IFunongbaoApplyService _funongbaoApplyService;
        private readonly IMpUserService _mpUserService;
        public FunongBaoController(IFunongbaoService funongbaoService, IFunongbaoApplyService funongbaoApplyService, IMpUserService mpUserService)
        {
            this._funongbaoService = funongbaoService;
            this._funongbaoApplyService = funongbaoApplyService;
            this._mpUserService = mpUserService;
        }

        /// <summary>
        /// 初始化福农宝列表页面
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            if (!base.HasPermission("funongbaodetail", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            IList<SelectListItem> importQuarterList = new List<SelectListItem>();
            int startQuarter = 1;
            int endQuarter = 4;
            DateRule dateR=DateRuleList.DateRules.Where(d=>d.Months.Where(m=>m==DateTime.Now.Month).Count()>0).First();
            int x = 0;
            for (int i =  (DateTime.Now.Year - DateRule.StartYear); i >=0; i--)
            {
                
                if ((i+DateRule.StartYear) ==DateRule.StartYear)
                {
                    startQuarter = DateRule.StartQuarter;
                }
                else
                {
                    startQuarter = 1;
                }
                if (i == (DateTime.Now.Year - DateRule.StartYear))
                {
                    endQuarter = dateR.Quarterly;
                }
                else
                {
                    endQuarter = 4;
                }
                for (int j = endQuarter; j >= startQuarter; j--)
                {
                    if (x == 0)
                    {
                        importQuarterList.Add(new SelectListItem() { Text = (i + DateRule.StartYear).ToString() + "年第" + StringHelper.GetQuarter(j) + "季度", Value = StringHelper.GetQuarter(DateRule.StartYear + i, j), Selected = true });
                        x++;
                    }
                    else
                    {
                        importQuarterList.Add(new SelectListItem() { Text = (i + DateRule.StartYear).ToString() + "年第" + StringHelper.GetQuarter(j) + "季度", Value = StringHelper.GetQuarter(DateRule.StartYear + i, j) });
                    }
                }
            }
            ViewBag.ImportQuarterList = importQuarterList;
            return View();
        }
        /// <summary>
        /// 初始化福农宝列表页面
        /// </summary>
        [HttpGet]
        public ActionResult Funongbao()
        {
            if (!base.HasPermission("funongbaodetail", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return View();
        }
        /// <summary>
        /// 获取福农宝列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_FunongBaoAllList")]
        public ActionResult FunongBaoListProvide(int isAll)
        {
            DateRule dateR = DateRuleList.DateRules.Where(d => d.Months.Where(m => m == DateTime.Now.Month).Count() > 0).First();
            Funongbao funongBaoDTO = new Funongbao() { IsAuth = -1, IsSignAgreement = -1, ImportQuarter = StringHelper.GetQuarter(DateTime.Now.Year, dateR.Quarterly) };
            TryUpdateModel<Funongbao>(funongBaoDTO);
            OceanDynamicList<object> list = list = _funongbaoService.GetPageDynamicList(PageIndex, PageSize, funongBaoDTO, 1);
            if (list != null)
            {
                return Content(list.ToJson(), "text/javascript");
            }
            else
            {
                return Content("{\"rows\":[],\"total\":0}", "text/javascript");
            }
        }
        /// <summary>
        /// 获取福农宝列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_FunongBaoList")]
        public ActionResult FunongBaoListProvide()
        {
            if (!base.HasPermission("funongbaodetail", PermissionOperate.manager))
            {
                return null;
            }

            DateRule dateR=DateRuleList.DateRules.Where(d=>d.Months.Where(m=>m==DateTime.Now.Month).Count()>0).First();
            Funongbao funongBaoDTO = new Funongbao() { IsAuth = -1, IsSignAgreement = -1, ImportQuarter = StringHelper.GetQuarter(DateTime.Now.Year, dateR.Quarterly) };
            TryUpdateModel<Funongbao>(funongBaoDTO);
            OceanDynamicList<object> list =_funongbaoService.GetPageDynamicList(PageIndex, PageSize, funongBaoDTO);
            //PagedList<Funongbao> list = _funongbaoService.GetPageList(PageIndex, PageSize, funongBaoDTO);
            //if (list != null && list.Count > 0)
            //{
            //    IList<FunongbaoApply> applyList = _funongbaoApplyService.GetByFunongbaoIds(list.Select(f => f.Id).ToArray());
            //    foreach (FunongbaoApply apply in applyList)
            //    {
            //        Funongbao fnb = list.Where(f => f.Id == apply.FunongbaoId).FirstOrDefault();
            //        if (fnb != null)
            //        {
            //            fnb.LimitProgramme = apply.LimitProgramme.Replace(";", "<br />");
            //        }
            //    }
            //}
            if (list != null)
            {
                return Content(list.ToJson(), "text/javascript");
            }
            else {
                return Content("{\"rows\":[],\"total\":0}", "text/javascript");
            }
            //return JsonList<Funongbao>(list, list.TotalItemCount);
        }

        /// <summary>
        /// 初始化福农宝调额申请列表页面
        /// </summary>
        [HttpGet]
        public ActionResult Apply()
        {
            if (!base.HasPermission("funongbaoapply", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            ViewBag.FunongbaoNO = RQuery["fnbno"];
            return View();
        }

        /// <summary>
        /// 获取福农宝调额申请列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_FunongBaoApplyList")]
        public ActionResult FunongBaoApplyListProvide()
        {
            if (!base.HasPermission("funongbaoapply", PermissionOperate.manager))
            {
                return null;
            }
            
            FunongbaoApply funongBaoApplyDTO = new FunongbaoApply();
            funongBaoApplyDTO.ProcessStatus = -1;
            funongBaoApplyDTO.RFunongbao = new Funongbao();
            TryUpdateModel<FunongbaoApply>(funongBaoApplyDTO);
            PagedList<FunongbaoApply> list = null;
            if (WebHelper.GetGuid("mpuserid",Guid.Empty) != Guid.Empty)
            {
                list = _funongbaoApplyService.GetPageList(PageIndex, PageSize, WebHelper.GetGuid("mpuserid", Guid.Empty));
            }
            else
            {
                list=_funongbaoApplyService.GetPageList(PageIndex, PageSize, funongBaoApplyDTO);
            }
            return Content(JsonConvert.SerializeObject(new { rows = list, total = list.TotalItemCount }), "text/javascript");
            //return JsonList<Funongbao>(list, list.TotalItemCount);
        }

        [ActionName("TaskUser")]
        public ActionResult TaskUser()
        {
            return View();
        }

        [ActionName("_TaskUserList")]
        public ActionResult TaskUserList()
        {

            PagedList<MpUserDTO> list = _mpUserService.GetForOutTaskUserList(PageIndex, PageSize);
            return JsonList<MpUserDTO>(list, list.TotalItemCount);
        }



        [ActionName("AuthUser")]
        public ActionResult AuthUser()
        {
            return View();
        }

        [ActionName("_AuthUserList")]
        public ActionResult AuthUserList()
        {

            PagedList<MpUserDTO> list = _mpUserService.GetForOutAuthUserList(PageIndex, PageSize);
            return JsonList<MpUserDTO>(list, list.TotalItemCount);
        }


        /// <summary>
        /// 初始化福农宝明细页面
        /// </summary>
        [HttpGet]
        public ActionResult FunongBaoView(Guid id)
        {
            Funongbao funongbao = _funongbaoService.GetById(id);
            return View(funongbao);
        }

        #region 测试
        public ActionResult CancelAuth(Guid id)
        {
            _funongbaoService.CancelIdentAuth(id);
            return JsonMessage(true, "操作成功，微信号已可以重新绑定其他用户！");
        }
        public ActionResult CancelApply(Guid id)
        {
            _funongbaoApplyService.CancelApply(id);
            return JsonMessage(true, "操作成功，已可以重新发起申请！");
        }
        #endregion

        #region  申请受理
        /// <summary>
        /// 受理福农宝业务
        /// </summary>
        [HttpGet]
        [ActionName("_AcceptEdit")]
        public ActionResult AcceptEditProvide(Guid id)
        {
            FunongbaoApply funongbao = _funongbaoApplyService.GetById(id);
            ViewBag.Success = true;
            if (funongbao != null)
            {
                if (funongbao.ProcessStatus != 0)
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "该申请已经处理过！";
                }
            }
            else
            {
                throw new OceanException("对不起，参数有误！");
            }
            return View(funongbao);
        }
        [HttpPost]
        [ActionName("_AcceptLimitProgrammes")]
        public ActionResult AcceptLimitProgrammesProvide(Guid id)
        {
            FunongbaoApply apply = _funongbaoApplyService.GetById(id);
            if (apply != null)
            {
                IList<LimitProgrammeDTO> limitList = _funongbaoApplyService.GetLimitProgramme(apply.LimitProgramme);
                if (limitList != null && limitList.Count > 0)
                {
                    return JsonList(limitList);
                }
                else
                {
                    return JsonMessage(true, "没有可以调整的方案");
                }
            }
            else
            {
                return JsonMessage(true, "不存在该调额信息");
            }
            
        }

        [HttpPost]
        [ActionName("_AcceptStatus")]
        public ActionResult AcceptStatusProvide(Guid id, decimal applyLimit)
        {
            FunongbaoApply apply = _funongbaoApplyService.GetById(id);
            if (apply != null)
            {
                Funongbao CurrentFunongbao = _funongbaoService.GetById(apply.FunongbaoId);
                IList<Funongbao> funongbaoGroup = _funongbaoService.GetGroupByNo(CurrentFunongbao.GroupNO);
                Guid[] funongbaoIds = funongbaoGroup.Select(f => f.Id).ToArray();
                IList<FunongbaoApply> funongbaoApplyGroup = _funongbaoApplyService.GetByFunongbaoIds(funongbaoIds);
                decimal GropuLimitCount = funongbaoGroup.Sum(f => f.CurrentLimit);
                if ((apply.ApplyStatus == 0 || apply.ApplyStatus == -1) && apply.ApplyDate > new DateTime(1900, 1, 1))
                {
                    if (applyLimit >= GropuLimitCount)
                    {
                        if (applyLimit > 30 && applyLimit <= 60)
                        {
                            if (funongbaoGroup.Count < 2)
                            {
                                ViewBag.Success = false;
                                return JsonMessage(false, "提示开卡");
                            }
                        }
                        if (applyLimit > 60)
                        {
                            if (funongbaoGroup.Count < 3)
                            {
                                ViewBag.Success = false;
                                return JsonMessage(false, "提示开卡");
                            }
                        }
                    }
                    else
                    {
                        if (funongbaoApplyGroup.Count < 2)
                        {
                            ViewBag.Success = false;
                            return JsonMessage(false, "提示开卡");
                        }
                        else
                        {
                            decimal leaveCount = funongbaoApplyGroup.Where(f => f.FunongbaoId != CurrentFunongbao.Id).Sum(f => f.PreLimit);

                            if (((GropuLimitCount - applyLimit)-CurrentFunongbao.CurrentLimit) > leaveCount)
                            {
                                ViewBag.Success = false;
                                return JsonMessage(false, "提示开卡");
                            }
                        }
                    }
                    ViewBag.Success = false;
                    ViewBag.Message = "等待家庭组成员完成后才能受理！";
                    return JsonMessage(false, "等待组成员");

                }
                else
                {
                    return JsonMessage(true);
                }
            }
            else
            {
                return JsonMessage(false, "参数有误!");
            }
        }

        /// <summary>
        /// 受理福农宝业务
        /// </summary>
        [HttpPost]
        [ActionName("_AcceptEdit")]
        public ActionResult AcceptEditProvide(Guid id, decimal changedLimit, decimal changedRates)
        {
            FunongbaoApply apply = _funongbaoApplyService.GetById(id);
            ViewBag.Success = true;
            DateTime finishDate = TypeConverter.StrToDateTime(WebHelper.GetString("finishDate"), DateTime.Now);
            if (apply != null)
            {
                if (apply.ProcessStatus != 0)
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "该申请已经处理过！";
                    return JsonMessage(false, "该申请已经处理过!");
                }
                else
                {
                    try
                    {
                        Funongbao CurrentFunongbao = _funongbaoService.GetById(apply.FunongbaoId);
                        IList<Funongbao> funongbaoGroup = _funongbaoService.GetGroupByNo(CurrentFunongbao.GroupNO);
                        _funongbaoApplyService.BeginTransaction();
                        apply.ProcessStatus = 1;
                        apply.FinishDate = finishDate;
                        apply.ChangedLimit = changedLimit;
                        apply.ChangedRates = changedRates;
                        _funongbaoApplyService.Update(apply);

                        CurrentFunongbao.CurrentLimit = apply.ChangedLimit;
                        CurrentFunongbao.CurrentRates = apply.ChangedRates;
                        CurrentFunongbao.GroupLimit = apply.ApplyLimit;
                        CurrentFunongbao.PreFinishDate = CurrentFunongbao.FinishDate;
                        CurrentFunongbao.FinishDate = finishDate;
                        string optLogs = "";
                        AdminLogger adminLogger = new AdminLogger();
                        adminLogger.AdminName = this.LoginAdmin.Name;
                        adminLogger.CreateDate = DateTime.Now;
                        adminLogger.FromIP = IpHelper.UserHostAddress;
                        adminLogger.Module = (int)AdminLoggerModuleEnum.FunongBao;
                        adminLogger.Description = "受理操作：福农宝用户" + CurrentFunongbao.Name + (!string.IsNullOrEmpty(optLogs) ? ",组成员" + optLogs.TrimEnd(',') + "的调额申请受理状态修改为通过[" + apply.ApplyLimit + "," + apply.ApplyRates + "]" : "");
                        AdminLoggerService.Insert(adminLogger);
                        _funongbaoApplyService.Commit();
                        return JsonMessage(true, "受理成功!");
                    }
                    catch(Exception ex)
                    {
                        _funongbaoApplyService.Rollback();
                        Log4NetImpl.Write("操作失败，系统故障！" + ex.ToString());
                        return JsonMessage(false, "操作失败，系统故障！!");
                    }

                }

            }
            else
            {
                return JsonMessage(false, "参数有误!");
            }
        }

        /// <summary>
        /// 受理福农宝业务
        /// </summary>
        [HttpPost]
        //[ActionName("_AcceptEdit")]
        public ActionResult AcceptEditProvideOld(Guid id, int processStatus, decimal applyLimit, decimal applyRates)
        {
            FunongbaoApply apply = _funongbaoApplyService.GetById(id);
            ViewBag.Success = true;
            DateTime finishDate = TypeConverter.StrToDateTime(WebHelper.GetString("finishDate"), DateTime.Now);
            if (apply != null)
            {
                if (apply.ProcessStatus != 0)
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "该申请已经处理过！";
                    return JsonMessage(false, "该申请已经处理过!");
                }
                else
                {
                    Funongbao CurrentFunongbao = _funongbaoService.GetById(apply.FunongbaoId);
                    IList<Funongbao> funongbaoGroup = _funongbaoService.GetGroupByNo(CurrentFunongbao.GroupNO);
                    Guid[] funongbaoIds = funongbaoGroup.Select(f => f.Id).ToArray();
                    IList<FunongbaoApply> funongbaoApplyGroup = _funongbaoApplyService.GetByFunongbaoIds(funongbaoIds);
                    decimal GropuLimitCount = funongbaoGroup.Sum(f => f.CurrentLimit);

                    if ((apply.ApplyStatus == 0 || apply.ApplyStatus == -1) && apply.ApplyDate > new DateTime(1900, 1, 1) && processStatus==1)
                    {
                        if (applyLimit >= GropuLimitCount)
                        {
                            if (applyLimit > 30 && applyLimit <= 60)
                            {
                                if (funongbaoGroup.Count < 2)
                                {
                                    ViewBag.Success = false;
                                    return JsonMessage(false, "提示开卡");
                                }
                            }
                            if (applyLimit > 60)
                            {
                                if (funongbaoGroup.Count < 3)
                                {
                                    ViewBag.Success = false;
                                    return JsonMessage(false, "提示开卡");
                                }
                            }
                        }
                        else
                        {
                            if (funongbaoApplyGroup.Count < 2)
                            {
                                ViewBag.Success = false;
                                return JsonMessage(false, "提示开卡");
                            }
                            else
                            {
                                decimal leaveCount = funongbaoApplyGroup.Where(f => f.FunongbaoId != CurrentFunongbao.Id).Sum(f => f.PreLimit);
                                if (((GropuLimitCount - applyLimit) - CurrentFunongbao.CurrentLimit) > leaveCount)
                                {
                                    ViewBag.Success = false;
                                    return JsonMessage(false, "提示开卡");
                                }
                            }
                        }
                        ViewBag.Success = false;
                        ViewBag.Message = "等待家庭组成员完成后才能受理！";
                        return JsonMessage(false, "等待组成员");

                    }
                    else
                    {

                        #region funongbao
                        AdminLogger adminLogger = new AdminLogger();
                        adminLogger.AdminName = this.LoginAdmin.Name;
                        adminLogger.CreateDate = DateTime.Now;
                        adminLogger.FromIP = IpHelper.UserHostAddress;
                        adminLogger.Module = (int)AdminLoggerModuleEnum.FunongBao;

                        try
                        {

                            if (processStatus != 3)//状态处理
                            {
                                _funongbaoApplyService.BeginTransaction();
                                if (processStatus == 1)//通过
                                {
                                    apply.ProcessStatus = 1;
                                    apply.FinishDate = finishDate;
                                    _funongbaoApplyService.Update(apply);

                                    CurrentFunongbao.CurrentLimit = apply.ChangedLimit;
                                    CurrentFunongbao.CurrentRates = apply.ChangedRates;
                                    CurrentFunongbao.GroupLimit = apply.ApplyLimit;
                                    CurrentFunongbao.PreFinishDate = CurrentFunongbao.FinishDate;
                                    CurrentFunongbao.FinishDate = finishDate;
                                    //if (apply.ApplyDate <= new DateTime(apply.ApplyDate.Year, apply.ApplyDate.Month, DateRule.ApplyMiddleDay))
                                    //{
                                    //    CurrentFunongbao.FinishDate = new DateTime(apply.ApplyDate.Year, apply.ApplyDate.Month, DateRule.ChangeDay);
                                    //}
                                    //else
                                    //{
                                    //    CurrentFunongbao.FinishDate = new DateTime(apply.ApplyDate.Year, apply.ApplyDate.Month + 1, DateRule.ChangeDay);
                                    //}
                                    //_funongbaoService.Update(CurrentFunongbao);
                                    string optLogs = "";
                                    foreach (FunongbaoApply _apply in funongbaoApplyGroup)
                                    {
                                        if (_apply.Id != apply.Id)//去掉当前
                                        {
                                            //if (_apply.ApplyLimit <= 0.00M)//同步组内时，当不涉及到其申请的
                                            //{
                                            //    _apply.ChangedLimit = apply.ChangedLimit;
                                            //    _apply.ChangedRates = apply.ChangedRates;
                                            //}
                                            Funongbao _fnb = funongbaoGroup.Where(f => f.Id == _apply.FunongbaoId).FirstOrDefault();
                                            if (_fnb != null)
                                            {
                                                optLogs += _fnb.Name + ",";
                                                if (_apply.ApplyDate > new DateTime(1900, 1, 1))
                                                {
                                                    _fnb.CurrentLimit = _apply.ChangedLimit;
                                                }
                                                _fnb.CurrentRates = apply.ChangedRates;
                                                _fnb.GroupLimit = apply.ApplyLimit;
                                                _fnb.PreFinishDate = _fnb.FinishDate;
                                                _fnb.FinishDate = finishDate;
                                                //if (apply.ApplyDate <= new DateTime(apply.ApplyDate.Year, apply.ApplyDate.Month, DateRule.ApplyMiddleDay))
                                                //{
                                                //    _fnb.FinishDate = new DateTime(apply.ApplyDate.Year, apply.ApplyDate.Month, DateRule.ChangeDay);
                                                //}
                                                //else
                                                //{
                                                //    _fnb.FinishDate = new DateTime(apply.ApplyDate.Year, apply.ApplyDate.Month + 1, DateRule.ChangeDay);
                                                //}
                                                //_funongbaoService.Update(_fnb);
                                            }
                                            else
                                            {
                                                Log4NetImpl.Write("当前申请[福农宝ID" + _apply.FunongbaoId.ToString() + "]的数据有误！");
                                                return JsonMessage(false, "当前申请[福农宝ID" + _apply.FunongbaoId.ToString() + "]的数据有误！");
                                                throw new Exception("当前申请[福农宝ID" + _apply.FunongbaoId.ToString() + "]的数据有误！");
                                            }
                                            if (_apply.ApplyDate > new DateTime(1900, 1, 1))//有申请的才进行更新
                                            {
                                                _apply.ProcessStatus = 1;
                                                _apply.FinishDate = finishDate;
                                            }
                                            else
                                            {
                                                _apply.ApplyDate = apply.ApplyDate;
                                                _apply.ApplyStatus = 1;
                                                _apply.GroupApplyStatus = 1;//联动
                                                _apply.ProcessStatus = 1;
                                                _apply.FinishDate = finishDate;
                                            }
                                            _funongbaoApplyService.Update(_apply);
                                        }
                                    }
                                    adminLogger.Description = "受理操作：福农宝用户" + CurrentFunongbao.Name + (!string.IsNullOrEmpty(optLogs) ? ",组成员" + optLogs.TrimEnd(',') + "的调额申请受理状态修改为通过[" + apply.ApplyLimit + "," + apply.ApplyRates + "]" : "");
                                    AdminLoggerService.Insert(adminLogger);
                                }
                                else//不过通
                                {
                                    apply.ProcessStatus = 2;
                                    apply.FinishDate = finishDate;
                                    apply.ChangedLimit = CurrentFunongbao.CurrentLimit;
                                    apply.ChangedRates = CurrentFunongbao.CurrentRates;
                                    apply.ProcessResult = WebHelper.GetString("ProcessResult");
                                    _funongbaoApplyService.Update(apply);
                                    string optLogs = "";
                                    foreach (FunongbaoApply _apply in funongbaoApplyGroup)
                                    {
                                        if (_apply.Id != apply.Id)//去掉当前
                                        {
                                            _apply.ProcessStatus = 2;
                                            _apply.FinishDate = finishDate;
                                            _apply.ProcessResult = WebHelper.GetString("ProcessResult");
                                            Funongbao _fnb = funongbaoGroup.Where(f => f.Id == _apply.FunongbaoId).FirstOrDefault();
                                            if (_fnb != null)
                                            {
                                                _apply.ChangedLimit = _fnb.CurrentLimit;
                                                _apply.ChangedRates = _fnb.CurrentRates;
                                                optLogs += _fnb.Name + ",";
                                            }
                                            _funongbaoApplyService.Update(_apply);
                                        }
                                    }
                                    adminLogger.Description = "受理操作：福农宝用户" + CurrentFunongbao.Name + (!string.IsNullOrEmpty(optLogs) ? ",组成员" + optLogs.TrimEnd(',') + "的调额申请受理状态修改为不通过[" + apply.ApplyLimit + "," + apply.ApplyRates + "]" : "");
                                    AdminLoggerService.Insert(adminLogger);
                                }
                                _funongbaoApplyService.Commit();
                                return JsonMessage(true, "受理成功!");
                            }
                            else //建议额度
                            {
                                apply.SuggestionLimit = apply.ApplyLimit.ToString() + "," + apply.ApplyRates.ToString();
                                apply.ApplyTempLimit = applyLimit;
                                apply.ApplyTempRates = applyRates;
                                apply.ProcessStatus = 3;
                                apply.FinishDate = finishDate;

                                string optLogs = "";

                                #region 升级额度情况
                                if (applyLimit >= GropuLimitCount)
                                {
                                    //获取满足条件的申请
                                    IList<FunongbaoApply> replationApplys = funongbaoApplyGroup.Where(f => f.ApplyLimit == apply.ApplyLimit && f.FunongbaoId != apply.FunongbaoId).ToList();
                                    //申请与现有的差值<当前申请卡的剩余额度-只需要当前申请-
                                    if (applyLimit - GropuLimitCount <= LimitRule.SingleLimitMax - CurrentFunongbao.CurrentLimit)
                                    {
                                        apply.ChangedLimit = CurrentFunongbao.CurrentLimit + (applyLimit - GropuLimitCount);
                                        apply.ChangedRates = apply.ApplyTempRates;
                                        apply.ApplyStatus = 1;
                                        _funongbaoApplyService.UpdateApllys(new FunongbaoApply[] { apply }, funongbaoApplyGroup.Where(fg => fg.Id != apply.Id));
                                    }
                                    //申请与现有的差值>当前申请卡的剩余额度-需要至少两张卡同时申请
                                    else
                                    {
                                        //必须属性家庭组和申请必须大于1个
                                        if (funongbaoGroup.Count > 1 && funongbaoApplyGroup.Count > 0 && replationApplys != null && replationApplys.Count() > 0)
                                        {
                                            //第一张卡
                                            apply.ChangedLimit = LimitRule.SingleLimitMax;
                                            apply.ChangedRates = applyRates;
                                            apply.ApplyStatus = 1;

                                            //计算第二张卡的额度-最小值的那张
                                            FunongbaoApply replationApply = replationApplys.Where(fa => funongbaoGroup.OrderBy(f => f.CurrentLimit).Where(f => fa.FunongbaoId == f.Id).Count() > 0).First();
                                            Funongbao replationFnb = funongbaoGroup.Where(f => f.Id == replationApply.FunongbaoId).First();
                                            replationApply.SuggestionLimit = replationApply.ApplyLimit + "," + replationApply.ApplyRates;
                                            //replationApply.ApplyLimit = applyLimit;
                                            //replationApply.ApplyRates = applyRates;
                                            decimal ChangeLimit = replationFnb.CurrentLimit + (applyLimit - GropuLimitCount - (LimitRule.SingleLimitMax - CurrentFunongbao.CurrentLimit));
                                            replationApply.ChangedLimit = ChangeLimit > LimitRule.SingleLimitMax ? LimitRule.SingleLimitMax : ChangeLimit;
                                            replationApply.ChangedRates = applyRates;
                                            replationApply.ApplyStatus = 1;
                                            replationApply.ProcessStatus = 3;
                                            replationApply.FinishDate = finishDate;
                                            optLogs = replationFnb.Name;

                                            if (ChangeLimit > LimitRule.SingleLimitMax)//判断是否大最大限额
                                            {
                                                //需要三张卡同时申请
                                                if (funongbaoGroup.Count > 2 && funongbaoApplyGroup.Count > 1 && replationApplys.Count() > 1)
                                                {
                                                    //第三张卡｜第三个申请
                                                    FunongbaoApply replationApply1 = replationApplys.Where(fa => funongbaoGroup.OrderBy(f => f.CurrentLimit).Where(f => fa.FunongbaoId == f.Id && fa.FunongbaoId != replationApply.FunongbaoId).Count() > 0).First();
                                                    Funongbao replationFnb1 = funongbaoGroup.Where(f => f.Id == replationApply1.FunongbaoId).First();
                                                    replationApply.SuggestionLimit = replationApply.ApplyLimit + "," + replationApply.ApplyRates;
                                                    //replationApply1.ApplyLimit = applyLimit;
                                                    //replationApply1.ApplyRates = applyRates;
                                                    replationApply1.ChangedLimit = (applyLimit - LimitRule.SingleLimitMax * 2);//replationFnb.CurrentLimit + (applyLimit - GropuLimitCount - (LimitRule.SingleLimitMax - CurrentFunongbao.CurrentLimit));
                                                    replationApply1.ChangedRates = applyRates;
                                                    replationApply1.ApplyStatus = 1;
                                                    replationApply1.ProcessStatus = 3;
                                                    replationApply1.FinishDate = finishDate;
                                                    optLogs += "," + replationFnb1.Name;
                                                    //放入更新事务：
                                                    _funongbaoApplyService.UpdateApllys(new FunongbaoApply[] { apply, replationApply, replationApply1 }, null);
                                                }
                                                else
                                                {
                                                    if (funongbaoApplyGroup.Count > 1)
                                                    {
                                                        //判断有几个需的提醒的福农宝名称
                                                        ViewBag.RelationNames = GetNoticeApplyNames(funongbaoGroup, apply, funongbaoApplyGroup);
                                                        if (ViewBag.RelationNames == "提示开卡!")
                                                        {
                                                            ViewBag.ApplyStatus = -1;

                                                            ViewBag.Success = false;
                                                            ViewBag.Message = "提示开卡!";
                                                            return JsonMessage(false, ViewBag.Message);
                                                        }
                                                        else
                                                        {
                                                            ViewBag.ApplyStatus = 2;

                                                            ViewBag.Success = false;
                                                            ViewBag.Message = "对不起，您建议的条件不满足，缺少[" + ViewBag.RelationNames + "]的申请！";
                                                            return JsonMessage(false, ViewBag.Message);
                                                        }
                                                        //return View("ApplyPost", apply);
                                                    }
                                                    else if (funongbaoApplyGroup.Count > 0)
                                                    {
                                                        //判断有几个需的提醒的福农宝名称
                                                        ViewBag.RelationNames = GetNoticeApplyNames(funongbaoGroup, apply, funongbaoIds);
                                                        if (ViewBag.RelationNames == "提示开卡!")
                                                        {
                                                            ViewBag.ApplyStatus = -1;

                                                            ViewBag.Success = false;
                                                            ViewBag.Message = "提示开卡!";
                                                            return JsonMessage(false, ViewBag.Message);
                                                        }
                                                        else
                                                        {
                                                            ViewBag.ApplyStatus = 2;

                                                            ViewBag.Success = false;
                                                            ViewBag.Message = "对不起，您建议的条件不满足，缺少[" + ViewBag.RelationNames + "]的申请！";
                                                            return JsonMessage(false, ViewBag.Message);
                                                        }
                                                        //return View("ApplyPost", apply);

                                                    }
                                                    else
                                                    {
                                                        Log4NetImpl.Write("Apply[" + apply.FunongbaoId.ToString() + "]数据异常，请联系管理员!");
                                                        ViewBag.ApplyStatus = -1;

                                                        ViewBag.Success = false;
                                                        ViewBag.Message = "提示开卡!";
                                                        return JsonMessage(false, ViewBag.Message);
                                                        //异常
                                                        //return View("ApplyError");
                                                        throw new OceanException("Apply[" + apply.FunongbaoId.ToString() + "]数据异常，请联系管理员!");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //放入更新事务：
                                                _funongbaoApplyService.UpdateApllys(new FunongbaoApply[] { apply, replationApply }, funongbaoApplyGroup.Where(fg => fg.Id != apply.Id && fg.Id != replationApply.Id));
                                            }

                                        }
                                        else
                                        {
                                            if (funongbaoApplyGroup.Count > 1)
                                            {
                                                //判断有几个需的提醒的福农宝名称
                                                ViewBag.RelationNames = GetNoticeApplyNames(funongbaoGroup, apply, funongbaoIds);
                                                if (ViewBag.RelationNames == "提示开卡!")
                                                {
                                                    ViewBag.ApplyStatus = -1;

                                                    ViewBag.Success = false;
                                                    ViewBag.Message = "提示开卡!";
                                                    return JsonMessage(false, ViewBag.Message);
                                                }
                                                else
                                                {
                                                    ViewBag.ApplyStatus = 2;

                                                    ViewBag.Success = false;
                                                    ViewBag.Message = "对不起，您建议的条件不满足，缺少[" + ViewBag.RelationNames + "]的申请！";
                                                }
                                                return JsonMessage(false, ViewBag.Message);
                                                //return View("ApplyPost", apply);
                                            }
                                            else
                                            {
                                                Log4NetImpl.Write("Apply[" + apply.FunongbaoId.ToString() + "]数据异常，请联系管理员!");
                                                ViewBag.ApplyStatus = -1;

                                                ViewBag.Success = false;
                                                ViewBag.Message = "提示开卡!";
                                                return JsonMessage(false, ViewBag.Message);
                                                //异常
                                                throw new OceanException("Apply[" + apply.FunongbaoId.ToString() + "]数据异常，请联系管理员!");
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region 降额情况
                                else
                                {
                                    apply.ApplyStatus = -1;
                                    apply.ApplyTempLimit = applyLimit;
                                    apply.ApplyTempRates = applyRates;
                                    //获取满足条件的申请
                                    IList<FunongbaoApply> replationApplys = funongbaoApplyGroup.Where(f => f.ApplyLimit == apply.ApplyLimit && f.FunongbaoId != apply.FunongbaoId).ToList();
                                    //申请与现有的差值<当前申请卡的剩余额度-只需要当前申请-
                                    if (CurrentFunongbao.CurrentLimit - (GropuLimitCount - applyLimit) >= 0)
                                    {
                                        apply.ChangedLimit = CurrentFunongbao.CurrentLimit - (GropuLimitCount - applyLimit);
                                        apply.ChangedRates = apply.ApplyTempRates;
                                        apply.ApplyStatus = 1;
                                        _funongbaoApplyService.UpdateApllys(new FunongbaoApply[] { apply }, funongbaoApplyGroup.Where(fg => fg.Id != apply.Id));

                                    }
                                    //申请与现有的差值>当前申请卡的剩余额度-需要至少两张卡同时申请
                                    else
                                    {
                                        //必须属性家庭组和申请必须大于1个
                                        if (funongbaoGroup.Count > 1 && funongbaoApplyGroup.Count > 0 && replationApplys != null && replationApplys.Count() > 0)
                                        {
                                            //第一张卡
                                            apply.ChangedLimit = 0;
                                            apply.ChangedRates = applyRates;
                                            apply.ApplyStatus = 1;
                                            //计算第二张卡的额度-最小值的那张
                                            FunongbaoApply replationApply = replationApplys.Where(fa => funongbaoGroup.OrderBy(f => f.CurrentLimit).Where(f => fa.FunongbaoId == f.Id).Count() > 0).First();
                                            Funongbao replationFnb = funongbaoGroup.Where(f => f.Id == replationApply.FunongbaoId).First();
                                            replationApply.SuggestionLimit = replationApply.ApplyLimit + "," + replationApply.ApplyRates;
                                            //replationApply.ApplyLimit = applyLimit;
                                            //replationApply.ApplyRates = applyRates;
                                            decimal ChangeLimit = replationFnb.CurrentLimit - ((GropuLimitCount - applyLimit) - CurrentFunongbao.CurrentLimit);
                                            replationApply.ChangedLimit = (ChangeLimit < 0.00M ? 0.00M : ChangeLimit);
                                            replationApply.ChangedRates = applyRates;
                                            replationApply.ApplyStatus = 1;
                                            replationApply.ProcessStatus = 3;
                                            replationApply.FinishDate = finishDate;
                                            optLogs = replationFnb.Name;
                                            if (ChangeLimit < 0)
                                            {
                                                //需要三张卡同时申请
                                                if (funongbaoGroup.Count > 2 && funongbaoApplyGroup.Count > 1 && replationApplys.Count() > 1)
                                                {
                                                    //第三张卡｜第三个申请
                                                    FunongbaoApply replationApply1 = replationApplys.Where(fa => funongbaoGroup.OrderBy(f => f.CurrentLimit).Where(f => fa.FunongbaoId == f.Id && fa.FunongbaoId != replationApply.FunongbaoId).Count() > 0).First();
                                                    Funongbao replationFnb1 = funongbaoGroup.Where(f => f.Id == replationApply1.FunongbaoId).First();
                                                    replationApply1.SuggestionLimit = replationApply1.ApplyLimit + "," + replationApply1.ApplyRates;
                                                    //replationApply1.ApplyLimit = applyLimit;
                                                    //replationApply1.ApplyRates = applyRates;
                                                    replationApply1.ChangedLimit = (replationFnb1.CurrentLimit - ((GropuLimitCount - applyLimit) - CurrentFunongbao.CurrentLimit - replationFnb.CurrentLimit));//replationFnb.CurrentLimit + (applyLimit - GropuLimitCount - (LimitRule.SingleLimitMax - CurrentFunongbao.CurrentLimit));
                                                    replationApply1.ChangedRates = applyRates;
                                                    replationApply1.ApplyStatus = 1;
                                                    replationApply1.ProcessStatus = 3;
                                                    replationApply1.FinishDate = finishDate;
                                                    optLogs += "," + replationFnb1.Name;
                                                    //放入更新事务：
                                                    _funongbaoApplyService.UpdateApllys(new FunongbaoApply[] { apply, replationApply, replationApply1 }, null);
                                                }
                                                else
                                                {
                                                    if (funongbaoApplyGroup.Count > 1)
                                                    {
                                                        //判断有几个需的提醒的福农宝名称
                                                        ViewBag.RelationNames = GetNoticeApplyNames(funongbaoGroup, apply, funongbaoApplyGroup);
                                                        if (ViewBag.RelationNames == "提示开卡!")
                                                        {
                                                            ViewBag.ApplyStatus = -1;

                                                            ViewBag.Success = false;
                                                            ViewBag.Message = "提示开卡!";
                                                            return JsonMessage(false, ViewBag.Message);
                                                        }
                                                        else
                                                        {
                                                            ViewBag.ApplyStatus = 3;

                                                            ViewBag.Success = false;
                                                            ViewBag.Message = "对不起，您建议的条件不满足，缺少[" + ViewBag.RelationNames + "]的申请！";
                                                        }
                                                        //return View("ApplyPost", apply);
                                                    }
                                                    else if (funongbaoApplyGroup.Count > 0)
                                                    {
                                                        //判断有几个需的提醒的福农宝名称
                                                        ViewBag.RelationNames = GetNoticeApplyNames(funongbaoGroup, apply, funongbaoIds);
                                                        if (ViewBag.RelationNames == "提示开卡!")
                                                        {
                                                            ViewBag.ApplyStatus = -1;

                                                            ViewBag.Success = false;
                                                            ViewBag.Message = "提示开卡!";
                                                            return JsonMessage(false, ViewBag.Message);
                                                        }
                                                        else
                                                        {
                                                            ViewBag.ApplyStatus = 3;

                                                            ViewBag.Success = false;
                                                            ViewBag.Message = "对不起，您建议的条件不满足，缺少[" + ViewBag.RelationNames + "]的申请！";
                                                            return JsonMessage(false, ViewBag.Message);
                                                        }
                                                        //return View("ApplyPost", apply);

                                                    }
                                                    else
                                                    {
                                                        Log4NetImpl.Write("Apply[" + apply.FunongbaoId.ToString() + "]数据异常，请联系管理员!");
                                                        //异常
                                                        ViewBag.ApplyStatus = -1;

                                                        ViewBag.Success = false;
                                                        ViewBag.Message = "提示开卡!";
                                                        return JsonMessage(false, ViewBag.Message);
                                                        throw new OceanException("Apply[" + apply.FunongbaoId.ToString() + "]数据异常，请联系管理员!");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //放入更新事务：
                                                _funongbaoApplyService.UpdateApllys(new FunongbaoApply[] { apply, replationApply }, funongbaoApplyGroup.Where(fg => fg.Id != apply.Id && fg.Id != replationApply.Id));
                                            }
                                        }
                                        else
                                        {
                                            if (funongbaoApplyGroup.Count > 1)
                                            {
                                                //判断有几个需的提醒的福农宝名称
                                                ViewBag.RelationNames = GetNoticeApplyNames(funongbaoGroup, apply, funongbaoIds);
                                                if (ViewBag.RelationNames == "提示开卡!")
                                                {
                                                    ViewBag.ApplyStatus = -1;

                                                    ViewBag.Success = false;
                                                    ViewBag.Message = "提示开卡!";
                                                    return JsonMessage(false, ViewBag.Message);
                                                }
                                                else
                                                {
                                                    ViewBag.ApplyStatus = 3;

                                                    ViewBag.Success = false;
                                                    ViewBag.Message = "对不起，您建议的条件不满足，缺少[" + ViewBag.RelationNames + "]的申请！";
                                                    return JsonMessage(false, ViewBag.Message);
                                                }
                                                //return View("ApplyPost", apply);
                                            }
                                            else
                                            {
                                                Log4NetImpl.Write("Apply[" + apply.FunongbaoId.ToString() + "]数据异常，请联系管理员!");
                                                //异常
                                                ViewBag.ApplyStatus = -1;

                                                ViewBag.Success = false;
                                                ViewBag.Message = "提示开卡!";
                                                return JsonMessage(false, ViewBag.Message);
                                                throw new OceanException("Apply[" + apply.FunongbaoId.ToString() + "]数据异常，请联系管理员!");
                                            }
                                        }
                                    }
                                }
                                #endregion

                                adminLogger.Description = "受理操作：福农宝用户" + CurrentFunongbao.Name + (!string.IsNullOrEmpty(optLogs) ? ",组成员" + optLogs.TrimEnd(',') + "的调额申请受理状态修改为建议额度[" + applyLimit + "," + applyRates + "]" : "");
                                AdminLoggerService.Insert(adminLogger);

                                return JsonMessage(true, "受理成功!");
                            }
                        }
                        catch (Exception ex)
                        {
                            _funongbaoApplyService.Rollback();
                            Log4NetImpl.Write("操作失败，系统故障！" + ex.ToString());
                            return JsonMessage(false, "操作失败，系统故障！!");
                            throw new OceanException("操作失败，系统故障！", ex);
                        }
                    #endregion
                    }
                }
            }
            else
            {
                //throw new OceanException("对不起，参数有误！");

                return JsonMessage(false, "参数有误!");
            }
            //return JsonMessage(true, "受理成功!");
            //return View(apply);
        }
        /// <summary>
        /// 获取服农宝申请者关联福农宝名称
        /// </summary>
        /// <param name="funongbaoGroup">福农宝组</param>
        /// <param name="apply">当前申请者</param>
        /// <param name="funongbaoIds">关联福农宝ID</param>
        /// <returns></returns>
        private string GetNoticeApplyNames(IList<Funongbao> funongbaoGroup,FunongbaoApply apply, Guid[] funongbaoIds)
        {
            string[] rnames = funongbaoGroup.Where(f => funongbaoIds.Contains(f.Id) && f.Id == apply.FunongbaoId).Select(f => f.Name).ToArray();
            if (rnames.Length > 1)
            {
                return string.Join("或", rnames);
            }
            else if (rnames.Length > 0)
            {
                return rnames[0];
            }
            else {
                return "提示开卡!";
            }
        }
        /// <summary>
        /// 获取服农宝申请者关联福农宝名称
        /// </summary>
        /// <param name="funongbaoGroup">福农宝组</param>
        /// <param name="apply">当前申请者</param>
        /// <param name="applys">已申请的关联福农宝</param>
        /// <returns></returns>
        private string GetNoticeApplyNames(IList<Funongbao> funongbaoGroup,FunongbaoApply apply, IEnumerable<FunongbaoApply> applys)
        {
            string[] rnames = funongbaoGroup.Where(f => (apply.FunongbaoId != f.Id && applys.Where(fa => fa.FunongbaoId == f.Id && fa.Id != apply.Id).Count()>0)).Select(f => f.Name).ToArray();
            if (rnames.Length > 1)
            {
                return string.Join("或", rnames);
            }
            else if (rnames.Length > 0)
            {
                return rnames[0];
            }
            else
            {
                return "提示开卡!";
            }
        }
        #endregion

        #region  导入福农宝调额申请
        [HttpGet]
        [ActionName("_ImportExcel")]
        public ActionResult ImportExcel()
        {
            ViewBag.Method = "get";
            ////是否限制导入的时间
            //DateRule drule = DateRuleList.DateRules.Where(d => d.ApplyMonth == DateTime.Now.Month).FirstOrDefault();
            //if (drule == null || DateRule.ImportEndDay > DateTime.Now.Day && DateRule.ImportStartDay >= DateTime.Now.Day)
            //{
            //    ViewBag.Denied = "必须在每季度首月1-10号导入调额申请！";
            //}
            return View();
        }
        /// <summary>
        /// 导入福农宝客户
        /// </summary>
        [HttpPost]
        [ActionName("_ImportExcel")]
        public ActionResult ImportExcel(HttpPostedFileBase fnbExcel)
        {
            ViewBag.Method = "post";
            ViewBag.Success = true;
            int countFnb = 0;
            int countFnbApply = 0;
            int countFnbApplyUpdate = 0;
            if (fnbExcel != null)
            {
                DataTable dt = null;
                if (fnbExcel.FileName.ToLower().EndsWith(".xlsx"))
                {
                    dt = NPOIHelper.ExcelToTableForXLSX(fnbExcel.InputStream, "组编号");
                }
                else
                {
                    dt = NPOIHelper.ExcelToTableForXLS(fnbExcel.InputStream, "组编号", string.Empty);
                }
                var columns = dt.Columns;
                //string value = string.Empty;

                //foreach (DataColumn column in columns)
                //{
                //    value += column.ColumnName + "    ";
                //}
                //LogFileImpl.Write(value);
                DateRule rule = DateRuleList.DateRules.Where(d => d.Months.Where(m => m == DateTime.Now.Month).Count() > 0).First();
                int month = rule.ApplyMonth;
                int year = DateTime.Now.Year;
                //if (DateTime.Now.Month > month || DateTime.Now.Day > DateRule.ApplyMiddleDay)
                //{
                //    month = (rule.ApplyMonth - 3) < 0 ? 10 : (rule.ApplyMonth - 3);
                //    year = (rule.ApplyMonth - 3) < 0 ? (year - 1) : year;
                //}

                try
                {
                    //_funongbaoApplyService.BeginTransaction();
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        //value = "";
                        //foreach (DataColumn column in columns)
                        //{
                        //    value += dt.Rows[i][column.ColumnName].ToString() + "    ";
                        //}
                        //LogFileImpl.Write(value);
                        //福农宝信息
                        Funongbao fnb = new Funongbao();
                        fnb.CreateDate = DateTime.Now;
                        fnb.CurrentLimit = TypeConverter.ObjectToDecimal(dt.Rows[i]["原卡授信额度"]);
                        fnb.CurrentRates = TypeConverter.ObjectToDecimal(dt.Rows[i]["原费率"]);
                        fnb.FunongbaoNO = dt.Rows[i]["福农宝卡号"].ToString();
                        if (!string.IsNullOrEmpty(fnb.FunongbaoNO) && fnb.FunongbaoNO.Length >= 8)
                        {
                            fnb.PassportNO = fnb.FunongbaoNO.Substring(fnb.FunongbaoNO.Length - 8, 8);
                        }
                        fnb.GroupLimit = TypeConverter.ObjectToDecimal(dt.Rows[i]["原组授信额度"]);
                        fnb.GroupNO = dt.Rows[i]["组编号"].ToString();
                        fnb.IsSignAgreement = (dt.Rows[i]["是否签订微信协议"].ToString() == "是" ? 1 : 0);//TypeConverter.StrToInt(dt.Rows[i]["激活标志"].ToString());
                        fnb.Name = dt.Rows[i]["姓名"].ToString();
                        fnb.MobilePhone = dt.Rows[i]["手机"].ToString();
                        fnb.PassportNO = dt.Rows[i]["证件号码"].ToString();//未见-先用卡号代替
                        if (!string.IsNullOrEmpty(fnb.PassportNO) && fnb.PassportNO.Length>=8)
                        {
                            if (Validator.IsIDentCard(fnb.PassportNO))
                            {
                                fnb.PassportType = 1;
                            }
                            fnb.PassportNO = fnb.PassportNO.Substring(fnb.PassportNO.Length - 8, 8);
                        }
                        fnb.ProcessStatus = 1;
                        fnb.Subbranch = dt.Rows[i]["营销机构"].ToString();
                        fnb.FinishDate = DateTime.Now;//new DateTime(year, month, DateRule.ChangeDay);
                        fnb.PreFinishDate = DateTime.Now;// new DateTime(year, month, DateRule.ChangeDay);
                        fnb.Marketer = dt.Rows[i]["营销人"].ToString();
                        //额度申请
                        FunongbaoApply fnbApply = new FunongbaoApply();
                        fnbApply.CreateDate = DateTime.Now;
                        if (dt.Columns.Contains("调整策略"))
                        {
                            fnbApply.LimitProgramme = StringHelper.GetLimitProgrammeStr(dt.Rows[i]["调整策略"].ToString()) ?? "";
                        }
                        else
                        {
                            fnbApply.LimitProgramme = StringHelper.GetLimitProgrammeStr(dt.Rows[i]["Columns0"].ToString()) ?? "";
                        }
                        fnbApply.PreGroupLimit = TypeConverter.ObjectToDecimal(dt.Rows[i]["原组授信额度"]);
                        fnbApply.PreLimit = TypeConverter.ObjectToDecimal(dt.Rows[i]["原卡授信额度"]);
                        fnbApply.PreRates = TypeConverter.ObjectToDecimal(dt.Rows[i]["原费率"]);
                        fnbApply.ProcessStatus = 0;
                        fnbApply.Subbranch = dt.Rows[i]["营销机构"].ToString();
                        fnbApply.Marketer = dt.Rows[i]["营销人"].ToString();
                        fnbApply.ApplyStatus = 0;

                        Funongbao newFunonbao;
                        int auth = _funongbaoService.IdentAuth(fnb, out newFunonbao,false);
                        if (auth == 1)//已存在的福农宝
                        {
                            if (newFunonbao.GroupLimit!=fnbApply.PreGroupLimit|| newFunonbao.CurrentLimit != fnbApply.PreLimit || newFunonbao.CurrentRates != fnbApply.PreRates)
                            {
                                newFunonbao.CurrentLimit = fnbApply.PreLimit;
                                newFunonbao.CurrentRates = fnbApply.PreRates;
                                _funongbaoService.Update(newFunonbao);
                            }
                            fnbApply.FunongbaoId = newFunonbao.Id;
                        }
                        else
                        {
                            countFnb++;
                            _funongbaoService.Insert(fnb);
                            fnbApply.FunongbaoId = fnb.Id;
                        }
                        FunongbaoApply oldfnbApply = _funongbaoApplyService.GetByFunongbaoIdAndDate(fnbApply.FunongbaoId);
                        if (oldfnbApply == null)
                        {
                            countFnbApply++;
                            _funongbaoApplyService.Insert(fnbApply);
                        }
                        else
                        {
                            if (oldfnbApply.ApplyDate <= new DateTime(1900, 1, 2))
                            {
                                oldfnbApply.CreateDate = DateTime.Now;
                                oldfnbApply.LimitProgramme = fnbApply.LimitProgramme;
                                oldfnbApply.PreGroupLimit = fnbApply.PreGroupLimit;
                                oldfnbApply.PreLimit = fnbApply.PreLimit;
                                oldfnbApply.PreRates = fnbApply.PreRates;
                                oldfnbApply.Subbranch = fnbApply.Subbranch;
                                oldfnbApply.RFunongbao = null;
                                _funongbaoApplyService.Update(oldfnbApply);
                                countFnbApplyUpdate++;
                            }
                            else
                            {
                                oldfnbApply.PreGroupLimit = fnbApply.PreGroupLimit;
                                oldfnbApply.PreLimit = fnbApply.PreLimit;
                                oldfnbApply.PreRates = fnbApply.PreRates; 
                                _funongbaoApplyService.Update(oldfnbApply);
                                countFnbApplyUpdate++;
                            }
                        }

                    }
                    //_funongbaoApplyService.Commit();
                }
                catch(Exception ex) {
                    //_funongbaoApplyService.Rollback();
                    throw new OceanException("导入失败，具体原因，请查看日志！", ex);
                }
                ViewBag.CountFnb = countFnb;
                ViewBag.CountFnbApply = countFnbApply;
                ViewBag.CountFnbApplyUpdate = countFnbApplyUpdate;
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message="请选择导入文件";
            }
            return View();
        }

        #endregion

        #region 导入福农宝
        [HttpGet]
        [ActionName("_ImportFnbExcel")]
        public ActionResult ImportFnbExcel()
        {
            ViewBag.Method = "get";
                return View();
        }
        /// <summary>
        /// 导入福农宝客户
        /// </summary>
        [HttpPost]
        [ActionName("_ImportFnbExcel")]
        public ActionResult ImportFnbExcel(HttpPostedFileBase fnbExcel)
        {
            ViewBag.Method = "post";
            ViewBag.Success = true;
            int countFnb = 0;
            int countFnbUpdate = 0;
            if (fnbExcel != null)
            {
                DataTable dt = null;
                if (fnbExcel.FileName.ToLower().EndsWith(".xlsx"))
                {
                    dt = NPOIHelper.ExcelToTableForXLSX(fnbExcel.InputStream, "组编号");
                }
                else
                {
                    dt=NPOIHelper.ExcelToTableForXLS(fnbExcel.InputStream, "组编号", string.Empty);
                }
                var columns = dt.Columns;
                //string value = string.Empty;

                //foreach (DataColumn column in columns)
                //{
                //    value += column.ColumnName + "    ";
                //}
                //LogFileImpl.Write(value);
                DateRule rule = DateRuleList.DateRules.Where(d => d.Months.Where(m => m == DateTime.Now.Month).Count() > 0).First();
                int month = rule.ApplyMonth;
                int year = DateTime.Now.Year;
                try
                {
                    //_funongbaoApplyService.BeginTransaction();
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        
                        //福农宝信息
                        Funongbao fnb = new Funongbao();
                        fnb.CreateDate = DateTime.Now;
                        fnb.CurrentLimit = TypeConverter.ObjectToDecimal(dt.Rows[i]["原卡授信额度"]);
                        fnb.CurrentRates = TypeConverter.ObjectToDecimal(dt.Rows[i]["原费率"]);
                        fnb.FunongbaoNO = dt.Rows[i]["福农宝卡号"].ToString();
                        if (!string.IsNullOrEmpty(fnb.FunongbaoNO) && fnb.FunongbaoNO.Length >= 8)
                        {
                            fnb.PassportNO = fnb.FunongbaoNO.Substring(fnb.FunongbaoNO.Length - 8, 8);
                        }
                        fnb.GroupLimit = TypeConverter.ObjectToDecimal(dt.Rows[i]["原组授信额度"]);
                        fnb.GroupNO = dt.Rows[i]["组编号"].ToString();
                        fnb.IsSignAgreement = 0;//TypeConverter.StrToInt(dt.Rows[i]["激活标志"].ToString());
                        fnb.Name = dt.Rows[i]["姓名"].ToString();
                        fnb.MobilePhone = dt.Rows[i]["手机"].ToString();
                        fnb.PassportNO = dt.Rows[i]["证件号码"].ToString();//未见-先用卡号代替
                        if (!string.IsNullOrEmpty(fnb.PassportNO) && fnb.PassportNO.Length >= 8)
                        {
                            if (Validator.IsIDentCard(fnb.PassportNO))
                            {
                                fnb.PassportType = 1;
                            }
                            fnb.PassportNO = fnb.PassportNO.Substring(fnb.PassportNO.Length - 8, 8);
                        }
                        fnb.ProcessStatus = 1;
                        fnb.Subbranch = dt.Rows[i]["营销机构"].ToString();
                        fnb.FinishDate = DateTime.Now;//new DateTime(year, month, DateRule.ChangeDay);
                        fnb.PreFinishDate = DateTime.Now;// new DateTime(year, month, DateRule.ChangeDay);
                        fnb.Marketer = dt.Rows[i]["营销人"].ToString();
                        //fnb.FinishDate = new DateTime(year, month, DateRule.ChangeDay);
                        Funongbao oldFunonbao;
                        int auth = _funongbaoService.IdentAuth(fnb, out oldFunonbao, false);
                        if (auth == 1)//已存在的福农宝
                        {
                            countFnbUpdate++;
                            oldFunonbao.CurrentLimit = fnb.CurrentLimit;
                            oldFunonbao.GroupLimit = fnb.GroupLimit;
                            oldFunonbao.CurrentRates = fnb.CurrentRates;
                            oldFunonbao.MobilePhone = fnb.MobilePhone;
                            oldFunonbao.Name = fnb.Name;
                            oldFunonbao.PassportNO = fnb.PassportNO;
                            oldFunonbao.ProcessResult = fnb.ProcessResult;
                            oldFunonbao.Subbranch = fnb.Subbranch;
                            oldFunonbao.Marketer = fnb.Marketer;
                            //oldFunonbao.IsSignAgreement = fnb.IsSignAgreement;
                            fnb.PreFinishDate = DateTime.Now;
                            oldFunonbao.FinishDate = DateTime.Now; //fnb.FinishDate;
                            
                            _funongbaoService.Update(oldFunonbao);
                        }
                        else
                        {
                            countFnb++;
                            _funongbaoService.Insert(fnb);
                        }
                    }
                    //_funongbaoApplyService.Commit();
                    //_funongbaoApplyService.ExcuteSql("update funongbao set GroupLimit=(select sum(CurrentLimit) from funongbao f where f.GroupNO=funongbao.GroupNO) where GroupLimit=0.000;");
                }
                catch(Exception ex) {
                    //_funongbaoApplyService.Rollback();
                    throw new OceanException("导入失败，具体原因，请查看日志！", ex);
                }
                ViewBag.CountFnb = countFnb;
                ViewBag.CountFnbUpdate = countFnbUpdate;
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message="请选择导入文件";
            }
            return View();
        }
        #endregion

        #region 导出工单
        /// <summary>
        /// 导出工单
        /// </summary>
        [HttpGet]
        public void ExportExcel(Guid id)
        {
            string fileName = string.Format("{0}.xls", id.ToString().ToLower());
            FunongbaoApply apply = _funongbaoApplyService.GetById(id);
            if (apply != null)
            {
                //if (apply.ProcessStatus == 1 || apply.ProcessStatus == 3)
                //{
                MemoryStream ms = RenderToExcel(apply);

                if (Request.Browser.Browser == "IE")
                {
                    fileName = HttpUtility.UrlEncode(fileName);
                }
                Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileName);
                Response.BinaryWrite(ms.ToArray());
                //}
                //else
                //{
                //      Response.Clear();
                //      Response.ContentType = "text/javascript";
                //      Response.Write("{\"message\":\"该调额申请还未受理!\"}");
                //}
            }
            else
            {
                Response.Clear();
                Response.ContentType = "text/javascript";
                Response.Write("{\"message\":\"不存在的调额申请!\"}");
            }
        }

        /// <summary>
        /// 把数据渲染到Excel
        /// </summary>
        private MemoryStream RenderToExcel(FunongbaoApply apply)
        {
                MemoryStream ms = new MemoryStream();
                IWorkbook workbook = null;
                ISheet sheet = null;
                try
                {
                    using (FileStream fileStream = System.IO.File.OpenRead(FileHelper.GetMapPath("/Content/ExcelTemplate/FunongbaoApply.xls")))   //打开xls文件
                    {
                        workbook = new HSSFWorkbook(fileStream);
                        sheet = workbook.GetSheetAt(0);
                        sheet.GetRow(1).GetCell(1).SetCellValue(apply.RFunongbao.Name);
                        sheet.GetRow(1).GetCell(3).SetCellValue(apply.RFunongbao.MobilePhone);
                        sheet.GetRow(1).GetCell(5).SetCellValue(apply.RFunongbao.PassportNO);
                        sheet.GetRow(2).GetCell(1).SetCellValue(apply.RFunongbao.FunongbaoNO);
                        sheet.GetRow(2).GetCell(3).SetCellValue((double)apply.PreLimit);
                        sheet.GetRow(2).GetCell(5).SetCellValue((double)apply.PreRates);
                        sheet.GetRow(3).GetCell(1).SetCellValue((double)apply.PreGroupLimit);
                        sheet.GetRow(3).GetCell(3).SetCellValue((apply.ApplyType == 1 ? "提升额度" : "降低费率"));
                        //sheet.GetRow(3).GetCell(5).SetCellValue((double)apply.ApplyRates);

                        sheet.GetRow(4).GetCell(1).SetCellValue("零售部");
                        sheet.GetRow(4).GetCell(3).SetCellValue(DateTime.Now.ToString("yyyy-MM-dd"));
                        sheet.GetRow(5).GetCell(1).SetCellValue(apply.Subbranch);
                        sheet.GetRow(5).GetCell(3).SetCellValue(apply.Marketer);
                        //sheet.GetRow(4).GetCell(1).SetCellValue(base.LoginAdmin.PermissionOrganization.Name);
                        //sheet.GetRow(4).GetCell(3).SetCellValue(DateTime.Now.ToString("yyyy-MM-dd"));
                        //sheet.GetRow(5).GetCell(1).SetCellValue("受理支行");
                        //sheet.GetRow(5).GetCell(3).SetCellValue("受理客户经理");
                        int applyMonth = apply.ApplyDate.Month;
                        int applyDay = apply.ApplyDate.Day;
                        DateRule dateRule = DateRuleList.DateRules.Where(d => d.Months.Where(m => m == applyMonth).Count() > 0).First();
                        //if (applyDay > DateRule.ApplyMiddleDay) //判断当月或者次月
                        //{
                        //    applyMonth = applyMonth + 1;
                        //}
                        DateTime resultDate = DateTime.Now.AddDays(6);
                        sheet.GetRow(7).GetCell(0).SetCellValue(string.Format("请于{0}之前反馈处理结果", resultDate.Month + "月" + (resultDate.Day) + "日"));
                        workbook.Write(ms);
                        ms.Flush();
                        ms.Position = 0;
                    }

                    return ms;
                }
                finally
                {
                    ms.Close();
                    workbook = null;
                    sheet = null;
                }
        }
        #endregion

        public ActionResult Excute()
        {
            //string sql = @"delete FunongbaoApply where CreateDate>'2014-04-01'";
            //_funongbaoService.ExcuteSql(sql);
            return Content("excute");
        }
    }
}