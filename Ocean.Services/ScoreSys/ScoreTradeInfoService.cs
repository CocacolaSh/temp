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
    public class ScoreTradeInfoService : ServiceBase<ScoreTradeInfo>, IScoreTradeInfoService
    {
        public ScoreTradeInfoService(IRepository<ScoreTradeInfo> ScoreTradeInfoRepository, IDbContext context)
            : base(ScoreTradeInfoRepository, context)
        {
        }

        public int ExistTradeInfo(ScoreTradeInfo scoreTradeInfo, out ScoreTradeInfo oldScoreTradeInfo, bool isVendorNo)
        {
            oldScoreTradeInfo = this.GetUnique(p => p.ClientPhone == scoreTradeInfo.ClientPhone && p.ClientName == scoreTradeInfo.ClientName && p.TradeTime == scoreTradeInfo.TradeTime);//
            if (oldScoreTradeInfo != null)
            {
                return 1;
            }
            return 0;
        }
    }
}