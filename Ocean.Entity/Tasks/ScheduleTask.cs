using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity.Tasks
{
    public class ScheduleTask : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// 获取或设置名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the run period (in seconds)
        /// 获取或设置运行时间(以秒为单位)
        /// </summary>
        public virtual int Seconds { get; set; }

        /// <summary>
        /// Gets or sets the type of appropriate ITask class
        /// 获取或设置适当的ITask类的类型
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether a task is enabled
        /// 获取或设置值指示是否启用了一个任务
        /// </summary>
        public virtual bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether a task should be stopped on some error
        /// 获取或设置值,该值指示一个任务是否应该停止一些错误
        /// </summary>
        public virtual bool StopOnError { get; set; }

        public virtual DateTime? LastStartUtc { get; set; }

        public virtual DateTime? LastEndUtc { get; set; }

        public virtual DateTime? LastSuccessUtc { get; set; }
    }
}