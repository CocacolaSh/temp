﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="PluginUsed.cs">
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


namespace Ocean.Services
{
    public class ScorePluginResultService : ServiceBase<ScorePluginResult>, IScorePluginResultService
    {
        public ScorePluginResultService(IRepository<ScorePluginResult> ScorePluginResultRepository, IDbContext context)
            : base(ScorePluginResultRepository, context)
        {
        }


    }
}
