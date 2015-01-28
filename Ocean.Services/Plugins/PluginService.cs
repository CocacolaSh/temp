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
using Ocean.Core.Data;
using Ocean.Data;
using Ocean.Core;
using Ocean.Core.Utility;


namespace Ocean.Services
{
    public class PluginService : ServiceBase<Plugin>, IPluginService
    {
        private IPluginBaseService _pluginBaseService;
        private IPluginBaseStyleService _pluginBaseStyleService;
		public PluginService(IRepository<Plugin> pluginRepository, IDbContext context
            , IPluginBaseService pluginBaseService, IPluginBaseStyleService pluginBaseStyleService)
            : base(pluginRepository, context)
        {
            _pluginBaseService = pluginBaseService;
            _pluginBaseStyleService = pluginBaseStyleService;
        }

        public Plugin SavePlugin(Plugin pluginDto)
        {
            PluginBase pluginBase = _pluginBaseService.GetById(pluginDto.PluginBaseId);

            if (pluginBase == null)
            {
                throw new OceanException("未能找到指定的插件，请确定您是否有权使用插件或插件已下架！");
            }
            Plugin plugin = new Plugin();

            plugin.LanguageId = 1;
            plugin.RPluginBase = pluginBase;
            plugin.Value = pluginDto.Value;

            plugin.CreateDate = DateTime.Now;
            this.Insert(plugin);

            return plugin;
        }

        public void UpdatePlugin(Plugin pluginDto,Plugin plugin)
        {
            plugin.EndDate = pluginDto.EndDate;
            plugin.StartDate = pluginDto.StartDate;
            plugin.Value = pluginDto.Value;
            plugin.StyleFolder = pluginDto.StyleFolder;
            plugin.Name = pluginDto.Name;
            if (pluginDto.StyleId != null && pluginDto.StyleId != plugin.StyleId)
            {
                PluginBaseStyle style = _pluginBaseStyleService.GetById(pluginDto.StyleId);
                if (style != null && style.PluginBaseId == plugin.PluginBaseId)
                {
                    plugin.StyleId = style.Id;
                }
            }
            this.Update(plugin);
        }

        public Guid OpenPlugin(Guid pluginBaseId, string isMulti)
        {
            PluginBase pluginBase = _pluginBaseService.GetById(pluginBaseId);
            if (string.IsNullOrEmpty(isMulti) && this.GetUnique(p => p.PluginBaseId == pluginBaseId) != null && pluginBase.IsMulti==0)
            {
                throw new OceanException("该插件无法被复制，您已经开通过此插件！");
            }
            Plugin plugin = new Plugin();
            if (pluginBase == null)
                return new Guid("00000000-0000-0000-0000-000000000000");
            PluginBaseStyle style = _pluginBaseStyleService.GetUnique(" from PluginBaseStyle order by IsDefault desc");
            plugin.CategoryId = pluginBase.CategoryId;

            plugin.Value = pluginBase.Value;
            plugin.Sort = 0;
            plugin.StartDate = plugin.EndDate = plugin.CreateDate = DateTime.Now;
            plugin.RPluginBase = pluginBase;
            plugin.PluginBaseId = pluginBase.Id;
            plugin.Name = pluginBase.Name;
            //风格
            //plugin.RPluginBaseStyle = style;
            plugin.StyleId = style.Id;
            plugin.StyleFolder = style.Folder;

            plugin.LanguageId = 1;
            this.Insert(plugin);
            return plugin.Id;
        }

        public void DelPlugin(Guid pluginId)
        {
            Plugin plugin = this.GetById(pluginId);
            if (plugin == null)
            {
                throw new OceanException("对不起，您不存在该插件！");
            }
            PluginBase pluginBase = plugin.RPluginBase;
            if (pluginBase == null)
            {
                throw new OceanException("对不起，系统不存在该插件！");
            }
            this.ExcuteSql("delete from PluginResult where PluginId='" + pluginId.ToString() + "'", null);
            this.Delete(plugin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginId"></param>
        public void ResetPlugin(Guid pluginId)
        {
            Plugin plugin = this.GetById(pluginId);
            if (plugin != null)
            {
                int useCount = TypeConverter.StrToInt(StringHelper.GetRegResult("<Cst_Plugin_UseCount><!\\[CDATA\\[(\\d+)]]></Cst_Plugin_UseCount>", plugin.Value, 1));
                this.ExcuteSql("delete from SitePluginResult where PluginId='" + pluginId.ToString() + "'");
                this.ExcuteSql("update SitePluginUsed set HasUseCount=" + useCount.ToString() + " where PluginId='" + pluginId.ToString() + "'");
                plugin.Value = StringHelper.RegReplace("<Cst_Plugin_Items_PrizeUsedNum(\\d+)>.*?</Cst_Plugin_Items_PrizeUsedNum(\\d+)>", plugin.Value, "");
                this.Update(plugin);
            }
            else
            {
                throw new OceanException("您不存在此插件！");
            }
        }

        public PagedList<Plugin> SeachPluginList(string name,int pageIndex, int pageSize)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append(" from Plugin ");
            buffer.Append(" where Name like '%" + name + "%' order by CreateDate desc");
            string queryString = buffer.ToString();
            return this.GetPageList(queryString, pageIndex, pageSize);
        }

        //public IList<Plugin> GetPluginList()
        //{
        //    string queryString = " from Site_Plugin where order by Create_Date desc";
        //    return this.GetQueryList(queryString);
        //}
    }
}
