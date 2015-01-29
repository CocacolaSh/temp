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
using Ocean.Entity.DTO;


namespace Ocean.Services
{
    public class ComplainService : ServiceBase<Complain>, IComplainService
    {
        public ComplainService(IRepository<Complain> ComplainRepository, IDbContext context)
            : base(ComplainRepository, context)
        {
        }
        public OceanDynamicList<object> GetPageDynamicList(int pageIndex, int pageSize, ComplainDTO complainDTO)
        {
            string condition = "";
            Dictionary<string, object> parms = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(complainDTO.Name))
            {
                parms.Add("Name", complainDTO.Name);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "f.Name=@Name";
            }
            if (!string.IsNullOrEmpty(complainDTO.Phone))
            {
                parms.Add("Phone", complainDTO.Phone);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "Phone like '%'+@Phone+'%'";
            }

            if (!string.IsNullOrEmpty(complainDTO.ContactName))
            {
                parms.Add("ContactName", complainDTO.ContactName);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "ContactName like '%'+@ContactName+'%'";
            }
            if (!string.IsNullOrEmpty(complainDTO.ContactPhone))
            {
                parms.Add("ContactPhone", complainDTO.ContactPhone);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "ContactPhone like '%'+@ContactPhone+'%'";
            }
            if (complainDTO.StartDate.HasValue)
            {
                parms.Add("StartDate", complainDTO.StartDate);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "f.CreateDate >= @StartDate";
            }
            if (complainDTO.EndDate.HasValue)
            {
                parms.Add("EndDate", complainDTO.EndDate);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "f.CreateDate <= @EndDate";
            }

            return this.GetDynamicList("SELECT f.Id, f.Name,f.Phone,f.ContactName,f.ContactPhone,f.ComplainContent,f.ProcessStatus,f.ProcessResult,f.ProcessDate,f.CreateDate FROM Complain f " + condition + " order by f.CreateDate desc", pageIndex, pageSize, parms);
        }
        public OceanDynamicList<object> GetPageDynamicList(int pageIndex, int pageSize, ComplainDTO complainDTO, int isAll)
        {
            string condition = "";
            Dictionary<string, object> parms = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(complainDTO.Name))
            {
                parms.Add("Name", complainDTO.Name);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "f.Name=@Name";
            }
            if (!string.IsNullOrEmpty(complainDTO.Phone))
            {
                parms.Add("Phone", complainDTO.Phone);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "Phone like '%'+@Phone+'%'";
            }

            if (!string.IsNullOrEmpty(complainDTO.ContactName))
            {
                parms.Add("ContactName", complainDTO.ContactName);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "ContactName like '%'+@ContactName+'%'";
            }
            if (!string.IsNullOrEmpty(complainDTO.ContactPhone))
            {
                parms.Add("ContactPhone", complainDTO.ContactPhone);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "ContactPhone like '%'+@ContactPhone+'%'";
            }
            if (complainDTO.StartDate.HasValue)
            {
                parms.Add("StartDate", complainDTO.StartDate);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "f.CreateDate >= @StartDate";
            }
            if (complainDTO.EndDate.HasValue)
            {
                parms.Add("EndDate", complainDTO.EndDate);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "f.CreateDate <= @EndDate";
            }

            if (isAll == 0)
            {
                parms.Add("MpUserId", complainDTO.MpUserId);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "MpUserId = @MpUserId";
            }
            return this.GetDynamicList("SELECT f.Id, f.Name,f.Phone,f.ContactName,f.ContactPhone,f.ComplainContent,f.ProcessStatus,f.ProcessResult,f.ProcessDate,f.CreateDate FROM Complain f " + condition + " order by f.CreateDate desc", pageIndex, pageSize, parms);
        }

    }
}
