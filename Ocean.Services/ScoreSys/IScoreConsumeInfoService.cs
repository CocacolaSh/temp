﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="Plugin.cs">
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
using Ocean.Core;
using Ocean.Entity.DTO;


namespace Ocean.Services
{
    public interface IScoreConsumeInfoService : IService<ScoreConsumeInfo>
    {
        PagedList<ScoreConsumeInfo> GetConsumeScoreList(Guid gid, int pageIndex, int pageSize);
    }
}
