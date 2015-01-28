﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="Pos.cs">
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
using Ocean.Core.Utility;


namespace Ocean.Services
{
    public class PosService : ServiceBase<Pos>, IPosService
    {
		public PosService(IRepository<Pos> posRepository, IDbContext context)
            : base(posRepository, context)
        {
        }

        public int PosBind(Pos pos)
        {
            Pos bindPos = null;
            if (!string.IsNullOrEmpty(pos.TerminalNO))
            {
                bindPos = this.GetUnique(p => p.VendorNO == pos.VendorNO && ((p.TerminalNO == pos.TerminalNO && !string.IsNullOrEmpty(p.TerminalNO)) || string.IsNullOrEmpty(p.TerminalNO)) && p.BankNO.Substring(p.BankNO.Length-8,8) == pos.BankNO);
            }
            else
            {
                bindPos = this.GetUnique(p => p.VendorNO == pos.VendorNO && p.BankNO.Substring(p.BankNO.Length - 8, 8) == pos.BankNO);
            }
            if (bindPos != null)
            {
                pos.Id = bindPos.Id;
                pos.MobilePhones = bindPos.MobilePhones;
                pos.MpUserIds = bindPos.MpUserIds;
                return 1;
            }
            return 0;
        }


        public Core.OceanDynamicList<object> GetPageDynamicList(int PageIndex, int PageSize, Pos posDTO)
        {
            string sql = "select * from Pos where 1=1";
            if (!string.IsNullOrEmpty(posDTO.VendorName))
            {
                sql += " and VendorName like '%" + StringHelper.SqlEscape(posDTO.VendorName) + "%'";
            }
            if (!string.IsNullOrEmpty(posDTO.VendorNO))
            {
                sql += " and VendorNO like '%" + StringHelper.SqlEscape(posDTO.VendorNO) + "%'";
            }
            if (!string.IsNullOrEmpty(posDTO.TerminalNO))
            {
                sql += " and TerminalNO like '%" + StringHelper.SqlEscape(posDTO.TerminalNO) + "%'";
            }
            sql += " order by CreateDate desc ";
            return this.GetDynamicList(sql, PageIndex, PageSize);
        }

        public int ExistPos(Pos pos, out Pos oldPos, bool isVendorNo)
        {
            oldPos = this.GetUnique(p => p.VendorNO == pos.VendorNO && p.TerminalNO == pos.TerminalNO&& p.BankNO == pos.BankNO);//
            if (oldPos != null)
            {
                return 1;
            }
            return 0;
        }
    }
}
