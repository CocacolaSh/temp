﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="PosAuth.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-05-10 17:58
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
    public class PosAuthService : ServiceBase<PosAuth>, IPosAuthService
    {
		public PosAuthService(IRepository<PosAuth> posAuthRepository, IDbContext context)
            : base(posAuthRepository, context)
        {
        }
    }
}
