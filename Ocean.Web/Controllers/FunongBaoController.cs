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

namespace Ocean.Web.Controllers
{
    public class FunongBaoController : IdenAuthBaseController
    {
        private readonly IFunongbaoApplyService _funongbaoApplyService;
        public FunongBaoController(IFunongbaoService funongbaoService,IFunongbaoApplyService funongbaoApplyService):base(funongbaoService)
        {
            _funongbaoApplyService = funongbaoApplyService;
        }
        public ActionResult Index()
        {
            //IList<Funongbao> funongbaoGroup = _funongbaoService.GetGroupByNo(CurrentFunongbao.GroupNO);
            //IEnumerable<Funongbao> funs = funongbaoGroup.Where(f => f.IsAuth == 0).ToList();
            return View();
        }
        public ActionResult myfnb()
        {
            FunongbaoApply apply = _funongbaoApplyService.GetByFunongbaoId(CurrentFunongbao.Id);
            ViewBag.Apply = apply;
            return View(CurrentFunongbao);
        }

        #region 调额申请
        //***************************疑问********************************
        //还有一个疑问：当同组的有一个提交了超额的申请，另外一个人如果提交不同的申请，需不需要提示？
        //如果已经提交了超额的申请，进去的提示直接提交时的提示一样吗？
        //***************************疑问********************************
        public ActionResult ApplyOld()
        {
            //if (CurrentFunongbao.IsSignAgreement == 1 || (!string.IsNullOrEmpty(RQuery["c"]) && RQuery["c"]=="go"))//判断是否签订协议
            //{
                FunongbaoApply apply = _funongbaoApplyService.GetByFunongbaoId(CurrentFunongbao.Id);
                if (apply != null && apply.ApplyDate > new DateTime(1900, 1, 1))//判断当季度是否已申请
                {
                    int applyMonth = apply.ApplyDate.Month;
                    int applyDay = apply.ApplyDate.Day;
                    DateRule dateRule = DateRuleList.DateRules.Where(d => d.Months.Where(m => m == applyMonth).Count() > 0).First();
                    if (apply.ApplyDate > new DateTime(apply.ApplyDate.Year, apply.ApplyDate.Month, DateRule.ApplyMiddleDay)) //判断当月或者次月
                    {
                        ViewBag.ApplyMonth = applyMonth + 1;
                    }
                    else
                    {
                        ViewBag.ApplyMonth = applyMonth;
                    }
                    ViewBag.ChangeDay = DateRule.ChangeDay;
                    if (apply.ApplyStatus <= 0)
                    {
                        IList<Funongbao> funongbaoGroup = _funongbaoService.GetGroupByNo(CurrentFunongbao.GroupNO);
                        Guid[] funongbaoIds = funongbaoGroup.Select(f => f.Id).ToArray();
                        IList<FunongbaoApply> funongbaoApplyGroup = _funongbaoApplyService.GetByFunongbaoIds(funongbaoIds);

                        ViewBag.RelationNames = GetNoticeApplyNames(funongbaoGroup, apply, funongbaoApplyGroup);
                        if (ViewBag.RelationNames == "-1")
                        {
                            ViewBag.ApplyStatus = -1;
                        }
                    }
                    return View("Applied", apply);
                }
                else
                {
                    IList<LimitProgrammeDTO> limitList = null;
                    if (apply != null)
                    {
                        limitList = _funongbaoApplyService.GetLimitProgramme(apply.LimitProgramme);
                        if (limitList != null && limitList.Count == 1)
                        {
                            ViewBag.LimitProgrammeList = limitList;
                        }
                        else
                        {
                            FunongbaoApply groupApply = _funongbaoApplyService.ExistGroupApply(apply.RFunongbao.GroupNO);
                            if (groupApply != null && limitList != null)
                            {
                                IEnumerable<LimitProgrammeDTO> groupLimits = limitList.Where(l => l.ApplyLimit == groupApply.ApplyLimit/10000 && l.ApplyRates == groupApply.ApplyRates);
                                if (groupLimits.Count() > 0)
                                {
                                    ViewBag.LimitProgrammeList = groupLimits.ToList();
                                }
                                else
                                {
                                    ViewBag.LimitProgrammeList = null;
                                }
                            }
                            else
                            {
                                ViewBag.LimitProgrammeList = limitList;
                            }
                        }
                    }
                    if (WebHelper.IsPost())
                    {
                        return ApplyPost(apply, limitList);
                    }

                    return View("LimitProgramme", apply);
                }
            //}
            //else
            //{
            //    return View("WeixinAgreement");
            //}
        }
        private ActionResult ApplyPostOld(FunongbaoApply apply, IList<LimitProgrammeDTO> limitList)
        {
            string applyLimitRates = WebHelper.GetString("ApplyLimitRates");
            if (!string.IsNullOrEmpty(applyLimitRates) && applyLimitRates.Split(',').Length==2)
            {
                string[] limits = applyLimitRates.Split(',');
                //非法数据的验证
                decimal applyLimit = TypeConverter.StrToDecimal(limits[0])*10000.00M;
                decimal applyRates = TypeConverter.StrToDecimal(limits[1]);
                if (applyLimit <= 0.00M || applyRates <= 0.00M)
                {
                    AppendError("ApplyLimitRates", "您提交了非系统提供调额方案，请检查");
                }
                else
                {
                    if (limitList != null && limitList.Where(l => l.ApplyLimit*10000.00M == applyLimit && l.ApplyRates == applyRates).Count() > 0)
                    {
                        return ApplyRule(apply, applyLimit, applyRates);
                    }
                    else
                    {
                        AppendError("ApplyLimitRates", "您提交了非系统提供调额方案，请检查");
                    }
                }
            }
            else
            {
                AppendError("ApplyLimitRates", "请您先选择调额方案");
            }
            return View("LimitProgramme", apply);
        }
        private ActionResult ApplyRule(FunongbaoApply apply, decimal applyLimit, decimal applyRates)
        {
            apply.ApplyDate = DateTime.Now;
            apply.ApplyLimit = applyLimit;
            apply.ApplyRates = applyRates;
            apply.ApplyStatus = 0;//默认为不能进行审批
            IList<Funongbao> funongbaoGroup = _funongbaoService.GetGroupByNo(CurrentFunongbao.GroupNO);

            Guid[] funongbaoIds = funongbaoGroup.Select(f => f.Id).ToArray();
            IList<FunongbaoApply> funongbaoApplyGroup = _funongbaoApplyService.GetByFunongbaoIds(funongbaoIds);
            //计算出当前组的总额度：--？此处疑问需再提问一次［1.如果原本有申请了50万，总额度］？
            decimal GropuLimitCount = funongbaoGroup.Sum(f => f.CurrentLimit);

           

            #region 升级额度情况
            if (applyLimit >= GropuLimitCount)
            {
                //if ((applyLimit > LimitRule.SingleLimitMax && funongbaoApplyGroup.Count <= 1) || (applyLimit > LimitRule.SingleLimitMax * 2 && funongbaoApplyGroup.Count <= 2))
                //{
                //    throw new OceanException("尊敬的"+this.Name+this.Sex+"，您好！您申请的额度已经超过组的最大额度，请联系客户经理再开一张卡。");
                //}
                //else
                //{
                    //获取满足条件的申请
                    IList<FunongbaoApply> replationApplys = funongbaoApplyGroup.Where(f => f.ApplyLimit == apply.ApplyLimit && f.FunongbaoId != apply.FunongbaoId).ToList();
                    //申请与现有的差值<当前申请卡的剩余额度-只需要当前申请-
                    if (applyLimit - GropuLimitCount <= LimitRule.SingleLimitMax - CurrentFunongbao.CurrentLimit)
                    {
                        apply.ChangedLimit = CurrentFunongbao.CurrentLimit + (applyLimit - GropuLimitCount);
                        apply.ChangedRates = apply.ApplyRates;
                        apply.ApplyStatus = 1;
                        //_funongbaoApplyService.Update(apply);
                        _funongbaoApplyService.UpdateApllysNew(new FunongbaoApply[] { apply }, funongbaoApplyGroup.Where(fg => fg.Id != apply.Id));
                    }
                    //申请与现有的差值>当前申请卡的剩余额度-需要至少两张卡同时申请
                    else
                    {
                        //必须属性家庭组和申请必须大于1个
                        if (funongbaoGroup.Count > 1 && funongbaoApplyGroup.Count > 0 && replationApplys != null && replationApplys.Count() > 0)
                        {
                            //计算第二张卡的额度-最小值的那张
                            FunongbaoApply replationApply = replationApplys.Where(fa => funongbaoGroup.OrderBy(f => f.CurrentLimit).Where(f => fa.FunongbaoId == f.Id).Count() > 0).First();
                            Funongbao replationFnb = funongbaoGroup.Where(f => f.Id == replationApply.FunongbaoId).First();
                            //当前卡
                            decimal ChangeLimit = replationFnb.CurrentLimit + (applyLimit - GropuLimitCount - (LimitRule.SingleLimitMax - CurrentFunongbao.CurrentLimit));
                            apply.ChangedLimit = ChangeLimit > LimitRule.SingleLimitMax ? LimitRule.SingleLimitMax : ChangeLimit;
                            apply.ChangedRates = apply.ApplyRates;

                            if (ChangeLimit > LimitRule.SingleLimitMax)//判断是否大最大限额
                            {
                                //需要三张卡同时申请
                                if (funongbaoGroup.Count > 2 && funongbaoApplyGroup.Count > 1 && replationApplys.Count() > 1)
                                {
                                    //当前卡
                                    apply.ChangedLimit = (applyLimit - LimitRule.SingleLimitMax * 2);
                                    apply.ChangedRates = apply.ApplyRates;
                                    apply.ApplyStatus = 1;
                                    //第二张卡
                                    replationApply.ApplyStatus = 1;
                                    //第三张卡｜第三个申请
                                    FunongbaoApply replationApply1 = replationApplys.Where(fa => funongbaoGroup.OrderBy(f => f.CurrentLimit).Where(f => fa.FunongbaoId == f.Id && fa.FunongbaoId != replationApply.FunongbaoId).Count() > 0).First();
                                    Funongbao replationFnb1 = funongbaoGroup.Where(f => f.Id == replationApply1.FunongbaoId).First();
                                    replationApply1.ApplyStatus = 1;
                                    //放入更新事务：
                                    _funongbaoApplyService.UpdateApllysNew(new FunongbaoApply[] { apply, replationApply, replationApply1 }, null);
                                }
                                else
                                {
                                    if (funongbaoApplyGroup.Count > 1)
                                    {
                                        //判断有几个需的提醒的福农宝名称
                                        ViewBag.RelationNames = GetNoticeApplyNames(funongbaoGroup, apply, funongbaoApplyGroup);
                                        _funongbaoApplyService.Update(apply);
                                        ViewBag.ApplyStatus = 2;
                                        return View("ApplyPost", apply);
                                    }
                                    else if (funongbaoApplyGroup.Count > 0)
                                    {
                                        //判断有几个需的提醒的福农宝名称
                                        _funongbaoApplyService.Update(apply);
                                        ViewBag.RelationNames = GetNoticeApplyNames(funongbaoGroup, apply, funongbaoIds);
                                        if (ViewBag.RelationNames == "-1")
                                        {
                                            ViewBag.ApplyStatus = -1;
                                        }
                                        else
                                        {
                                            ViewBag.ApplyStatus = 2;
                                        }
                                        return View("ApplyPost", apply);

                                    }
                                    else
                                    {
                                        apply.ChangedLimit = LimitRule.SingleLimitMax;
                                        apply.ChangedRates = applyRates;
                                        _funongbaoApplyService.Update(apply);
                                        ViewBag.ApplyStatus = -1;
                                        return View("ApplyPost", apply);
                                        //异常
                                        //throw new OceanException("尊敬的" + this.Name + this.Sex + "，您好！您申请的额度已经超过组的最大额度，请联系客户经理再开一张卡。");
                                    }
                                }
                            }
                            else
                            {
                                //当前卡
                                apply.ApplyStatus = 1;
                                //第二张卡
                                replationApply.ApplyStatus = 1;
                                //放入更新事务：
                                _funongbaoApplyService.UpdateApllysNew(new FunongbaoApply[] { apply, replationApply }, funongbaoApplyGroup.Where(fg => fg.Id != apply.Id && fg.Id != replationApply.Id));
                            }

                        }
                        else
                        {
                            if (funongbaoApplyGroup.Count > 1)
                            {
                                //判断有几个需的提醒的福农宝名称
                                apply.ChangedLimit = LimitRule.SingleLimitMax;
                                apply.ChangedRates = applyRates;
                                _funongbaoApplyService.Update(apply);
                                ViewBag.RelationNames = GetNoticeApplyNames(funongbaoGroup, apply, funongbaoIds);
                                if (ViewBag.RelationNames == "-1")
                                {
                                    ViewBag.ApplyStatus = -1;
                                }
                                else
                                {
                                    ViewBag.ApplyStatus = 2;
                                }
                                return View("ApplyPost", apply);
                            }
                            else
                            {
                                apply.ChangedLimit = LimitRule.SingleLimitMax;
                                apply.ChangedRates = applyRates;
                                _funongbaoApplyService.Update(apply);
                                ViewBag.ApplyStatus = -1;
                                return View("ApplyPost", apply);
                                //异常
                                //throw new OceanException("尊敬的" + this.Name + this.Sex + "，您好！您申请的额度已经超过组的最大额度，请联系客户经理再开一张卡。");
                            }
                        }
                    }
                //}
            }
            #endregion

            #region 降额情况
            else
            {
                //if (((CurrentFunongbao.CurrentLimit - (GropuLimitCount - applyLimit)) < 0 && funongbaoApplyGroup.Count <= 1))
                //{
                //    throw new OceanException("对不起，您申请的额度超出了限额，请联系客户经理联系（再开一张卡！)");
                //}
                //else
                //{
                    apply.ApplyStatus = -1;
                    //获取满足条件的申请
                    IList<FunongbaoApply> replationApplys = funongbaoApplyGroup.Where(f => f.ApplyLimit == apply.ApplyLimit && f.FunongbaoId != apply.FunongbaoId).ToList();
                    //申请与现有的差值<当前申请卡的剩余额度-只需要当前申请-
                    if (CurrentFunongbao.CurrentLimit - (GropuLimitCount - applyLimit) >= 0)
                    {
                        apply.ChangedLimit = CurrentFunongbao.CurrentLimit - (GropuLimitCount - applyLimit);
                        apply.ChangedRates = apply.ApplyRates;
                        apply.ApplyStatus = 1;
                        //_funongbaoApplyService.Update(apply);
                        _funongbaoApplyService.UpdateApllysNew(new FunongbaoApply[] { apply }, funongbaoApplyGroup.Where(fg => fg.Id != apply.Id));

                    }
                    //申请与现有的差值>当前申请卡的剩余额度-需要至少两张卡同时申请
                    else
                    {
                        //必须属性家庭组和申请必须大于1个
                        if (funongbaoGroup.Count > 1 && funongbaoApplyGroup.Count > 0 && replationApplys != null && replationApplys.Count() > 0)
                        {
                            //计算第二张卡的额度-最小值的那张
                            FunongbaoApply replationApply = replationApplys.Where(fa => funongbaoGroup.OrderBy(f => f.CurrentLimit).Where(f => fa.FunongbaoId == f.Id).Count() > 0).First();
                            Funongbao replationFnb = funongbaoGroup.Where(f => f.Id == replationApply.FunongbaoId).First();
                            //当前卡
                            decimal ChangeLimit = CurrentFunongbao.CurrentLimit - ((GropuLimitCount - applyLimit) - replationFnb.CurrentLimit);
                            apply.ChangedLimit = (ChangeLimit < 0.00M ? 0.00M : ChangeLimit);
                            apply.ChangedRates = apply.ApplyRates;

                            if (ChangeLimit < 0)
                            {
                                //需要三张卡同时申请
                                if (funongbaoGroup.Count > 2 && funongbaoApplyGroup.Count > 1 && replationApplys.Count() > 1)
                                {
                                    //第三张卡｜第三个申请
                                    FunongbaoApply replationApply1 = replationApplys.Where(fa => funongbaoGroup.OrderBy(f => f.CurrentLimit).Where(f => fa.FunongbaoId == f.Id && fa.FunongbaoId != replationApply.FunongbaoId).Count() > 0).First();
                                    Funongbao replationFnb1 = funongbaoGroup.Where(f => f.Id == replationApply1.FunongbaoId).First();
                                    //当前卡
                                    apply.ChangedLimit = (CurrentFunongbao.CurrentLimit - ((GropuLimitCount - applyLimit) - replationFnb1.CurrentLimit - replationFnb.CurrentLimit));
                                    apply.ChangedRates = apply.ApplyRates;
                                    apply.ApplyStatus = 1;
                                    //第二张卡
                                    replationApply.ApplyStatus = 1;
                                    //第三张卡
                                    replationApply1.ApplyStatus = 1;
                                    //放入更新事务：
                                    _funongbaoApplyService.UpdateApllysNew(new FunongbaoApply[] { apply, replationApply, replationApply1 }, null);
                                }
                                else
                                {
                                    if (funongbaoApplyGroup.Count > 1)
                                    {
                                        //判断有几个需的提醒的福农宝名称
                                        ViewBag.RelationNames = GetNoticeApplyNames(funongbaoGroup, apply, funongbaoApplyGroup);
                                        _funongbaoApplyService.Update(apply);
                                        ViewBag.ApplyStatus = 3;
                                        return View("ApplyPost", apply);
                                    }
                                    else if (funongbaoApplyGroup.Count > 0)
                                    {
                                        //判断有几个需的提醒的福农宝名称
                                        _funongbaoApplyService.Update(apply);
                                        ViewBag.RelationNames = GetNoticeApplyNames(funongbaoGroup, apply, funongbaoIds);
                                        if (ViewBag.RelationNames == "-1")
                                        {
                                            ViewBag.ApplyStatus = -1;
                                        }
                                        else
                                        {
                                            ViewBag.ApplyStatus = 3;
                                        }
                                        return View("ApplyPost", apply);

                                    }
                                    else
                                    {
                                        apply.ChangedLimit = 0.00M;
                                        apply.ChangedRates = applyRates;
                                        _funongbaoApplyService.Update(apply);
                                        ViewBag.ApplyStatus = -1;
                                        return View("ApplyPost", apply);
                                        //异常
                                        //throw new OceanException("尊敬的" + this.Name + this.Sex + "，您好！您申请的额度已经超过组的最大额度，请联系客户经理再开一张卡。");
                                    }
                                }
                            }
                            else
                            {
                                //当前卡
                                apply.ApplyStatus = 1;
                                //第二张卡
                                replationApply.ApplyStatus = 1;
                                //放入更新事务：
                                _funongbaoApplyService.UpdateApllysNew(new FunongbaoApply[] { apply, replationApply }, funongbaoApplyGroup.Where(fg => fg.Id != apply.Id && fg.Id != replationApply.Id));
                            }
                        }
                        else
                        {
                            if (funongbaoApplyGroup.Count > 1)
                            {
                                //判断有几个需的提醒的福农宝名称
                                apply.ChangedLimit = 0.00M;
                                apply.ChangedRates = applyRates;
                                _funongbaoApplyService.Update(apply);
                                ViewBag.RelationNames = GetNoticeApplyNames(funongbaoGroup, apply, funongbaoIds);
                                if (ViewBag.RelationNames == "-1")
                                {
                                    ViewBag.ApplyStatus = -1;
                                }
                                else
                                {
                                    ViewBag.ApplyStatus = 3;
                                }
                                return View("ApplyPost", apply);
                            }
                            else
                            {
                                apply.ChangedLimit = 0.00M;
                                apply.ChangedRates = applyRates;
                                _funongbaoApplyService.Update(apply);
                                ViewBag.ApplyStatus = -1;
                                return View("ApplyPost", apply);
                                //异常
                                //throw new OceanException("尊敬的" + this.Name + this.Sex + "，您好！您申请的额度已经超过组的最大额度，请联系客户经理再开一张卡。");
                            }
                        }
                    }
                //}
            }
            #endregion
            //申请成功
            ViewBag.ApplyStatus = 1;
            return View("ApplyPost", apply);
        }

