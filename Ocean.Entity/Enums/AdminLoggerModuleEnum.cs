using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Enums;

namespace Ocean.Entity.Enums.AdminLoggerModule
{
    /// <summary>
    /// 系统日记管理模块
    /// </summary>
    public enum AdminLoggerModuleEnum
    {
        /// <summary>
        /// 管理员
        /// </summary>
        [EnumDescription("管理员")]
        Admin = 1,
        /// <summary>
        /// 部门
        /// </summary>
        [EnumDescription("部门")]
        Organization = 2,
        /// <summary>
        /// 模块
        /// </summary>
        [EnumDescription("模块")]
        Module = 3,
        /// <summary>
        /// 权限
        /// </summary>
        [EnumDescription("权限")]
        Permission = 4,
        /// <summary>
        /// 角色
        /// </summary>
        [EnumDescription("角色")]
        Role = 5,
        /// <summary>
        /// 系统日记
        /// </summary>
        [EnumDescription("系统日记")]
        AdminLogger = 6,
        /// <summary>
        /// 贷款
        /// </summary>
        [EnumDescription("贷款")]
        Loan = 7,
        /// <summary>
        /// 福农宝
        /// </summary>
        [EnumDescription("福农宝")]
        FunongBao = 8,
        /// <summary>
        /// 周边网点
        /// </summary>
        [EnumDescription("周边网点")]
        Branch = 9,
        /// <summary>
        /// 微信基本设置
        /// </summary>
        [EnumDescription("微信基本设置")]
        WeixinBaseSetting = 10,
        /// <summary>
        /// 微信用户管理
        /// </summary>
        [EnumDescription("微信用户管理")]
        WeixinUser = 11,
        /// <summary>
        /// 微信素材管理
        /// </summary>
        [EnumDescription("微信素材管理")]
        WeixinMaterial = 12,
        /// <summary>
        /// 微信回复设置
        /// </summary>
        [EnumDescription("微信回复设置")]
        WeixinReplySetting = 13,
        /// <summary>
        /// 微信营销活动
        /// </summary>
        [EnumDescription("微信营销活动")]
        WeixinActivity = 14,
        /// <summary>
        /// 枚举管理
        /// </summary>
        [EnumDescription("枚举管理")]
        Enum = 15,
        /// <summary>
        /// 配置管理
        /// </summary>
        [EnumDescription("配置管理")]
        Configuration = 16,
        /// <summary>
        /// 任务调度
        /// </summary>
        [EnumDescription("任务调度")]
        Task = 17,
        /// <summary>
        /// 其他
        /// </summary>
        [EnumDescription("其他")]
        Other = 18,
        [EnumDescription("POS申请")]
        Pos=19,
        [EnumDescription("行驶证")]
        VehicleLicense = 20,

        [EnumDescription("驾驶证")]
        DrivingLicense = 21,

        [EnumDescription("投诉")]
        Complain = 22,
    
        [EnumDescription("保单")]
        BaoXian = 23
    }
}