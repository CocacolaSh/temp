﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="PluginBase.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-02-22 14:37
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity;
using Ocean.Core.Data;
using Ocean.Data;
using Ocean.Core;


namespace Ocean.Services
{
    public class PluginSceneResultService : ServiceBase<PluginSceneResult>, IPluginSceneResultService
    {
        public PluginSceneResultService(IRepository<PluginSceneResult> pluginSceneResultRepository, IDbContext context)
            : base(pluginSceneResultRepository, context)
        {
        }
        public OceanDynamicList<object> GetPageDynamicList(Guid pluginId)
        {
            return this.GetDynamicList("select u.CardNo, u.TradeSeqNo, f.[Summary], f.[CreateDate] from SceneResult f inner join xyPluginUser u on u.Id=f.MpUserId where f.[PluginId]='" + pluginId.ToString() + "' order by f.CreateDate desc");

        }

    }
}