﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="MpReply.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-02-20 09:40
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity;


namespace Ocean.Services
{
    public interface IMpReplyService : IService<MpReply>
    {
        MpReply GetByAction(string action);
        bool SaveMpRepy(string data);
    }
}