        [HttpPost]
        public ActionResult ApplyDaied()
        {

            return JsonMessage(true);
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
            string[] rnames = funongbaoGroup.Where(f => funongbaoIds.Contains(f.Id) && f.Id != apply.FunongbaoId).Select(f => f.Name).ToArray();
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
                return "-1";
                //throw new OceanException("尊敬的" + this.Name + this.Sex + "，您好！您申请的额度已经超过组的最大额度，请联系客户经理再开一张卡。");
            }
        }
        /// <summary>
        /// 获取服农宝申请者关联福农宝名称
        /// </summary>
        /// <param name="funongbaoGroup">福农宝组</param>
        /// <param name="apply">当前申请者</param>
        /// <param name="applys">已申请的关联福农宝</param>
        /// <returns></returns>
        private string GetNoticeApplyNames(IList<Funongbao> funongbaoGroup, FunongbaoApply apply, IEnumerable<FunongbaoApply> applys)
        {
            string[] rnames = funongbaoGroup.Where(f => (apply.FunongbaoId != f.Id && applys.Where(fa => fa.FunongbaoId == f.Id && fa.Id != apply.Id && fa.ApplyDate == new DateTime(1900,1,1)).Count() > 0)).Select(f => f.Name).ToArray();
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
                return "-1";
                //throw new OceanException("对不起，您申请的额度超出了限额，请联系客户经理联系（再开一张卡！)");
            }
        }
        #endregion

        #region 调额申请-2014.4.14
        public ActionResult Apply()
        {
            FunongbaoApply apply = _funongbaoApplyService.GetByFunongbaoId(CurrentFunongbao.Id);
            if (apply != null && apply.ApplyDate > new DateTime(1900, 1, 1))//判断当季度是否已申请
            {
                int applyMonth = apply.ApplyDate.Month;
                int applyDay = apply.ApplyDate.Day;
                DateRule dateRule = DateRuleList.DateRules.Where(d => d.Months.Where(m => m == applyMonth).Count() > 0).First();
                if (apply.ApplyDate > new DateTime(apply.ApplyDate.Year, apply.ApplyDate.Month, DateRule.ApplyMiddleDay)) //判断当月或者次月
                {
                    ViewBag.ApplyMonth = applyMonth + 1;
                }
                else
                {
                    ViewBag.ApplyMonth = applyMonth;
                }
                if (apply.ApplyType > 0)
                {
                    ViewBag.ApplyStatus = 1;
                }
                else
                {
                    ViewBag.ApplyStatus = 2;
                }
                ViewBag.ChangeDay = DateRule.ChangeDay;
                ViewBag.ApplyType = apply.ApplyType;
                return View("Applied", apply);
            }
            else
            {
                IList<LimitProgrammeDTO> limitList = null;
                if (apply != null)
                {
                    limitList = _funongbaoApplyService.GetLimitProgramme(apply.LimitProgramme);
                    if (limitList != null && limitList.Count == 1)
                    {
                        ViewBag.LimitProgrammeList = limitList;
                    }
                    else
                    {
                        //FunongbaoApply groupApply = _funongbaoApplyService.ExistGroupApply(apply.RFunongbao.GroupNO);
                        //if (groupApply != null && limitList != null)
                        //{
                        //    IEnumerable<LimitProgrammeDTO> groupLimits = limitList.Where(l => l.ApplyLimit == groupApply.ApplyLimit / 10000 && l.ApplyRates == groupApply.ApplyRates);
                        //    if (groupLimits.Count() > 0)
                        //    {
                        //        ViewBag.LimitProgrammeList = groupLimits.ToList();
                        //    }
                        //    else
                        //    {
                        //        ViewBag.LimitProgrammeList = null;
                        //    }
                        //}
                        //else
                        //{
                        ViewBag.LimitProgrammeList = limitList;
                        //}
                    }
                }
                if (WebHelper.IsPost())
                {
                    return ApplyPost(apply, limitList);
                }

                return View("LimitProgramme", apply);
            }
        }

        private ActionResult ApplyPost(FunongbaoApply apply, IList<LimitProgrammeDTO> limitList)
        {
            string applyLimitRates = WebHelper.GetString("ApplyLimitRates");
            if (!string.IsNullOrEmpty(applyLimitRates) && (applyLimitRates == "1" || applyLimitRates == "2"))
            {
                ViewBag.ApplyType = TypeConverter.StrToInt(applyLimitRates);
                if (applyLimitRates == "1")
                {
                    if(limitList.Where(l=>(l.ApplyLimit*10000.00m)>apply.PreLimit).Count()==0)
                    {
                        ViewBag.ApplyStatus = 2;
                        return View("ApplyPost", apply);
                    }
                }
                else
                {
                    if (limitList.Where(l => l.ApplyRates < apply.PreRates).Count() == 0)
                    {
                        ViewBag.ApplyStatus = 2;
                        return View("ApplyPost", apply);
                    }
                }

                return ApplyRule(apply, TypeConverter.StrToInt(applyLimitRates));
            }
            else
            {
                AppendError("ApplyLimitRates", "请您先选择调额方案");
            }
            return View("LimitProgramme", apply);
        }

        private ActionResult ApplyRule(FunongbaoApply apply, int applyType)
        {
            apply.ApplyDate = DateTime.Now;
            apply.ApplyStatus = 0;//默认为不能进行审批
            IList<Funongbao> funongbaoGroup = _funongbaoService.GetGroupByNo(CurrentFunongbao.GroupNO);

            Guid[] funongbaoIds = funongbaoGroup.Select(f => f.Id).ToArray();
            IList<FunongbaoApply> funongbaoApplyGroup = _funongbaoApplyService.GetByFunongbaoIds(funongbaoIds);
            //计算出当前组的总额度：--？此处疑问需再提问一次［1.如果原本有申请了50万，总额度］？
            decimal GropuLimitCount = funongbaoGroup.Sum(f => f.CurrentLimit);

            #region 升级额度情况
            if (applyType == 1)
            {
                apply.ApplyStatus = 1;
                apply.ApplyType = 1;
                _funongbaoApplyService.UpdateApllysNew(new FunongbaoApply[] { apply }, null);
            }
            #endregion

            #region 降额情况
            else
            {
                apply.ApplyStatus = 1;
                apply.ApplyType = 2;
                _funongbaoApplyService.UpdateApllysNew(new FunongbaoApply[] { apply }, null);
            }
            #endregion
            //申请成功
            ViewBag.ApplyStatus = 1;
            return View("ApplyPost", apply);
        }
        #endregion
    }
}
