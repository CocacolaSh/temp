using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Page;
using Ocean.Core;
using Ocean.Entity.Enums;
using Ocean.Entity.DTO;
using Ocean.Framework.Caching.Cache;
using Ocean.Core.Utility;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.HSSF.UserModel;
using Newtonsoft.Json;
using Ocean.Entity.Enums.Loan;
using Ocean.Entity.Enums.AdminLoggerModule;
using Ocean.Entity;
using Ocean.Entity.Enums.TypeIdentifying;
using Ocean.Services;

namespace Ocean.Admin.Controllers
{
    public class LoanController : AdminBaseController
    {
        private readonly ILoanService _loanService;
        private readonly ILoanAssignLoggerService _loanAssignLoggerService;

        public LoanController(ILoanService loanService, ILoanAssignLoggerService loanAssignLoggerService)
        {
            this._loanService = loanService;
            this._loanAssignLoggerService = loanAssignLoggerService;
        }

        /// <summary>
        /// 初始化贷款列表页面
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            if (!base.HasPermission("loan", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            ViewBag.Status = RQuery["Status"];
            ViewBag.ProcessStatus = RQuery["ProcessStatus"];
            ViewBag.MpUserId = RQuery["MpUserId"];
            //贷款种类
            ViewBag.ListLoanCategory = GetEnumDataSource(TypeIdentifyingEnum.LoanCategory, true, string.Empty);
            //贷款期限
            ViewBag.ListLoanDeadline = GetEnumDataSource(TypeIdentifyingEnum.LoanDeadline, true, string.Empty);
            return View();
        }

        /// <summary>
        /// 初始化贷款明细页面
        /// </summary>
        [HttpGet]
        public ActionResult LoanView(Guid id)
        {
            if (!base.HasPermission("loan", PermissionOperate.view))
            {
                return base.ShowNotPermissionTip("");
            }

            return View();
        }

        /// <summary>
        /// 获取贷款明细
        /// </summary>
        [HttpPost]
        [ActionName("_GetLoanDetail")]
        public ActionResult GetLoanDetail(Guid id)
        {
            if (!base.HasPermission("loan", PermissionOperate.track))
            {
                return null;
            }

            Loan loan = _loanService.GetById(id);
            return Content(JsonConvert.SerializeObject(loan));
        }

        /// <summary>
        /// 导出工单
        /// </summary>
        [HttpGet]
        public void ExportExcel(Guid id)
        {
            if (!base.HasPermission("loan", PermissionOperate.export))
            {
                return;
            }

            Loan loan = _loanService.GetById(id);
            string fileName = string.Format("loan{0}.xls", id.ToString().ToLower());
            MemoryStream ms = RenderToExcel(loan);
            
            if (Request.Browser.Browser == "IE")
            {
                fileName = HttpUtility.UrlEncode(fileName);
            }

            base.AddLog(string.Format("导出工单[{0}]成功", loan.MpUser.Name), AdminLoggerModuleEnum.Loan);
            Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileName);
            Response.BinaryWrite(ms.ToArray());
        }

