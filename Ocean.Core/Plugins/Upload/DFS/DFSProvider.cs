using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Configuration;
using System.Reflection;
using Ocean.Core.ExceptionHandling;

namespace Ocean.Core.Plugins.DFS
{
    public class DFSProvider
    {
        static Dictionary<string, IDFS> DFSDic = new Dictionary<string, IDFS>();
        public static IDFS Instrance(string groupName)
        {
            IDFS dfs = null;
            DFSDic.TryGetValue(groupName, out dfs);
            if (dfs == null)
            {
                FastDFSConfigInfo dfsConfig = FastDFSConfigs.GetFastDFS(groupName);
                if (string.IsNullOrEmpty(groupName))
                {
                    return CurrentDFS;
                }
                else
                {
                    if (!string.IsNullOrEmpty(dfsConfig.AssemblyType))
                    {
                        string[] filenames = dfsConfig.AssemblyType.Split(',');
                        Assembly ass = Assembly.LoadFrom(System.Web.HttpRuntime.BinDirectory + filenames[1] + ".dll");
                        dfs = (IDFS)Activator.CreateInstance(ass.GetType(filenames[0], false, true), new object[] { dfsConfig });
                        dfs.DfsConfig = dfsConfig;
                        if (dfs == null)
                        {
                            throw ExceptionManager.MessageException("分布式存储系统配置文件有误，请检查！");
                        }
                        DFSDic[groupName] = dfs;
                    }
                    else
                    {
                        throw ExceptionManager.MessageException("分布式存储系统配置文件有误，请检查！");
                    }
                }
            }
            return dfs;
        }
        public static IDFS CurrentDFS
        {
            get
            {
                FastDFSConfigInfo dfsConfig = FastDFSConfigs.CurrentFastDFS;
                IDFS dfs = null;
                DFSDic.TryGetValue(dfsConfig.Groupname, out dfs);
                if (dfs == null)
                {
                    if (!string.IsNullOrEmpty(dfsConfig.AssemblyType))
                    {
                        string[] filenames = dfsConfig.AssemblyType.Split(',');
                        Assembly ass = Assembly.LoadFrom(System.Web.HttpRuntime.BinDirectory + filenames[1] + ".dll");
                        dfs = (IDFS)Activator.CreateInstance(ass.GetType(filenames[0], false, true), new object[] { dfsConfig });
                        dfs.DfsConfig = dfsConfig;
                        if (dfs == null)
                        {
                            throw ExceptionManager.MessageException("分布式存储系统配置文件有误，请检查！");
                        }
                    }
                    else
                    {
                        throw ExceptionManager.MessageException("分布式存储系统配置文件有误，请检查！");
                    }
                    DFSDic[dfsConfig.Groupname] = dfs;
                }
                return dfs;
            }
        }
        
    }
}
