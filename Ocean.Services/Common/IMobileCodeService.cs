﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="MobileCode.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-02-07 12:32
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity;


namespace Ocean.Services
{
    public interface IMobileCodeService : IService<MobileCode>
    {
        /// <summary>
        /// 手机验证码对比
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="code">验证码</param>
        /// <param name="mpUserId">用户ID</param>
        /// <param name="operationType">操作类型［页面］</param>
        /// <returns></returns>
        MobileCode GetMobileCode(string mobile, string code,Guid mpUserId,int operationType);

        /// <summary>
        /// 通过手机号获取最新的验证码
        /// </summary>
        /// <param name="mpUserId">用户ID</param>
        /// <param name="operationType">操作类型［页面］</param>
        /// <returns></returns>
        MobileCode GetMobileCode(Guid mpUserId, int operationType);

        /// <summary>
        /// 获取操作类型［页面］当天使用的验证码次数
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="operationType">操作类型［页面］</param>
        /// <returns></returns>
        int GetMobileCodeCount(string mobile, int operationType);
        /// <summary>
        /// 获取操作类型［页面］当天使用的验证码次数
        /// </summary>
        /// <param name="mobile">当前用户ID</param>
        /// <param name="operationType">操作类型［页面］</param>
        /// <returns></returns>
        int GetMobileCodeCount(Guid mpUserId, int operationType);
    }
}