        /// <summary>
        /// 把数据渲染到Excel
        /// </summary>
        private MemoryStream RenderToExcel(Loan loan)
        {
            string assetSituation = string.Empty;

            //if (!string.IsNullOrWhiteSpace(loan.AssetSituation))
            //{
            //    foreach (string value in loan.AssetSituation.Split(','))
            //    {
            //        if (!string.IsNullOrWhiteSpace(value) && Validator.IsNumber(value))
            //        {
            //            assetSituation += EnumDataCache.Instance.GetEnumDataName(TypeIdentifyingEnum.AssetSituation, value) + ",";
            //        }
            //    }
            //}

            MemoryStream ms = new MemoryStream();
            IWorkbook workbook = null;
            ISheet sheet = null;
            
            try
            {
                if (loan.LoanCategoryId == 4)
                {
                    using (FileStream fileStream = System.IO.File.OpenRead(FileHelper.GetMapPath("/Content/ExcelTemplate/GongzhiBaoTemplate.xls")))   //打开xls文件
                    {
                        workbook = new HSSFWorkbook(fileStream);
                        sheet = workbook.GetSheetAt(0);
                        sheet.GetRow(1).GetCell(1).SetCellValue(loan.MpUser.Name);
                        sheet.GetRow(1).GetCell(3).SetCellValue(loan.MpUser.MobilePhone);
                        sheet.GetRow(1).GetCell(5).SetCellValue(loan.Company);
                        sheet.GetRow(2).GetCell(1).SetCellValue(loan.ApplyMoney.ToString("0.00") + "(万元)");
                        sheet.GetRow(2).GetCell(3).SetCellValue(EnumDataCache.Instance.GetEnumDataName(TypeIdentifyingEnum.LoanCategory, loan.LoanCategoryId.ToString()));
                        sheet.GetRow(2).GetCell(5).SetCellValue(loan.Position);
                        //sheet.GetRow(3).GetCell(1).SetCellValue(EnumDataCache.Instance.GetEnumDataName(TypeIdentifyingEnum.CompanyType, loan.LoanCompanyId.ToString()));
                        //sheet.GetRow(3).GetCell(3).SetCellValue(EnumDataCache.Instance.GetEnumDataName(TypeIdentifyingEnum.FormalType, loan.LoanFormalId.ToString()));
                        //sheet.GetRow(3).GetCell(5).SetCellValue(EnumDataCache.Instance.GetEnumDataName(TypeIdentifyingEnum.AssetHourse, loan.LoanAssetHourseId.ToString()));
                        sheet.GetRow(4).GetCell(1).SetCellValue(EnumDataCache.Instance.GetEnumDataName(TypeIdentifyingEnum.RepaymentMode, loan.RepaymentModeId.ToString()));
                        //sheet.GetRow(4).GetCell(3).SetCellValue(EnumDataCache.Instance.GetEnumDataName(TypeIdentifyingEnum.GuaranteeWay, loan.GuaranteeWayId.ToString()));
                        sheet.GetRow(4).GetCell(5).SetCellValue(assetSituation.Trim(','));
                        sheet.GetRow(5).GetCell(1).SetCellValue(base.LoginAdmin.PermissionOrganization.Name);
                        sheet.GetRow(5).GetCell(3).SetCellValue(DateTime.Now.ToString("yyyy-MM-dd"));
                        sheet.GetRow(6).GetCell(1).SetCellValue(loan.AssignSubbranch);
                        sheet.GetRow(6).GetCell(3).SetCellValue(loan.AssignCustomerManager);
                        sheet.GetRow(8).GetCell(0).SetCellValue(string.Format("请于{0}之前反馈处理结果", loan.CreateDate.AddDays(5).ToString("yyyy-MM-dd")));
                        workbook.Write(ms);
                        ms.Flush();
                        ms.Position = 0;
                    }
                }
                else
                {
                    using (FileStream fileStream = System.IO.File.OpenRead(FileHelper.GetMapPath("/Content/ExcelTemplate/LoanTemplate.xls")))   //打开xls文件
                    {
                        workbook = new HSSFWorkbook(fileStream);
                        sheet = workbook.GetSheetAt(0);
                        sheet.GetRow(1).GetCell(1).SetCellValue(loan.MpUser.Name);
                        sheet.GetRow(1).GetCell(3).SetCellValue(loan.MpUser.MobilePhone);
                        sheet.GetRow(1).GetCell(5).SetCellValue(loan.Address);
                        sheet.GetRow(2).GetCell(1).SetCellValue(loan.ApplyMoney.ToString("0.00") + "(万元)");
                        sheet.GetRow(2).GetCell(3).SetCellValue(EnumDataCache.Instance.GetEnumDataName(TypeIdentifyingEnum.LoanCategory, loan.LoanCategoryId.ToString()));
                        sheet.GetRow(2).GetCell(5).SetCellValue(EnumDataCache.Instance.GetEnumDataName(TypeIdentifyingEnum.LoanDeadline, loan.DeadlineId.ToString()));
                        sheet.GetRow(3).GetCell(1).SetCellValue(EnumDataCache.Instance.GetEnumDataName(TypeIdentifyingEnum.RepaymentMode, loan.RepaymentModeId.ToString()));
                        //sheet.GetRow(3).GetCell(3).SetCellValue(EnumDataCache.Instance.GetEnumDataName(TypeIdentifyingEnum.GuaranteeWay, loan.GuaranteeWayId.ToString()));
                        sheet.GetRow(3).GetCell(5).SetCellValue(assetSituation.Trim(','));
                        sheet.GetRow(4).GetCell(1).SetCellValue(base.LoginAdmin.PermissionOrganization.Name);
                        sheet.GetRow(4).GetCell(3).SetCellValue(DateTime.Now.ToString("yyyy-MM-dd"));
                        sheet.GetRow(5).GetCell(1).SetCellValue(loan.AssignSubbranch);
                        sheet.GetRow(5).GetCell(3).SetCellValue(loan.AssignCustomerManager);
                        sheet.GetRow(7).GetCell(0).SetCellValue(string.Format("请于{0}之前反馈处理结果", loan.CreateDate.AddDays(5).ToString("yyyy-MM-dd")));
                        workbook.Write(ms);
                        ms.Flush();
                        ms.Position = 0;
                    }
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

        /// <summary>
        /// 分配客户经理
        /// </summary>
        [HttpPost]
        [ActionName("_AllotEdit")]
        public ActionResult AllotEditProvide(Guid id)
        {
            if (!base.HasPermission("loan", PermissionOperate.track))
            {
                return JsonMessage(false, "你没有分配客户经理的权限");
            }

            Loan loan = _loanService.GetById(id);

            if (loan.Status == 1)
            {
                return JsonMessage(false, "该笔工单已被撤销，不能继续操作！");
            }

            if (loan.ProcessStatus == 1 || loan.ProcessStatus == 2)
            {
                return JsonMessage(false, "该笔工单已被受理，不能继续操作！");
            }

            UpdateModel<Loan>(loan);
            loan.AssignStatus = 1;
            _loanService.Update(loan);
            ////增加客户经理分配记录（修改日记）
            //LoanAssignLogger loanAssignLogger = new LoanAssignLogger();
            //UpdateModel<LoanAssignLogger>(loanAssignLogger);
            //loanAssignLogger.LoanId = loan.Id;
            //loanAssignLogger.ModifyName = base.LoginAdmin.Name;
            //loanAssignLogger.ModifyDate = DateTime.Now;
            //_loanAssignLoggerService.Insert(loanAssignLogger);
            base.AddLog(string.Format("分配客户经理[{0}]成功", loan.MpUser.Name + "To" + loan.AssignCustomerManager), AdminLoggerModuleEnum.Loan);
            return JsonMessage(true, "处理成功");
        }

        /// <summary>
        /// 修改联系地址
        /// </summary>
        [HttpPost]
        [ActionName("_AddressEdit")]
        public ActionResult AddressEditProvide(Guid id)
        {
            Loan loan = _loanService.GetById(id);
            UpdateModel<Loan>(loan);
            _loanService.Update(loan);
            base.AddLog(string.Format("修改联系地址[{0}]成功", loan.Id.ToString()), AdminLoggerModuleEnum.Loan);
            return JsonMessage(true, "修改成功");
        }

        /// <summary>
        /// 受理贷款业务
        /// </summary>
        [HttpPost]
        [ActionName("_AcceptEdit")]
        public ActionResult AcceptEditProvide(Guid id)
        {
            if (!base.HasPermission("loan", PermissionOperate.track))
            {
                return JsonMessage(false, "你没有受理情况登记的权限");
            }

            Loan loan = _loanService.GetById(id);

            if (loan.Status == 1)
            {
                return JsonMessage(false, "该笔工单已被撤销，不能继续操作！");
            }

            if (loan.ProcessStatus == 1 || loan.ProcessStatus == 2)
            {
                return JsonMessage(false, "该笔工单已被受理，不能继续操作！");
            }

            UpdateModel<Loan>(loan);
            _loanService.Update(loan);
            string processStatus = string.Empty;
            if (loan.ProcessStatus == 1)
            {
                processStatus = "通过";
            }
            else if (loan.ProcessStatus == 2)
            {
                processStatus = "不通过";
            }
            else if (loan.ProcessStatus == 3)
            {
                processStatus = "受理中";
            }
            base.AddLog(string.Format("受理贷款业务[{0}]成功,受理状态变更为:{1}", loan.MpUser.Name, processStatus), AdminLoggerModuleEnum.Loan);
            return JsonMessage(true, "处理成功");
        }

        /// <summary>
        /// 回访贷款业务
        /// </summary>
        [HttpPost]
        [ActionName("_ReturnVisitEdit")]
        public ActionResult ReturnVisitEditProvide(Guid id)
        {
            if (!base.HasPermission("loan", PermissionOperate.track))
            {
                return JsonMessage(false, "你没有回访结果登记的权限");
            }

            Loan loan = _loanService.GetById(id);

            if (loan.Status == 1)
            {
                return JsonMessage(false, "该笔工单已被撤销，不能继续操作！");
            }

            if (loan.ReturnVisitDate.HasValue)
            {
                return JsonMessage(false, "该笔工单已被回访，不能继续操作！");
            }

            UpdateModel<Loan>(loan);
            _loanService.Update(loan);
            base.AddLog(string.Format("回访贷款业务[{0}]成功", loan.MpUser.Name), AdminLoggerModuleEnum.Loan);
            return JsonMessage(true, "处理成功");
        }

        /// <summary>
        /// 取消申请（删除）
        /// </summary>
        [HttpPost]
        [ActionName("_Cancel")]
        public ActionResult Cancel(Guid id)
        {
            if (!base.HasPermission("loan", PermissionOperate.freeze))
            {
                return JsonMessage(false, "你没有取消贷款申请的权限");
            }

            Loan loan = _loanService.GetById(id);
            string name = loan.MpUser.Name;
            //后台已进行相关处理,不能进行编辑操作
            _loanService.Delete(loan);
            base.AddLog(string.Format("取消贷款申请[{0}]成功", name), AdminLoggerModuleEnum.Loan);
            return JsonMessage(true, "取消贷款申请成功");
        }
    }
}