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
using System.Data;
using Ocean.Core.Npoi;

namespace Ocean.Admin.Controllers
{
    public class PosController : AdminBaseController
    {
        private readonly IPosService _posService;
        private readonly IPosApplyService _posApplyService;
        private readonly IMpUserService _mpUserService;
        private readonly IPosAuthService _posAuthService;
        public PosController(IPosService posService, IPosApplyService posApplyService, IMpUserService mpUserService, IPosAuthService posAuthService)
        {
            this._posService = posService;
            this._posApplyService = posApplyService;
            _mpUserService = mpUserService;
            _posAuthService = posAuthService;
        }


        #region 申请
        /// <summary>
        /// 初始化POS申请列表页面
        /// </summary>
        [HttpGet]
        public ActionResult ApplyList()
        {
            if (!base.HasPermission("PosApply", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            ViewBag.Status = RQuery["Status"];
            ViewBag.ProcessStatus = RQuery["ProcessStatus"];
            ViewBag.ProcessStatus = RQuery["ProcessStatus"];
            return View();
        }

        /// <summary>
        /// 获取POS申请列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_PosApplyList")]
        public ActionResult PosListProvide(PosApply posApplyDTO)
        {
            if (!base.HasPermission("PosApply", PermissionOperate.manager))
            {
                return null;
            }

            OceanDynamicList<object> list = _posApplyService.GetPageList(PageIndex, PageSize, posApplyDTO)??new OceanDynamicList<object>();

            return Content(list.ToJson(), "text/javascript");
        }

        /// <summary>
        /// 初始化POS申请明细页面
        /// </summary>
        [HttpGet]
        public ActionResult PosApplyView(Guid id)
        {
            if (!base.HasPermission("PosApply", PermissionOperate.view))
            {
                return base.ShowNotPermissionTip("");
            }
            PosApply apply = _posApplyService.GetById(id);
            PosAuth posAuth = _posAuthService.GetUnique(p => p.MpUserId == apply.MpUserId);
            ViewBag.PosAuth = posAuth;
            return View(apply);
        }

        /// <summary>
        /// 获取POS申请明细
        /// </summary>
        [HttpPost]
        [ActionName("_GetPosApplyDetail")]
        public ActionResult GetPosApplyDetail(Guid id)
        {
            if (!base.HasPermission("PosApply", PermissionOperate.track))
            {
                return null;
            }

            PosApply posApply = _posApplyService.GetById(id);
            return Content(JsonConvert.SerializeObject(posApply));
        }

        /// <summary>
        /// 导出工单
        /// </summary>
        [HttpGet]
        public void ExportExcel(Guid id)
        {
            if (!base.HasPermission("PosApply", PermissionOperate.export))
            {
                return;
            }

            PosApply apply = _posApplyService.GetById(id);
            string fileName = string.Format("PosApply{0}.xls", id.ToString().ToLower());
            MemoryStream ms = RenderToExcel(apply);
            
            if (Request.Browser.Browser == "IE")
            {
                fileName = HttpUtility.UrlEncode(fileName);
            }

            base.AddLog(string.Format("导出POS申请工单[{0}]成功", apply.VendorName), AdminLoggerModuleEnum.Pos);
            Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileName);
            Response.BinaryWrite(ms.ToArray());
        }

        /// <summary>
        /// 把数据渲染到Excel
        /// </summary>
        private MemoryStream RenderToExcel(PosApply apply)
        {
            MemoryStream ms = new MemoryStream();
            IWorkbook workbook = null;
            ISheet sheet = null;
            MpUser mpUser = _mpUserService.GetById(apply.MpUserId);
            PosAuth posAuth = _posAuthService.GetUnique(p => p.MpUserId == mpUser.Id);
            try
            {
                using (FileStream fileStream = System.IO.File.OpenRead(FileHelper.GetMapPath("/Content/ExcelTemplate/PosTemplate.xls")))   //打开xls文件
                {
                    workbook = new HSSFWorkbook(fileStream);
                    sheet = workbook.GetSheetAt(0);
                    sheet.GetRow(1).GetCell(1).SetCellValue(posAuth.Name);
                    sheet.GetRow(1).GetCell(3).SetCellValue(posAuth.MobilePhone);
                    sheet.GetRow(1).GetCell(5).SetCellValue(apply.VendorAddress);
                    sheet.GetRow(2).GetCell(1).SetCellValue(base.LoginAdmin.PermissionOrganization.Name);
                    sheet.GetRow(2).GetCell(3).SetCellValue(DateTime.Now.ToString("yyyy-MM-dd"));
                    sheet.GetRow(3).GetCell(1).SetCellValue(apply.AssignSubbranch);
                    sheet.GetRow(3).GetCell(3).SetCellValue(apply.AssignCustomerManager);
                    sheet.GetRow(5).GetCell(0).SetCellValue(string.Format("请于{0}之前反馈处理结果", apply.CreateDate.AddDays(5).ToString("yyyy-MM-dd")));
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

        /// <summary>
        /// 分配客户经理
        /// </summary>
        [HttpPost]
        [ActionName("_AllotEdit")]
        public ActionResult AllotEditProvide(Guid id)
        {
            if (!base.HasPermission("PosApply", PermissionOperate.track))
            {
                return JsonMessage(false, "你没有分配客户经理的权限");
            }

            PosApply apply = _posApplyService.GetById(id);

            if (apply.Status == 1)
            {
                return JsonMessage(false, "该笔工单已被撤销，不能继续操作！");
            }

            if (apply.ProcessStatus == 1 || apply.ProcessStatus == 2)
            {
                return JsonMessage(false, "该笔工单已被受理，不能继续操作！");
            }

            UpdateModel<PosApply>(apply);
            apply.AssignStatus = 1;
            _posApplyService.Update(apply);;
            base.AddLog(string.Format("分配客户经理[{0}]成功", apply.VendorName + "To" + apply.AssignCustomerManager), AdminLoggerModuleEnum.Pos);
            return JsonMessage(true, "处理成功");
        }

        /// <summary>
        /// 修改联系地址
        /// </summary>
        [HttpPost]
        [ActionName("_AddressEdit")]
        public ActionResult AddressEditProvide(Guid id)
        {
            PosApply apply = _posApplyService.GetById(id);
            UpdateModel<PosApply>(apply);
            _posApplyService.Update(apply);
            base.AddLog(string.Format("修改联系地址[{0}]成功", apply.Id.ToString()), AdminLoggerModuleEnum.Pos);
            return JsonMessage(true, "修改成功");
        }

        /// <summary>
        /// 受理POS申请业务
        /// </summary>
        [HttpPost]
        [ActionName("_AcceptEdit")]
        public ActionResult AcceptEditProvide(Guid id)
        {
            if (!base.HasPermission("PosApply", PermissionOperate.track))
            {
                return JsonMessage(false, "你没有受理情况登记的权限");
            }

            PosApply apply = _posApplyService.GetById(id);

            if (apply.Status == 1)
            {
                return JsonMessage(false, "该笔工单已被撤销，不能继续操作！");
            }

            if (apply.ProcessStatus == 1 || apply.ProcessStatus == 2)
            {
                return JsonMessage(false, "该笔工单已被受理，不能继续操作！");
            }

            UpdateModel<PosApply>(apply);
            _posApplyService.Update(apply);
            string processStatus = string.Empty;
            if (apply.ProcessStatus == 1)
            {
                processStatus = "已装机";
            }
            else if (apply.ProcessStatus == 2)
            {
                processStatus = "不通过";
            }
            else if (apply.ProcessStatus == 3)
            {
                processStatus = "受理中";
            }
            base.AddLog(string.Format("受理POS申请业务[{0}]成功,受理状态变更为:{1}", apply.VendorName, processStatus), AdminLoggerModuleEnum.Pos);
            return JsonMessage(true, "处理成功");
        }

        /// <summary>
        /// 回访POS申请业务
        /// </summary>
        [HttpPost]
        [ActionName("_ReturnVisitEdit")]
        public ActionResult ReturnVisitEditProvide(Guid id)
        {
            if (!base.HasPermission("PosApply", PermissionOperate.track))
            {
                return JsonMessage(false, "你没有回访结果登记的权限");
            }

            PosApply apply = _posApplyService.GetById(id);

            if (apply.Status == 1)
            {
                return JsonMessage(false, "该笔工单已被撤销，不能继续操作！");
            }

            if (apply.ReturnVisitDate>new DateTime(1900,1,2))
            {
                return JsonMessage(false, "该笔工单已被回访，不能继续操作！");
            }

            UpdateModel<PosApply>(apply);
            _posApplyService.Update(apply);
            base.AddLog(string.Format("回访POS申请业务[{0}]成功", apply.VendorName), AdminLoggerModuleEnum.Pos);
            return JsonMessage(true, "处理成功");
        }

        /// <summary>
        /// 取消申请（删除）
        /// </summary>
        [HttpPost]
        [ActionName("_Cancel")]
        public ActionResult Cancel(Guid id)
        {
            if (!base.HasPermission("PosApply", PermissionOperate.freeze))
            {
                return JsonMessage(false, "你没有取消POS申请申请的权限");
            }

            PosApply apply = _posApplyService.GetById(id);
            //后台已进行相关处理,不能进行编辑操作
            base.AddLog(string.Format("取消POS申请申请[{0}]成功", apply.VendorName), AdminLoggerModuleEnum.Pos);
            _posApplyService.Delete(apply);
            return JsonMessage(true, "取消POS申请申请成功");
        }
        #endregion

        #region 存量

        /// <summary>
        /// 初始化福农宝列表页面
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            if (!base.HasPermission("Pos", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }
            return View();
        }
        /// <summary>
        /// 获取福农宝列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_PosList")]
        public ActionResult PosListProvide()
        {
            if (!base.HasPermission("Pos", PermissionOperate.manager))
            {
                return null;
            }

            Pos posDTO = new Pos() { };
            TryUpdateModel<Pos>(posDTO);
            OceanDynamicList<object> list = _posService.GetPageDynamicList(PageIndex, PageSize, posDTO);
           
            if (list != null)
            {
                return Content(list.ToJson(), "text/javascript");
            }
            else
            {
                return Content("{\"rows\":[],\"total\":0}", "text/javascript");
            }
        }
        #region 导入
        [HttpGet]
        [ActionName("_ImportPosExcel")]
        public ActionResult ImportPosExcel()
        {
            ViewBag.Method = "get";
            return View();
        }
        /// <summary>
        /// 导入福农宝客户
        /// </summary>
        [HttpPost]
        [ActionName("_ImportPosExcel")]
        public ActionResult ImportPosExcel(HttpPostedFileBase posExcel)
        {
            ViewBag.Method = "post";
            ViewBag.Success = true;
            int countPos = 0;
            int countPosUpdate = 0;
            if (posExcel != null)
            {
                DataTable dt = null;
                if (posExcel.FileName.ToLower().EndsWith(".xlsx"))
                {
                    dt = NPOIHelper.ExcelToTableForXLSX(posExcel.InputStream, "序号");
                }
                else
                {
                    dt = NPOIHelper.ExcelToTableForXLS(posExcel.InputStream, "序号", string.Empty);
                }
                var columns = dt.Columns;

                DateRule rule = DateRuleList.DateRules.Where(d => d.Months.Where(m => m == DateTime.Now.Month).Count() > 0).First();
                int month = rule.ApplyMonth;
                int year = DateTime.Now.Year;
                try
                {
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        Pos pos = new Pos();
                        pos.CreateDate = DateTime.Now;
                        pos.VendorName = dt.Rows[i]["商户名称"].ToString();
                        pos.VendorNO = dt.Rows[i]["商户号"].ToString();
                        pos.TerminalNO = dt.Rows[i]["终端号"].ToString();
                        pos.InstallAddress = dt.Rows[i]["装机地址"].ToString();
                        pos.DeductRate = dt.Rows[i]["扣率"].ToString();
                        pos.BankNO = dt.Rows[i]["银行卡号"].ToString();
                        Pos oldPos;
                        int exits = _posService.ExistPos(pos, out oldPos, false);
                        if (exits == 1)//已存在的POS
                        {
                            countPosUpdate++;
                            oldPos.CreateDate=DateTime.Now;
                            oldPos.VendorName = pos.VendorName;
                            oldPos.InstallAddress = pos.InstallAddress;
                            oldPos.DeductRate = pos.DeductRate;
                            _posService.Update(oldPos);
                        }
                        else
                        {
                            countPos++;
                            _posService.Insert(pos);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new OceanException("导入失败，具体原因，请查看日志！", ex);
                }
                ViewBag.countPos = countPos;
                ViewBag.countPosUpdate = countPosUpdate;
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = "请选择导入文件";
            }
            return View();
        }
        #endregion
        #endregion
    }
}