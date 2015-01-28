﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
// </auto-generated>
//
// <copyright file="MpMaterial.cs">
//		Copyright(c)2014 Ocean.All rights reserved.
//		CLR版本：4.0.30319.239
//		生成时间：2014-02-08 19:57
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity;
using Ocean.Core.Data;
using Ocean.Data;
using System.Transactions;
using Newtonsoft.Json;
using Ocean.Core.Infrastructure;
using Ocean.Core.Logging;


namespace Ocean.Services
{
    public class MpMaterialService : ServiceBase<MpMaterial>, IMpMaterialService
    {
        private readonly IMpMaterialItemService MpMaterialItemService;
        /// <summary>
        /// WebHelper
        /// </summary>
        protected IMpReplyService MpReplyService
        {
            get
            {
                return EngineContext.Current.Resolve<IMpReplyService>();
            }
        }
        public MpMaterialService(IRepository<MpMaterial> mpMaterialRepository, IDbContext context, IMpMaterialItemService mpMaterialItemService)
            : base(mpMaterialRepository, context)
        {
            this.MpMaterialItemService = mpMaterialItemService;
        }

        public bool SaveMaterial(string data = "")
        {
            try
            {
                this.BeginTransaction();
                MpMaterial material = JsonConvert.DeserializeObject<MpMaterial>(data);
                IList<MpMaterialItem> newItems = material.MpMaterialItems.ToList();
                if (material.Id == Guid.Empty)
                {
                    foreach (var item in newItems)
                    {
                        if (item.Id == Guid.Empty)
                        {
                            MpMaterialItemService.Insert(item);
                        }
                    }
                    this.Insert(material);
                }
                else
                {
                    IList<MpMaterialItem> orginMaterialItems = MpMaterialItemService.GetALL(m => m.MaterialID == material.Id);

                    foreach (var item in newItems)
                    {
                        if (item.Id == Guid.Empty)
                        {
                            MpMaterialItemService.Insert(item);
                        }
                        else
                        {                               
                            MpMaterialItemService.Update(item);
                        }
                    }
                    foreach (var item in orginMaterialItems)
                    {
                        if (newItems.Where(i => i.Id == item.Id).Count() == 0)
                        {
                            MpMaterialItemService.Delete(item);
                        }
                    }
                    material.UpateDate = DateTime.Now;
                    this.Update(material);
                }
                this.Commit();
                return true;

            }
            catch (Exception e)
            {

                Log4NetImpl.Write("SaveMaterial失败:" + e.Message);
                this.Rollback();
                return false;
            }

        }

        public bool DelMaterial(Guid id)
        {
            try
            {
                this.BeginTransaction();
                MpMaterial mp = this.GetById(id);
                if (mp != null)
                {                    
                    var param = new Dictionary<string, object>();
                    param.Add("Id", mp.MpMaterialItems.Select(l=>l.Id).ToArray());
                    MpMaterialItemService.Delete(param);
                    var param1 = new Dictionary<string, object>();
                    Guid[] guidArr = MpReplyService.GetALL(x => x.MaterialID == mp.Id).Select(l => l.Id).ToArray();
                    if (guidArr != null && guidArr.Length>0)
                    {
                        param1.Add("Id", MpReplyService.GetALL(x => x.MaterialID == mp.Id).Select(l => l.Id).ToArray());
                        MpReplyService.Delete(param1);
                    }
                    this.Delete("delete from MpMaterial where Id='"+id+"'");
                }
                this.Commit();
                return true;

            }
            catch (Exception e)
            {
                Log4NetImpl.Write("DelMaterial失败:" + e.Message);
                this.Rollback();
                return false;
            }

        }

        public MpMaterial SaveByText(MpMaterial mp, string replyContent)
        {
            MpMaterialItem materitem = new MpMaterialItem
            {
                CreateDate = DateTime.Now,
                Description = "",
                HQMusicUrl = "",
                PicUrl = "",
                ReplyContent = replyContent,
                Summary = "",
                Title = "",
                Url = ""
            };
            MpMaterialItemService.Insert(materitem);
            IList<MpMaterialItem> items = new List<MpMaterialItem> { materitem };
            mp.MpMaterialItems = items;
            this.Insert(mp);
            return mp;

        }


    }
}
