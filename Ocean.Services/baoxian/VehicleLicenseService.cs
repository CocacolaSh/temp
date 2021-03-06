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
    public class VehicleLicenseService : ServiceBase<VehicleLicense>, IVehicleLicenseService
    {
        public VehicleLicenseService(IRepository<VehicleLicense> VehicleLicenseRepository, IDbContext context)
            : base(VehicleLicenseRepository, context)
        {
        }


        public OceanDynamicList<object> GetPageDynamicList(int pageIndex, int pageSize, VehicleLicenseDTO vehicleLicenseDTO, int isAll)
        {
            string condition = "";
            Dictionary<string, object> parms = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(vehicleLicenseDTO.Owner))
            {
                parms.Add("Owner", vehicleLicenseDTO.Owner);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "Owner=@Owner";
            }
            if (!string.IsNullOrEmpty(vehicleLicenseDTO.PlateNo))
            {
                parms.Add("PlateNo", vehicleLicenseDTO.PlateNo);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "PlateNo like '%'+@PlateNo+'%'";
            }
            if (vehicleLicenseDTO.StartDate.HasValue)
            {
                parms.Add("StartDate", vehicleLicenseDTO.StartDate);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "f.IssueDate >= @StartDate";
            }
            if (vehicleLicenseDTO.EndDate.HasValue)
            {
                parms.Add("EndDate", vehicleLicenseDTO.EndDate);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "f.IssueDate <= @EndDate";
            }
            if (!string.IsNullOrEmpty(vehicleLicenseDTO.MpUser))
            {
                parms.Add("Name", vehicleLicenseDTO.MpUser);
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " and ";
                }
                else
                {
                    condition += " where ";
                }
                condition += "Name like '%'+@Name+'%'";
            }
           
            if (isAll == 0)
            {
                parms.Add("MpUserId", vehicleLicenseDTO.MpUserId);
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
            return this.GetDynamicList("SELECT f.Id,f.MpUserId MpUserId,f.PlateNo,f.VehicleType,f.Owner,f.Address,f.UseCharacter,f.CarModel,f.VIN,f.EngineNo,f.RegisterDate,f.IssueDate,m.Name,f.CreateDate CreateDate FROM VehicleLicense f left join MpUser m on  f.MpUserId = m.ID " + condition + " order by f.CreateDate desc", pageIndex, pageSize, parms);
        }

    }
}
