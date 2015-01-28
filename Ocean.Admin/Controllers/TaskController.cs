using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocean.Services.Tasks;
using Ocean.Page;
using Ocean.Core;
using Ocean.Entity.Tasks;
using Ocean.Entity;
using Ocean.Core.Utility;
using Ocean.Entity.Enums.AdminLoggerModule;

namespace Ocean.Admin.Controllers
{
    public class TaskController : AdminBaseController
    {
        // GET: /Task/

        private readonly IScheduleTaskService _scheduleTaskService;

        public TaskController(IScheduleTaskService scheduleTaskService)
        {
            this._scheduleTaskService = scheduleTaskService;
        }

        /// <summary>
        /// 初始化TaskIndex页面
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            if (!base.HasPermission("scheduletask", PermissionOperate.manager))
            {
                return base.ShowNotPermissionTip("");
            }

            return View();
        }

        /// <summary>
        /// 获取任务调度列表数据
        /// </summary>
        [HttpPost]
        [ActionName("_TaskList")]
        public ActionResult TaskListProvide()
        {
            PagedList<ScheduleTask> list = _scheduleTaskService.GetPageList("CreateDate", PageIndex, PageSize, true);
            return JsonList<ScheduleTask>(list, list.TotalItemCount);
        }

        /// <summary>
        /// 初始化TaskEdit页面
        /// </summary>
        [HttpGet]
        public ActionResult TaskEdit()
        {
            string id = RQuery["Id"];
            ScheduleTask scheduleTask = string.IsNullOrWhiteSpace(id) ? null : _scheduleTaskService.GetById(new Guid(id));
            return AdminView(scheduleTask);
        }

        /// <summary>
        /// 编辑任务
        /// </summary>
        [HttpPost]
        [ActionName("_TaskEdit")]
        public ActionResult BranchEditProvide()
        {
            if (string.IsNullOrWhiteSpace(RQuery["Name"]))
            {
                return JsonMessage(false, "任务名称不能为空");
            }

            if (string.IsNullOrWhiteSpace(RQuery["Type"]))
            {
                return JsonMessage(false, "ITask类型不能为空");
            }

            if (string.IsNullOrWhiteSpace(RQuery["Seconds"]))
            {
                return JsonMessage(false, "间隔时间不能为空");
            }

            if (!Validator.IsInt(RQuery["Seconds"]))
            {
                return JsonMessage(false, "间隔时间格式错误");
            }

            if (int.Parse(RQuery["Seconds"]) <= 0)
            {
                return JsonMessage(false, "请输入大于0的时间间隔");
            }

            if (string.IsNullOrWhiteSpace(RQuery["Enabled"]))
            {
                return JsonMessage(false, "请选择是否启用");  
            }

            if (string.IsNullOrWhiteSpace(RQuery["StopOnError"])) 
            {
                return JsonMessage(false, "请选择是否错误停止");  
            }

            ScheduleTask scheduleTask = new ScheduleTask();

            if (!string.IsNullOrWhiteSpace(RQuery["TaskId"]))
            {
                scheduleTask = _scheduleTaskService.GetById(new Guid(RQuery["TaskId"]));
            }

            UpdateModel<ScheduleTask>(scheduleTask);

            if (string.IsNullOrWhiteSpace(RQuery["TaskId"]))
            {
                _scheduleTaskService.Insert(scheduleTask);
                base.AddLog(string.Format("添加任务[{0}]成功", scheduleTask.Name), AdminLoggerModuleEnum.Task);
                return JsonMessage(true, "添加任务成功");
            }
            else
            {
                _scheduleTaskService.Update(scheduleTask);
                base.AddLog(string.Format("修改任务[{0}]成功", scheduleTask.Name), AdminLoggerModuleEnum.Task);
                return JsonMessage(true, "修改任务成功");
            }
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        [HttpPost]
        [ActionName("_TaskDelete")]
        public ActionResult TaskDeleteProvide(Guid id)
        {
            ScheduleTask scheduleTask = _scheduleTaskService.GetById(id);
            _scheduleTaskService.Delete(id.ToString());
            base.AddLog(string.Format("删除任务[{0}]成功", scheduleTask.Name), AdminLoggerModuleEnum.Task);
            return JsonMessage(true, "删除成功");
        }
    }
}