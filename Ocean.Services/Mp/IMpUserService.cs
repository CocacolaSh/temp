﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="MpUser.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-02-10 16:38
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity;
using Ocean.Core;
using Ocean.Entity.DTO;


namespace Ocean.Services
{
    public interface IMpUserService : IService<MpUser>
    {
        void GetUserFromApi(string token);
        PagedList<MpUserDTO> GetUsers(int pageIndex, int pageSize, MpUserDTO mpDto);
        PagedList<MpUser> GetUnderUsers(int pageIndex, int pageSize, MyUnderDTO mpUnderDto);
        void UpdateGroup(Guid[] ids, Guid gid);
        int GetCountByGroup(Guid groupId);
        MpUser GetByOpenID(string openid);
        string GetAccessToken();
        MpUser UpdateUserInfo(string OpenId, string token);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="OpenId">OpenId</param>
        /// <param name="content">发送的内容</param>
        /// <param name="token">不填则自动获取</param>
        /// <returns></returns>
        bool SendMessage(string openId, string content, string token = "");

        /// <summary>
        /// 获取我的业务列表
        /// </summary>
        /// <param name="mpUserID"></param>
        /// <returns></returns>
        OceanDynamicList<object> GetMyBusiness(Guid mpUserID);


        #region plugin
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="whereStr">条件拼接字符串</param>
        /// <param name="isFunongbao">是否为福农宝用户</param>
        /// <returns></returns>
        PagedList<MpUser> GetMpUsers(int pageIndex,int pageSize,string whereStr,bool isFunongbao);
        #endregion

        #region Task
        /// <summary>
        /// 获取不在48小时互动的关注会员
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PagedList<MpUserDTO> GetForOutTaskUserList(int pageIndex, int pageSize);
        PagedList<MpUserDTO> GetForOutAuthUserList(int pageIndex, int pageSize);
        #endregion

        /// <summary>
        /// 更改用户状态
        /// </summary>
        /// <param name="id">用户的主键GUID</param>
        /// <param name="state">0断开客服，1接入客服</param>
        void ChangeMpUserState(Guid id, int state);
    }
}
